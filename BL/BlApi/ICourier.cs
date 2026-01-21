namespace BlApi;

/// <summary>
/// Service interface for Courier entity business logic.
/// Provides operations for courier management, authentication, and retrieval.
/// </summary>
public interface ICourier: IObservable
{
    /// <summary>
    /// Authenticates a courier user with username and password.
    /// </summary>
    /// <param name="username">Courier username/ID</param>
    /// <param name="password">Courier password</param>
    /// <returns>User role ("Manager" or "Courier")</returns>
    /// <exception cref="BO.BLItemNotFoundException">If courier not found or password incorrect</exception>
    string AuthenticateCourier(string username, string password);

    /// <summary>
    /// Gets a filtered list of couriers for management operations.
    /// </summary>
    /// <param name="requesterId">ID of the requesting user (for audit/authorization)</param>
    /// <param name="isActive">Optional filter: true for active couriers, false for inactive, null for all</param>
    /// <param name="sortBy">Optional sort property: null sorts by ID, otherwise sorts by specified property</param>
    /// <returns>Collection of CourierInList entities matching filter and sort criteria</returns>
    IEnumerable<BO.CourierInList> GetCouriersList(int requesterId, bool? isActive = null, string? sortBy = null);

    /// <summary>
    /// Gets detailed information about a specific courier including current delivery if any.
    /// </summary>
    /// <param name="requesterId">ID of the requesting user (for audit/authorization)</param>
    /// <param name="courierId">ID of the courier to retrieve</param>
    /// <returns>Courier object with full details and current order in progress if applicable</returns>
    /// <exception cref="BO.BLItemNotFoundException">If courier not found</exception>
    BO.Courier GetCourierDetails(int requesterId, int courierId);

    /// <summary>
    /// Updates existing courier information.
    /// </summary>
    /// <param name="requesterId">ID of the requesting user (for audit/authorization)</param>
    /// <param name="courier">Courier object with updated information</param>
    /// <exception cref="BO.BLItemNotFoundException">If courier not found</exception>
    /// <exception cref="BO.BLInvalidIdException">If courier data is invalid</exception>
    void UpdateCourier(int requesterId, BO.Courier courier);

    /// <summary>
    /// Deletes a courier from the system.
    /// Courier can only be deleted if not currently handling any deliveries.
    /// </summary>
    /// <param name="requesterId">ID of the requesting user (for audit/authorization)</param>
    /// <param name="courierId">ID of the courier to delete</param>
    /// <exception cref="BO.BLItemNotFoundException">If courier not found</exception>
    /// <exception cref="BO.BLInvalidIdException">If courier cannot be deleted (has active/historical deliveries)</exception>
    void DeleteCourier(int requesterId, int courierId);

    /// <summary>
    /// Creates and adds a new courier to the system.
    /// </summary>
    /// <param name="requesterId">ID of the requesting user (for audit/authorization)</param>
    /// <param name="courier">New courier object to add</param>
    /// <exception cref="BO.BLItemAlreadyExistsException">If courier with same ID already exists</exception>
    /// <exception cref="BO.BLInvalidIdException">If courier data is invalid</exception>
    void AddCourier(int requesterId, BO.Courier courier);
}
