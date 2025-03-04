using System;
using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaOrdering.API.Services
{
    public interface IMealDeliveryService
    {
        event Action<int, string> OnOrderStatusChanged; // Event
        void notifyUpdateOrderStatus(int orderId, string status);
    }
}