using CafeteriaOrdering.API.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace CafeteriaOrdering.API.Vnpay
{
    public class VnpayHelper
    {
        private readonly VnpayConfig _config;
        private readonly CafeteriaOrderingDBContext _context;
        private IOptions<VnpayConfig> config;

        public VnpayHelper(IOptions<VnpayConfig> config, CafeteriaOrderingDBContext context)
        {
            _config = config.Value;
            _context = context;
        }

        public VnpayHelper(IOptions<VnpayConfig> config)
        {
            this.config = config;
        }

        public string GetHashSecret()
        {
            return _config.vnp_HashSecret;
        }
        public string CreatePaymentUrl(PaymentRequest request, decimal amount, string tempTxnRef)
        {
            SortedList<string, string> vnp_Params = new SortedList<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _config.vnp_TmnCode },
                { "vnp_Amount", ((int)(amount *100 )).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", tempTxnRef },
                { "vnp_OrderInfo", request.OrderInfo ?? "Thanh toan don hang"},
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", _config.vnp_Returnurl },
                { "vnp_IpAddr", "127.0.0.1" },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            if (!string.IsNullOrEmpty(request.BankCode))
            {
                vnp_Params.Add("vnp_BankCode", request.BankCode);
            }

            // Tạo chuỗi dữ liệu để ký hash
            StringBuilder queryString = new StringBuilder();
            foreach (var kvp in vnp_Params)
            {
                queryString.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
            }

            string rawData = queryString.ToString().TrimEnd('&');
            string vnp_SecureHash = HashHelper.HmacSHA512(_config.vnp_HashSecret, rawData);
            string paymentUrl = $"{_config.vnp_Url}?{rawData}&vnp_SecureHash={vnp_SecureHash}";

            return paymentUrl;
        }
    }
}
