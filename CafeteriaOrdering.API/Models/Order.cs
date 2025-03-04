namespace CafeteriaOrdering.API.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }

    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}