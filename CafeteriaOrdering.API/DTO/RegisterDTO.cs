using System.ComponentModel.DataAnnotations;

namespace CafeteriaOrdering.API.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(PATRON|MANAGER|DELIVER)$", ErrorMessage = "Role must be PATRON, MANAGER, or DELIVER.")]
        public string Role { get; set; } = null!;
    }
}
