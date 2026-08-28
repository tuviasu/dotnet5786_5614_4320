namespace DO;

/// <summary>
/// Creates a new delivery record with all relevant details
/// </summary>
/// <param name="DeliveryID">Unique identifier of the delivery</param>
/// <param name="OrderID">Identifier of the related order</param>
/// <param name="CourierID">Identifier of the courier assigned to the delivery</param>
/// <param name="deliveryType">Type of delivery (e.g., express, scheduled, etc.)</param>
/// <param name="DeliveryStartTime">Date and time when the delivery started</param>
/// <param name="DeliveryDistance">Total distance of the delivery route</param>
/// <param name="DeliveryDoneType">Indicates how the delivery was completed (e.g., successful, canceled, failed)</param>
/// <param name="DeliveryDoneTime">Date and time when the delivery was completed</param>

public record Delivery
(
    int DeliveryID,
    int OrderID,
    int CourierID,
    DeliveryType? DeliveryType,
    DateTime DeliveryStartTime,
    double? DeliveryDistance,
    ProcessResult? DeliveryDoneType,
    DateTime? DeliveryDoneTime
)
{
    public Delivery() : this(0, 0, 0, null, DateTime.MinValue, null, null, null) { }
}
