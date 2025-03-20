using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CafeteriaOrdering.API.Models;
using Microsoft.Extensions.Configuration;

namespace CafeteriaOrderingFrontend.Pages
{
    public class MenuModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        [BindProperty]
        public List<Menu> Menus { get; set; } = new();
        [BindProperty]
        public Menu NewMenu { get; set; }
        [BindProperty]
        public Menu EditingMenu { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public MenuModel(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            NewMenu = new Menu();
            EditingMenu = new Menu();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/menus");
                // if (response.IsSuccessStatusCode)
                // {
                //     Menus = await response.Content.ReadFromJsonAsync<List<Menu>>();
                // }
                // else
                // {
                //     Message = "Failed to load menus.";
                //     IsSuccess = false;
                // }

                // Mock data for testing
                Menus = GetMockMenus();
            }
            catch (Exception ex)
            {
                Message = "An error occurred while loading menus.";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetMenuAsync(int menuId)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/menus/{menuId}");
                // if (response.IsSuccessStatusCode)
                // {
                //     var menu = await response.Content.ReadFromJsonAsync<Menu>();
                //     return new JsonResult(menu);
                // }
                // return new JsonResult(new { error = "Menu not found" }) { StatusCode = 404 };

                // Mock data for testing
                var menu = GetMockMenus().FirstOrDefault(m => m.MenuId == menuId);
                if (menu != null)
                {
                    return new JsonResult(menu);
                }
                return new JsonResult(new { error = "Menu not found" }) { StatusCode = 404 };
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "An error occurred" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnGetMenuItemsAsync(int menuId)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/menus/{menuId}/items");
                // if (response.IsSuccessStatusCode)
                // {
                //     var items = await response.Content.ReadFromJsonAsync<List<MenuItem>>();
                //     return new JsonResult(items);
                // }
                // return new JsonResult(new { error = "Menu items not found" }) { StatusCode = 404 };

                // Mock data for testing
                var menu = GetMockMenus().FirstOrDefault(m => m.MenuId == menuId);
                if (menu != null && menu.MenuItems != null)
                {
                    Response.Headers.Add("Content-Type", "application/json");
                    return new JsonResult(new { 
                        success = true, 
                        items = menu.MenuItems 
                    });
                }
                Response.Headers.Add("Content-Type", "application/json");
                return new JsonResult(new { 
                    success = false, 
                    error = "Menu items not found" 
                }) 
                { 
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                Response.Headers.Add("Content-Type", "application/json");
                return new JsonResult(new { 
                    success = false, 
                    error = "An error occurred while loading menu items" 
                }) 
                { 
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> OnGetMenuItemAsync(int itemId)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/menu-items/{itemId}");
                // if (response.IsSuccessStatusCode)
                // {
                //     var item = await response.Content.ReadFromJsonAsync<MenuItem>();
                //     return new JsonResult(item);
                // }
                // return new JsonResult(new { error = "Menu item not found" }) { StatusCode = 404 };

                // Mock data for testing
                var menu = GetMockMenus().FirstOrDefault(m => m.MenuItems.Any(i => i.ItemId == itemId));
                if (menu != null)
                {
                    var item = menu.MenuItems.FirstOrDefault(i => i.ItemId == itemId);
                    if (item != null)
                    {
                        return new JsonResult(item);
                    }
                }
                return new JsonResult(new { error = "Menu item not found" }) { StatusCode = 404 };
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "An error occurred" }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostSaveMenuAsync([FromForm] int menuId, [FromForm] string menuName, 
            [FromForm] string description, [FromForm] bool isStatus)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var menu = new Menu
                // {
                //     MenuId = menuId,
                //     MenuName = menuName,
                //     Description = description,
                //     IsStatus = isStatus
                // };
                // HttpResponseMessage response;
                // if (menuId == 0)
                // {
                //     response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/menus", menu);
                // }
                // else
                // {
                //     response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/menus/{menuId}", menu);
                // }
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = menuId == 0 ? "Menu created successfully." : "Menu updated successfully.";
                //     IsSuccess = true;
                // }
                // else
                // {
                //     Message = "Failed to save menu.";
                //     IsSuccess = false;
                // }

                // Mock success response
                Message = menuId == 0 ? "Menu created successfully." : "Menu updated successfully.";
                IsSuccess = true;
            }
            catch (Exception)
            {
                Message = "An error occurred while saving the menu.";
                IsSuccess = false;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSaveMenuItemAsync([FromForm] int itemId, [FromForm] int menuId,
            [FromForm] string itemName, [FromForm] string description, [FromForm] decimal price,
            [FromForm] string itemType, [FromForm] bool isStatus)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var menuItem = new MenuItem
                // {
                //     ItemId = itemId,
                //     MenuId = menuId,
                //     ItemName = itemName,
                //     Description = description,
                //     Price = price,
                //     ItemType = itemType,
                //     IsStatus = isStatus
                // };
                // HttpResponseMessage response;
                // if (itemId == 0)
                // {
                //     response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/menu-items", menuItem);
                // }
                // else
                // {
                //     response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/menu-items/{itemId}", menuItem);
                // }
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = itemId == 0 ? "Menu item created successfully." : "Menu item updated successfully.";
                //     IsSuccess = true;
                // }
                // else
                // {
                //     Message = "Failed to save menu item.";
                //     IsSuccess = false;
                // }

                // Mock success response
                Message = itemId == 0 ? "Menu item created successfully." : "Menu item updated successfully.";
                IsSuccess = true;
            }
            catch (Exception)
            {
                Message = "An error occurred while saving the menu item.";
                IsSuccess = false;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMenuAsync([FromForm] int menuId)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/menus/{menuId}");
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = "Menu deleted successfully.";
                //     IsSuccess = true;
                // }
                // else
                // {
                //     Message = "Failed to delete menu.";
                //     IsSuccess = false;
                // }

                // Mock success response
                Message = "Menu deleted successfully.";
                IsSuccess = true;
            }
            catch (Exception)
            {
                Message = "An error occurred while deleting the menu.";
                IsSuccess = false;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMenuItemAsync([FromForm] int itemId)
        {
            try
            {
                // TODO: Replace with actual API call when ready
                // var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/menu-items/{itemId}");
                // if (response.IsSuccessStatusCode)
                // {
                //     Message = "Menu item deleted successfully.";
                //     IsSuccess = true;
                // }
                // else
                // {
                //     Message = "Failed to delete menu item.";
                //     IsSuccess = false;
                // }

                // Mock success response
                Message = "Menu item deleted successfully.";
                IsSuccess = true;
            }
            catch (Exception)
            {
                Message = "An error occurred while deleting the menu item.";
                IsSuccess = false;
            }

            return RedirectToPage();
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

        // Mock data method - REMOVE THIS WHEN READY TO USE API
        private List<Menu> GetMockMenus()
        {
            return new List<Menu>
            {
                new Menu
                {
                    MenuId = 1,
                    MenuName = "Breakfast Menu",
                    Description = "Start your day with our delicious breakfast options",
                    IsStatus = true,
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            ItemId = 1,
                            MenuId = 1,
                            ItemName = "Classic Breakfast Set",
                            Description = "Two eggs, bacon, toast, and coffee",
                            Price = 12.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 2,
                            MenuId = 1,
                            ItemName = "Pancake Stack",
                            Description = "Fluffy pancakes with maple syrup and butter",
                            Price = 9.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 3,
                            MenuId = 1,
                            ItemName = "Fresh Orange Juice",
                            Description = "100% pure squeezed orange juice",
                            Price = 3.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 13,
                            MenuId = 1,
                            ItemName = "Eggs Benedict",
                            Description = "Poached eggs on English muffin with hollandaise sauce",
                            Price = 14.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 14,
                            MenuId = 1,
                            ItemName = "Breakfast Burrito",
                            Description = "Scrambled eggs, cheese, and sausage wrapped in a tortilla",
                            Price = 11.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 15,
                            MenuId = 1,
                            ItemName = "Greek Yogurt Parfait",
                            Description = "Layered with granola and fresh berries",
                            Price = 7.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        }
                    }
                },
                new Menu
                {
                    MenuId = 2,
                    MenuName = "Lunch Specials",
                    Description = "Daily lunch specials and favorites",
                    IsStatus = true,
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            ItemId = 4,
                            MenuId = 2,
                            ItemName = "Grilled Chicken Salad",
                            Description = "Fresh mixed greens with grilled chicken breast",
                            Price = 14.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 5,
                            MenuId = 2,
                            ItemName = "Caesar Salad",
                            Description = "Classic Caesar salad with croutons and parmesan",
                            Price = 12.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 6,
                            MenuId = 2,
                            ItemName = "French Fries",
                            Description = "Crispy golden fries with sea salt",
                            Price = 4.99m,
                            ItemType = "Side Dish",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 16,
                            MenuId = 2,
                            ItemName = "Club Sandwich",
                            Description = "Triple-decker with turkey, bacon, lettuce, and tomato",
                            Price = 13.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 17,
                            MenuId = 2,
                            ItemName = "Soup of the Day",
                            Description = "Chef's daily special soup",
                            Price = 6.99m,
                            ItemType = "Appetizer",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 18,
                            MenuId = 2,
                            ItemName = "Onion Rings",
                            Description = "Crispy battered onion rings with dipping sauce",
                            Price = 5.99m,
                            ItemType = "Side Dish",
                            IsStatus = true
                        }
                    }
                },
                new Menu
                {
                    MenuId = 3,
                    MenuName = "Dinner Menu",
                    Description = "Evening dining options and specialties",
                    IsStatus = true,
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            ItemId = 7,
                            MenuId = 3,
                            ItemName = "Grilled Salmon",
                            Description = "Fresh salmon with seasonal vegetables",
                            Price = 24.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 8,
                            MenuId = 3,
                            ItemName = "Beef Tenderloin",
                            Description = "8oz tenderloin with red wine sauce",
                            Price = 29.99m,
                            ItemType = "Main Course",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 9,
                            MenuId = 3,
                            ItemName = "Chocolate Lava Cake",
                            Description = "Warm chocolate cake with vanilla ice cream",
                            Price = 8.99m,
                            ItemType = "Dessert",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 19,
                            MenuId = 3,
                            ItemName = "Bruschetta",
                            Description = "Toasted bread with tomatoes, garlic, and basil",
                            Price = 7.99m,
                            ItemType = "Appetizer",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 20,
                            MenuId = 3,
                            ItemName = "Mashed Potatoes",
                            Description = "Creamy mashed potatoes with butter",
                            Price = 5.99m,
                            ItemType = "Side Dish",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 21,
                            MenuId = 3,
                            ItemName = "Tiramisu",
                            Description = "Classic Italian dessert with coffee and mascarpone",
                            Price = 7.99m,
                            ItemType = "Dessert",
                            IsStatus = true
                        }
                    }
                },
                new Menu
                {
                    MenuId = 4,
                    MenuName = "Beverages",
                    Description = "Refreshing drinks and specialty coffees",
                    IsStatus = true,
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem
                        {
                            ItemId = 10,
                            MenuId = 4,
                            ItemName = "Espresso",
                            Description = "Double shot of premium espresso",
                            Price = 3.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 11,
                            MenuId = 4,
                            ItemName = "Green Tea Latte",
                            Description = "Matcha green tea with steamed milk",
                            Price = 4.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 12,
                            MenuId = 4,
                            ItemName = "Smoothie Bowl",
                            Description = "Mixed berry smoothie with granola topping",
                            Price = 8.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 22,
                            MenuId = 4,
                            ItemName = "Caramel Macchiato",
                            Description = "Vanilla-flavored drink marked with espresso",
                            Price = 5.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 23,
                            MenuId = 4,
                            ItemName = "Iced Tea",
                            Description = "Freshly brewed iced tea with lemon",
                            Price = 3.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        },
                        new MenuItem
                        {
                            ItemId = 24,
                            MenuId = 4,
                            ItemName = "Hot Chocolate",
                            Description = "Rich and creamy hot chocolate with whipped cream",
                            Price = 4.99m,
                            ItemType = "Beverage",
                            IsStatus = true
                        }
                    }
                }
            };
        }
    }
}