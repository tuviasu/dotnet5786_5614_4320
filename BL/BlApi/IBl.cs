namespace BlApi;

/// <summary>
/// Main BL (Business Logic) facade interface.
/// Provides centralized access to all business logic services: Courier, Order, and Admin operations.
/// Implements the Facade pattern to simplify client interaction with the BL layer.
/// </summary>
public interface IBl
{
    /// <summary>
    /// Gets the Courier service for courier-related business logic operations.
    /// </summary>
    ICourier Courier { get; }

    /// <summary>
    /// Gets the Order service for order-related business logic operations.
    /// </summary>
    IOrder Order { get; }

    /// <summary>
    /// Gets the Admin service for system administration and configuration.
    /// </summary>
    IAdmin Admin { get; }
}
