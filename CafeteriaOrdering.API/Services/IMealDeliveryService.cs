using System.Threading.Tasks;
using CafeteriaOrdering.API.Constants;

namespace CafeteriaOrdering.API.Services
{
    public interface IMealDeliveryService
    {
        Task<bool> DeliverMealAsync(int orderId);
        // Task<bool> CancelDeliveryAsync(int orderId);
        // Task<OrderConstants.OrderStatus> GetDeliveryStatusAsync(int orderId);
    }
}