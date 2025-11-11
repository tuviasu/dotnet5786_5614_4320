namespace DalApi;

/// <summary>
/// Generic CRUD interface for all data entities.
/// Defines basic Create, Read, Update and Delete operations.
/// </summary>
/// <typeparam name="T">Entity type (for example: Student, Course, etc.)</typeparam>
public interface ICrud<T> where T : class
{
    /// <summary>
    /// Creates a new entity in the data source.
    /// </summary>
    void Create(T item);

    /// <summary>
    /// Reads a single entity by its unique ID.
    /// Returns null if the entity does not exist.
    /// </summary>
    T? Read(int id);

    /// <summary>
    /// Reads all entities (stage 1 version - returns all items in a List).
    /// </summary>
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2

    /// <summary>
    /// Updates an existing entity in the data source.
    /// </summary>
    void Update(T item);

    /// <summary>
    /// Deletes an entity by its unique ID.
    /// </summary>
    void Delete(int id);

    /// <summary>
    /// Deletes all entities from the data source.
    /// </summary>
    void DeleteAll();
    T? Read(Func<T, bool> filter); // stage 2
}
