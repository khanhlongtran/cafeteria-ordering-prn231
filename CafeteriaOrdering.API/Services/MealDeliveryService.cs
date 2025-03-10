using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Constants;

namespace CafeteriaOrdering.API.Services
{
    // Define the delegate (event signature)
    public delegate void OrderStatusChangedHandler(int orderId, string newStatus);

    public class MealDeliveryService : IMealDeliveryService
    {
        // Declare an event based on the delegate
        // public event OrderStatusChangedHandler? OnOrderStatusChanged;
        public event Action<int, string>? OnOrderStatusChanged; // Define event
        // private readonly CafeteriaOrderingDBContext _context;

        // public MealDeliveryService(CafeteriaOrderingDBContext context)
        // {
        //     _context = context;
        // }

        public void notifyUpdateOrderStatus(int orderId, string status)
        {
            // 🔥 Fire the event when the order status is updated
            OnOrderStatusChanged?.Invoke(orderId, status);
        }
    }
}