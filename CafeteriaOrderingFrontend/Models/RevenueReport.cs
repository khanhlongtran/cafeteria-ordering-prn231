using System;
using System.Collections.Generic;

namespace CafeteriaOrderingFrontend.Models
{
    public partial class RevenueReport
    {
        public int ReportId { get; set; }
        public int ManagerId { get; set; }
        public DateTime ReportDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime GeneratedAt { get; set; }

        public virtual User Manager { get; set; } = null!;
    }
}
