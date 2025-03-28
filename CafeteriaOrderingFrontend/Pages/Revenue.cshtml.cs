using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeteriaOrdering.API.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CafeteriaOrderingFrontend.Pages
{
    public class RevenueModel : BasePageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RevenueModel> _logger;

        public RevenueModel(IHttpClientFactory clientFactory, IConfiguration configuration, HttpClient httpClient, ILogger<RevenueModel> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<RevenueReport> RevenueReports { get; set; } = new();
        public List<MenuItem> TopSellingItems { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<Order> Orders { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            // Check authentication first
            var authResult = await CheckAuthenticationAsync();
            if (authResult != null)
            {
                return authResult;
            }

            try
            {
                var _token = HttpContext.Session.GetString("Token");
                var _userId = HttpContext.Session.GetString("UserId");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

                // var token = HttpContext.Session.GetString("Token");
                // _httpClient.DefaultRequestHeaders.Clear();
                // _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/ViewOrder";
                // _logger.LogInformation("Requesting orders from: {Url}", apiUrl);

                // var response = await _httpClient.GetAsync(apiUrl);
                // var content = await response.Content.ReadAsStringAsync();
                // _logger.LogInformation("Response status: {StatusCode}, Content: {Content}", response.StatusCode, content);

                // if (response.IsSuccessStatusCode)
                // {
                //     var options = new JsonSerializerOptions
                //     {
                //         PropertyNameCaseInsensitive = true,
                //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //         ReferenceHandler = ReferenceHandler.Preserve,
                //         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                //     };

                //     // Parse the root object to get the $values array
                //     using var doc = JsonDocument.Parse(content);
                //     var root = doc.RootElement;
                //     var values = root.GetProperty("$values");
                    
                //     Orders = JsonSerializer.Deserialize<List<Order>>(values.GetRawText(), options) ?? new List<Order>();
                //     _logger.LogInformation("Successfully deserialized {Count} orders", Orders.Count);
                // }
                // else
                // {
                //     ErrorMessage = "Failed to load orders. Please try again later.";
                //     _logger.LogError("Failed to load orders. Status code: {StatusCode}, Content: {Content}", response.StatusCode, content);
                // }

                // // Get revenue details for the current month
                // var currentDate = DateTime.Now;
                // var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                // var client = _clientFactory.CreateClient();
                // var responseDetails = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/GetRevenueDetails?managerId=1&filterType=month&date={firstDayOfMonth:yyyy-MM-dd}");
                
                // if (responseDetails.IsSuccessStatusCode)
                // {
                //     var contentDetails = await responseDetails.Content.ReadAsStringAsync();
                //     var result = JsonSerializer.Deserialize<RevenueResponse>(contentDetails);
                    
                //     // get last generated report
                //     var lastReportResponse = 

                //     RevenueReports = result.Details;
                //     TotalRevenue = result.TotalRevenue;
                //     TotalOrders = RevenueReports.Sum(r => r.TotalOrders);
                //     GeneratedAt = RevenueReports.FirstOrDefault()?.GeneratedAt;
                // }

                // // Get top selling items
                // apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/ViewTopSellingItemsByManager?managerId={_userId}";
                // var topItemsResponse = await _httpClient.GetAsync(apiUrl);
                // if (topItemsResponse.IsSuccessStatusCode)
                // {
                //     var topItemsContent = await topItemsResponse.Content.ReadAsStringAsync();
                //     TopSellingItems = JsonSerializer.Deserialize<List<MenuItem>>(topItemsContent, new JsonSerializerOptions
                //     {
                //         PropertyNameCaseInsensitive = true
                //     });
                // }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading revenue data.";
                _logger.LogError(ex, "Error loading revenue data");
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

        public async Task<IActionResult> OnPostFilterRevenueAsync(int month, int year)
        {
            try
            {
                var _token = HttpContext.Session.GetString("Token");
                var _userId = HttpContext.Session.GetString("UserId");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/FilterRevenueByTime?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    RevenueReports = JsonSerializer.Deserialize<List<RevenueReport>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var latestReport = RevenueReports.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
                    GeneratedAt = latestReport?.GeneratedAt;
                    TotalRevenue = RevenueReports.Sum(r => r.TotalRevenue);
                    TotalOrders = RevenueReports.Sum(r => r.TotalOrders);
                    IsSuccess = true;
                    Message = "Revenue data filtered successfully.";
                }
                else
                {
                    IsSuccess = false;
                    Message = "Failed to filter revenue data.";
                }

                // Get top selling items
                apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/ViewTopSellingItemsByManager?managerId={_userId}";
                var topItemsResponse = await _httpClient.GetAsync(apiUrl);
                if (topItemsResponse.IsSuccessStatusCode)
                {
                    var topItemsContent = await topItemsResponse.Content.ReadAsStringAsync();
                    TopSellingItems = JsonSerializer.Deserialize<List<MenuItem>>(topItemsContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Error filtering revenue data: " + ex.Message;
            }

            return Page();
        }
    }

    public class RevenueResponse
    {
        public decimal TotalRevenue { get; set; }
        public List<RevenueReport> Details { get; set; }
    }
}