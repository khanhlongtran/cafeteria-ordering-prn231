using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.DTO;

namespace CafeteriaOrderingFrontend.Pages
{
    public class MenuModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7001/api";

        public List<Menu> Menus { get; set; }
        public MenuDto NewMenu { get; set; }
        public MenuDto EditingMenu { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public MenuModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Menus = new List<Menu>();
            NewMenu = new MenuDto();
            EditingMenu = new MenuDto();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Manager/ViewMenu");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Menus = JsonSerializer.Deserialize<List<Menu>>(content);
                }
                else
                {
                    Message = "Failed to load menu data";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred while loading the menu";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateMenuAsync()
        {
            try
            {
                // Create form data
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(NewMenu.ManagerId.ToString()), "ManagerId");
                formData.Add(new StringContent(NewMenu.MenuName), "MenuName");
                formData.Add(new StringContent(NewMenu.Description ?? ""), "Description");
                formData.Add(new StringContent(NewMenu.IsStatus.ToString()), "IsStatus");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Manager/CreateMenu", formData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Menu created successfully";
                    IsSuccess = true;
                    NewMenu = new MenuDto();
                    await OnGetAsync(); // Refresh the menu list
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Failed to create menu: {errorContent}";
                    IsSuccess = false;
                }
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
                // Create form data
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(EditingMenu.ManagerId.ToString()), "ManagerId");
                formData.Add(new StringContent(EditingMenu.MenuName), "MenuName");
                formData.Add(new StringContent(EditingMenu.Description ?? ""), "Description");
                formData.Add(new StringContent(EditingMenu.IsStatus.ToString()), "IsStatus");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Manager/UpdateMenu/{EditingMenu.MenuId}", formData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Menu updated successfully";
                    IsSuccess = true;
                    EditingMenu = new MenuDto();
                    await OnGetAsync(); // Refresh the menu list
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Failed to update menu: {errorContent}";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred while updating the menu";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteMenuAsync(int menuId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Manager/DeleteMenu/{menuId}");
                if (response.IsSuccessStatusCode)
                {
                    Message = "Menu deleted successfully";
                    IsSuccess = true;
                    await OnGetAsync(); // Refresh the menu list
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Failed to delete menu: {errorContent}";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred while deleting the menu";
                IsSuccess = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetEditMenuAsync(int menuId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Manager/ViewMenu/{menuId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var menu = JsonSerializer.Deserialize<Menu>(content);
                    
                    // Map Menu to MenuDto
                    EditingMenu = new MenuDto
                    {
                        MenuId = menu.MenuId,
                        ManagerId = menu.ManagerId,
                        MenuName = menu.MenuName,
                        Description = menu.Description,
                        IsStatus = menu.IsStatus
                    };
                }
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