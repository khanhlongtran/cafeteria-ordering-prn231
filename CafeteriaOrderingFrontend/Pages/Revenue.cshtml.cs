using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeteriaOrdering.API.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace CafeteriaOrderingFrontend.Pages
{
    public class RevenueModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public RevenueModel(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public List<RevenueReport> RevenueReports { get; set; } = new();
        public List<MenuItem> TopSellingItems { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiSettings:BaseUrl"];

                // Get revenue details for the current month
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var response = await client.GetAsync($"{apiUrl}/api/Manager/GetRevenueDetails?managerId=1&filterType=month&date={firstDayOfMonth:yyyy-MM-dd}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<RevenueResponse>(content);
                    
                    RevenueReports = result.Details;
                    TotalRevenue = result.TotalRevenue;
                    TotalOrders = RevenueReports.Sum(r => r.TotalOrders);
                }

                // Get top selling items
                var topItemsResponse = await client.GetAsync($"{apiUrl}/api/Manager/ViewTopSellingItemsByMenu?menuId=1");
                if (topItemsResponse.IsSuccessStatusCode)
                {
                    var content = await topItemsResponse.Content.ReadAsStringAsync();
                    TopSellingItems = JsonSerializer.Deserialize<List<MenuItem>>(content);
                }

                return Page();
            }
            catch (Exception ex)
            {
                Message = "Error loading revenue data: Please try again later!";
                IsSuccess = false;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGenerateReportAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                
                var response = await client.PostAsync($"{apiUrl}/api/Manager/GenerateRevenueReport?managerId=1", null);
                
                if (response.IsSuccessStatusCode)
                {
                    Message = "Revenue report generated successfully";
                    IsSuccess = true;
                }
                else
                {
                    Message = "Failed to generate revenue report";
                    IsSuccess = false;
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Message = "Error generating report: " + ex.Message;
                IsSuccess = false;
                return RedirectToPage();
            }
        }
    }

    public class RevenueResponse
    {
        public decimal TotalRevenue { get; set; }
        public List<RevenueReport> Details { get; set; }
    }
} 