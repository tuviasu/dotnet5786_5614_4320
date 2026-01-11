namespace Dal;

/// <summary>
/// Central, in-memory configuration holder for the delivery system.
/// Contains global constants, runtime settings and simple auto-increment ID generators.
/// This type is internal to the DAL implementation and is not thread-safe.
/// </summary>
internal static class Config
{
    /// <summary>
    /// Starting value for order IDs.
    /// IDs produced by <see cref="NextOrderId"/> begin from this value.
    /// </summary>
    internal const int startOrderId = 1000;

    /// <summary>
    /// Backing counter for order IDs; initialized from <see cref="startOrderId"/>.
    /// </summary>
    private static int nextOrderId = startOrderId;

    /// <summary>
    /// Returns the next order ID (post-increment).
    /// NOTE: this property is not synchronized — concurrent calls may race.
    /// </summary>
    internal static int NextOrderId { get => nextOrderId++; }

    /// <summary>
    /// Starting value for deliver (courier) IDs.
    /// IDs produced by <see cref="NextDeliverId"/> begin from this value.
    /// </summary>
    internal const int startDeliverId = 2000;

    /// <summary>
    /// Backing counter for deliver IDs; initialized from <see cref="startDeliverId"/>.
    /// </summary>
    private static int nextDeliverId = startDeliverId;

    /// <summary>
    /// Returns the next deliver ID (post-increment).
    /// NOTE: this property is not synchronized — concurrent calls may race.
    /// </summary>
    internal static int NextDeliverId { get => nextDeliverId++; }

    /// <summary>
    /// Application clock used by the DAL (defaults to <see cref="DateTime.Now"/>).
    /// Can be set for testing or simulation.
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

    /// <summary>
    /// Manager national ID (or system manager unique identifier).
    /// Default: 111111111. Note: <see cref="Reset"/> sets this to 0.
    /// </summary>
    internal static int ManagerId { get; set; } = 111111111;

    /// <summary>
    /// Manager password (plain text here for simplicity).
    /// SECURITY: Do NOT store plain-text passwords in production — use secure storage and hashing.
    /// Default: "1234".
    /// </summary>
    internal static string ManagerPassword { get; set; } = "1234";

    /// <summary>
    /// Optional company address used as the delivery origin.
    /// </summary>
    internal static string? CompanyAddress { get; set; } = null;

    /// <summary>
    /// Optional company latitude (decimal degrees). Null when unspecified.
    /// </summary>
    internal static double? Latitude { get; set; } = null;

    /// <summary>
    /// Optional company longitude (decimal degrees). Null when unspecified.
    /// </summary>
    internal static double? Longitude { get; set; } = null;

    /// <summary>
    /// Optional maximum delivery range (in kilometers). Null indicates no explicit range limit.
    /// </summary>
    internal static int? MaxRange { get; set; } = null;

    /// <summary>
    /// Average speeds used for travel time estimations (units: km/h).
    /// Defaults are conservative average values per transport type.
    /// </summary>
    internal static double AvgCarSpeed { get; set; } = 50;
    internal static double AvgMotorbikeSpeed { get; set; } = 40;
    internal static double AvgBicycleSpeed { get; set; } = 15;
    internal static double AvgWalkingSpeed { get; set; } = 5;

    /// <summary>
    /// Time span after which a delivery is considered too long (default: 4 hours).
    /// </summary>
    internal static TimeSpan MaxDeliveryTime { get; set; } = TimeSpan.FromHours(4);

    /// <summary>
    /// Time window used to determine risk-related behaviors (default: 1 hour).
    /// </summary>
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Time span used to detect courier inactivity (default: 2 hours).
    /// </summary>
    internal static TimeSpan InactivityRange { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Reset all configuration values to their defaults.
    /// Resets ID counters, clears location data, and restores default speeds and time windows.
    /// Note: default ManagerId initial value (111111111) differs from the Reset value (0).
    /// </summary>
    internal static void Reset()
    {
        nextOrderId = startOrderId;
        nextDeliverId = startDeliverId;

        Clock = DateTime.Now;

        ManagerId = 111111111;
        ManagerPassword = "1234";

        CompanyAddress = null;
        Latitude = null;
        Longitude = null;

        MaxRange = 4;
        AvgCarSpeed = 50;
        AvgMotorbikeSpeed = 40;
        AvgBicycleSpeed = 15;
        AvgWalkingSpeed = 5;

        MaxDeliveryTime = TimeSpan.FromHours(4);
        RiskRange = TimeSpan.FromHours(1);
        InactivityRange = TimeSpan.FromHours(2);
    }
}