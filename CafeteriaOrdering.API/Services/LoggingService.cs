public class LoggingService
{
    public void LogStatusChange(int orderId, string status)
    {
        Console.WriteLine($"Log: Order {orderId} changed to {status}");
    }
}
