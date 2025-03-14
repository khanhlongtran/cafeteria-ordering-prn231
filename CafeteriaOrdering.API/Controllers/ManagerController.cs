using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeteriaOrdering.API.Models;

namespace ManagerAPI.Controllers
{
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
        public async Task<IActionResult> CreateMenu([FromForm] int manager_id, [FromForm] string menu_name, [FromForm] string description)
        {
            var menu = new Menu
            {
                ManagerId = manager_id,
                MenuName = menu_name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsStatus = true // Mặc định giá trị IsStatus = true
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
        public async Task<IActionResult> UpdateMenu(int id, [FromForm] int managerId, [FromForm] string menuName, [FromForm] string description)
        {
            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound(new { message = "Menu not found" });
            }

            // Cập nhật các trường được chỉ định
            existingMenu.ManagerId = managerId;
            existingMenu.MenuName = menuName;
            existingMenu.Description = description;
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

            return Ok(menuItems);
        }

        [HttpGet("ViewMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ItemId == id && m.IsStatus == true);

            if (menuItem == null)
                return NotFound(new { message = "Menu item not found or inactive" });

            return Ok(menuItem);
        }

        [HttpPost("CreateMenuItems")]
        public ActionResult<MenuItem> CreateMenuItem(
            [FromForm] int menuId,
            [FromForm] string itemName,
            [FromForm] string? description,
            [FromForm] decimal price,
            [FromForm] string? itemType)
        {
            var menuItem = new MenuItem
            {
                MenuId = menuId,
                ItemName = itemName,
                Description = description,
                Price = price,
                ItemType = itemType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsStatus = true // Set mặc định IsStatus = true
            };

            _context.MenuItems.Add(menuItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.ItemId }, menuItem);
        }


        [HttpPut("UpdateMenuItems/{id}")]
        public IActionResult UpdateMenuItem(int id, [FromForm] int menuId, [FromForm] string itemName,
                                     [FromForm] string? description, [FromForm] decimal price,
                                     [FromForm] string? itemType)
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
                menuItem.MenuId = menuId;
                menuItem.ItemName = itemName;
                menuItem.Description = description;
                menuItem.Price = price;
                menuItem.ItemType = itemType;
                menuItem.UpdatedAt = DateTime.UtcNow;

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
