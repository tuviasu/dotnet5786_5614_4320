namespace Dal;
using DalApi;
using System.Diagnostics;

sealed internal class DalXml : IDal
{
    private static readonly (int Id, string Name, string Password, DO.DeliveryTransport Transport, double MaxDistance)[] FixedCouriers =
    {
        (312458962, "Yossi Cohen", "courier123", DO.DeliveryTransport.Motorcycle, 35.0),
    };

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
        foreach (var fixedCourier in FixedCouriers)
        {
            try
            {
                if (Courier.Read(fixedCourier.Id) is not null)
                    continue;
            }
            catch
            {
                // not exists
            }

            try
            {
                Courier.Create(new DO.Courier(
                    CourierID: fixedCourier.Id,
                    FullName: fixedCourier.Name,
                    Phone: "0500000000",
                    Email: $"courier.{fixedCourier.Id}@company.local",
                    StartWorkInCompany: Config.Clock,
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
            catch
            {
                // ignore duplicates
            }
        }
    }
}
