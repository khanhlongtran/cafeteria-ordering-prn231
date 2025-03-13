using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace ManagerAPI.Controllers
{
    [Authorize("MANAGER")]
    [Route("api/Manager/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public MenuItemsController(CafeteriaOrderingDBContext context)
        {
            _context = context;
        }

        // Get all menu items
        [HttpGet]
        public ActionResult<IEnumerable<MenuItem>> GetMenuItems()
        {
            return _context.MenuItems.ToList();
        }

        // Get menu item by ID
        [HttpGet("{id}")]
        public ActionResult<MenuItem> GetMenuItem(int id)
        {
            var menuItem = _context.MenuItems.Find(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return menuItem;
        }

        // Create a new menu item
        [HttpPost]
        public ActionResult<MenuItem> CreateMenuItem([FromForm] int menuId, [FromForm] string itemName, [FromForm] string? description, [FromForm] decimal price, [FromForm] string? itemType)
        {
            var menuItem = new MenuItem
            {
                MenuId = menuId,
                ItemName = itemName,
                Description = description,
                Price = price,
                ItemType = itemType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MenuItems.Add(menuItem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.ItemId }, menuItem);
        }

        // Update an existing menu item
        [HttpPut("{id}")]
        public IActionResult UpdateMenuItem(int id, [FromForm] int menuId, [FromForm] string itemName, [FromForm] string? description, [FromForm] decimal price, [FromForm] string? itemType)
        {
            var menuItem = _context.MenuItems.Find(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            menuItem.MenuId = menuId;
            menuItem.ItemName = itemName;
            menuItem.Description = description;
            menuItem.Price = price;
            menuItem.ItemType = itemType;
            menuItem.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            return Ok(menuItem);
        }

        // Delete a menu item
        [HttpDelete("{id}")]
        public IActionResult DeleteMenuItem(int id)
        {
            var menuItem = _context.MenuItems.Find(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
