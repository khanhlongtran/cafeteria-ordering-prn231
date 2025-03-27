using CafeteriaOrdering.API.DTO;

namespace CafeteriaOrdering.API.Vnpay
{
    public class PaymentRequest
    {
        public string OrderInfo { get; set; }
        public string BankCode { get; set; }
    }
    public class PaymentResponse
    {
        public string PaymentUrl { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    public class PaymentOrderRequest
    {
        public PaymentRequest Payment { get; set; }
        public CreateOrderRequest Order { get; set; }
    }
}
