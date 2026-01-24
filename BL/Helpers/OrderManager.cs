namespace Helpers;
using DalApi;

/// <summary>
/// Manages order-related operations.
/// Provides methods for CRUD operations and business logic for orders.
/// </summary>
internal static class OrderManager
{
    internal static ObserverManager Observers = new();  //stage 5
    private static readonly IDal s_dal = Factory.Get;

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve.</param>
    /// <returns>The order object if found, null otherwise.</returns>
    internal static DO.Order? GetOrderById(int orderId)
    {
        lock (AdminManager.BlMutex) //stage 7
            return s_dal.Order.Read(o => o.OrderID == orderId);
    }

    /// <summary>
    /// Gets all orders from the database.
    /// </summary>
    /// <returns>Collection of all orders.</returns>
    internal static IEnumerable<DO.Order> GetAllOrders()
    {
        lock (AdminManager.BlMutex) //stage 7
            return s_dal.Order.ReadAll().ToList();
    }

    /// <summary>
    /// Gets orders that match the specified filter.
    /// </summary>
    /// <param name="filter">Filter function to apply on orders.</param>
    /// <returns>Collection of orders matching the filter.</returns>
    internal static IEnumerable<DO.Order> GetOrdersByFilter(Func<DO.Order, bool> filter)
    {
        lock (AdminManager.BlMutex) //stage 7
            return s_dal.Order.ReadAll(filter).ToList();
    }

    /// <summary>
    /// Adds a new order to the database.
    /// </summary>
    /// <param name="order">The order object to add.</param>
    internal static void AddOrder(DO.Order order)
    {
        lock (AdminManager.BlMutex) //stage 7
            s_dal.Order.Create(order);
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Updates an existing order in the database.
    /// </summary>
    /// <param name="order">The order object with updated information.</param>
    internal static void UpdateOrder(DO.Order order)
    {
        lock (AdminManager.BlMutex) //stage 7
            s_dal.Order.Update(order);
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Deletes an order from the database by its ID.
    /// </summary>
    /// <param name="orderId">The ID of the order to delete.</param>
    internal static void DeleteOrder(int orderId)
    {
        lock (AdminManager.BlMutex) //stage 7
            s_dal.Order.Delete(orderId);
        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Checks if an order exists in the database.
    /// </summary>
    /// <param name="orderId">The ID of the order to check.</param>
    /// <returns>True if the order exists, false otherwise.</returns>
    internal static bool IsOrderExists(int orderId)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Order.Read(o => o.OrderID == orderId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the destination coordinates of an order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>Tuple containing latitude and longitude of the order destination.</returns>
    internal static (double Latitude, double Longitude) GetOrderDestination(int orderId)
    {
        var order = GetOrderById(orderId);
        return (order!.Latitude, order.Longitude);
    }

    /// <summary>
    /// Gets the creation/opening time of an order.
    /// </summary>
    /// <param name="orderId">The ID of the order.</param>
    /// <returns>The date and time when the order was created.</returns>
    internal static DateTime GetOrderCreationTime(int orderId)
    {
        var order = GetOrderById(orderId);
        return order!.OrderOpeningTime;
    }

    /// <summary>
    /// Calculates the distance between two geographic coordinates using the Haversine formula.
    /// </summary>
    /// <param name="lat1">Latitude of the first point.</param>
    /// <param name="lon1">Longitude of the first point.</param>
    /// <param name="lat2">Latitude of the second point.</param>
    /// <param name="lon2">Longitude of the second point.</param>
    /// <returns>Distance in kilometers.</returns>
    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Angle in degrees.</param>
    /// <returns>Angle in radians.</returns>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /// <summary>
    /// Gets all unique order IDs that have been assigned to a specific courier.
    /// </summary>
    /// <param name="courierId">The ID of the courier.</param>
    /// <returns>Collection of distinct order IDs.</returns>
    internal static IEnumerable<int> GetOrderIdsByCourier(int courierId)
    {
        IEnumerable<DO.Delivery> deliveries;
        lock (AdminManager.BlMutex) //stage 7
            deliveries = s_dal.Delivery.ReadAll(d => d.CourierID == courierId).ToList();
        return deliveries.Select(d => d.OrderID).Distinct();
    }

    /// <summary>
    /// Counts the number of orders that match the specified status filter.
    /// </summary>
    /// <param name="statusFilter">Filter function to apply on orders.</param>
    /// <returns>Count of orders matching the status filter.</returns>
    internal static int CountOrdersByStatus(Func<DO.Order, bool> statusFilter)
    {
        lock (AdminManager.BlMutex) //stage 7
            return s_dal.Order.ReadAll(statusFilter).Count();
    }
}
