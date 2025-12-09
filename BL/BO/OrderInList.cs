namespace BO;
using Helpers;
/// <summary>
/// Logical view of an order inside a list (manager screen).
/// This BO is view-only and is fully constructed in the BL layer.
/// </summary>
public class OrderInList
{
    // ---------------------------------------------------------
    // Identifiers
    // ---------------------------------------------------------

    // ID of the delivery record related to this order.
    // Source: DO.Delivery.Id
    // May be null (if no deliveries exist yet).
    // Not updatable.
    public int? DeliverId { get; init; }

    // ID of the order itself.
    // Source: DO.Order.Id
    // Cannot be null. Not updatable.
    public int OrderId { get; init; }

    // ---------------------------------------------------------
    // Type and Distance
    // ---------------------------------------------------------

    // Type of order (ENUM).
    // Source: DO.Order.Type
    // Not updatable.
    public OrderType Type { get; init; }

    // Air distance in kilometers (calculated in BL).
    // Cannot be null. Not updatable.
    public double AirDistance { get; init; }

    // ---------------------------------------------------------
    // Status Fields
    // ---------------------------------------------------------

    // Current order status (Created, Assigned, PickedUp, Delivered).
    // Enum defined in BO. Not updatable.
    public OrderStatus Status { get; init; }

    // Work schedule status of the courier (Assigned / InTransit / Delayed / Completed).
    // Enum defined in BO. Not updatable.
    public ScheduleStatus ScheduleStatus { get; init; }

    // ---------------------------------------------------------
    // Time Calculations
    // ---------------------------------------------------------

    // Total remaining time until the order's delivery deadline.
    // Calculated in BL: (MaxDeliveryTime - DateTime.Now).
    // Cannot be null. Not updatable.
    public TimeSpan RemainingTime { get; init; }

    // Total time spent handling this order.
    // If completed → duration from pickup to delivery.
    // If not completed → duration from pickup to now.
    // Cannot be null. Not updatable.
    public TimeSpan HandlingTime { get; init; }

    // ---------------------------------------------------------
    // Deliveries Count
    // ---------------------------------------------------------

    // Number of deliveries (attempts / actions) related to this order.
    // Built in BL based on DO.Delivery history.
    // Cannot be null. Not updatable.
    public int DeliveriesCount { get; init; }

    public override string ToString() => this.ToStringProperty();
}
