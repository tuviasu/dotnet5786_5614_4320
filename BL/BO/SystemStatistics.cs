namespace BO;

/// <summary>
/// System statistics and performance metrics.
/// Provides aggregated data about the entire delivery system.
/// </summary>
public class SystemStatistics
{
    /// <summary>
    /// Total number of couriers in the system.
    /// </summary>
    public int TotalCouriers { get; set; }

    /// <summary>
    /// Number of currently active couriers.
    /// </summary>
    public int ActiveCouriers { get; set; }

    /// <summary>
    /// Total number of orders in the system.
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Number of orders currently pending delivery.
    /// </summary>
    public int PendingOrders { get; set; }

    /// <summary>
    /// Number of orders successfully delivered.
    /// </summary>
    public int DeliveredOrders { get; set; }

    /// <summary>
    /// Number of failed deliveries.
    /// </summary>
    public int FailedDeliveries { get; set; }

    /// <summary>
    /// Number of active deliveries in progress.
    /// </summary>
    public int ActiveDeliveries { get; set; }

    /// <summary>
    /// Completion rate as percentage (0-100).
    /// </summary>
    public double CompletionRate { get; set; }

    /// <summary>
    /// Average delivery time in minutes.
    /// </summary>
    public double AverageDeliveryTime { get; set; }

    /// <summary>
    /// Success rate as percentage (0-100).
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Current system clock time.
    /// </summary>
    public DateTime CurrentTime { get; set; }

    /// <summary>
    /// Timestamp when these statistics were generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; }
}
