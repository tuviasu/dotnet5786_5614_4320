

namespace BlApi
{
    /// <summary>
    /// Logical Order service interface for all order-related operations.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Returns an array of counts of orders grouped by combined status (order status + on-time status).
        /// </summary>
        int[] GetOrdersSummary(int requesterId);

        /// <summary>
        /// Returns a filtered and sorted list of orders (OrderInList).
        /// </summary>
        IEnumerable<BO.OrderInList> ReadAll(
            int requesterId,
            BO.OrderInListFilterBy? filterBy = null,
            object? filterValue = null,
            BO.OrderInListSortBy? sortBy = null);

        /// <summary>
        /// Returns full logical order details (BO.Order).
        /// </summary>
        BO.Order Read(int requesterId, int orderId);

        /// <summary>
        /// Updates an existing order using a full BO.Order object.
        /// </summary>
        void Update(int requesterId, BO.Order order);

        /// <summary>
        /// Cancels an order (open → simulated canceled delivery, in-progress → close delivery as canceled).
        /// </summary>
        void Cancel(int requesterId, int orderId);

        /// <summary>
        /// Deletes an order. Not allowed. Always throws an exception.
        /// </summary>
        void Delete(int requesterId, int orderId);

        /// <summary>
        /// Creates a new order. Order ID is generated automatically by DAL.
        /// </summary>
        void Create(int requesterId, BO.Order order);

        /// <summary>
        /// Marks a delivery as completed ("Supplied") and updates end time to system clock.
        /// </summary>
        void CompleteTreatment(int requesterId, int courierId, int deliveryId);

        /// <summary>
        /// Assigns an open order to a courier and creates a new delivery record.
        /// </summary>
        void TakeOrder(int requesterId, int courierId, int orderId);

        /// <summary>
        /// Returns a filtered and sorted list of closed deliveries for a courier.
        /// </summary>
        IEnumerable<BO.ClosedDeliveryInList> ReadClosedDeliveries(
            int requesterId,
            int courierId,
            BO.ClosedDeliveryFilterBy? filterBy = null,
            BO.ClosedDeliverySortBy? sortBy = null);

        /// <summary>
        /// Returns a filtered and sorted list of open orders available for a courier,
        /// including aerial distance.
        /// </summary>
        IEnumerable<BO.OpenOrderInList> ReadOpenOrders(
            int requesterId,
            int courierId,
            BO.OpenOrderFilterBy? filterBy = null,
            BO.OpenOrderSortBy? sortBy = null);
    }
}
