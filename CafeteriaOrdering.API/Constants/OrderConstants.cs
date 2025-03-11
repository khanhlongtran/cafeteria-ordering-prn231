namespace CafeteriaOrdering.API.Constants
{
    public static class OrderConstants
    {
        public enum OrderStatus
        {
            REQUEST_DELIVERY,  // after cafeteria request delivery
            DELIVERY_ACCEPTED,  // after deliverer accepted the order
            DELIVERY_IN_PROGRESS,  // after deliverer picked up the order
            COMPLETED,  // after deliverer delivered the order and customer paid (optional)
            CANCELED
        }

        public static bool isValidStatusForDeliver(string status) {
            return status == OrderStatus.REQUEST_DELIVERY.ToString() || 
                    status == OrderStatus.DELIVERY_ACCEPTED.ToString() ||
                    status == OrderStatus.DELIVERY_IN_PROGRESS.ToString();
        }
    }
}