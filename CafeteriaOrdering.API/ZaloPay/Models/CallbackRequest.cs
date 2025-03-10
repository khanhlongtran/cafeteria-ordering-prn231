namespace CafeteriaOrdering.API.ZaloPay.Models
{
    public class CallbackRequest
    {
        public string Data { get; set; }
        public string Mac { get; set; }
        public int Type { get; set; }
    }
}
