using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using CafeteriaOrderingFrontend.Models;

namespace CafeteriaOrderingFrontend.Pages
{
    public class MenuModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7001/api";

        [BindProperty]
        public List<Menu> Menus { get; set; }
        [BindProperty]
        public Menu NewMenu { get; set; }
        [BindProperty]
        public Menu EditingMenu { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public MenuModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Menus = new List<Menu>();
            NewMenu = new Menu();
            EditingMenu = new Menu();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Manager/ViewMenu");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Menus = JsonSerializer.Deserialize<List<Menu>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Handle null IsStatus values
                    if (Menus != null)
                    {
                        foreach (var menu in Menus)
                        {
                            menu.IsStatus = menu.IsStatus ?? false;
                        }
                    }
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
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(NewMenu.ManagerId.ToString()), "ManagerId");
                formData.Add(new StringContent(NewMenu.MenuName), "MenuName");
                formData.Add(new StringContent(NewMenu.Description ?? ""), "Description");
                formData.Add(new StringContent((NewMenu.IsStatus ?? true).ToString()), "IsStatus");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Manager/CreateMenu", formData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Menu created successfully";
                    IsSuccess = true;
                    NewMenu = new Menu();
                    await OnGetAsync();
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
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(EditingMenu.ManagerId.ToString()), "manager_id");
                formData.Add(new StringContent(EditingMenu.MenuName), "menu_name");
                formData.Add(new StringContent(EditingMenu.Description ?? ""), "description");
                formData.Add(new StringContent(EditingMenu.IsStatus.ToString()), "is_status");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Manager/UpdateMenu/{EditingMenu.MenuId}", formData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Menu updated successfully";
                    IsSuccess = true;
                    EditingMenu = new Menu();
                    await OnGetAsync();
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
                    await OnGetAsync();
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
                    EditingMenu = menu;
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