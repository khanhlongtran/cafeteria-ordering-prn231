using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class Menu
    {
        public Menu()
        {
            MenuItems = new HashSet<MenuItem>();
        }

        public int MenuId { get; set; }
        public int ManagerId { get; set; }
        public string? MenuName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool? IsStatus { get; set; }

        public virtual User Manager { get; set; } = null!;
        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}
