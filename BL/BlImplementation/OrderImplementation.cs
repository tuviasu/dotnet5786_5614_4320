namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;

/// <summary>
/// BL service implementation for logical Order operations.
/// All logic uses OrderManager + DeliveryManager + AdminManager + DAL.
/// </summary>
internal class OrderImplementation : IOrder
{
    // =============================================================
    // 1) GetOrdersSummary
    // =============================================================
    public int[] GetOrdersSummary(int requesterId)
    {
        // Only logic delegation – business rules are inside OrderManager
        return OrderManager.GetOrdersSummary(requesterId);
    }

    // =============================================================
    // 2) Read (single order)
    // =============================================================
    public BO.Order Read(int requesterId, int orderId)
    {
        return OrderManager.GetOrder(requesterId, orderId);
    }

    // =============================================================
    // 3) ReadAll (list of order-in-list)
    // =============================================================
    public IEnumerable<BO.OrderInList> ReadAll(
        int requesterId,
        BO.OrderInListFilterBy? filterBy = null,
        object? filterValue = null,
        BO.OrderInListSortBy? sortBy = null)
    {
        return OrderManager.GetOrders(requesterId, filterBy, filterValue, sortBy);
    }

    // =============================================================
    // 4) Create new order
    // =============================================================
    public void Create(int requesterId, BO.Order order)
    {
        OrderManager.CreateOrder(requesterId, order);
    }

    // =============================================================
    // 5) Update existing order
    // =============================================================
    public void Update(int requesterId, BO.Order order)
    {
        OrderManager.UpdateOrder(requesterId, order);
    }

    // =============================================================
    // 6) Delete order (BLTest only)
    // =============================================================
    public void Delete(int requesterId, int orderId)
    {
        // OrderManager does not have DeleteOrder. Use CancelOrder instead.
        OrderManager.CancelOrder(requesterId, orderId);
    }

    // =============================================================
    // 7) Cancel order
    // =============================================================
    public void Cancel(int requesterId, int orderId)
    {
        OrderManager.CancelOrder(requesterId, orderId);
    }

    // =============================================================
    // 8) TakeOrder – courier selects order for treatment
    // =============================================================
    public void TakeOrder(int requesterId, int courierId, int orderId)
    {
        OrderManager.AssignOrderToCourier(requesterId, courierId, orderId);
    }

    // =============================================================
    // 9) CompleteTreatment – finish delivery
    // =============================================================
    public void CompleteTreatment(int requesterId, int courierId, int deliveryId)
    {
        DeliveryManager.CompleteDelivery(deliveryId);
    }

    // =============================================================
    // 10) Read closed deliveries of a courier
    // =============================================================
    public IEnumerable<BO.ClosedDeliveryInList> ReadClosedDeliveries(
        int requesterId,
        int courierId,
        BO.ClosedDeliveryFilterBy? filterBy = null,
        BO.ClosedDeliverySortBy? sortBy = null)
    {
        return DeliveryManager.GetClosedDeliveries(courierId, filterBy, sortBy, null);
    }

    // =============================================================
    // 11) Read open orders available for courier assignment
    // =============================================================
    public IEnumerable<BO.OpenOrderInList> ReadOpenOrders(
        int requesterId,
        int courierId,
        BO.OpenOrderFilterBy? filterBy = null,
        BO.OpenOrderSortBy? sortBy = null)
    {
        return DeliveryManager.GetOpenOrders(courierId, filterBy, sortBy);
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
        OrderManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        OrderManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        OrderManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        OrderManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

}
