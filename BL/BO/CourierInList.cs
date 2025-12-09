namespace BO;
using Helpers;
/// <summary>
/// Logical representation of a courier as displayed in list view.
/// This BO is view-only and constructed entirely by BL.
/// </summary>
public class CourierInList
{
    // -------------------------------------------------------
    // Basic Details
    // -------------------------------------------------------

    // Courier ID (unique identifier).
    // Source: DO.Courier.Id
    // Cannot be null. Not updatable.
    public int Id { get; init; }

    // Full name of the courier.
    // Source: DO.Courier.Name
    // Cannot be null. Not updatable.
    public string Name { get; init; } = string.Empty;

    // -------------------------------------------------------
    // Status
    // -------------------------------------------------------

    // Whether the courier is active in the system.
    // Source: DO.Courier.IsActive
    // Cannot be null. Updatable only through manager (BL logic).
    public bool IsActive { get; init; }

    // The courier's vehicle type.
    // Source: DO.Courier.Vehicle (ENUM)
    // Cannot be null. Not updatable.
    public DeliveryTransport Transport{ get; init; }

    // Date when the courier started working in the company.
    // Source: DO.Courier.StartWorkingDate
    // Cannot be null. Not updatable.
    public DateTime StartWorkingDate { get; init; }

    // -------------------------------------------------------
    // Delivery Counters (calculated by BL)
    // -------------------------------------------------------

    // Total number of deliveries completed by the courier.
    // Calculated in BL based on DO.Order / DO.Delivery history.
    // Cannot be null. Not updatable.
    public int DeliveredCountOnTime { get; init; }

    // Number of deliveries completed recently (last period defined by system).
    // Calculated in BL. Cannot be null. Not updatable.
    public int DeliveredCountLate { get; init; }

    // -------------------------------------------------------
    // Real-Time Activity
    // -------------------------------------------------------


    // The ID of the order currently assigned to this courier.
    // Nullable: null if the courier has no active order.
    // Calculated in BL.
    public int? ActiveOrderId { get; init; }

    public override string ToString() => this.ToStringProperty();
}
