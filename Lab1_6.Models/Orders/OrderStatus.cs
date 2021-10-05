namespace Lab1_6.Models.Orders
{
    public enum OrderStatus
    {
        PendingWarehouseCommit,
        PendingDeliveryCommit,
        PendingBillingCommit,
        Creating,
        Cancelled,
        Created
    }
}
