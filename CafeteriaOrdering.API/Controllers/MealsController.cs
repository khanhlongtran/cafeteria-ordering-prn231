﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeteriaOrdering.API.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public MealsController(CafeteriaOrderingDBContext context)
        {
            _context = context;
        }
        [HttpGet("GeoLocation")]
        public async Task<IActionResult> GetAllManagersandGeoLocation()
        {
            var result = await _context.Users
                .Where(u => u.Role.ToLower() == "manager")
                .Select(u => new
                {
                    user_id = u.UserId,
                    user_name = u.FullName,
                    address = _context.Addresses
                        .Where(a => a.UserId == u.UserId)
                        .Select(a => new
                        {
                            geoLocation = a.GeoLocation,
                            image = a.Image
                        })
                        .FirstOrDefault(),
                    menus = u.Menus.Select(m => new
                    {
                        menu_id = m.MenuId,
                        menu_name = m.MenuName,
                        menu_items = m.MenuItems.Select(mi => new
                        {
                            item_id = mi.ItemId,
                            item_name = mi.ItemName,
                            price = mi.Price
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(result);
        }

    }
}
