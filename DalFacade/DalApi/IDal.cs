namespace DalApi;

/// <summary>
/// IDal represents the whole Data Access Layer (DAL).
/// It exposes all entity DAL interfaces (Courier, Order, Delivery, Config)
/// as properties, so upper layers can work with one DAL object.
/// </summary>
public interface IDal
{
    /// <summary>
    /// Provides access to courier CRUD operations.
    /// </summary>
    ICourier Courier { get; }

    /// <summary>
    /// Provides access to order CRUD operations.
    /// </summary>
    IOrder Order { get; }

    /// <summary>
    /// Provides access to delivery CRUD operations.
    /// </summary>
    IDelivery Delivery { get; }

    /// <summary>
    /// Provides access to configuration settings (global system parameters).
    /// </summary>
    IConfig Config { get; }

    /// <summary>
    /// Resets the whole database (all lists) and all configuration values.
    /// </summary>
    void ResetDB();
}
