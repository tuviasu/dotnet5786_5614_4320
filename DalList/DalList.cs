namespace Dal;

using DalApi;

/// <summary>
/// DalList class implements the IDal interface.
/// It provides access to all DAL entity interfaces (Courier, Order, Delivery, Config)
/// through initialized properties.
/// </summary>


  

sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    // Each property below exposes one DAL entity interface.
    // Each of them is initialized with the actual implementation class
    // (which you created in Stage 1, for example CourierImplementation, etc.)

    /// <summary>
    /// Access to courier data operations.
    /// </summary>
    public ICourier Courier { get; } = new CourierImplementation();

    /// <summary>
    /// Access to order data operations.
    /// </summary>
    public IOrder Order { get; } = new OrderImplementation();

    /// <summary>
    /// Access to delivery data operations.
    /// </summary>
    public IDelivery Delivery { get; } = new DeliveryImplementation();

    /// <summary>
    /// Access to system configuration operations.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();


    /// <summary>
    /// Resets the entire database and configuration.
    /// </summary>
    public void ResetDB()
    {
        // Delete all data lists using the CRUD interface
        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();

        // Reset system configuration values
        Config.Reset();
    }
}
