public class NotificationService
{
    public void SendNotification(int orderId, string status)
    {
        Console.WriteLine($"Notification: Order {orderId} is now {status}.");
    }
}