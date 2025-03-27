namespace CafeteriaOrdering.API.DTO
{
    public class UserDto
    {
        public string Name { get; set; } = null!;
        public List<string> DefaultCuisine { get; set; } = new();
        public string? GeoLocation { get; set; }
        public List<MenuDto> Menus { get; set; } = new();
    }
    public class UserOrderDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Number { get; set; }
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public decimal TotalAmount { get; set; }
        public int AddressId { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string GeoLocation { get; set; }
    }

    public class DeliveryOrderDTO
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public int DeliverUserId { get; set; }
        public string OrderName { get;set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public string PatronName { get; set; }
        public string Number { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
