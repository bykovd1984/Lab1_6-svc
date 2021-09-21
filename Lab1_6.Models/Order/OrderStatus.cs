namespace Lab1_6.Models.Order
{
    public enum OrderStatus
    {
        PendingWarehouseCommit,
        PendingDeliveryCommit,
        PendingBillingCommit,
        Created,
        Cancelled
    }
}
