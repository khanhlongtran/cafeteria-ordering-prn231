using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Constants;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/v1/delivery")]
    [ApiController]
    public class MealDeliveryController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public MealDeliveryController(CafeteriaOrderingDBContext context)
        {
            _context = context;
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

            _context.SaveChanges();

            return Ok("Successful");
        }
    }
}