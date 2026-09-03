namespace Dal;
using DalApi;

sealed internal class DalList : IDal
{
    private static readonly (int Id, string Name, string Password, DO.DeliveryTransport Transport, double MaxDistance)[] FixedCouriers =
    {
        (312458962, "Yossi Cohen", "courier123", DO.DeliveryTransport.Motorcycle, 35.0),
    };

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
        foreach (var fixedCourier in FixedCouriers)
        {
            if (DataSource.Couriers.Any(c => c.CourierID == fixedCourier.Id))
                continue;

            DataSource.Couriers.Add(new DO.Courier(
                CourierID: fixedCourier.Id,
                FullName: fixedCourier.Name,
                Phone: "0500000000",
                Email: $"courier.{fixedCourier.Id}@gmail.com",
                StartWorkInCompany: Dal.Config.Clock,
                TransportType: fixedCourier.Transport,
                DeliveryType: null,
                Password: fixedCourier.Password,
                IsActive: true,
                MaxDeliveryDistanceKm: fixedCourier.MaxDistance,
                OrderInProgress: null,
                DeliveredInTime: 0,
                DeliveredNotInTime: 0
            ));
        }
    }
}
