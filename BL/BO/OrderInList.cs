using Helpers;

namespace BO;

public class OrderInList
{
    /// <summary>
    /// Unique identifier for the delivery
    /// </summary>
    public int? DeliveryID { get; init; }

    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int OrderID { get; init; }

    /// <summary>
    /// Type of the order (e.g., pizza, pasta, etc.)
    /// </summary>
    public OrderType orderType { get; init; }

    /// <summary>
    /// Straight-line distance to the delivery address
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Current status of the order
    /// </summary>
    public OrderStatus orderStatus { get; init; }

    /// <summary>
    /// Status of the delivery schedule (e.g., on time, delayed, etc.)
    /// </summary>
    public ScheduleStatus scheduleStatus { get; init; }

    /// <summary>
    /// Time remaining to complete the delivery
    /// </summary>
    public TimeSpan TimeRemainingToComplete { get; init; }

    /// <summary>
    /// Total time taken to handle the order
    /// </summary>
    public TimeSpan TotalHandlingTime { get; init; }

    /// <summary>
    /// Total number of deliveries associated with the order
    /// </summary>
    public int TotalDeliveries { get; init; }

    public override string ToString() => this.ToStringProperty();
}