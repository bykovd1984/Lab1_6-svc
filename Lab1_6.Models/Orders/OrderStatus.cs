namespace Lab1_6.Models.Orders
{
    public enum OrderStatus
    {
        PendingWarehouseAndDeliveryCommit,
        PendingWarehouseCommit,
        PendingDeliveryCommit,
        PendingBillingCommit,
        Cancelled,
        Created
    }
}
