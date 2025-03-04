using System;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Models
{
    public partial class User
    {
        public User()
        {
            AccountActivities = new HashSet<AccountActivity>();
            Addresses = new HashSet<Address>();
            Deliveries = new HashSet<Delivery>();
            Feedbacks = new HashSet<Feedback>();
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
            RecommendedMeals = new HashSet<RecommendedMeal>();
            RevenueReports = new HashSet<RevenueReport>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string? DefaultCuisine { get; set; }
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AccountActivity> AccountActivities { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RecommendedMeal> RecommendedMeals { get; set; }
        public virtual ICollection<RevenueReport> RevenueReports { get; set; }
    }
}
