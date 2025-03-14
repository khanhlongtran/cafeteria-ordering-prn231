using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class MenuItem
    {
        public MenuItem()
        {
            OrderItems = new HashSet<OrderItem>();
            RecommendedMeals = new HashSet<RecommendedMeal>();
        }

        public int ItemId { get; set; }
        public int MenuId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ItemType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CountItemsSold { get; set; }
        public bool? IsStatus { get; set; }

        public virtual Menu Menu { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<RecommendedMeal> RecommendedMeals { get; set; }
    }
}
