using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.ZaloPay.Models;
using CafeteriaOrdering.API.ZaloPay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ZaloPayService _zaloPayService;
        private readonly CafeteriaOrderingDBContext _context;

        public CheckoutController(ZaloPayService zaloPayService, CafeteriaOrderingDBContext context)
        {
            _zaloPayService = zaloPayService;
            _context = context;
        }

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
    }
}
