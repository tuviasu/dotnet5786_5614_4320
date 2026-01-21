namespace Dal;
using DalApi;

sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();

    private DalList()
    {
        EnsureFixedCouriersExist();
    }

    public ICourier Courier { get; } = new CourierImplementation();

    public IOrder Order { get; } = new OrderImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

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
        foreach (var kvp in Dal.Config.Couriers)
        {
            if (DataSource.Couriers.Any(c => c.CourierID == kvp.Key))
                continue;

            DataSource.Couriers.Add(new DO.Courier(
                CourierID: kvp.Key,
                FullName: "Test Courier",
                Phone: "0500000000",
                Email: $"courier.{kvp.Key}@gmail.com",
                StartWorkInCompany: Dal.Config.Clock,
                TransportType: DO.DeliveryTransport.Car,
                DeliveryType: null,
                Password: kvp.Value,
                IsActive: true,
                MaxDeliveryDistanceKm: Dal.Config.MaxDeliveryDistance,
                OrderInProgress: null,
                DeliveredInTime: 0,
                DeliveredNotInTime: 0
            ));
        }
    }
}
