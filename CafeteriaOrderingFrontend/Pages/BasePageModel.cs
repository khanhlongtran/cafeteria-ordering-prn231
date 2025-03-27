using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeteriaOrderingFrontend.Pages
{
    public abstract class BasePageModel : PageModel
    {
        protected async Task<IActionResult> CheckAuthenticationAsync()
        {
            // Skip authentication check for Login and Logout pages
            if (this is LoginModel || this is LogoutModel)
            {
                return null;
            }

            // Check if user is logged in
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            return null;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            // Check authentication first
            var authResult = await CheckAuthenticationAsync();
            if (authResult != null)
            {
                return authResult;
            }

            return Page();
        }
    }
} 