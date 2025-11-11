namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Implements CRUD operations for Order entities inside the DAL.
/// </summary>
internal class OrderImplementation : IOrder
{
    /// <summary>
    /// Create a new order in the DataSource list.
    /// Throws exception if an order with the same ID already exists.
    /// </summary>
    public void Create(Order item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Order with ID={item.Id} already exists");

        DataSource.Orders.Add(item);
    }

    /// <summary>
    /// Read an order by ID. Returns null if not found.
    /// </summary>
    public Order? Read(int id)
    {
        //return DataSource.Orders.Find(o => o.Id == id);// stage 1
        return DataSource.Orders.FirstOrDefault(o => o.Id == id);// stage 2
    }

    /// <summary>
    /// Read all orders.
    /// </summary>
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) //stage 2
    {
        return filter == null
            ? DataSource.Orders.Select(item => item)
            : DataSource.Orders.Where(filter);
    }
    public Order? Read(Func<Order, bool> filter)
    {
        // Stage 2 - Use LINQ to return the first item that matches the condition
        return DataSource.Orders.FirstOrDefault(filter);
    }

    /// <summary>
    /// Update an existing order.
    /// Throws exception if not found.
    /// </summary>
    public void Update(Order item)
    {
        Order? existing = Read(item.Id);
        if (existing is null)
            throw new DalDoesNotExistException($"Order with ID={item.Id} not found");
 
        DataSource.Orders.Remove(existing);
        DataSource.Orders.Add(item);
    }

    /// <summary>
    /// Delete an order by ID.
    /// Throws exception if not found.
    /// </summary>
    public void Delete(int id)
    {
        Order? existing = Read(id);
        if (existing is null)
            throw new DalDoesNotExistException($"Order with ID={id} not found");

        DataSource.Orders.Remove(existing);
    }

    /// <summary>
    /// Delete all orders.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Orders.Clear();
    }
}
