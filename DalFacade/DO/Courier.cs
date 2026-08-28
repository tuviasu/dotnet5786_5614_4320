namespace DO;

/// <summary>
/// Creates a new courier object with all relevant details
/// </summary>
/// <param name="CourierID">Unique identifier of the courier</param>
/// <param name="FullName">Full name of the courier</param>
/// <param name="Phone">Courier’s phone number</param>
/// <param name="Email">Courier’s email address</param>
/// <param name="StartWorkInCompany">Date when the courier started working in the company</param>
/// <param name="TransportType">Type of transport used by the courier (e.g., car, motorcycle, bicycle)</param>
/// <param name="Password">Password for the courier’s system account</param>
/// <param name="IsActive">Indicates whether the courier is currently active</param>
/// <param name="MaxDeliveryDistanceKm">Maximum delivery distance allowed for the courier, in kilometers</param>
/// <param name="DeliveredInTime">Number of deliveries completed on time by the courier</param>
/// <param name="DeliveredNotInTime">Number of deliveries completed not on time by the courier</param>
public record Courier
(
    int CourierID,
    string FullName,
    string Phone,
    string Email,
    DateTime StartWorkInCompany,
    DeliveryTransport TransportType,
    DeliveryType? DeliveryType = null,
    string Password = "A123456",
    Boolean IsActive = true,
    double? MaxDeliveryDistanceKm = null,
    string? OrderInProgress = null,

    int DeliveredInTime = 0,
    int DeliveredNotInTime = 0
)
{
    public Courier() : this(0, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DeliveryTransport.Car) { }
}
