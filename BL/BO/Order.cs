namespace BO;
using Helpers;
/// <summary>
/// Logical representation of an order (for managerial use).
/// All fields marked as non-updatable use 'init' and are constructed in BL.
/// </summary>
public class Order
{
    /// <summary>
    /// Unique order identifier (read-only after construction).
    /// Maps to DO.Order.Id.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Optional textual description of the order contents.
    /// May be empty when no description was provided.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// Customer full name for this order (used for display and contact).
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer phone number (used by couriers/support to contact the customer).
    /// Validation/normalization should be done in BL before persistence.
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// Full textual address where delivery should be performed.
    /// Prepared/validated by BL (street, number, city, etc.).
    /// </summary>
    public string FullAddressForDelivery { get; set; } = string.Empty;

    /// <summary>
    /// Geographic latitude of delivery address (decimal degrees).
    /// Used to calculate distances and for courier assignment.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Geographic longitude of delivery address (decimal degrees).
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Straight-line (air) distance from origin/warehouse to delivery address.
    /// Calculated by BL; used for estimates and routing heuristics.
    /// </summary>
    public double AirDistance { get; set; }

    /// <summary>
    /// Weight or size indicator for the order contents.
    /// Kept as object to allow flexible representation — BL should define and validate the expected format.
    /// </summary>
    public object? weight { get; set; }

    /// <summary>
    /// Date and time when the order was placed (read-only after creation).
    /// </summary>
    public DateTime OrderDate { get; init; }

    /// <summary>
    /// Nullable expected delivery date/time (computed or provided by BL when available).
    /// </summary>
    public DateTime? ExpectedDelivery { get; init; }

    /// <summary>
    /// The latest allowed delivery DateTime (deadline). Read-only after BL constructs the object.
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    /// <summary>
    /// Remaining time until the delivery deadline (MaxDeliveryTime - now).
    /// Calculated/populated by BL for scheduling and UI.
    /// </summary>
    public TimeSpan RemainingTime { get; init; }

    /// <summary>
    /// High-level status of the order (Created, Shipping, Delivered, Cancelled).
    /// Set and updated by BL business workflows.
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Optional order type (Regular, Express, International).
    /// Influences scheduling and expected delivery logic.
    /// </summary>
    public OrderType? Type { get; set; }

    /// <summary>
    /// Optional fragility classification for handling instructions.
    /// Use <see cref="FragilityLevel"/> values defined in BO.
    /// </summary>
    public FragilityLevel? Fragility { get; set; }

    /// <summary>
    /// Scheduling state used by the dispatcher/scheduler (Pending, InProgress, Completed, Cancelled).
    /// </summary>
    public ScheduleStatus ScheduleStatus_ { get; init; }

    // A list of all deliveries associated with this order.
    // May be null if the order has no recorded deliveries yet.
    // Populated by the BL layer. Not updatable.
    public List<DeliveryPerOrderInList>? DeliveriesHistory { get; init; }

    /// <summary>
    /// Returns a formatted string representation of the order.
    /// Relies on a `ToStringProperty` extension/helper being available in the project.
    /// If missing, implement that extension or replace this override with a self-contained implementation.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}