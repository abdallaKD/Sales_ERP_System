namespace ERP.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 2
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Partial = 1,
        Paid = 2
    }

    public enum PurchaseStatus
    {
        Pending = 0,
        Received = 1,
        Cancelled = 2
    }

    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1,
        Transfer = 2
    }

    public enum InventoryMovementType
    {
        In = 0,
        Out = 1,
        Adjustment = 2
    }

    public enum ReferenceType
    {
        Order = 0,
        Purchase = 1,
        Manual = 2
    }
}
