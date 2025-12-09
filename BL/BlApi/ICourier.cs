namespace BlApi;

public interface ICourier
{
    /// <summary>
    /// Attempts to log in the user using username .
    /// Returns the user's role (Manager / Courier).
    /// Throws exception if credentials are wrong.
    /// </summary>
    string Login(string username);

    // === Basic CRUD Operations ===
    IEnumerable<BO.CourierInList> ReadAll(
        int requesterId,
        bool? activeFilter = null,
        BO.CourierListSortBy? sortBy = null
    );
    /// <summary>
    /// Returns full logical data of a courier (BO.Courier).
    /// </summary>
    /// <param name="requesterId">ID of the user requesting the information</param>
    /// <param name="courierId">ID of the courier whose details are requested</param>
    /// <returns>BO.Courier object with full courier details</returns>
    BO.Courier Read(int requesterId, int courierId);

    /// <summary>
    /// Updates an existing courier using a full BO.Courier object.
    /// </summary>
    /// <param name="requesterId">ID of the user requesting the update</param>
    /// <param name="courier">Full BO.Courier object with updated fields</param>
    void Update(int requesterId, BO.Courier courier);

    /// <summary>
    /// Deletes a courier if possible (not currently handling or never handled deliveries).
    /// Throws exception if deletion is not allowed.
    /// </summary>
    /// <param name="requesterId">ID of the user requesting the deletion</param>
    /// <param name="courierId">ID of the courier to delete</param>
    void Delete(int requesterId, int courierId);


    /// <summary>
    /// Creates a new courier using a full BO.Courier object.
    /// </summary>
    /// <param name="requesterId">ID of the user requesting creation</param>
    /// <param name="courier">New courier object to create</param>
    void Create(int requesterId, BO.Courier courier);




}




























