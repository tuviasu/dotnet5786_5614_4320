namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// /// Implementation of ICourier interface for managing Courier entities in memory.
/// </summary>

internal class CourierImplementation : ICourier
{
    /// <summary>
    /// Creates a new Courier item.
    /// </summary>
    /// <param name="item">The Courier item to create.</param>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdAlreadyExist">Thrown when an item with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        if (item.CourierID <= 0)
        {
            throw new DalInvalidId("Invalid ID: " + item.CourierID);
        }

        if (DataSource.Couriers.Any(courier => courier.CourierID == item.CourierID))
        {
            throw new DalIdAlreadyExist("Item with ID " + item.CourierID + " already exists.");
        }

        DataSource.Couriers.Add(item);
    }


    /// <summary>
    /// Deletes a Courier item.
    /// </summary>
    /// <param name="id">The ID of the Courier item to delete.</param>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var courier = Read(id);
        if (courier is null)
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }
        DataSource.Couriers.Remove(courier);
    }

    /// <summary>
    /// Deletes all couriers from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Couriers.Clear();
    }

    /// <summary>
    /// Reads a Courier item by its ID.
    /// </summary>
    /// <param name="id">The ID of the Courier item to read.</param>
    /// <returns>The Courier item if found; otherwise, null.</returns>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id)
    {
        if (id <= 0)
        {
            throw new DalInvalidId("Invalid ID: " + id);
        }

        if (DataSource.Couriers.Count == 0)
        {
            return null;
        }

        if (DataSource.Couriers.All(courier => courier.CourierID != id))
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }

        return DataSource.Couriers.FirstOrDefault(item => item.CourierID == id);
    }

    /// <summary>
    /// Reads all Courier items from the data source.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the Courier items.</param>
    /// <returns>An enumerable collection of Courier items.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        if (filter == null)
            return DataSource.Couriers.Select(item => item);

        else
            return DataSource.Couriers.Where(filter);
    }

    /// <summary>
    /// Updates an existing Courier item.
    /// </summary>
    /// <param name="item">The Courier item to update.</param>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        var courier = Read(item.CourierID);
        if (courier is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }
        DataSource.Couriers.Remove(courier);
        DataSource.Couriers.Add(item);
    }

    /// <summary>
    /// Reads a Courier item from the data source.
    /// </summary>
    /// <param name="filter">The filter to apply when searching for the Courier item.</param>
    /// <returns>The Courier item if found; otherwise, null.</returns>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(Func<Courier, bool> filter)
    {
        var courier = DataSource.Couriers.FirstOrDefault(filter);
        if (courier is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }
        return courier;
    }
}
