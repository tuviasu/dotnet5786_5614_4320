namespace Dal;
using DalApi;
using System.Diagnostics;

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml()
    {
        EnsureFixedCouriersExist();
    }

    public IConfig Config { get; } = new ConfigImplementation();

    public ICourier Courier { get; } = new CourierImplementation();

    public IOrder Order { get; } = new OrderImplementation();

    public IDelivery Delivery { get; } = new DeliveryImplementation();

    public void ResetDB()
    {
        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();
        Config.Reset();
        EnsureFixedCouriersExist();
    }

    private void EnsureFixedCouriersExist()
    {
        foreach (var kvp in Config.Couriers)
        {
            try
            {
                if (Courier.Read(kvp.Key) is not null)
                    continue;
            }
            catch
            {
                // not exists
            }

            try
            {
                Courier.Create(new DO.Courier(
                    CourierID: kvp.Key,
                    FullName: "Fixed Courier",
                    Phone: "0500000000",
                    Email: $"courier.{kvp.Key}@company.local",
                    StartWorkInCompany: Config.Clock,
                    TransportType: DO.DeliveryTransport.Car,
                    DeliveryType: null,
                    Password: kvp.Value,
                    IsActive: true,
                    MaxDeliveryDistanceKm: Config.MaxDeliveryDistance,
                    OrderInProgress: null,
                    DeliveredInTime: 0,
                    DeliveredNotInTime: 0
                ));
            }
            catch
            {
                // ignore duplicates
            }
        }
    }
}
