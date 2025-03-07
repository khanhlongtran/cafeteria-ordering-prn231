using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class RecommendedMeal
    {
        public int RecommendId { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public decimal? Score { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual MenuItem Item { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
