namespace BO;
using Helpers;
/// <summary>
/// Logical representation of an open (not yet completed) order inside a list view.
/// This BO object is constructed entirely by the BL layer and is view-only.
/// </summary>
public class OpenOrderInList
{
    // ---------------------------------------------------------
    // Identifiers
    // ---------------------------------------------------------

    // ID of the courier currently associated with the order (if exists).
    // May be null if no courier is assigned yet.
    // Not updatable.
    public int? CourierId { get; init; }

    // Order ID.
    // Cannot be null. Not updatable.
    public int OrderId { get; init; }

    // ---------------------------------------------------------
    // Order Type
    // ---------------------------------------------------------

    // The type of the order (ENUM defined in BO).
    // Cannot be null. Not updatable.
    public OrderType Type { get; init; }

    // ---------------------------------------------------------
    // Distances
    // ---------------------------------------------------------

    // Air distance between courier and destination (if courier exists).
    // May be null if order is unassigned.
    // Not updatable.
    public double? AirDistance { get; init; }

    // Actual distance (calculated, may be null depending on BL logic).
    // Not updatable.
    public double? ActualDistance { get; init; }

    // ---------------------------------------------------------
    // Time Calculations
    // ---------------------------------------------------------

    // Time already spent on processing the order.
    // If no courier is assigned yet → 00:00:00.
    // Calculated in BL. Cannot be null. Not updatable.
    public TimeSpan HandlingTime { get; init; }
    /// <summary>
    /// Gets the address of the customer.
    /// </summary>
    public string CustomerAddress { get; init; }= string.Empty;
    /// <summary>
    ///  Gets the fragility level of the order.
    /// </summary>
    public FragilityLevel? Fragility { get; init; }
    // ---------------------------------------------------------
    // Work Schedule
    // ---------------------------------------------------------

    // Status of courier’s progress (Assigned, InTransit, Delayed, etc.).
    // Enum defined in BO. Not updatable.
    public ScheduleStatus ScheduleStatus { get; init; }

    // ---------------------------------------------------------
    // Delivery Timing
    // ---------------------------------------------------------

    // Remaining time until the maximum delivery deadline.
    // Calculated in BL. Cannot be null. Not updatable.
    public TimeSpan RemainingTime { get; init; }

    // Maximum allowed delivery time.
    // Not updatable. Never null.
    public DateTime MaxDeliveryTime { get; init; }
    // Actual time efficiency of handling the order.
    // Calculated in BL based on expected handling time vs. actual handling time.
    // Nullable: may be null if efficiency cannot be determined yet.
    // Not updatable.
    public TimeSpan? ActualEfficiency { get; init; }

    public override string ToString() => this.ToStringProperty();
}
