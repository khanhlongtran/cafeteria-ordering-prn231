using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class Delivery
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public int DeliverUserId { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string DeliveryStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User DeliverUser { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
