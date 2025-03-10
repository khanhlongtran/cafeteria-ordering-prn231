using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual MenuItem Item { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
