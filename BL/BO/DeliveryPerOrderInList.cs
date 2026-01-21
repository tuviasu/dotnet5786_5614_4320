using Helpers;

namespace BO;

public class DeliveryPerOrderInList
{
    /// <summary>
    /// Unique identifier for the delivery
    /// </summary>
    public int DeliveryID { get; init; }

    /// <summary>
    /// Unique identifier for the courier
    /// </summary>
    public int? CourierID { get; init; }

    /// <summary>
    /// Full name of the courier
    /// </summary>
    public string CourierName { get; init; }

    /// <summary>
    /// Type of delivery (e.g., standard, express, etc.)
    /// </summary>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Date and time when the delivery started
    /// </summary>
    public DateTime DeliveryStartTime { get; init; }

    /// <summary>
    /// Status of the delivery (e.g., completed, canceled, etc.)
    /// </summary>
    public DeliveryDoneType? deliveryDoneType { get; init; }

    /// <summary>
    /// Date and time when the delivery was completed
    /// </summary>
    public DateTime? DeliveryDoneTime { get; init; }

    public override string ToString() => this.ToStringProperty();
}