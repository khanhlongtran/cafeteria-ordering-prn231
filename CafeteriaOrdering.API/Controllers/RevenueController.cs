using Microsoft.AspNetCore.Mvc;
using CafeteriaOrdering.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ManagerAPI.Controllers
{
    [Authorize("MANAGER")]
    [Route("api/Manager")]
    [ApiController]  // Thiếu [ApiController], thêm vào để tránh lỗi Model Binding
    public class RevenueController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _contexts;

        public RevenueController(CafeteriaOrderingDBContext context)
        {
            _contexts = context;
        }

        // Tạo báo cáo doanh thu
        [HttpPost("GenerateRevenueReport")]
        public async Task<IActionResult> GenerateRevenueReport([FromForm] int managerId)
        {
            if (_contexts.RevenueReports == null) // Kiểm tra null tránh lỗi NullReferenceException
                return BadRequest(new { message = "RevenueReports table not found" });

            var totalOrders = await _contexts.RevenueReports.CountAsync();
            var totalRevenue = await _contexts.RevenueReports.SumAsync(r => r.TotalRevenue);

            var report = new RevenueReport
            {
                ManagerId = managerId,
                ReportDate = DateTime.UtcNow,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                GeneratedAt = DateTime.UtcNow
            };

            _contexts.RevenueReports.Add(report);
            await _contexts.SaveChangesAsync();

            return CreatedAtAction(nameof(GenerateRevenueReport), new { id = report.ReportId }, report);
        }

        // Lọc doanh thu theo thời gian
        [HttpGet("FilterRevenueByTime")]
        public async Task<IActionResult> FilterRevenueByTime([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (_contexts.RevenueReports == null)
                return BadRequest(new { message = "RevenueReports table not found" });

            var reports = await _contexts.RevenueReports
                .Where(r => r.ReportDate >= startDate && r.ReportDate <= endDate)
                .ToListAsync();

            return Ok(reports);
        }

        // Lọc doanh thu theo loại sản phẩm
        //[HttpGet("FilterByProductType")]
        //public async Task<IActionResult> FilterByProductType([FromQuery] string productType)
        //{
        //    if (_contexts.Orders == null)
        //        return BadRequest(new { message = "Orders table not found" });

        //    var totalRevenue = await _contexts.Orders
        //        .Where(o => o.ProductType == productType)
        //        .SumAsync(o => o.TotalPrice);

        //    return Ok(new { ProductType = productType, TotalRevenue = totalRevenue });
        //}


        // lấy ra sản phẩm có số lượng bán nhiều nhất
        [HttpGet("ViewTopSellingItem")]
        public async Task<IActionResult> ViewTopSellingItem([FromQuery] int menuId)
        {
            if (_contexts.MenuItems == null)
                return BadRequest(new { message = "MenuItems table not found" });

            var topItem = await _contexts.MenuItems
                .Where(m => m.MenuId == menuId)
                .OrderByDescending(m => m.CountItemsSold)
                .FirstOrDefaultAsync();

            if (topItem == null)
                return NotFound(new { message = "No items found for this menu" });

            return Ok(topItem);
        }


        // lấy ra tất cả sản phẩm và sắp xếp theo số lượng bán giảm dần của quán
        [HttpGet("ViewTopSellingItemsByMenu")]
        public async Task<IActionResult> ViewTopSellingItemsByMenu([FromQuery] int menuId)
        {
            if (_contexts.MenuItems == null)
                return BadRequest(new { message = "MenuItems table not found" });

            var items = await _contexts.MenuItems
                .Where(m => m.MenuId == menuId)
                .OrderByDescending(m => m.CountItemsSold)
                .ToListAsync();

            if (!items.Any())
                return NotFound(new { message = "No items found for this menu" });

            return Ok(items);
        }

        [HttpGet("ViewTopSellingItemsByManager")]
        public async Task<IActionResult> ViewTopSellingItemsByManager([FromQuery] int managerId)
        {
            if (_contexts.MenuItems == null)
                return BadRequest(new { message = "MenuItems table not found" });

            var items = await _contexts.MenuItems
                .Where(m => m.Menu.ManagerId == managerId)
                .OrderByDescending(m => m.CountItemsSold)
                .ToListAsync();

            if (!items.Any())
                return NotFound(new { message = "No items found for this menu" });

            return Ok(items);
        }


        // ngày thì nhập: day và nhập đúng ngày
        // tháng thì nhập : month và nhập ngày đầu tháng (2025-03-01)
        // năm thì nhập : year và nhập ngày đầu năm (2025-01-01)

        [HttpGet("GetRevenueDetails")]
        public async Task<IActionResult> GetRevenueDetails([FromQuery] int managerId, [FromQuery] string filterType, [FromQuery] DateTime date)
        {
            if (_contexts.RevenueReports == null)
                return BadRequest(new { message = "RevenueReports table not found" });

            IQueryable<RevenueReport> query = _contexts.RevenueReports.Where(r => r.ManagerId == managerId);

            switch (filterType.ToLower())
            {
                case "day":
                    query = query.Where(r => r.ReportDate.Date == date.Date);
                    break;
                case "month":
                    query = query.Where(r => r.ReportDate.Year == date.Year && r.ReportDate.Month == date.Month);
                    break;
                case "year":
                    query = query.Where(r => r.ReportDate.Year == date.Year);
                    break;
                default:
                    return BadRequest(new { message = "Invalid filterType. Use 'day', 'month', or 'year'." });
            }

            var reports = await query.ToListAsync();

            var totalOrders = reports.Sum(r => r.TotalOrders);
            var totalRevenue = reports.Sum(r => r.TotalRevenue);

            return Ok(new
            {
                //ManagerId = managerId,
                //FilterType = filterType,
                //Date = date,
                //TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                Details = reports
            });
        }




    }
}
