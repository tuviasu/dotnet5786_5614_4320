

namespace BO;
using Helpers;
public class Courier
{
    // Unique identifier of the courier
    // DO: Courier.Id (int)
    public int Id { get; set; }

    // Courier full name
    // DO: Courier.Name (string)
    public string Name { get; set; }= string.Empty;


    // Courier phone number
    // DO: Courier.Phone (string)
    public string Phone { get; set; } = string.Empty;
    // Courier login password
    // Source: DO.Courier.Password
    // Cannot be null, cannot be updated directly
    // Validation (length, characters) is done in BL
    public string Password { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;
    // Indicates whether the courier is active or disabled
    // DO: Courier.IsActive (bool)
    public bool IsActive { get; set; }
    // Max travel distance that the courier is willing/capable to cover
    // According to business logic – must be checked in BL before setting
    // DO: Courier.MaxRange (double)
    public double? MaxRange { get; set; }

    // Vehicle type used by the courier (Enum)
    // DO: Courier.Vehicle (Enum)
    public DeliveryTransport Transport { get; init; }

    // The date the courier started working in the company
    // DO: Courier.StartWorkingDate (DateTime)
    public DateTime StartWorkingDate { get; init; }

    // Count of deliveries performed by the courier in time (to show in full details screen)
    // Calculated in BL (not taken directly from DO)
    public int DeliveriesCountOnTime { get; init; }
    // Count of deliveries completed late (to show in full details screen)
    // Calculated in BL (not taken directly from DO)
    public int DeliveriesCountLate { get; init; }
    // Count of deliveries completed by the courier recently (optional, depends on BL logic)
    public int DeliveredRecently { get; set; }

    // Order currently assigned to the courier (if exists)
    // BO: OrderInProgress (PDS partial allowed)
    public OrderInProgress? CurrentOrder { get; set; }

    // Use extension method to auto-generate formatted ToString
    public override string ToString() => this.ToStringProperty();
}
