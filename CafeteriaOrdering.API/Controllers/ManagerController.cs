using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Authorization;
using CafeteriaOrdering.API.DTO;

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

            // Cập nhật IsStatus thành 0 (false) thay vì xóa
            menu.IsStatus = false;
            menu.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
            await _context.SaveChangesAsync();
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
                .Where(m => m.ItemId == id && m.IsStatus == true)
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
                //CountItemsSold = menuItemDto.CountItemsSold = 0,
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

            menuItem.IsStatus = false; // Đánh dấu là không hoạt động thay vì xóa
            menuItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu item deactivated successfully" });
        }


    }
}
