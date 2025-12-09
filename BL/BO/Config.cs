namespace BO;
using Helpers;
/// <summary>
/// Public configuration settings exposed to the presentation layer (PL).
/// Contains only properties that the manager can view or modify.
/// Internal system values from DAL.Config are *not* shown here.
/// </summary>
public class Config
{
    // ---------------------------
    // Company Details
    // ---------------------------

    // Company address. Nullable because it is optional.
    public string? CompanyAddress { get; set; }

    // Company geographic coordinates (optional).
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // ---------------------------
    // Operational Settings
    // ---------------------------

    // Maximum allowed delivery range (km).
    public int MaxRange { get; set; }

    // Average speeds for distance/time calculations.
    public double AvgCarSpeed { get; set; }
    public double AvgMotorbikeSpeed { get; set; }
    public double AvgBicycleSpeed { get; set; }
    public double AvgWalkingSpeed { get; set; }

    // Maximum allowed delivery time (deadline).
    public TimeSpan MaxDeliveryTime { get; set; }

    // Risk range (time window for potential delay).
    public TimeSpan RiskRange { get; set; }

    // Time after which a courier is considered inactive.
    public TimeSpan InactivityRange { get; set; }

    // ---------------------------
    // Internal display fields
    // ---------------------------

    // For display only – manager cannot change the running IDs.
    // (These values appear on the settings screen as read-only.)
    public int NextOrderId { get; init; }
    public int NextDeliveryId { get; init; }

    public override string ToString() => this.ToStringProperty();
}
