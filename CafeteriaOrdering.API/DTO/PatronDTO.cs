namespace CafeteriaOrdering.API.DTO
{
    public class AddressRequest
    {
        public string AddressLine { get; set; } = null!;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public bool IsDefault { get; set; }
        public string? GeoLocation { get; set; }
    }

    public class FeedbackRequest
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class OrderItemRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
    }
}
