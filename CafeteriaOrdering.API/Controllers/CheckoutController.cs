using CafeteriaOrdering.API.DTO;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Vnpay;
using CafeteriaOrdering.API.ZaloPay.Models;
using CafeteriaOrdering.API.ZaloPay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CafeteriaOrdering.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ZaloPayService _zaloPayService;
        private readonly CafeteriaOrderingDBContext _context;
        private readonly VnpayHelper _vnpayHelper;
        private VnpayConfig _config;
        public CheckoutController(ZaloPayService zaloPayService, CafeteriaOrderingDBContext context, IOptions<VnpayConfig> config, VnpayHelper vnpayHelper)
        {
            _zaloPayService = zaloPayService;
            _context = context;
            _vnpayHelper = vnpayHelper;
            _config = config.Value;
        }
        //[Authorize]
        [HttpPost("BankingTranfer")]
        public async Task<IActionResult> CreatePayment(int orderId, [FromQuery] string paymentMethod = "all")
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Error = "Order not found" });
            }

            var amount = order.TotalAmount;

            var (orderUrl, errorMessage, appTransId) = await _zaloPayService.CreateOrderAsync(amount, paymentMethod, orderId);

            if (orderUrl != null)
            {
                order.PaymentMethod = "BANKING";
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new { OrderUrl = orderUrl });
            }
            return BadRequest(new { Error = errorMessage });
        }

        [HttpPost("callback")]
        public IActionResult Callback([FromBody] CallbackRequest request)
        {
            var result = _zaloPayService.ProcessCallback(request.Data, request.Mac);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    return_code = result.ReturnCode,
                    return_message = result.Message,
                    app_trans_id = result.AppTransId,
                    amount = result.Amount
                });

            }
            return BadRequest(new { return_code = -1, return_message = result.Message });
        }
        //[Authorize]
        [HttpPost("COD")]
        public async Task<IActionResult> CreateCODPayment(int orderId, [FromQuery] string paymentMethod = "all")
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Error = "Order not found" });
            }

            var amount = order.TotalAmount;

            var (orderUrl, errorMessage, appTransId) = await _zaloPayService.CreateOrderAsync(amount, paymentMethod, orderId);

            if (orderUrl != null)
            {
                order.PaymentMethod = "COD";
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                Console.WriteLine($"Order {orderId} created with COD payment method");

                return Ok(new
                {
                    OrderUrl = orderUrl,
                });
            }
            return BadRequest(new { Error = errorMessage });
        }

        [HttpPost("VnpayTransfer")]
        public IActionResult CreatePayment([FromBody] PaymentOrderRequest request, [FromServices] IMemoryCache memoryCache)
        {
            PaymentRequest paymentRequest = request.Payment;
            CreateOrderRequest orderRequest = request.Order;
            // Tính tổng số tiền
            decimal totalAmount = 0;
            foreach (var orderItem in orderRequest.OrderItems)
            {
                var menuItem = _context.MenuItems.Find(orderItem.ItemId);
                if (menuItem == null)
                    return NotFound($"MenuItem with ID {orderItem.ItemId} not found");

                var price = menuItem.Price * orderItem.Quantity;
                totalAmount += price;
            }

            if (totalAmount <= 0)
            {
                return BadRequest(new PaymentResponse
                {
                    Success = false,
                    Message = "Số tiền không hợp lệ",
                    PaymentUrl = null
                });
            }

            // Tạo mã tham chiếu tạm thời
            string tempTxnRef = DateTime.Now.Ticks.ToString();

            // Lưu CreateOrderRequest vào IMemoryCache với thời gian sống 30 phút
            memoryCache.Set(tempTxnRef, orderRequest, TimeSpan.FromMinutes(30));

            string returnUrl = string.IsNullOrWhiteSpace(request.Payment.ReturnUrl)
            ? _config.vnp_Returnurl
            : request.Payment.ReturnUrl;

            // Tạo URL thanh toán
            string paymentUrl = _vnpayHelper.CreatePaymentUrl(paymentRequest, totalAmount, tempTxnRef, returnUrl);
            return Ok(new PaymentResponse
            {
                Success = true,
                Message = "Tạo URL thanh toán thành công",
                PaymentUrl = paymentUrl
            });
        }
        [HttpGet("vnpay/callback")]
        public async Task<IActionResult> PaymentCallback([FromServices] IMemoryCache memoryCache)
        {
            var vnp_Params = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
            string hashSecret = _vnpayHelper.GetHashSecret();
            string vnp_SecureHash = vnp_Params.ContainsKey("vnp_SecureHash") ? vnp_Params["vnp_SecureHash"] : "";
            vnp_Params.Remove("vnp_SecureHash");

            string rawData = string.Join("&", vnp_Params.OrderBy(k => k.Key).Select(kv => $"{kv.Key}={kv.Value}"));
            string computedHash = HashHelper.HmacSHA512(hashSecret, rawData);

            if (computedHash != vnp_SecureHash)
            {
                return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ (Sai chữ ký hash)" });
            }

            string vnp_ResponseCode = vnp_Params["vnp_ResponseCode"];
            string vnp_TransactionStatus = vnp_Params["vnp_TransactionStatus"];
            string vnp_TxnRef = vnp_Params["vnp_TxnRef"];
            string vnp_Amount = vnp_Params["vnp_Amount"];
            decimal amount = decimal.Parse(vnp_Amount) / 100;

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                if (!memoryCache.TryGetValue(vnp_TxnRef, out CreateOrderRequest orderRequest))
                {
                    return BadRequest(new { Success = false, Message = "Không tìm thấy thông tin đơn hàng tạm thời" });
                }

                decimal totalAmount = 0;
                var orderItemsToAdd = new List<OrderItem>();

                foreach (var orderItem in orderRequest.OrderItems)
                {
                    var menuItem = await _context.MenuItems.FindAsync(orderItem.ItemId);
                    if (menuItem == null)
                        return NotFound($"MenuItem with ID {orderItem.ItemId} not found");

                    var price = menuItem.Price * orderItem.Quantity;
                    totalAmount += price;

                    orderItemsToAdd.Add(new OrderItem
                    {
                        ItemId = orderItem.ItemId,
                        Quantity = orderItem.Quantity,
                        Price = price
                    });
                }

                if (totalAmount != amount)
                {
                    return BadRequest(new { Success = false, Message = "Số tiền thanh toán không khớp với đơn hàng" });
                }

                var order = new Order
                {
                    UserId = orderRequest.UserId,
                    PaymentMethod = "VNPAY",
                    AddressId = orderRequest.AddressId,
                    OrderDate = DateTime.UtcNow,
                    Status = "REQUEST_DELIVERY",
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = orderItemsToAdd
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                memoryCache.Remove(vnp_TxnRef);

                //return Ok(new { Success = true, Message = "Thanh toán thành công", OrderId = order.OrderId, Amount = amount });
                return HandleResponse(true, order.OrderId, amount);
               // return Ok();
            }
            else
            {
                memoryCache.Remove(vnp_TxnRef);
                //return BadRequest(new { Success = false, Message = "Thanh toán thất bại hoặc bị hủy" });
                return HandleResponse(false);
               // return BadRequest(new { Success = false, });
            }
        }

        private IActionResult HandleResponse(bool success, int orderId = 0, decimal amount = 0)
        {
            // Kiểm tra User-Agent
            bool isAndroidApp = Request.Headers["User-Agent"].ToString().Contains("Android");

            string successUrl = isAndroidApp ? "myapp://payment-success" : "url_cho_web_success";
            string failedUrl = isAndroidApp ? "myapp://payment-failed" : "url_cho_web_fail";

            string redirectUrl = success ? successUrl : failedUrl;

            if (isAndroidApp)
            {
                return Content($"<script>window.location.href='{redirectUrl}';</script>", "text/html");
            }
            else
            {
                return Redirect(redirectUrl);
            }
        }

    }
}
