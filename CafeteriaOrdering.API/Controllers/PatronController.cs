using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CafeteriaOrdering.API.Controllers
{
    [Authorize("PATRON")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatronController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _dbContext;
        public PatronController(CafeteriaOrderingDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPut("MyAccount/ChangeDefaultCuisine/{userId}")]
        public async Task<IActionResult> ChangeDefaultCuisine(int userId, [FromBody] string defaultCuisine)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User không tồn tại");
            }

            user.DefaultCuisine = defaultCuisine;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Món ăn mặc định đã được cập nhật thành công.");
        }

        [HttpPost("MyAccount/ChangeAddress/{userId}")]
        public async Task<IActionResult> ChangeAddress(int userId, [FromBody] IDictionary<string, object> request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User không tồn tại");
            }

            var addressLine = request.ContainsKey("addressLine") ? request["addressLine"]?.ToString() : null;
            var city = request.ContainsKey("city") ? request["city"]?.ToString() : null;
            var state = request.ContainsKey("state") ? request["state"]?.ToString() : null;
            var zipCode = request.ContainsKey("zipCode") ? request["zipCode"]?.ToString() : null;
            var isDefault = request.ContainsKey("isDefault") && bool.TryParse(request["isDefault"]?.ToString(), out var result) ? result : false;

            if (string.IsNullOrEmpty(addressLine))
            {
                return BadRequest("AddressLine là bắt buộc");
            }

            // Reset địa chỉ mặc định nếu có
            if (isDefault)
            {
                var existingAddresses = _dbContext.Addresses.Where(a => a.UserId == userId && a.IsDefault);
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            var newAddress = new Address
            {
                UserId = userId,
                AddressLine = addressLine!,
                City = city,
                State = state,
                ZipCode = zipCode,
                IsDefault = isDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Addresses.Add(newAddress);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tạo mới địa chỉ thành công.",
                AddressId = newAddress.AddressId
            });
        }

        [HttpPut("MyAccount/UpdateAddress/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] IDictionary<string, object> request)
        {
            var address = await _dbContext.Addresses.FindAsync(addressId);
            if (address == null)
            {
                return NotFound("Địa chỉ không tồn tại.");
            }

            if (request.ContainsKey("addressLine")) address.AddressLine = request["addressLine"]?.ToString();
            if (request.ContainsKey("city")) address.City = request["city"]?.ToString();
            if (request.ContainsKey("state")) address.State = request["state"]?.ToString();
            if (request.ContainsKey("zipCode")) address.ZipCode = request["zipCode"]?.ToString();
            if (request.ContainsKey("isDefault") && bool.TryParse(request["isDefault"]?.ToString(), out var isDefault))
            {
                if (isDefault)
                {
                    var existingAddresses = _dbContext.Addresses.Where(a => a.UserId == address.UserId && a.IsDefault);
                    foreach (var addr in existingAddresses)
                    {
                        addr.IsDefault = false;
                    }
                }
                address.IsDefault = isDefault;
            }

            address.UpdatedAt = DateTime.UtcNow;

            _dbContext.Addresses.Update(address);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật địa chỉ thành công.", AddressId = address.AddressId });
        }

        //[HttpDelete("MyAccount/DeleteAddress/{addressId}")]
        //public async Task<IActionResult> DeleteAddress(int addressId)
        //{
        //    var address = await _dbContext.Addresses.FindAsync(addressId);
        //    if (address == null)
        //    {
        //        return NotFound("Địa chỉ không tồn tại.");
        //    }

        //    _dbContext.Addresses.Remove(address);
        //    await _dbContext.SaveChangesAsync();

        //    return Ok(new { Message = "Xóa địa chỉ thành công.", AddressId = addressId });
        //}

        [HttpGet("MyOrder/{userId}")]
        public async Task<IActionResult> GetMyOrders(int userId)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.TotalAmount,
                    o.AddressId,
                    o.CreatedAt,
                    o.UpdatedAt,
                    Address = new
                    {
                        o.Address.AddressLine,
                        o.Address.City,
                        o.Address.State,
                        o.Address.ZipCode
                    },
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        oi.ItemId,
                        oi.Quantity,
                        oi.Price
                    }),
                    Deliveries = o.Deliveries.Select(d => new
                    {
                        d.DeliveryId,
                        d.PickupTime,
                        d.DeliveryTime,
                        d.DeliveryStatus
                    }),
                    Feedbacks = o.Feedbacks.Select(f => new
                    {
                        f.FeedbackId,
                        f.Rating,
                        f.Comment,
                        f.CreatedAt
                    })
                })
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("User không có đơn hàng nào.");
            }

            return Ok(orders);
        }

        [HttpPost("MyOrder/MakeAFeedback")]
        public async Task<IActionResult> MakeAFeedback([FromBody] IDictionary<string, object> request)
        {
            var userId = ((JsonElement)request["userId"]).GetInt32();
            var orderId = ((JsonElement)request["orderId"]).GetInt32();
            var rating = ((JsonElement)request["rating"]).GetInt32();
            var comment = ((JsonElement)request["comment"]).GetString();

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về user.");
            }

            var feedback = new Feedback
            {
                UserId = userId,
                OrderId = orderId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Feedbacks.Add(feedback);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Gửi feedback thành công.",
                Feedback = new
                {
                    feedback.FeedbackId,
                    feedback.UserId,
                    feedback.OrderId,
                    feedback.Rating,
                    feedback.Comment,
                    feedback.CreatedAt
                }
            });
        }

        [HttpGet("MyOrder/TrackOrderStatus/{orderId}")]
        public async Task<IActionResult> TrackOrderStatus(int orderId)
        {

            var order = await _dbContext.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.Status,
                    o.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng với orderId này.");
            }

            return Ok(new
            {
                Message = "Trạng thái đơn hàng hiện tại.",
                Order = order
            });
        }
        [HttpPost("OrderAMeal")]
        public async Task<IActionResult> CreateOrder([FromBody] IDictionary<string, object> request)
        {
            try
            {
                var userId = ((JsonElement)request["userId"]).GetInt32();
                //var paymentMethod = ((JsonElement)request["paymentMethod"]).GetString();
                var addressId = ((JsonElement)request["addressId"]).GetInt32();

                var orderItemsElement = (JsonElement)request["orderItems"];
                var orderItems = new List<(int itemId, int quantity)>();

                foreach (var item in orderItemsElement.EnumerateArray())
                {
                    var itemId = item.GetProperty("itemId").GetInt32();
                    var quantity = item.GetProperty("quantity").GetInt32();
                    orderItems.Add((itemId, quantity));
                }

                decimal totalAmount = 0;
                var orderItemsToAdd = new List<OrderItem>();

                foreach (var (itemId, quantity) in orderItems)
                {
                    var menuItem = await _dbContext.MenuItems.FindAsync(itemId);
                    if (menuItem == null)
                        return NotFound($"MenuItem with ID {itemId} not found");

                    var price = menuItem.Price * quantity;
                    totalAmount += price;

                    orderItemsToAdd.Add(new OrderItem
                    {
                        ItemId = itemId,
                        Quantity = quantity,
                        Price = price
                    });
                }

                var order = new Order
                {
                    UserId = userId,
                    PaymentMethod = null,
                    AddressId = addressId,
                    OrderDate = DateTime.UtcNow,
                    Status = "PENDING",
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = orderItemsToAdd
                };

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                return Ok(new { orderId = order.OrderId, totalAmount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
