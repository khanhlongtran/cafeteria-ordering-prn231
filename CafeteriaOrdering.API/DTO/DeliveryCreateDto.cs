namespace CafeteriaOrdering.API.DTO
{
    public class DeliveryCreateDto
    {
        public int OrderId { get; set; }
        public int DeliverUserId { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
