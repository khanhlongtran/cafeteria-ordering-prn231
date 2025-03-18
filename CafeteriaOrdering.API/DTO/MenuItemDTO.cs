namespace CafeteriaOrdering.API.DTO
{
    public class MenuItemDTO
    {
        //public int ItemId { get; set; }
        public int MenuId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ItemType { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        public int CountItemsSold { get; set; }
        public bool? IsStatus { get; set; }
        public string? Image { get; set; } // Nếu lưu đường dẫn ảnh
                                           // public byte[] Image { get; set; } // Nếu lưu ảnh dạng nhị phân
    }
}
