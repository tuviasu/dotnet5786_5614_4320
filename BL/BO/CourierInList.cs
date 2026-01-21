using Helpers;

namespace BO;

public class CourierInList
{
    /// <summary>
    /// Unique identifier for the courier
    /// </summary>
    public int CourierID { get; init; }

    /// <summary>
    /// Full name of the courier
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Phone number of the courier
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Start date of the courier's employment
    /// </summary>
    public DateTime? StartWorkInCompany { get; init; }

    /// <summary>
    /// Type of transport used by the courier
    /// </summary>
    public DeliveryTransport TransportType { get; set; }

    /// <summary>
    /// Password for the courier's account
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Indicates if the courier is currently active
    /// </summary>
    public Boolean IsActive { get; set; }

    /// <summary>
    /// Maximum delivery distance for the courier
    /// </summary>
    public double? MaxDeliveryDistanceKM { get; set; }

    /// <summary>
    /// Type of delivery assigned to the courier
    /// </summary>
    public DeliveryType? DeliveryType { get; init; }

    /// <summary>
    /// Number of deliveries made on time
    /// </summary>
    public int DeliveredInTime { get; init; }

    /// <summary>
    /// Number of deliveries made late
    /// </summary>
    public int DeliveredNotInTime { get; init; }

    /// <summary>
    /// Current order in progress for the courier
    /// </summary>
    public BO.OrderInProgress? orderInProgress { get; init; }

    /// <summary>
    /// Email address of the courier
    /// </summary>
    public string? Email { get; set; }

    public override string ToString() => this.ToStringProperty();
}