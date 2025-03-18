namespace CafeteriaOrdering.API.DTO
{
    public class MenuDto
    {
        //public int MenuId { get; set; }
        public int ManagerId { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        public bool? IsStatus { get; set; } = true; // Mặc định là true nếu không có giá trị
    }
}