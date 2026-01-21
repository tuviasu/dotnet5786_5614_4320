using Helpers;

namespace BO;

public class OpenOrderInList
{
    /// <summary>
    /// Unique identifier for the courier
    /// </summary>
    public int? CourierID { get; init; }

    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int OrderID { get; init; }

    /// <summary>
    /// Type of the order (e.g., pizza, pasta, etc.)
    /// </summary>
    public OrderType orderType { get; init; }

    /// <summary>
    /// Size of the pizza (e.g., small, medium, large)
    /// </summary>
    public DeviceType pizzaSize { get; init; }

    /// <summary>
    /// Full delivery address
    /// </summary>
    public string FullAddress { get; init; }

    /// <summary>
    /// Straight-line distance to the delivery address
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Actual distance traveled for the delivery
    /// </summary>
    public double? RealDistance { get; init; }

    /// <summary>
    /// Estimated time to complete the delivery in reality
    /// </summary>
    public TimeSpan? EstimatedTimeInReality { get; init; }

    /// <summary>
    /// Status of the delivery schedule (e.g., on time, delayed, etc.)
    /// </summary>
    public ScheduleStatus scheduleStatus { get; init; }

    /// <summary>
    /// Time remaining to complete the delivery
    /// </summary>
    public TimeSpan TimeRemainingToComplete { get; init; }

    /// <summary>
    /// Maximum allowed delivery time
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    public override string ToString() => this.ToStringProperty();
}