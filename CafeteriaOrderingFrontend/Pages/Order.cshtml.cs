using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.DTO;
using System.Text.Json.Serialization;

namespace CafeteriaOrderingFrontend.Pages
{
    public class OrderModel : BasePageModel
    {
        private readonly ILogger<OrderModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string? userId;

        public List<Order> Orders { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public OrderModel(ILogger<OrderModel> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

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
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                var userId = HttpContext.Session.GetString("UserId");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                string apiUrl;
                switch (role)
                {
                    case "MANAGER":
                        apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/ViewOrder";
                        break;
                    case "DELIVER":
                        apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Deliver/ViewOrder";
                        break;
                    case "PATRON":
                        apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Patron/ViewOrder";
                        break;
                    default:
                        Message = "Invalid user role";
                        IsSuccess = false;
                        return Page();
                }

                _logger.LogInformation("Requesting orders from: {Url}", apiUrl);

                var response = await _httpClient.GetAsync(apiUrl);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response status: {StatusCode}, Content: {Content}", response.StatusCode, content);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        ReferenceHandler = ReferenceHandler.Preserve,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    // Parse the root object to get the $values array
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;
                    var values = root.GetProperty("$values");
                    
                    Orders = JsonSerializer.Deserialize<List<Order>>(values.GetRawText(), options) ?? new List<Order>();
                    _logger.LogInformation("Successfully deserialized {Count} orders", Orders.Count);
                    Message = "Orders loaded successfully";
                    IsSuccess = true;
                }
                else
                {
                    Message = "Failed to load orders. Please try again later.";
                    IsSuccess = false;
                    _logger.LogError("Failed to load orders. Status code: {StatusCode}, Content: {Content}", response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred while loading orders.";
                IsSuccess = false;
                _logger.LogError(ex, "Error loading orders");
            }

            return Page();
        }

        public async Task<IActionResult> OnGetTrackOrderAsync(int orderId)
        {
            try
            {
                var url = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/GetOrderItems/{orderId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                return new JsonResult(new { success = response.IsSuccessStatusCode, content });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                string apiUrl;
                switch (role)
                {
                    case "MANAGER":
                        apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/UpdateOrderStatus/{orderId}";
                        break;
                    case "DELIVER":
                        apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/api/Deliver/UpdateOrderStatus/{orderId}";
                        break;
                    default:
                        return new JsonResult(new { success = false, message = "Invalid user role" });
                }

                var content = new StringContent(
                    JsonSerializer.Serialize(new { status }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }
                else
                {
                    _logger.LogError("Failed to update order status. Status code: {StatusCode}, Content: {Content}", response.StatusCode, responseContent);
                    return new JsonResult(new { success = false, message = "Failed to update order status" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                return new JsonResult(new { success = false, message = "An error occurred while updating order status" });
            }
        }

        public async Task<IActionResult> OnPostSubmitFeedbackAsync([FromBody] FeedbackRequest request)
        {
            try
            {
                _logger.LogInformation("=== Starting Submit Feedback API Call ===");
                var url = $"{_configuration["ApiSettings:BaseUrl"]}/api/Patron/MyOrder/MakeAFeedback";
                
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
                var url = $"{_configuration["ApiSettings:BaseUrl"]}/api/Checkout/{paymentMethod}?orderId={orderId}";
                
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
                var url = $"{_configuration["ApiSettings:BaseUrl"]}/api/Manager/GetOrderItems/{orderId}";
                
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