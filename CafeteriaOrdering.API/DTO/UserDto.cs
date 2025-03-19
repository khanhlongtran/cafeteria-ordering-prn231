namespace CafeteriaOrdering.API.DTO
{
    public class UserDto
    {
        public string Name { get; set; } = null!;
        public List<string> DefaultCuisine { get; set; } = new();
        public string? GeoLocation { get; set; }
        public List<MenuDto> Menus { get; set; } = new();
    }
}
