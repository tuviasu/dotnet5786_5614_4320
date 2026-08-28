namespace BO;
using Helpers;

public class Courier
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
    /// Email address of the courier
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Start date of the courier's employment
    /// </summary>
    public DateTime? StartWorkInCompany { get; init; }

    /// <summary>
    /// Type of transport used by the courier
    /// </summary>
    public DeliveryTransport TransportType { get; set; }

    /// <summary>
    /// Type of delivery service provided by the courier
    /// </summary>
    public DeliveryType? DeliveryType { get; set; }


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
    /// Number of deliveries made on time
    /// </summary>
    public int DeliveredInTime { get; set; }

    /// <summary>
    /// Number of deliveries made late
    /// </summary>
    public int DeliveredNotInTime { get; set; }

    /// <summary>
    /// Current order in progress for the courier
    /// </summary>
    public BO.OrderInProgress? orderInProgress { get; set; }

    public override string ToString() => this.ToStringProperty();
}
