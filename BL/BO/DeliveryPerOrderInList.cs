namespace BO;
using Helpers;
/// <summary>
/// Logical view of a delivery record inside an order (list view).
/// Read-only object fully constructed by BL.
/// </summary>
public class DeliveryPerOrderInList
{
    public BO.ScheduleStatus? ScheduleStatus { get; init; }


    // ---------------------------------------------------------
    // Identifiers
    // ---------------------------------------------------------

    // ID of the delivery.
    // Source: DO.Delivery.Id
    // Cannot be updated.
    public int DeliveryId { get; init; }

    // ID of the courier who handled this delivery.
    // Source: DO.Delivery.CourierId
    // Cannot be updated.
    public int? CourierId { get; init; }

    // ---------------------------------------------------------
    // Courier Details
    // ---------------------------------------------------------

    // Full name of the courier (from DO.Courier).
    // Cannot be updated.
    public string CourierName { get; init; } = string.Empty;

    // Courier's vehicle type (enum).
    // Cannot be updated.
    public DeliveryTransport DeliveryType { get; init; }

    // ---------------------------------------------------------
    // Timing
    // ---------------------------------------------------------

    // Time when delivery was picked up.
    // Source: DO.Delivery.PickedUpTime
    // Cannot be null. Cannot be updated.
    public DateTime  StartDeliver { get; init; }

    // Time when delivery was completed.
    // May be null if not yet delivered.
    // Cannot be updated.
    public DateTime? EndDelivery { get; init; }

    // ---------------------------------------------------------
    // Status
    // ---------------------------------------------------------

    // Delivery status (enum).
    // Derived in BL.
    // Cannot be updated.
    public DeliveryCompletionType? CompletionType { get; init; }

    public override string ToString() => this.ToStringProperty();
}
