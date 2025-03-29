using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Authorization;
using CafeteriaOrdering.API.DTO;
using CafeteriaOrdering.API.Constants;

namespace ManagerAPI.Controllers
{
    [Authorize("MANAGER")]
    [Route("api/Manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public ManagerController(CafeteriaOrderingDBContext context)
        {
            _context = context;
        }

        // ----------------------- MENU -----------------------
        [HttpGet("ViewMenu")]
        public async Task<IActionResult> ViewMenu()
        {
            var menus = await _context.Menus
                .AsNoTracking()
                .Where(m => m.IsStatus == true) // Lọc chỉ lấy những menu có IsStatus = true
                .ToListAsync();

            return Ok(menus);
        }


        //[HttpPost("CreateMenu")]
        //public async Task<IActionResult> CreateMenu([FromForm] Menu menu)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    menu.CreatedAt = DateTime.UtcNow;
        //    menu.UpdatedAt = DateTime.UtcNow;
        //    _context.Menus.Add(menu);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(ViewMenu), new { id = menu.MenuId }, menu);
        //}

        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromForm] MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menu = new Menu
            {
                ManagerId = menuDto.ManagerId,
                MenuName = menuDto.MenuName,
                Description = menuDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsStatus = menuDto.IsStatus ?? true // Mặc định là true nếu không có giá trị
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ViewMenu), new { id = menu.MenuId }, menu);
        }




        //[HttpPut("UpdateMenu/{id}")]
        //public async Task<IActionResult> UpdateMenu(int id, [FromForm] Menu menuUpdate)
        //{
        //    var menu = await _context.Menus.FindAsync(id);
        //    if (menu == null)
        //        return NotFound(new { message = "Menu not found" });

        //    menu.ManagerId = menuUpdate.ManagerId;
        //    menu.MenuName = menuUpdate.MenuName;
        //    menu.Description = menuUpdate.Description;
        //    menu.UpdatedAt = DateTime.UtcNow;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Menu updated successfully", updatedMenu = menu });
        //}
        [HttpPut("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromForm] MenuDto menuDto)
        {
            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound(new { message = "Menu not found" });
            }

            // Cập nhật thông tin menu từ DTO
            existingMenu.ManagerId = menuDto.ManagerId;
            existingMenu.MenuName = menuDto.MenuName;
            existingMenu.Description = menuDto.Description;
            existingMenu.IsStatus = menuDto.IsStatus ?? existingMenu.IsStatus; // Giữ nguyên trạng thái nếu không có giá trị mới
            existingMenu.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian hiện tại

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Menu updated successfully", updatedMenu = existingMenu });
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new { message = "Error updating menu", error = ex.Message });
            }
        }


        [HttpDelete("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            var menuItems = await _context.MenuItems
                .Where(m => m.MenuId == id)
                .ToListAsync();

            if (menuItems.Any()) {
                foreach (var item in menuItems)
                {
                    _context.MenuItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }

            _context.Menus.Remove(menu); // Xóa menu
            await _context.SaveChangesAsync();

            // // Cập nhật IsStatus thành 0 (false) thay vì xóa
            // menu.IsStatus = false;
            // menu.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
            // await _context.SaveChangesAsync();
            return Ok(new { message = "Menu has been deactivated" });
        }



        // ----------------------- MENU ITEMS -----------------------

        [HttpGet("ViewMenuItems/{menuId}")]
        public async Task<IActionResult> GetMenuItems(int menuId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.MenuId == menuId && m.IsStatus == true)
                .AsNoTracking()
                .ToListAsync();

            if (!menuItems.Any())
                return NotFound(new { message = "No active menu items found for this menu." });

            return Ok(menuItems);
        }

        [HttpGet("ViewMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                // .Where(m => m.ItemId == id && m.IsStatus == true)
                .Where(m => m.ItemId == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (menuItem == null)
                return NotFound(new { message = "Menu item not found or inactive." });

            return Ok(menuItem);
        }


        //[HttpPost("CreateMenuItems")]
        //public ActionResult<MenuItem> CreateMenuItem(
        //    [FromForm] int menuId,
        //    [FromForm] string itemName,
        //    [FromForm] string? description,
        //    [FromForm] decimal price,
        //    [FromForm] string? itemType)
        //{
        //    var menuItem = new MenuItem
        //    {
        //        MenuId = menuId,
        //        ItemName = itemName,
        //        Description = description,
        //        Price = price,
        //        ItemType = itemType,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow,
        //        IsStatus = true // Set mặc định IsStatus = true
        //    };

        //    _context.MenuItems.Add(menuItem);
        //    _context.SaveChanges();

        //    return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.ItemId }, menuItem);
        //}



        [HttpPost("CreateMenuItems")]
        public async Task<IActionResult> CreateMenuItem([FromForm] MenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menuItem = new MenuItem
            {
                MenuId = menuItemDto.MenuId,
                ItemName = menuItemDto.ItemName,
                Description = menuItemDto.Description,
                Price = menuItemDto.Price,
                ItemType = menuItemDto.ItemType,
                CountItemsSold = 0,
                IsStatus = menuItemDto.IsStatus ?? true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Image = menuItemDto.Image
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateMenuItem), new { id = menuItem.ItemId }, menuItem);
        }

        [HttpPut("UpdateMenuItems/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] MenuItemDto menuItemDto, [FromForm] IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menuItem = _context.MenuItems.FirstOrDefault(m => m.ItemId == id);
            if (menuItem == null)
            {
                return NotFound(new { message = "Menu item not found" });
            }

            try
            {
                menuItem.MenuId = menuItemDto.MenuId;
                menuItem.ItemName = menuItemDto.ItemName;
                menuItem.Description = menuItemDto.Description;
                menuItem.Price = menuItemDto.Price;
                menuItem.ItemType = menuItemDto.ItemType;
                menuItem.IsStatus = menuItemDto.IsStatus ?? menuItem.IsStatus;
                menuItem.UpdatedAt = DateTime.UtcNow;

                // Nếu có ảnh mới, lưu vào thư mục và cập nhật đường dẫn vào DB
                if (imageFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    menuItem.Image = $"/images/{fileName}";
                }

                _context.SaveChanges();
                return Ok(new { message = "Menu item updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }





        //[HttpPut("{id}")]
        //public IActionResult UpdateMenuItem(int id, [FromForm] int menuId, [FromForm] string itemName, [FromForm] string? description, [FromForm] decimal price, [FromForm] string? itemType)
        //{
        //    var menuItem = _context.MenuItems.Find(id);
        //    if (menuItem == null)
        //    {
        //        return NotFound();
        //    }

        //    menuItem.MenuId = menuId;
        //    menuItem.ItemName = itemName;
        //    menuItem.Description = description;
        //    menuItem.Price = price;
        //    menuItem.ItemType = itemType;
        //    menuItem.UpdatedAt = DateTime.UtcNow;

        //    _context.SaveChanges();
        //    return NoContent();
        //}
        [HttpDelete("DeleteMenuItem/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound(new { message = "Menu item not found" });

            // menuItem.IsStatus = false; // Đánh dấu là không hoạt động thay vì xóa
            // menuItem.UpdatedAt = DateTime.UtcNow;

            _context.MenuItems.Remove(menuItem);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu item deactivated successfully" });
        }



        //================================Order===================================

        [HttpGet("GetAllOrder")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrder()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("GetOrderByAddress/{addressId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderByAddress(int addressId)
        {
            var orders = await _context.Orders
                                       .Where(o => o.AddressId == addressId)
                                       .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this address." });
            }

            return Ok(orders);
        }


        [HttpGet("GetOrderItems/{orderId}")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(x => x.Item) // Nếu muốn lấy thông tin chi tiết về món ăn
                .AsNoTracking()
                .ToListAsync();

            if (!orderItems.Any())
                return NotFound(new { message = "No order items found for this order." });

            return Ok(orderItems);
        }




        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            if (request.Status == OrderConstants.OrderStatus.REQUEST_DELIVERY.ToString())
            {
                order.Status = OrderConstants.OrderStatus.DELIVERY_ACCEPTED.ToString();

                // Chèn vào bảng Deliveries
                // var delivery = new Delivery { OrderId = orderId };
                // _context.Deliveries.Add(delivery);
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order status updated successfully" });
        }


        // Model cho request body
        public class UpdateStatusRequest
        {
            public string Status { get; set; } = null!;
        }



        [HttpGet("GetOrdersByManager/{managerId}")]
        public IActionResult GetOrdersByManager(int managerId)
        {
            var orders = _context.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Item.Menu.ManagerId == managerId))
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .ToList();

            if (!orders.Any())
            {
                return NotFound("No orders found for this manager.");
            }

            return Ok(orders);
        }



        [HttpGet("GetMenusByManager/{managerId}")]
        public IActionResult GetMenusByManager(int managerId)
        {
            var menus = _context.Menus
                .Where(m => m.ManagerId == managerId)
                .Include(m => m.MenuItems)
                .ToList();

            if (!menus.Any())
            {
                return NotFound("No menus found for this manager.");
            }

            return Ok(menus);
        }

    }
}


