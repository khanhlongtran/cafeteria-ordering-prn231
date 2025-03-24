using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CafeteriaOrdering.API.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;

namespace CafeteriaOrderingFrontend.Pages
{
    public class MenuModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly ILogger<MenuModel> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        [BindProperty]
        public List<Menu> Menus { get; set; } = new();
        [BindProperty]
        public Menu NewMenu { get; set; }
        [BindProperty]
        public Menu EditingMenu { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public MenuModel(HttpClient httpClient, IConfiguration configuration, ILogger<MenuModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5110";
            _logger = logger;
            NewMenu = new Menu();
            EditingMenu = new Menu();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("=== Starting Get Menus API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/ViewMenu";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: GET");
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Get Menus API Call ===");

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
                    
                    // Deserialize the $values array into List<Menu>
                    var menus = JsonSerializer.Deserialize<List<Menu>>(values.GetRawText(), options);
                    if (menus != null)
                    {
                        Menus = menus;
                        return Page();
                    }
                    else
                    {
                        Message = "No menus found";
                        IsSuccess = false;
                        return Page();
                    }
                }
                else
                {
                    Message = "Failed to load menus";
                    IsSuccess = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menus");
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
                return Page();
            }
        }

        public async Task<IActionResult> OnGetGetMenuAsync(int menuId)
        {
            try
            {
                _logger.LogInformation("=== Starting Get Menu API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/ViewMenu";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: GET");
                _logger.LogInformation("Request Data: MenuId={MenuId}", menuId);
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Get Menu API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    var menus = JsonSerializer.Deserialize<List<Menu>>(content, _jsonOptions);
                    var menu = menus.FirstOrDefault(m => m.MenuId == menuId);
                    if (menu != null)
                    {
                        return new JsonResult(menu, _jsonOptions);
                    }
                }
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu {MenuId}", menuId);
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> OnGetGetMenuItemsAsync(int menuId)
        {
            try
            {
                _logger.LogInformation("=== Starting Get Menu Items API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/ViewMenuItems/{menuId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: GET");
                _logger.LogInformation("Request Data: MenuId={MenuId}", menuId);

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Get Menu Items API Call ===");

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
                    
                    // Deserialize the $values array into List<MenuItem>
                    var items = JsonSerializer.Deserialize<List<MenuItem>>(values.GetRawText(), options);
                    return new JsonResult(items, options);
                }
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu items for menu {MenuId}", menuId);
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> OnGetGetMenuItemAsync(int itemId)
        {
            try
            {
                _logger.LogInformation("=== Starting Get Menu Item API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/ViewMenuItem/{itemId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: GET");
                _logger.LogInformation("Request Data: ItemId={ItemId}", itemId);
                
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Get Menu Item API Call ===");

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
                    
                    // If the response is a single item, it won't have $values
                    if (root.TryGetProperty("$values", out var values))
                    {
                        // If it's an array, get the first item
                        var items = JsonSerializer.Deserialize<List<MenuItem>>(values.GetRawText(), options);
                        var item = items?.FirstOrDefault();
                        return new JsonResult(item, options);
                    }
                    else
                    {
                        // If it's a single item, deserialize directly
                        var item = JsonSerializer.Deserialize<MenuItem>(content, options);
                        return new JsonResult(item, options);
                    }
                }
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu item {ItemId}", itemId);
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> OnPostSaveMenuAsync([FromForm] int menuId, [FromForm] string menuName, [FromForm] string description, [FromForm] bool isStatus)
        {
            try
            {
                _logger.LogInformation("=== Starting Save Menu API Call ===");
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(menuId.ToString()), "MenuId");
                formData.Add(new StringContent(menuName), "MenuName");
                formData.Add(new StringContent(description), "Description");
                formData.Add(new StringContent(isStatus.ToString().ToLower()), "IsStatus");
                formData.Add(new StringContent("1"), "ManagerId");

                var url = menuId == 0 
                    ? $"{_apiBaseUrl}/api/Manager/CreateMenu"
                    : $"{_apiBaseUrl}/api/Manager/UpdateMenu/{menuId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: {Method}", menuId == 0 ? "POST" : "PUT");
                _logger.LogInformation("Request Data: MenuId={MenuId}, MenuName={MenuName}, Description={Description}, IsStatus={IsStatus}, ManagerId=1", 
                    menuId, menuName, description, isStatus);

                HttpResponseMessage response;
                if (menuId == 0)
                {
                    response = await _httpClient.PostAsync(url, formData);
                }
                else
                {
                    response = await _httpClient.PutAsync(url, formData);
                }

                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Save Menu API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    var successMessage = menuId == 0 ? "Menu created successfully" : "Menu updated successfully";
                    return RedirectToPage(new { message = Uri.EscapeDataString(successMessage) });
                }
                else
                {
                    Message = menuId == 0 ? "Failed to create menu" : "Failed to update menu";
                    IsSuccess = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving menu");
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSaveMenuItemAsync([FromForm] int itemId, [FromForm] int menuId, [FromForm] string itemName, [FromForm] string description, [FromForm] decimal price, [FromForm] string itemType, [FromForm] bool isStatus)
        {
            try
            {
                _logger.LogInformation("=== Starting Save Menu Item API Call ===");
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(menuId.ToString()), "MenuId");
                formData.Add(new StringContent(itemName), "ItemName");
                formData.Add(new StringContent(description), "Description");
                formData.Add(new StringContent(price.ToString()), "Price");
                formData.Add(new StringContent(itemType), "ItemType");
                formData.Add(new StringContent(isStatus.ToString()), "IsStatus");

                var url = itemId == 0 
                    ? $"{_apiBaseUrl}/api/Manager/CreateMenuItems"
                    : $"{_apiBaseUrl}/api/Manager/UpdateMenuItems/{itemId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: {Method}", itemId == 0 ? "POST" : "PUT");
                _logger.LogInformation("Request Data: ItemId={ItemId}, MenuId={MenuId}, ItemName={ItemName}, Description={Description}, Price={Price}, ItemType={ItemType}, IsStatus={IsStatus}", 
                    itemId, menuId, itemName, description, price, itemType, isStatus);

                HttpResponseMessage response;
                if (itemId == 0)
                {
                    response = await _httpClient.PostAsync(url, formData);
                }
                else
                {
                    response = await _httpClient.PutAsync(url, formData);
                }

                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Save Menu Item API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    Message = itemId == 0 ? "Menu item created successfully" : "Menu item updated successfully";
                    IsSuccess = true;
                }
                else
                {
                    Message = itemId == 0 ? "Failed to create menu item" : "Failed to update menu item";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving menu item");
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMenuAsync(int menuId)
        {
            try
            {
                _logger.LogInformation("=== Starting Delete Menu API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/DeleteMenu/{menuId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: DELETE");
                _logger.LogInformation("Request Data: MenuId={MenuId}", menuId);

                var response = await _httpClient.DeleteAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Delete Menu API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                    Message = result["message"] ?? "Menu deactivated successfully";
                    IsSuccess = true;
                }
                else
                {
                    var error = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                    Message = error["message"] ?? "Failed to deactivate menu";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu {MenuId}", menuId);
                Message = $"Error: {ex.Message}";
                IsSuccess = false;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMenuItemAsync([FromQuery] int itemId)
        {
            try
            {
                _logger.LogInformation("=== Starting Delete Menu Item API Call ===");
                var url = $"{_apiBaseUrl}/api/Manager/DeleteMenuItem/{itemId}";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: DELETE");
                _logger.LogInformation("Request Data: ItemId={ItemId}", itemId);

                var response = await _httpClient.DeleteAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Delete Menu Item API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = "Menu item deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to delete menu item" }, StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu item {ItemId}", itemId);
                return new JsonResult(new { success = false, message = ex.Message }, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> OnPostCreateMenuAsync()
        {
            try
            {
                // TODO: Replace this with actual API call when ready
                // var formData = new MultipartFormDataContent();
                // formData.Add(new StringContent(NewMenu.ManagerId.ToString()), "ManagerId");
                // formData.Add(new StringContent(NewMenu.MenuName), "MenuName");
                // formData.Add(new StringContent(NewMenu.Description ?? ""), "Description");
                // formData.Add(new StringContent((NewMenu.IsStatus ?? true).ToString()), "IsStatus");
                // var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Manager/CreateMenu", formData);
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = "Menu created successfully";
                //     IsSuccess = true;
                //     NewMenu = new Menu();
                //     await OnGetAsync();
                // }
                // else
                // {
                //     Message = "Failed to create menu";
                //     IsSuccess = false;
                // }

                // Mock success response - REMOVE THIS WHEN READY TO USE API
                Message = "Menu created successfully";
                IsSuccess = true;
                NewMenu = new Menu();
                await OnGetAsync();
            }
            catch (Exception ex)
            {
                Message = "An error occurred while creating the menu";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateMenuAsync()
        {
            try
            {
                // TODO: Replace this with actual API call when ready
                // var formData = new MultipartFormDataContent();
                // formData.Add(new StringContent(EditingMenu.ManagerId.ToString()), "ManagerId");
                // formData.Add(new StringContent(EditingMenu.MenuName), "MenuName");
                // formData.Add(new StringContent(EditingMenu.Description ?? ""), "Description");
                // formData.Add(new StringContent((EditingMenu.IsStatus ?? true).ToString()), "IsStatus");
                // var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Manager/UpdateMenu/{EditingMenu.MenuId}", formData);
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = "Menu updated successfully";
                //     IsSuccess = true;
                //     EditingMenu = new Menu();
                //     await OnGetAsync();
                // }
                // else
                // {
                //     Message = "Failed to update menu";
                //     IsSuccess = false;
                // }

                // Mock success response - REMOVE THIS WHEN READY TO USE API
                Message = "Menu updated successfully";
                IsSuccess = true;
                EditingMenu = new Menu();
                await OnGetAsync();
            }
            catch (Exception ex)
            {
                Message = "An error occurred while updating the menu";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetEditMenuAsync(int menuId)
        {
            try
            {
                // TODO: Replace this with actual API call when ready
                // var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Manager/ViewMenu/{menuId}");
                // if (response.IsSuccessStatusCode)
                // {
                //     var content = await response.Content.ReadAsStringAsync();
                //     EditingMenu = JsonSerializer.Deserialize<Menu>(content, new JsonSerializerOptions
                //     {
                //         PropertyNameCaseInsensitive = true
                //     });
                // }

                // Mock menu data for editing - REMOVE THIS WHEN READY TO USE API
                EditingMenu = Menus.FirstOrDefault(m => m.MenuId == menuId) ?? new Menu();
            }
            catch (Exception ex)
            {
                Message = "Failed to load menu details";
                IsSuccess = false;
            }

            return Page();
        }
    }
}