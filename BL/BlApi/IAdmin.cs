namespace BlApi;

/// <summary>
/// Service interface for system administration and configuration.
/// Provides operations for system management, database initialization, and simulator control.
/// </summary>
public interface IAdmin
{
    /// <summary>
    /// Authenticates a manager with the given credentials.
    /// </summary>
    /// <param name="managerId">The ID of the manager</param>
    /// <param name="password">The password of the manager</param>
    /// <returns>True if authentication is successful, false otherwise</returns>
    bool AuthenticateManager(int managerId, string password);

    /// <summary>
    /// Resets the database to initial state.
    /// </summary>
    void ResetDB(int requesterId);

    /// <summary>
    /// Initializes the database with default data.
    /// </summary>
    void InitializeDB(int requesterId);

    /// <summary>
    /// Gets the current system clock time.
    /// </summary>
    /// <returns>Current system DateTime</returns>
    DateTime GetClock(int requesterId);

    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="timeUnit">Time unit to advance by</param>
    void ForwardClock(int requesterId, BO.TimeUnit timeUnit);

    /// <summary>
    /// Gets the current configuration settings.
    /// </summary>
    /// <returns>Configuration object</returns>
    BO.Config GetConfig(int requesterId);

    /// <summary>
    /// Sets the configuration settings.
    /// </summary>
    /// <param name="config">Configuration object to set</param>
    void SetConfig(int requesterId, BO.Config config);

    #region Stage 7
    /// <summary>
    /// Starts the simulator thread. Advances the clock by <paramref name="intervalMinutes"/> minutes every second.
    /// </summary>
    /// <param name="requesterId">Manager requester ID</param>
    /// <param name="intervalMinutes">Clock advance interval in minutes per second</param>
    void StartSimulator(int requesterId, int intervalMinutes);

    /// <summary>
    /// Stops the simulator thread.
    /// </summary>
    /// <param name="requesterId">Manager requester ID</param>
    void StopSimulator(int requesterId);
    void ResetClock(string requesterId);

    #endregion Stage 7



    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
}
