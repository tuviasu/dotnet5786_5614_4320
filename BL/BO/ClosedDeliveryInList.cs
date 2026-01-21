using Helpers;

namespace BO;

public class ClosedDeliveryInList
{
    /// <summary>
    /// Unique identifier for the delivery
    /// </summary>
    public int DeliveryID { get; init; }                        // Unique identifier for the delivery

    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int OrderID { get; init; }                           // Unique identifier for the order

    /// <summary>
    /// Type of the order (e.g., pizza, pasta, etc.)
    /// </summary>
    public OrderType orderType { get; init; }                   // Type of the order (e.g., pizza, pasta, etc.)
    public string FullAddress { get; init; }                    // Full delivery address
    public DeliveryType deliveryType { get; init; }             // Type of delivery (e.g., standard, express, etc.)
    public double? RealDistance { get; init; }                  // Actual distance traveled for the delivery
    public TimeSpan TotalHandlingTime { get; init; }            // Total time taken to handle the delivery
    public DeliveryDoneType? deliveryDoneType { get; init; }    // Status of the delivery (e.g., completed, canceled, etc.)
    public override string ToString() => this.ToStringProperty();

}
