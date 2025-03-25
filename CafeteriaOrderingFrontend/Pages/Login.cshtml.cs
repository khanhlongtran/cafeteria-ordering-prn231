using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using CafeteriaOrdering.API.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CafeteriaOrderingFrontend.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginRequestDTO LoginRequest { get; set; } = new();

        public string ErrorMessage { get; set; }

        public LoginModel(HttpClient httpClient, IConfiguration configuration, ILogger<LoginModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5110";
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // If user is already logged in, redirect to home page
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("=== Starting Login API Call ===");
                var url = $"{_apiBaseUrl}/api/Accounts/login";
                
                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: POST");
                _logger.LogInformation("Request Data: Email={Email}", LoginRequest.Email);
                
                var response = await _httpClient.PostAsJsonAsync(url, LoginRequest);
                var content = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Login API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(content);
                    
                    // Store user information in session
                    HttpContext.Session.SetString("Token", loginResponse.Token);
                    HttpContext.Session.SetString("Role", loginResponse.Role);
                    HttpContext.Session.SetString("UserId", loginResponse.AccountId);

                    // Redirect based on role
                    return RedirectToPage("/Index");
                    // return loginResponse.Role switch
                    // {
                    //     "MANAGER" => RedirectToPage("/Menu"),
                    //     "DELIVER" => RedirectToPage("/Delivery"),
                    //     _ => RedirectToPage("/Index")
                    // };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<dynamic>(content);
                    ErrorMessage = errorResponse?.GetProperty("Message").GetString() ?? "Login failed";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }
} 