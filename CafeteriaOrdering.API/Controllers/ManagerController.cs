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
            var menus = await _context.Menus.ToListAsync();
            return Ok(menus);
        }

        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromBody] Menu menu)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            menu.CreatedAt = DateTime.UtcNow;
            menu.UpdatedAt = DateTime.UtcNow;
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ViewMenu), new { id = menu.MenuId }, menu);
        }

        [HttpDelete("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            if (await _context.MenuItems.AnyAsync(m => m.MenuId == id))
                return Conflict(new { message = "Cannot delete menu because it contains menu items" });

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Menu deleted successfully" });
        }

        [HttpPut("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] Menu menuUpdate)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            menu.ManagerId = menuUpdate.ManagerId;
            menu.MenuName = menuUpdate.MenuName;
            menu.Description = menuUpdate.Description;
            menu.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu updated successfully", updatedMenu = menu });
        }

        // ----------------------- MENU ITEMS -----------------------

        [HttpGet("ViewMenuItems/{menuId}")]
        public async Task<IActionResult> GetMenuItems(int menuId)
        {
            var menuItems = await _context.MenuItems.Where(m => m.MenuId == menuId).ToListAsync();
            return Ok(menuItems);
        }

        [HttpGet("ViewMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound(new { message = "Menu item not found" });

            return Ok(menuItem);
        }

        [HttpPost("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItem menuItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            menuItem.CreatedAt = DateTime.UtcNow;
            menuItem.UpdatedAt = DateTime.UtcNow;
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.ItemId }, menuItem);
        }

        [HttpPut("UpdateMenuItem/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItem menuItemUpdate)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound(new { message = "Menu item not found" });

            menuItem.MenuId = menuItemUpdate.MenuId;
            menuItem.ItemName = menuItemUpdate.ItemName;
            menuItem.Description = menuItemUpdate.Description;
            menuItem.Price = menuItemUpdate.Price;
            menuItem.ItemType = menuItemUpdate.ItemType;
            menuItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu item updated successfully", updatedMenuItem = menuItem });
        }

        [HttpDelete("DeleteMenuItem/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound(new { message = "Menu item not found" });

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Menu item deleted successfully" });
        }
    }
}
