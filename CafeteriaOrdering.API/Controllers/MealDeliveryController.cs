using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Constants;
using CafeteriaOrdering.API.Services;
using Microsoft.AspNetCore.Authorization;
using CafeteriaOrdering.API.DTO;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaOrdering.API.Controllers
{
    //[Authorize("DELIVER")]
    [Route("api/v1/delivery")]
    [ApiController]
    public class MealDeliveryController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;
        private readonly IMealDeliveryService _service;

        public MealDeliveryController(CafeteriaOrderingDBContext context, IMealDeliveryService mealDeliveryService)
        {
            _context = context;
            _service = mealDeliveryService;
        }

        // GET: api/v1/delivery/orders
        [HttpGet("{userId}/orders")]
        public ActionResult<IEnumerable<Order>> Get(int userId)
        {
            var deliveries = _context.Deliveries.Where(d => d.DeliverUserId == userId).ToList();
            List<string> orderIds = new List<string>();
            foreach (var delivery in deliveries)
            {
                orderIds.Add(delivery.OrderId.ToString());
            }

            var orders = _context.Orders.Where(o => orderIds.Contains(o.OrderId.ToString())).ToList();
            return orders;
        }

        // GET: api/v1/delivery/orders/5
        [HttpGet("{userId}/orders/{id}")]
        public ActionResult<Order> Get(int userId, int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            var delivery = _context.Deliveries.Where(d => d.OrderId == id && d.DeliverUserId == userId).FirstOrDefault();
            if (delivery == null)
            {
                return NotFound("Delivery not found");
            }

            return order;
        }

        // PUT: api/v1/delivery/orders/5
        [HttpPut("orders/{id}")]
        public ActionResult<Order> Put(int id, [FromBody] string status)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (OrderConstants.isValidStatusForDeliver(status))
            {
                return BadRequest("Invalid status for deliverer");
            }

            if (status == OrderConstants.OrderStatus.REQUEST_DELIVERY.ToString())
            {
                order.Status = OrderConstants.OrderStatus.DELIVERY_ACCEPTED.ToString();
            }
            else if (status == OrderConstants.OrderStatus.DELIVERY_ACCEPTED.ToString())
            {
                order.Status = OrderConstants.OrderStatus.DELIVERY_IN_PROGRESS.ToString();
            }
            else if (status == OrderConstants.OrderStatus.DELIVERY_IN_PROGRESS.ToString())
            {
                order.Status = OrderConstants.OrderStatus.COMPLETED.ToString();
            }
            else
            {
                return BadRequest("Invalid status");
            }

            _context.Orders.Update(order);
            _context.SaveChanges();
            _service.notifyUpdateOrderStatus(id, order.Status);

            return Ok("Successful");
        }

        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<UserOrderDTO>>> GetUserOrders()
        {
            var userOrders = await _context.Orders
                .Where(o => o.Status == "REQUEST_DELIVERY")  
                .Include(o => o.OrderItems)   
                .Include(o => o.Address)        
                .Select(o => new UserOrderDTO
                {
                    UserId = o.UserId,
                    FullName = o.User.FullName,
                    Number = o.User.Phone,
                    OrderId = o.OrderId,
                    OrderName = string.Join(", ", o.OrderItems.Select(oi => oi.Item.ItemName)),  
                    TotalAmount = o.TotalAmount,
                    AddressId = o.Address.AddressId,
                    AddressLine = o.Address.AddressLine,
                    City = o.Address.City,
                    State = o.Address.State,
                    GeoLocation = o.Address.GeoLocation
                })
                .ToListAsync();

            if (userOrders == null || !userOrders.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(userOrders);
        }
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            var order = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Item)
        .ThenInclude(mi => mi.Menu)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var delivery = await _context.Deliveries.FirstOrDefaultAsync(d => d.OrderId == orderId);

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (delivery != null)
            {
                delivery.DeliveryStatus = newStatus;
                delivery.UpdatedAt = DateTime.UtcNow;

                switch (newStatus)
                {
                    case "DELIVERY_ACCEPTED":
                        delivery.PickupTime ??= DateTime.UtcNow;
                        break;
                    case "COMPLETED":
                        delivery.DeliveryTime ??= DateTime.UtcNow;
                        break;
                    case "CANCELED":
                        delivery.DeliveryTime = null;
                        break;
                }
            }

    
            if (newStatus == "COMPLETED")
            {
                var revenueData = new Dictionary<int, (int TotalOrders, decimal TotalRevenue)>();

                foreach (var orderItem in order.OrderItems)
                {
                    var menuItem = orderItem.Item;
                    int managerId = orderItem.Item.Menu.ManagerId;
                    decimal itemRevenue = orderItem.Price;

                    menuItem.CountItemsSold = (menuItem.CountItemsSold ?? 0) + orderItem.Quantity;
                    if (revenueData.ContainsKey(managerId))
                    {
                        revenueData[managerId] = (
                            revenueData[managerId].TotalOrders + orderItem.Quantity,
                            revenueData[managerId].TotalRevenue + itemRevenue
                        );
                    }
                    else
                    {
                        revenueData[managerId] = (orderItem.Quantity, itemRevenue);
                    }
                }

                foreach (var entry in revenueData)
                {
                    int managerId = entry.Key;
                    int totalOrders = entry.Value.TotalOrders;
                    decimal totalRevenue = entry.Value.TotalRevenue;

                    var revenueReport = new RevenueReport
                    {
                        ManagerId = managerId,
                        ReportDate = DateTime.UtcNow,
                        TotalOrders = totalOrders,
                        TotalRevenue = totalRevenue,
                        GeneratedAt = DateTime.UtcNow
                    };

                    _context.RevenueReports.Add(revenueReport);
                }
            }

            try
            {
                _context.Orders.Update(order);
                if (delivery != null)
                {
                    _context.Deliveries.Update(delivery);
                }

                await _context.SaveChangesAsync();
                return Ok("Order, Delivery status, and RevenueReport updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetOrderInProgessDeliveries/{delivererUserId}")]
        public async Task<ActionResult<IEnumerable<DeliveryOrderDTO>>> GetDeliveriesByUser(int delivererUserId)
        {
            var deliveries = await _context.Deliveries
                .Where(d => d.DeliverUserId == delivererUserId
                    && d.DeliveryStatus != "REQUEST_DELIVERY"
                    && d.DeliveryStatus != "COMPLETED")
                .Select(d => new DeliveryOrderDTO
                {
                    DeliveryId = d.DeliveryId,
                    OrderId = d.OrderId,
                    OrderName = string.Join(", ", d.Order.OrderItems.Select(oi => oi.Item.ItemName)),
                    PatronName = d.Order.User.FullName,
                    Number = d.Order.User.Phone,
                    TotalAmount = d.Order.TotalAmount,
                    Address = d.Order.Address.AddressLine,
                    DeliverUserId = d.DeliverUserId,
                    PickupTime = d.PickupTime,
                    DeliveryTime = d.DeliveryTime,
                    DeliveryStatus = d.DeliveryStatus,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();

            if (deliveries == null || !deliveries.Any())
            {
                return NotFound("No deliveries found for this user.");
            }

            return Ok(deliveries);
        }

        [HttpPost("CreateDelivery")]
        public async Task<ActionResult<Delivery>> PostDelivery([FromBody] DeliveryCreateDto deliveryDto)
        {
            var order = await _context.Orders.FindAsync(deliveryDto.OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var delivery = new Delivery
            {
                OrderId = deliveryDto.OrderId,
                DeliverUserId = deliveryDto.DeliverUserId,
                PickupTime = deliveryDto.PickupTime,
                DeliveryTime = deliveryDto.DeliveryTime,
                DeliveryStatus = "DELIVERY_ACCEPTED", 
                CreatedAt = deliveryDto.CreatedAt,
                UpdatedAt = deliveryDto.UpdatedAt
            };


            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostDelivery), new { id = delivery.DeliveryId }, delivery);
        }
    }
}