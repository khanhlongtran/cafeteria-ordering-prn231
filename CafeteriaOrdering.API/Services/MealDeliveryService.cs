using System.Threading.Tasks;

namespace CafeteriaOrdering.API.Services
{
    public interface IMealDeliveryService
    {
        Task DeliverMealAsync(int orderId);
    }

    public class MealDeliveryService : IMealDeliveryService
    {
        public async Task DeliverMealAsync(int orderId)
        {
            // Implementation for delivering the meal
            // This is a placeholder for the actual delivery logic
            await Task.Run(() => 
            {
                // Simulate meal delivery process
                System.Console.WriteLine($"Delivering meal for order ID: {orderId}");
            });
        }
    }
}