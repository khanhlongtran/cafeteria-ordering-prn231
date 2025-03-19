using System;
using System.Collections.Generic;

namespace CafeteriaOrderingFrontend.Models
{
    public partial class AccountActivity
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string? ActivityType { get; set; }
        public DateTime ActivityTime { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
