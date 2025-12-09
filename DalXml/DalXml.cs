
using DalApi;
namespace Dal;


sealed internal class DalXml : IDal
{
    /// <summary>
    /// Singleton instance – the ONLY instance of DalXml
    /// </summary>
    public static IDal Instance { get; } = new DalXml();

    /// <summary>
    /// Private constructor – prevents creating new DalXml from outside
    /// </summary>
    private DalXml() { }

    public IOrder Order { get; } = new OrderImplementation();
    public IDelivery Delivery { get; } = new DeliveryImplementation();
    public ICourier Courier { get; } = new CourierImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public void ResetDB() {

        Order.DeleteAll();
        Delivery.DeleteAll();
        Courier.DeleteAll();
        Config.Reset();
    }

}
