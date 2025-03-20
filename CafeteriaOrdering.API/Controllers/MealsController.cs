using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("managers-by-location")]
        public async Task<IActionResult> GetManagersByLocation([FromQuery] string geoLocation)
        {
            if (string.IsNullOrEmpty(geoLocation))
            {
                return BadRequest("GeoLocation is required.");
            }

            var result = await _context.Users
                .Where(u => u.Role == "manager")
                .Select(u => new
                {
                    user_id = u.UserId,
                    user_name = u.FullName,
                    cuisines = u.DefaultCuisine,
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

            // Lọc theo khoảng cách
            var filteredResult = result
                .Where(u => u.address?.geoLocation != null && IsNearby(u.address.geoLocation, geoLocation))
                .ToList();

            return Ok(filteredResult);
        }


        // Hàm kiểm tra vị trí gần (có thể dùng thư viện tính khoảng cách nếu cần)
        private bool IsNearby(string userGeoLocation, string requestGeoLocation)
        {
            // Nếu cần so sánh chính xác theo tọa độ:
            return userGeoLocation == requestGeoLocation;

            // Nếu cần so sánh theo khoảng cách, có thể dùng Haversine Formula hoặc thư viện hỗ trợ.
        }
    }
    }
