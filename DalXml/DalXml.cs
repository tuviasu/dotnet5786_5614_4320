
using DalApi;
namespace Dal;

//stage 3
sealed public class DalXml : IDal
{
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
