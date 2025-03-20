using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.DTO;
using System.Text.Json.Serialization;

namespace CafeteriaOrderingFrontend.Pages
{
    public class OrderModel : PageModel
    {
        private readonly ILogger<OrderModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public List<Order> Orders { get; set; } = new();
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public OrderModel(ILogger<OrderModel> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("=== Starting Get Orders API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/GetAllOrder";
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);

                if (response.IsSuccessStatusCode)
                {
                    Orders = JsonSerializer.Deserialize<List<Order>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.Preserve
                    });
                    return Page();
                }
                else
                {
                    Message = "Failed to load orders";
                    IsSuccess = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
                return Page();
            }
        }

        public async Task<IActionResult> OnGetTrackOrderAsync(int orderId)
        {
            try
            {
                var url = $"{_apiBaseUrl}/api/Manager/GetOrderItems/{orderId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                return new JsonResult(new { success = response.IsSuccessStatusCode, content });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateOrderStatusAsync(int orderId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                _logger.LogInformation("=== Starting Update Order Status API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/{orderId}/status";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: PUT");
                _logger.LogInformation("Request Body: {Body}", JsonSerializer.Serialize(request));

                var response = await _httpClient.PutAsJsonAsync(url, request);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Update Order Status API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, content });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to update order status", content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostSubmitFeedbackAsync([FromBody] FeedbackRequest request)
        {
            try
            {
                _logger.LogInformation("=== Starting Submit Feedback API Call ===");
                var url = $"{_apiBaseUrl}/api/Patron/MyOrder/MakeAFeedback";
                
                // Set the user ID to 1 for testing
                request.UserId = 1;
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: POST");
                _logger.LogInformation("Request Data: {Data}", JsonSerializer.Serialize(request));

                var response = await _httpClient.PostAsJsonAsync(url, request);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Submit Feedback API Call ===");

                return new JsonResult(new { success = response.IsSuccessStatusCode, content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostProcessPaymentAsync(int orderId, string paymentMethod)
        {
            try
            {
                _logger.LogInformation("=== Starting Process Payment API Call ===");
                var url = $"{_apiBaseUrl}/api/Checkout/{paymentMethod}?orderId={orderId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: POST");

                var response = await _httpClient.PostAsync(url, null);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Process Payment API Call ===");

                return new JsonResult(new { success = response.IsSuccessStatusCode, content });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetGetOrderItemsAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("=== Starting Get Order Items API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/GetOrderItems/{orderId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: GET");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    // Deserialize the root object first
                    var rootObject = JsonSerializer.Deserialize<JsonDocument>(content, options);
                    var itemsArray = rootObject.RootElement.GetProperty("$values");
                    
                    if (itemsArray.GetArrayLength() == 0)
                    {
                        return new JsonResult(new { success = false, message = "No items found" });
                    }

                    var simplifiedItems = new List<object>();
                    foreach (var item in itemsArray.EnumerateArray())
                    {
                        var orderItem = item.Deserialize<OrderItem>(options);
                        simplifiedItems.Add(new
                        {
                            itemName = orderItem.Item?.ItemName ?? "Unknown Item",
                            quantity = orderItem.Quantity,
                            price = orderItem.Price,
                            total = orderItem.Price
                        });
                    }

                    return new JsonResult(new { success = true, items = simplifiedItems });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to load order items" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order items for order {OrderId}: {Message}", orderId, ex.Message);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public class UpdateStatusRequest
        {
            public string Status { get; set; }
        }
    }
} 