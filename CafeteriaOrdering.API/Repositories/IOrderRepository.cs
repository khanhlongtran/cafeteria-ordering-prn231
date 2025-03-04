using System.Collections.Generic;
using System.Threading.Tasks;
using CafeteriaOrdering.API.Models;

namespace CafeteriaOrdering.API.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
    }
}