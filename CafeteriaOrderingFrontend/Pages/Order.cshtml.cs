using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeteriaOrdering.API.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace CafeteriaOrderingFrontend.Pages
{
    public class OrderModel : PageModel
    {
        [BindProperty]
        public List<Order> Orders { get; set; }
        [BindProperty]
        public Order SelectedOrder { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        private readonly List<Order> _mockOrders;
        private readonly Dictionary<int, MenuItem> _mockMenuItems;

        public OrderModel()
        {
            Orders = new List<Order>();
            SelectedOrder = new Order();

            // Initialize mock menu items
            _mockMenuItems = new Dictionary<int, MenuItem>
            {
                { 1, new MenuItem { ItemId = 1, ItemName = "Classic Burger", Description = "Juicy beef patty with fresh lettuce and tomato", Price = 8.99m, ItemType = "Main Course", IsStatus = true } },
                { 2, new MenuItem { ItemId = 2, ItemName = "Chicken Caesar Salad", Description = "Crisp romaine lettuce, grilled chicken, parmesan cheese", Price = 12.99m, ItemType = "Main Course", IsStatus = true } },
                { 3, new MenuItem { ItemId = 3, ItemName = "French Fries", Description = "Crispy golden fries with sea salt", Price = 4.99m, ItemType = "Side Dish", IsStatus = true } },
                { 4, new MenuItem { ItemId = 4, ItemName = "Coca Cola", Description = "Ice-cold Coca Cola", Price = 2.99m, ItemType = "Beverage", IsStatus = true } },
                { 5, new MenuItem { ItemId = 5, ItemName = "Pizza Margherita", Description = "Fresh tomatoes, mozzarella, and basil", Price = 24.99m, ItemType = "Main Course", IsStatus = true } },
                { 6, new MenuItem { ItemId = 6, ItemName = "Chocolate Cake", Description = "Rich chocolate cake with ganache", Price = 6.99m, ItemType = "Dessert", IsStatus = true } },
                { 7, new MenuItem { ItemId = 7, ItemName = "Ice Cream Sundae", Description = "Vanilla ice cream with hot fudge and nuts", Price = 5.99m, ItemType = "Dessert", IsStatus = true } },
                { 8, new MenuItem { ItemId = 8, ItemName = "Mineral Water", Description = "Pure spring water", Price = 1.99m, ItemType = "Beverage", IsStatus = true } }
            };

            // Initialize mock orders
            _mockOrders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    UserId = 1,
                    OrderDate = DateTime.Now.AddDays(-2),
                    Status = "Pending",
                    PaymentMethod = "Credit Card",
                    TotalAmount = 45.97m,
                    AddressId = 1,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    UpdatedAt = DateTime.Now.AddDays(-2),
                    User = new User 
                    { 
                        UserId = 1,
                        FullName = "John Doe",
                        Email = "john@example.com",
                        Phone = "123-456-7890",
                        Password = "hashed_password",
                        Role = "Customer",
                        CreatedAt = DateTime.Now.AddDays(-30),
                        UpdatedAt = DateTime.Now.AddDays(-30)
                    },
                    Address = new Address
                    {
                        AddressId = 1,
                        UserId = 1,
                        AddressLine = "123 Main St",
                        City = "New York",
                        State = "NY",
                        ZipCode = "10001",
                        IsDefault = true,
                        CreatedAt = DateTime.Now.AddDays(-30),
                        UpdatedAt = DateTime.Now.AddDays(-30)
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { OrderItemId = 1, OrderId = 1, ItemId = 1, Quantity = 2, Price = 8.99m },
                        new OrderItem { OrderItemId = 2, OrderId = 1, ItemId = 3, Quantity = 1, Price = 4.99m },
                        new OrderItem { OrderItemId = 3, OrderId = 1, ItemId = 4, Quantity = 2, Price = 2.99m }
                    }
                },
                new Order
                {
                    OrderId = 2,
                    UserId = 2,
                    OrderDate = DateTime.Now.AddDays(-1),
                    Status = "Delivered",
                    PaymentMethod = "Cash",
                    TotalAmount = 29.99m,
                    AddressId = 2,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    UpdatedAt = DateTime.Now.AddDays(-1),
                    User = new User 
                    { 
                        UserId = 2,
                        FullName = "Jane Smith",
                        Email = "jane@example.com",
                        Phone = "098-765-4321",
                        Password = "hashed_password",
                        Role = "Customer",
                        CreatedAt = DateTime.Now.AddDays(-25),
                        UpdatedAt = DateTime.Now.AddDays(-25)
                    },
                    Address = new Address
                    {
                        AddressId = 2,
                        UserId = 2,
                        AddressLine = "456 Oak Ave",
                        City = "Los Angeles",
                        State = "CA",
                        ZipCode = "90001",
                        IsDefault = true,
                        CreatedAt = DateTime.Now.AddDays(-25),
                        UpdatedAt = DateTime.Now.AddDays(-25)
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { OrderItemId = 4, OrderId = 2, ItemId = 5, Quantity = 1, Price = 24.99m },
                        new OrderItem { OrderItemId = 5, OrderId = 2, ItemId = 6, Quantity = 1, Price = 6.99m }
                    }
                }
            };
        }

        public IActionResult OnGet()
        {
            Orders = _mockOrders;
            return Page();
        }

        public IActionResult OnGetOrderDetails(int orderId)
        {
            var order = _mockOrders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return new JsonResult(new { error = "Order not found" });
            }

            // Add menu item details to each order item
            foreach (var item in order.OrderItems)
            {
                if (_mockMenuItems.TryGetValue(item.ItemId, out var menuItem))
                {
                    item.Item = menuItem;
                }
            }

            // Create an anonymous object with the correct property names for JSON serialization
            var orderDetails = new
            {
                orderId = order.OrderId,
                orderDate = order.OrderDate,
                status = order.Status,
                paymentMethod = order.PaymentMethod,
                totalAmount = order.TotalAmount,
                user = new
                {
                    fullName = order.User.FullName,
                    email = order.User.Email,
                    phone = order.User.Phone
                },
                address = new
                {
                    addressLine = order.Address.AddressLine,
                    city = order.Address.City,
                    state = order.Address.State,
                    zipCode = order.Address.ZipCode
                },
                orderItems = order.OrderItems.Select(item => new
                {
                    quantity = item.Quantity,
                    price = item.Price,
                    item = new
                    {
                        itemName = item.Item?.ItemName ?? "Unknown Item",
                        description = item.Item?.Description ?? "No description available",
                        itemType = item.Item?.ItemType ?? "Unknown Type"
                    }
                })
            };

            return new JsonResult(orderDetails);
        }

        public IActionResult OnPostUpdateStatus(int orderId, string status)
        {
            var order = _mockOrders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                Message = "Order status updated successfully";
                IsSuccess = true;
            }
            else
            {
                Message = "Order not found";
                IsSuccess = false;
            }
            Orders = _mockOrders;
            return Page();
        }
    }
} 