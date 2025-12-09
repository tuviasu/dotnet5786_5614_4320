namespace BO;
using Helpers;
/// <summary>
/// Logical representation of an order currently being handled by a courier.
/// This BO object is read-only for fields defined as "not updatable", and is
/// fully constructed by the BL layer.
/// </summary>
public class OrderInProgress
{
    /// <summary>
    /// Gets the name of the customer.
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the delivery address.
    /// </summary>
    public string DeliveryAddress { get; init; } = string.Empty;

    /// <summary>
    /// Gets the phone number of the customer.
    /// </summary>
    public string CustomerPhone { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets or initializes the description associated with the object.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the unique identifier of the delivered item.
    /// </summary>
    public int DeliveredId { get; init; }

   /// <summary>
   /// Gets the unique identifier for the order.
   /// </summary>
    public int OrderId { get; init; }

  /// <summary>
  /// Gets the actual distance value, if available.
  /// </summary>
    private double? ActualDistance { get; init; }

    /// <summary>
    /// Gets the air distance in units determined by the context, such as kilometers or miles.
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Gets the date and time when the order was placed.
    /// </summary>
    public DateTime  OrderDate { get; init; }
    
    /// <summary>
    /// Gets the date and time when the delivery process is scheduled to start.
    /// </summary>
    public DateTime StartDelivery { get; init; }
   
    /// <summary>
    /// Gets the expected delivery time for the item.
    /// </summary>
    public DateTime ExpectedDeliveryTime { get; init; }

    /// <summary>
    /// Gets the maximum delivery time for the operation.
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    /// <summary>
    /// Gets the remaining time for the operation or process.
    /// </summary>
    public TimeSpan RemainingTime { get; init; }

    /// <summary>
    /// Gets the current schedule status of the entity.
    /// </summary>
    public ScheduleStatus ScheduleStatus_ { get; init; }

    /// <summary>
    /// Gets the type of the order.
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Gets the current status of the order.
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => this.ToStringProperty();
}
