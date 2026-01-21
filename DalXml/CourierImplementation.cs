namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class CourierImplementation : ICourier
{
    /// <summary>
    /// Creates a new Courier item.
    /// </summary>
    /// <param name="item">The Courier item to create.</param>
    /// <exception cref="ArgumentNullException">Thrown when the item is null.</exception>
    /// <exception cref="DalIdAlreadyExist">Thrown when an item with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        if (item == null) {
            throw new ArgumentNullException("item");
        }
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);
        if (couriers.Exists(it => it.CourierID == item.CourierID))
            throw new DalIdAlreadyExist("Courier with ID " + item.CourierID + " already exists.");
        couriers.Add(item);
        XMLTools.SaveListToXMLSerializer(couriers, Config.CouriersFileName);
    }

    /// <summary>
    /// Deletes a Courier item.
    /// </summary>
    /// <param name="id">The ID of the Courier item to delete.</param>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);
        if (Couriers.RemoveAll(it => it.CourierID == id) == 0)
            throw new DalIdNotExist("Courier with ID " + id + " does not exist.");
        XMLTools.SaveListToXMLSerializer(Couriers, Config.CouriersFileName);
    }

    /// <summary>
    /// Deletes all Courier items.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Courier>(), Config.CouriersFileName);
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
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);

        if(id <= 0)
            throw new DalInvalidId("Invalid ID: " + id);

        if(couriers.All(courier => courier.CourierID != id))
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");

        return couriers.FirstOrDefault(it => it.CourierID == id);
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
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);

        if(couriers.FirstOrDefault(filter) is null)
            throw new DalItemNotExist("This item does not exist.");

        return couriers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Courier items from the data source.
    /// </summary>
    /// <param name="filter">The filter to apply when searching for Courier items.</param>
    /// <returns>An enumerable collection of Courier items.</returns>
    /// <exception cref="DalEmptyCollection">Thrown when the collection is empty.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);

        // Align behavior with DalList: empty list is not an error; return empty sequence.
        if (couriers.Count == 0)
            return Enumerable.Empty<Courier>();

        if (filter is null)
            return couriers;

        return couriers.Where(filter);
    }

    /// <summary>
    /// Updates an existing Courier item.
    /// </summary>
    /// <param name="item">The Courier item to update.</param>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.CouriersFileName);
        if (couriers.RemoveAll(it => it.CourierID == item.CourierID) == 0)
            throw new DalItemNotExist("Courier with ID " + item.CourierID + " does not exist.");
        couriers.Add(item);
        XMLTools.SaveListToXMLSerializer(couriers, Config.CouriersFileName);
    }
}
