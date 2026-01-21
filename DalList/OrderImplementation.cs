namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class OrderImplementation : IOrder
{
    /// <summary>
    /// Creates a new Order item.
    /// </summary>
    /// <param name="item">The Order item to create.</param>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdAlreadyExist">Thrown when an item with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        if (item.OrderID < 0)
            throw new DalInvalidId("Invalid ID: " + item.OrderID);

        // DAL assigns ID when caller passes 0
        if (item.OrderID == 0)
            item = item with { OrderID = Config.NextOrderID };

        if (item.OrderID <= 0)
            throw new DalInvalidId("Invalid ID: " + item.OrderID);

        if (DataSource.Orders.Any(order => order.OrderID == item.OrderID))
            throw new DalIdAlreadyExist("Item with ID " + item.OrderID + " already exists.");

        DataSource.Orders.Add(item);
    }

    /// <summary>
    /// Deletes an Order item.
    /// </summary>
    /// <param name="id">The ID of the Order item to delete.</param>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var order = Read(id);
        if (order is null)
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }
        DataSource.Orders.Remove(order);
    }

    /// <summary>
    /// Deletes all orders from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Orders.Clear();
    }

    /// <summary>
    /// Reads an Order item by its ID.
    /// </summary>
    /// <param name="id">The ID of the Order item to read.</param>
    /// <returns>The Order item if found; otherwise, null.</returns>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {
        if (id <= 0)
        {
            throw new DalInvalidId("Invalid ID: " + id);
        }

        if (DataSource.Orders.Count == 0)
        {
            return null;
        }

        if (DataSource.Orders.All(order => order.OrderID != id))
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }

        return DataSource.Orders.FirstOrDefault(item => item.OrderID == id);

    }

    /// <summary>
    /// Reads all Order items from the data source.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the Order items.</param>
    /// <returns>An enumerable collection of Order items.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        if (filter == null)
            return DataSource.Orders.Select(item => item);

        else
            return DataSource.Orders.Where(filter);
    }

    /// <summary>
    /// Updates an existing Order item.
    /// </summary>
    /// <param name="item">The Order item to update.</param>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        var order = Read(item.OrderID);
        if (order is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }
        DataSource.Orders.Remove(order);
        DataSource.Orders.Add(item);
    }

    /// <summary>
    /// Reads an Order item from the data source.
    /// </summary>
    /// <param name="filter">The filter to apply when searching for the Order item.</param>
    /// <returns>The Order item if found; otherwise, null.</returns>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter)
    {
        return DataSource.Orders.FirstOrDefault(filter);
    }
}