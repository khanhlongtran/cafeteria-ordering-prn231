using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.ZaloPay.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace CafeteriaOrdering.API.ZaloPay.Services
{
    public class ZaloPayService
    {
        private readonly ZaloPayConfig _config;
        private readonly HttpClient _httpClient;
        private readonly CafeteriaOrderingDBContext _context;

        public ZaloPayService(IConfiguration config, HttpClient httpClient, CafeteriaOrderingDBContext context)
        {
            _config = config.GetSection("ZaloPay").Get<ZaloPayConfig>();
            _httpClient = httpClient;
            _context = context;
        }

        public string ComputeHmacSha256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        //khởi tạo data tạo hoá đơn
        public async Task<(string OrderUrl, string ErrorMessage, string AppTransId)> CreateOrderAsync(decimal amount, string paymentMethod = "all", int? orderId = null)
        {
            var randomPart = Guid.NewGuid().ToString().Substring(0, 8);
            var appTransId = orderId.HasValue
                ? $"{DateTime.Now.ToString("yyMMdd")}_{orderId.Value}_{randomPart}"
                : $"{DateTime.Now.ToString("yyMMdd")}_{randomPart}";
            var appTime = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(7)).ToUnixTimeMilliseconds();

            string embedData;
            string bankCode;
            switch (paymentMethod.ToLower())
            {
                case "atm":
                    embedData = "{\"promotioninfo\":\"\",\"merchantinfo\":\"embeddata123\",\"bankgroup\":\"ATM\"}";
                    bankCode = "";
                    break;
                case "visa":
                case "mastercard":
                    embedData = "{\"promotioninfo\":\"\",\"merchantinfo\":\"embeddata123\"}";
                    bankCode = "CC";
                    break;
                case "zalopay":
                    embedData = "{\"promotioninfo\":\"\",\"merchantinfo\":\"embeddata123\"}";
                    bankCode = "zalopayapp";
                    break;
                case "all":
                default:
                    embedData = "{\"promotioninfo\":\"\",\"merchantinfo\":\"embeddata123\"}";
                    bankCode = "";
                    break;
            }

            var request = new CreateZaloOrderRequest
            {
                app_id = int.Parse(_config.AppId),
                app_user = "UserTest",
                app_trans_id = appTransId,
                app_time = appTime,
                amount = (long)amount,
                item = "[]",
                description = $"Thanh toán đơn hàng #{appTransId}",
                embed_data = embedData,
                bank_code = bankCode,
                callback_url = _config.CallbackUrl
            };

            var data = $"{request.app_id}|{request.app_trans_id}|{request.app_user}|{request.amount}|{request.app_time}|{request.embed_data}|{request.item}";
            request.mac = ComputeHmacSha256(data, _config.Key1);

            var json = JsonConvert.SerializeObject(request);
            Console.WriteLine($"Request gửi đến ZaloPay: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_config.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response từ ZaloPay: {responseContent}");

            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
            if (result.return_code == 1)
            {
                return (result.order_url.ToString(), null, appTransId);
            }
            else
            {
                return (null, $"Lỗi từ ZaloPay: {result.return_message} (SubCode: {result.sub_return_code})", null);
            }
        }
        // Phương thức xử lý callback
        public (bool IsSuccess, string Message, string AppTransId, long Amount, int ReturnCode) ProcessCallback(string data, string mac)
        {
            try
            {
                // Tính lại MAC để xác thực callback
                var computedMac = ComputeHmacSha256(data, _config.Key2);
                Console.WriteLine($"Callback received - Data: {data}");
                Console.WriteLine($"Callback received - Mac: {mac}");
                Console.WriteLine($"Computed Mac: {computedMac}");

                if (computedMac != mac)
                {
                    Console.WriteLine("Callback MAC verification failed!");
                    return (false, "Invalid MAC", null, 0, -1); // returncode = -1 (invalid callback)
                }

                // Parse dữ liệu
                var callbackData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string appTransId = callbackData.ContainsKey("app_trans_id") ? callbackData["app_trans_id"].ToString() : null;
                long amount = callbackData.ContainsKey("amount") ? Convert.ToInt64(callbackData["amount"]) : 0;

                Console.WriteLine($" Valid callback - AppTransId: {appTransId}, Amount: {amount}");

                // Kiểm tra và cập nhật trạng thái đơn hàng nếu thanh toán thành công
                if (appTransId != null)
                {
                    // Trích xuất OrderId từ app_trans_id
                    var parts = appTransId.Split('_');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int orderId))
                    {
                        var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                        if (order != null)
                        {
                            if (order.PaymentMethod == "COD")
                            {
                                if (order.Status != "COMPLETED")
                                {
                                    order.Status = "COMPLETED";
                                    order.UpdatedAt = DateTime.UtcNow;
                                    _context.SaveChanges();
                                    Console.WriteLine($"Order {orderId} updated to COMPLETED (COD)");
                                }
                                else
                                {
                                    Console.WriteLine($"Order {orderId} already COMPLETED (COD)");
                                }
                            }
                            else if (order.PaymentMethod == "BANKING")
                            {
                                if (order.Status != "DELIVERY_ACCEPTED")
                                {
                                    order.Status = "DELIVERY_ACCEPTED";
                                    order.UpdatedAt = DateTime.UtcNow;
                                    _context.SaveChanges();
                                    Console.WriteLine($"Order {orderId} updated to DELIVERY_ACCEPTED (BANKING)");
                                }
                                else
                                {
                                    Console.WriteLine($"Order {orderId} already DELIVERY_ACCEPTED (BANKING)");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Order {orderId} has unknown PaymentMethod: {order.PaymentMethod}");
                            }

                            return (true, "Success", appTransId, amount, 1); // returncode = 1 (success)
                        }
                        else
                        {
                            Console.WriteLine($"Order with OrderId {orderId} not found");
                            return (false, "Order not found", appTransId, amount, -1);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid app_trans_id format: {appTransId}");
                        return (false, "Invalid app_trans_id format", null, 0, -1);
                    }
                }
                else
                {
                    Console.WriteLine($"Missing app_trans_id in callback data");
                    return (false, "Missing app_trans_id", null, 0, -1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Exception in ProcessCallback: {ex.Message}");
                return (false, ex.Message, null, 0, 0); // returncode = 0 (exception, ZaloPay sẽ gọi lại)
            }
        }
    }
}
