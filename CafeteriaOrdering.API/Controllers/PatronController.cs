using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CafeteriaOrdering.API.DTO;
using Microsoft.AspNetCore.Authorization;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatronController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _dbContext;
        public PatronController(CafeteriaOrderingDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize("PATRON")]
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

        [HttpGet("GetUserAddressesAndDefaultCuisine/{userId}")]
        public async Task<IActionResult> GetUserAddresses(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User không tồn tại.");
            }

            var addresses = user.Addresses.Select(a => new
            {

                a.AddressId,
                a.AddressLine,
                a.City,
                a.User.DefaultCuisine,
                a.State,
                a.ZipCode,
                a.IsDefault,
                a.GeoLocation,
                a.CreatedAt,
                a.UpdatedAt,
            }).ToList();

            return Ok(addresses);
        }

        [Authorize("PATRON")]
        [HttpPost("MyAccount/ChangeAddress/{userId}")]
        public async Task<IActionResult> ChangeAddress(int userId, [FromBody] AddressRequest request)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User không tồn tại");
            }

            if (string.IsNullOrEmpty(request.AddressLine))
            {
                return BadRequest("AddressLine là bắt buộc");
            }

            // Reset địa chỉ mặc định nếu có
            if (request.IsDefault)
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
                AddressLine = request.AddressLine,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                IsDefault = request.IsDefault,
                GeoLocation = request.GeoLocation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

            };

            _dbContext.Addresses.Add(newAddress);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Tạo mới địa chỉ thành công.",
                AddressId = newAddress.AddressId
            });
        }

        [Authorize("PATRON")]
        [HttpPut("MyAccount/UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromQuery] int userId, [FromQuery] int addressId, [FromBody] AddressRequest request)
        {
            var address = await _dbContext.Addresses.FindAsync(addressId);
            if (address == null)
            {
                return NotFound("Địa chỉ không tồn tại.");
            }

            // Kiểm tra xem địa chỉ có thuộc về userId không
            if (address.UserId != userId)
            {
                return BadRequest("Bạn không có quyền cập nhật địa chỉ này.");
            }

            address.AddressLine = request.AddressLine ?? address.AddressLine;
            address.City = request.City ?? address.City;
            address.State = request.State ?? address.State;
            address.ZipCode = request.ZipCode ?? address.ZipCode;
            address.GeoLocation = request.GeoLocation ?? address.GeoLocation;

            if (request.IsDefault)
            {
                var existingAddresses = _dbContext.Addresses.Where(a => a.UserId == userId && a.IsDefault);
                foreach (var addr in existingAddresses)
                {
                    addr.IsDefault = false;
                }
            }
            address.IsDefault = request.IsDefault;
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

        [Authorize("PATRON")]
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

        [Authorize("PATRON")]
        [HttpPost("MyOrder/MakeAFeedback")]
        public async Task<IActionResult> MakeAFeedback([FromBody] FeedbackRequest request)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.UserId == request.UserId);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về user.");
            }

            var feedback = new Feedback
            {
                UserId = request.UserId,
                OrderId = request.OrderId,
                Rating = request.Rating,
                Comment = request.Comment,
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

        [Authorize("PATRON")]
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

        [Authorize("PATRON")]
        [HttpPost("OrderAMeal")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var user = await _dbContext.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId);

                var addressExists = user.Addresses.Any(a => a.AddressId == request.AddressId);
                if (!addressExists)
                {
                    return BadRequest("Địa chỉ không hợp lệ hoặc không thuộc về user.");
                }

                decimal totalAmount = 0;
                var orderItemsToAdd = new List<OrderItem>();

                foreach (var orderItem in request.OrderItems)
                {
                    var menuItem = await _dbContext.MenuItems.FindAsync(orderItem.ItemId);
                    if (menuItem == null)
                        return NotFound($"MenuItem with ID {orderItem.ItemId} not found");

                    var price = menuItem.Price * orderItem.Quantity;
                    totalAmount += price;

                    orderItemsToAdd.Add(new OrderItem
                    {
                        ItemId = orderItem.ItemId,
                        Quantity = orderItem.Quantity,
                        Price = price
                    });
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    PaymentMethod = null,
                    AddressId = request.AddressId,
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
            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //[Authorize(Roles = "MANAGER")], bỏ tạm thời để gọi từ python :v, lấy cả 2.
        [HttpPut("{addressId}/geo-location")]
        public async Task<IActionResult> UpdateGeoLocation(int addressId, [FromBody] UpdateGeoLocationRequest request)
        {
            var address = await _dbContext.Addresses.FindAsync(addressId);
            if (address == null)
            {
                return NotFound(new { message = "Address not found" });
            }

            address.GeoLocation = request.GeoLocation;
            address.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "GeoLocation updated successfully" });
        }

        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetMenuItemById(int itemId)
        {
            var menuItem = await _dbContext.MenuItems
                .Include(m => m.Menu)
                .Where(m => m.ItemId == itemId)
                .Select(m => new
                {
                    ItemId = m.ItemId,
                    ItemName = m.ItemName,
                    Description = m.Description,
                    Price = m.Price,
                    Type = m.ItemType,
                    Status = m.IsStatus,
                    Image = m.Image
                })
                .FirstOrDefaultAsync();

            if (menuItem == null)
            {
                return NotFound(new { message = "Item not found" });
            }

            return Ok(menuItem);
        }
    }
}
