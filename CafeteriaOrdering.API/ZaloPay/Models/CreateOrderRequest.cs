namespace CafeteriaOrdering.API.ZaloPay.Models
{
    public class CreateZaloOrderRequest
    {
        public int app_id { get; set; }
        public string app_user { get; set; }
        public string app_trans_id { get; set; }
        public long app_time { get; set; }
        public long amount { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public string embed_data { get; set; }
        public string bank_code { get; set; }
        public string? callback_url { get; set; }
        public string mac { get; set; }
    }
}
