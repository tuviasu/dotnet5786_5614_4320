namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Implements CRUD operations for Delivery entities inside the DAL.
/// </summary>
internal class DeliveryImplementation : IDelivery
{
    /// <summary>
    /// Create a new delivery in the DataSource list.
    /// Throws exception if delivery with same ID already exists.
    /// </summary>
    public void Create(Delivery item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Delivery with ID={item.Id} already exists");

        DataSource.Deliveries.Add(item);
    }

    /// <summary>
    /// Read a delivery by ID. Returns null if not found.
    /// </summary>
    public Delivery? Read(int id)
    {
        //return DataSource.Deliveries.FirstOrDefault(d => d.Id == id);//stage 1
        return DataSource.Deliveries.FirstOrDefault(d => d.Id == id);//stage 2
    }

    /// <summary>
    /// Read all deliveries.
    /// </summary>
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) //stage 2
    {
        return filter == null
            ? DataSource.Deliveries.Select(item => item)
            : DataSource.Deliveries.Where(filter);
    }

    // Stage 2 - New generic Read method with filter
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        // Returns the first Delivery matching the given condition
        return DataSource.Deliveries.FirstOrDefault(filter);
    }

    /// <summary>
    /// Update an existing delivery.
    /// Throws exception if not found.
    /// </summary>
    public void Update(Delivery item)
    {
        Delivery? existing = Read(item.Id);
        if (existing is null)
            throw new DalDoesNotExistException($"Delivery with ID={item.Id} not found");

        DataSource.Deliveries.Remove(existing);
        DataSource.Deliveries.Add(item);
    }

    /// <summary>
    /// Delete a delivery by ID.
    /// Throws exception if not found.
    /// </summary>
    public void Delete(int id)
    {
        Delivery? existing = Read(id);
        if (existing is null)
            throw new DalDoesNotExistException($"Delivery with ID={id} not found");

        DataSource.Deliveries.Remove(existing);
    }

    /// <summary>
    /// Delete all deliveries.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Deliveries.Clear();
    }
}

