namespace CafeteriaOrdering.API.Services
{
    public interface IMealDeliveryService
    {
        Task<bool> DeliverMealAsync(int orderId);
        Task<bool> CancelDeliveryAsync(int orderId);
        Task<OrderStatus> GetDeliveryStatusAsync(int orderId);
    }

    public enum OrderStatus
    {
        Pending,
        InProgress,
        Delivered,
        Cancelled
    }
}