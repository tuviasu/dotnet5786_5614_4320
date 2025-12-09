namespace BO;
using Helpers;
/// <summary>
/// Logical representation of a closed (completed) delivery inside a list.
/// This BO is view-only and fully constructed by BL.
/// </summary>
public class ClosedDeliveryInList
{
    internal ScheduleStatus ScheduleStatus;
    internal DeliveryCompletionType? completionType;

    // ---------------------------------------------------------
    // Identifiers
    // ---------------------------------------------------------

    // ID of the delivery.
    // Source: DO.Delivery.Id
    // This field can be null if no valid delivery record exists.
    // Not updatable.
    public int? DeliveredId { get; init; }

    // ID of the related order.
    // Source: DO.Delivery.OrderId
    // Cannot be null. Not updatable.
    public int OrderId { get; init; }


    // Order type (ENUM).
    // Cannot be updated.
    public OrderType Type { get; init; }
    /// <summary>
    /// Gets the actual distance measured, in units relevant to the context, or <see langword="null"/> if the distance
    /// is not available.
    /// </summary>
    public double? ActualDistance { get; init; }

    // Total time spent handling the delivery.
    // Calculated by BL using pickup and delivery timestamps.
    // Cannot be updated.
    public TimeSpan HandlingTime { get; init; }

    /// <summary>
    /// Gets the type of delivery transport used for the operation.
    /// </summary>
    public DeliveryTransport DeliveryType { get; init; }

    /// <summary>
    /// Gets the completion type of the delivery.
    /// </summary>
    DeliveryCompletionType? CompletionType { get; init; }
    /// <summary>
    /// Gets the address of the customer.
    /// </summary>
    public string CustomerAddress { get; init; }= string.Empty;
    public object EndDelivery { get; internal set; }

    public override string ToString() => this.ToStringProperty();
}
