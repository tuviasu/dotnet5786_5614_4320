namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class DeliveryImplementation : IDelivery
{
    /// <summary>
    /// Creates a new Delivery item.
    /// </summary>
    /// <param name="item">The Delivery item to create.</param>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdAlreadyExist">Thrown when an item with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        if (item.DeliveryID < 0)
            throw new DalInvalidId("Invalid ID: " + item.DeliveryID);

        // DAL assigns ID when caller passes 0 (same behavior as DalXml)
        if (item.DeliveryID == 0)
            item = item with { DeliveryID = Config.NextDeliveryID };

        if (item.DeliveryID <= 0)
            throw new DalInvalidId("Invalid ID: " + item.DeliveryID);

        if (DataSource.Deliverys.Any(delivery => delivery.DeliveryID == item.DeliveryID))
            throw new DalIdAlreadyExist("Item with ID " + item.DeliveryID + " already exists.");

        DataSource.Deliverys.Add(item);
    }

    /// <summary>
    /// Deletes a Delivery item.
    /// </summary>
    /// <param name="id">The ID of the Delivery item to delete.</param>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var delivery = Read(id);
        if (delivery is null)
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }
        DataSource.Deliverys.Remove(delivery);
    }

    /// <summary>
    /// Deletes all Delivery items from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Deliverys.Clear();
    }

    /// <summary>
    /// Reads a Delivery item by its ID.
    /// </summary>
    /// <param name="id">The ID of the Delivery item to read.</param>
    /// <returns>The Delivery item if found; otherwise, null.</returns>
    /// <exception cref="DalInvalidId">Thrown when the provided ID is invalid.</exception>
    /// <exception cref="DalIdNotExist">Thrown when the item with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {
        if (id <= 0)
        {
            throw new DalInvalidId("Invalid ID: " + id);
        }

        if (DataSource.Deliverys.Count == 0)
        {
            return null;
        }

        if (DataSource.Deliverys.All(delivery => delivery.DeliveryID != id))
        {
            throw new DalIdNotExist("Item with ID " + id + " does not exist.");
        }

        return DataSource.Deliverys.FirstOrDefault(item => item.DeliveryID == id);
    }

    /// <summary>
    /// Reads all Delivery items from the data source.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the Delivery items.</param>
    /// <returns>An enumerable collection of Delivery items.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        if (filter == null)
            return DataSource.Deliverys.Select(item => item);

        else
            return DataSource.Deliverys.Where(filter);
    }

    /// <summary>
    /// Updates an existing Delivery item.
    /// </summary>
    /// <param name="item">The Delivery item to update.</param>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        var delivery = Read(item.DeliveryID);
        if (delivery is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }
        DataSource.Deliverys.Remove(delivery);
        DataSource.Deliverys.Add(item);
    }

    /// <summary>
    /// Reads a Delivery item from the data source.
    /// </summary>
    /// <param name="filter">The filter to apply when searching for the Delivery item.</param>
    /// <returns>The Delivery item if found; otherwise, null.</returns>
    /// <exception cref="DalItemNotExist">Thrown when the item does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        var delivery = DataSource.Deliverys.FirstOrDefault(filter);
        if (delivery is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }
        return delivery;
    }   
}