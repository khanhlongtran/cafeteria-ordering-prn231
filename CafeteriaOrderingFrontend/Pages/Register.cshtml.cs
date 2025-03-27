using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using CafeteriaOrdering.API.DTO;
using Microsoft.Extensions.Logging;

namespace CafeteriaOrderingFrontend.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public RegisterDTO RegisterRequest { get; set; } = new();

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public RegisterModel(HttpClient httpClient, IConfiguration configuration, ILogger<RegisterModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("=== Starting Register API Call ===");
                var url = $"{_configuration["ApiSettings:BaseUrl"]}/api/Accounts/register";

                _logger.LogInformation("Request URL: {Url}", url);
                _logger.LogInformation("Request Method: POST");
                _logger.LogInformation("Request Data: Email={Email}", RegisterRequest.Email);

                RegisterRequest.Role = RegisterRequest.Role.ToUpper();

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("FullName", RegisterRequest.FullName),
                    new KeyValuePair<string, string>("Email", RegisterRequest.Email),
                    new KeyValuePair<string, string>("Password", RegisterRequest.Password),
                    new KeyValuePair<string, string>("Phone", RegisterRequest.Phone),
                    new KeyValuePair<string, string>("Role", RegisterRequest.Role)
                });

                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(RegisterRequest.FullName), "FullName");
                formData.Add(new StringContent(RegisterRequest.Email), "Email");
                formData.Add(new StringContent(RegisterRequest.Password), "Password");
                formData.Add(new StringContent(RegisterRequest.Phone), "Phone");
                formData.Add(new StringContent(RegisterRequest.Role), "Role");

                var response = await _httpClient.PostAsync(url, formData);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Content: {Content}", content);
                _logger.LogInformation("=== End Register API Call ===");

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Registration successful! You can now log in.";
                    return RedirectToPage("/Login");
                }
                else
                {
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<dynamic>(content);
                    ErrorMessage = errorResponse?.GetProperty("Message").GetString() ?? "Registration failed.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ErrorMessage = "An error occurred during registration. Please try again.";
                return Page();
            }
        }
    }
}
