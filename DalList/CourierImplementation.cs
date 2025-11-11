namespace Dal;
using DalApi;
using DO;
using System.Linq;

/// <summary>
/// Implements CRUD operations for Courier entities inside the DAL.
/// </summary>
internal class CourierImplementation : ICourier
{
    /// <summary>
    /// Create a new courier in the DataSource list.
    /// Throws exception if courier with same ID already exists.
    /// </summary>
    public void Create(Courier item)
    {
        // check if courier already exists
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Courier with ID={item.Id} already exists");

        DataSource.Couriers.Add(item);
    }

    /// <summary>
    /// Read a courier by ID. Returns null if not found.
    /// </summary>
    public Courier? Read(int id)
    {
        //return DataSource.Couriers.Find(c => c.Id == id);// stage 1
        return DataSource.Couriers.FirstOrDefault(c => c.Id == id);// stage 2

    }

    /// <summary>
    /// Read all couriers.
    /// </summary>
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) //stage 2
    {
        return filter == null
            ? DataSource.Couriers.Select(item => item)
            : DataSource.Couriers.Where(filter);
    }


    /// <summary>
    /// Update an existing courier.
    /// Throws exception if courier not found.
    /// </summary>
    public void Update(Courier item)
    {
        Courier? existing = Read(item.Id);
        if (existing is null)
            throw new DalDoesNotExistException($"Courier with ID={item.Id} not found");

        DataSource.Couriers.Remove(existing);
        DataSource.Couriers.Add(item);
    }

    /// <summary>
    /// Delete a courier by ID.
    /// Throws exception if not found.
    /// </summary>
    public void Delete(int id)
    {
        Courier? existing = Read(id);
        if (existing is null)
            throw new DalDoesNotExistException($"Courier with ID={id} not found");

        DataSource.Couriers.Remove(existing);
    }
    // Stage 2 - New generic Read method with filter
    public Courier? Read(Func<Courier, bool> filter)
    {
        // Returns the first Courier matching the given condition
        return DataSource.Couriers.FirstOrDefault(filter);
    }
    /// <summary>
    /// Delete all couriers.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Couriers.Clear();
    }
}
