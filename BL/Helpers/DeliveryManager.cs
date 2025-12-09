using DalApi;


namespace Helpers;

/// <summary>
/// Internal BL helper for all logical delivery operations.
/// Converts DO ↔ BO, validates actions, and interacts with DAL.
/// </summary>
internal static class DeliveryManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static readonly IDal dal = DalApi.Factory.Get;

    // ================================================================
    // 1) Convert DO.Delivery → BO.DeliveryPerOrderInList (history item)
    // ================================================================
    internal static BO.DeliveryPerOrderInList ConvertToDeliveryInOrder(DO.Delivery delivery)
    {
        return new BO.DeliveryPerOrderInList
        {
            DeliveryId = delivery.Id,
            CourierId = delivery.CourierId,
            CourierName = string.Empty,
            DeliveryType = (BO.DeliveryTransport)delivery.DeliveryType,
            StartDeliver = delivery.StartDelivery,
            EndDelivery = delivery.EndDelivery,
            CompletionType = (BO.DeliveryCompletionType?)delivery.CompletionType,
            ScheduleStatus = delivery.EndDelivery == null
                                ? null
                                : (BO.ScheduleStatus?)CalculateScheduleStatus(delivery)
        };
    }

    // ================================================================
    // 3) Calculate schedule status
    // ================================================================
    internal static BO.ScheduleStatus CalculateScheduleStatus(DO.Delivery delivery)
    {
        if (delivery.EndDelivery == null)
            return BO.ScheduleStatus.InProgress;

        TimeSpan max = dal.Config.MaxDeliveryTime;
        TimeSpan actual = delivery.EndDelivery.Value - delivery.StartDelivery;

        if (actual <= max)
            return BO.ScheduleStatus.Completed;

        if (actual <= max + dal.Config.RiskRange)
            return BO.ScheduleStatus.InProgress;

        return BO.ScheduleStatus.Cancelled;
    }

    // ================================================================
    // 4) Create new active delivery
    // ================================================================
    internal static DO.Delivery CreateNewDelivery(int orderId, int courierId)
    {
        int newId = dal.Config.NextDeliveryId;

        var delivery = new DO.Delivery(
            Id: newId,
            OrderedId: orderId,
            CourierId: courierId,
            DeliveryType: GetCourierTransport(courierId),
            StartDelivery: AdminManager.Now
        );

        dal.Delivery.Create(delivery);
        dal.Config.NextDeliveryId = newId + 1;

        Observers.NotifyItemUpdated(newId); //stage 5
        Observers.NotifyListUpdated(); //stage 5

        return delivery;
    }

    // ================================================================
    // 5) Create cancelled delivery
    // ================================================================
    internal static DO.Delivery CreateCancelledDelivery(int orderId)
    {
        int newId = dal.Config.NextDeliveryId;

        var delivery = new DO.Delivery(
            Id: newId,
            OrderedId: orderId,
            CourierId: 0,
            DeliveryType: DO.DeliveryTransport.Motorcycle,
            StartDelivery: AdminManager.Now,
            EndDelivery: AdminManager.Now,
            CompletionType: DO.DeliveryCompletionType.Canceled
        );

        dal.Delivery.Create(delivery);
        dal.Config.NextDeliveryId = newId + 1;

        Observers.NotifyItemUpdated(newId); //stage 5
        Observers.NotifyListUpdated(); //stage 5

        return delivery;
    }

    // ================================================================
    // 6) Finish cancelled delivery
    // ================================================================
    internal static void FinishCancelledDelivery(int deliveryId)
    {
        var delivery = dal.Delivery.Read(deliveryId)
            ?? throw new BO.BlDoesNotExistException($"Delivery {deliveryId} not found.");

        var updated = delivery with
        {
            EndDelivery = AdminManager.Now,
            CompletionType = DO.DeliveryCompletionType.Canceled
        };

        dal.Delivery.Update(updated);

        Observers.NotifyItemUpdated(deliveryId); //stage 5
        Observers.NotifyListUpdated(); //stage 5
    }

    // ================================================================
    // 7) Complete delivery
    // ================================================================
    internal static void CompleteDelivery(int deliveryId)
    {
        var delivery = dal.Delivery.Read(deliveryId)
            ?? throw new BO.BlDoesNotExistException($"Delivery {deliveryId} not found.");

        var updated = delivery with
        {
            EndDelivery = AdminManager.Now,
            CompletionType = DO.DeliveryCompletionType.Successful
        };

        dal.Delivery.Update(updated);

        Observers.NotifyItemUpdated(deliveryId); //stage 5
        Observers.NotifyListUpdated(); //stage 5
    }

    // ================================================================
    // 8) Get all closed deliveries
    // ================================================================
    internal static IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveries(
        int courierId,
        BO.ClosedDeliveryFilterBy? filterBy,
        BO.ClosedDeliverySortBy? sortBy, BO.DeliveryCompletionType? completionType)
    {
        var results =
            from d in dal.Delivery.ReadAll(del => del!.CourierId == courierId && del.EndDelivery != null)
            select new BO.ClosedDeliveryInList
            {
                DeliveredId = d!.Id,
                OrderId = d.OrderedId,
                HandlingTime = d.EndDelivery!.Value - d.StartDelivery,
                completionType = (BO.DeliveryCompletionType?)d.CompletionType,
                ScheduleStatus = (BO.ScheduleStatus)CalculateScheduleStatus(d)
            };

        if (filterBy != null)
        {
            results = filterBy switch
            {
                BO.ClosedDeliveryFilterBy.DeliveryStatus =>
                    results.Where(r => r.completionType == (BO.DeliveryCompletionType?)filterBy),

                BO.ClosedDeliveryFilterBy.OnTimeStatus =>
                    results.Where(r => r.ScheduleStatus == (BO.ScheduleStatus?)filterBy),

                _ => results
            };
        }

        results = sortBy switch
        {
            BO.ClosedDeliverySortBy.EndTime =>
                results.OrderBy(r => r.EndDelivery),

            BO.ClosedDeliverySortBy.DeliveryStatus =>
                results.OrderBy(r => r.completionType),

            BO.ClosedDeliverySortBy.OnTimeStatus =>
                results.OrderBy(r => r.ScheduleStatus),

            _ => results
        };

        return results;
    }

    // ================================================================
    // 9) Get open orders for a courier
    // ================================================================
    internal static IEnumerable<BO.OpenOrderInList> GetOpenOrders(
        int courierId,
        BO.OpenOrderFilterBy? filterBy,
        BO.OpenOrderSortBy? sortBy)
    {
        var courier = dal.Courier.Read(courierId)
            ?? throw new BO.BlDoesNotExistException($"Courier {courierId} not found.");

        var (courierLat, courierLon) = Tools.GetCoordinates(courier.Address!);

        var orders =
            from o in dal.Order.ReadAll()
            where o != null && o.Status == DO.OrderStatus.Created
            let dist = Tools.AirDistance(o.Latitude, o.Longitude, courierLat, courierLon)
            where courier.MaxDistance == null || dist <= courier.MaxDistance
            select new BO.OpenOrderInList
            {
                OrderId = o!.Id,
                Type = (BO.OrderType)o.Type,
                AirDistance = dist,
                ScheduleStatus = BO.ScheduleStatus.Pending
            };

        if (filterBy != null)
        {
            orders = filterBy switch
            {
                BO.OpenOrderFilterBy.Type =>
                    orders.Where(o => o.Type == (BO.OrderType)filterBy),

                BO.OpenOrderFilterBy.OnTimeStatus =>
                    orders.Where(o => o.ScheduleStatus == (BO.ScheduleStatus)filterBy),

                _ => orders
            };
        }

        orders = sortBy switch
        {
            BO.OpenOrderSortBy.Distance =>
                orders.OrderBy(o => o.AirDistance),

            BO.OpenOrderSortBy.Status =>
                orders.OrderBy(o => o.ScheduleStatus),

            BO.OpenOrderSortBy.OnTimeStatus =>
                orders.OrderBy(o => o.ScheduleStatus),

            _ => orders
        };

        return orders;
    }

    private static DO.DeliveryTransport GetCourierTransport(int courierId)
    {
        var courier = dal.Courier.Read(courierId)
            ?? throw new BO.BlDoesNotExistException($"Courier {courierId} not found.");

        return courier.Transport;
    }
}

