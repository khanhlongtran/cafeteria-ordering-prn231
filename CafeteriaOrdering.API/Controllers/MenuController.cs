using Microsoft.AspNetCore.Mvc;
using CafeteriaOrdering.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ManagerAPI.Controllers
{
    [Authorize("MANAGER")]
    [Route("api/Manager/MaintainMenu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public MenuController(CafeteriaOrderingDBContext context)
        {
            _context = context;
        }

        // Xem danh sách menu
        [HttpGet("ViewMenu")]
        public async Task<IActionResult> ViewMenu()
        {
            var menus = await _context.Menus.ToListAsync();
            return Ok(menus);
        }

        // Thêm món mới
        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromForm] int manager_id, [FromForm] string menu_name, [FromForm] string description)
        {
            var menu = new Menu
            {
                ManagerId = manager_id,
                MenuName = menu_name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ViewMenu), new { id = menu.MenuId }, menu);
        }


        // Xóa món
        [HttpDelete("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound(new { message = "Menu not found" });
            }

            try
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Menu deleted successfully", deletedMenu = menu });
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new { message = "Cannot delete this menu because it is referenced in another table", error = ex.Message });
            }
        }

        // Cập nhật món
        [HttpPut("UpdateMenuItem/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] int managerId, [FromForm] string menuName, [FromForm] string description)
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


    }
}
