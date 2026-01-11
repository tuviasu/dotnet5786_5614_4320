


using BO;

namespace BlApi
{
    /// <summary>
    /// Logical Admin service interface for system management operations.
    /// </summary>
    public interface IAdmin
    {
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5

        /// <summary>
        /// Returns the current system clock value.
        /// </summary>
        DateTime GetClock();

        /// <summary>
        /// Advances the system clock by a selected time unit (minute, hour, day, month, year).
        /// </summary>
        /// <param name="unit">Time unit to advance by</param>
        void ForwardClock(BO.TimeUnit unit);

        /// <summary>
        /// Gets the value of a configuration variable (X).
        /// </summary>
        /// <param name="variable">The configuration variable to fetch</param>
        /// <returns>The value of the configuration variable</returns>
        object? GetConfigValue(BO.ConfigVariable variable);

        /// <summary>
        /// Sets a configuration variable (X) to a new value.
        /// </summary>
        /// <param name="variable">Configuration variable to update</param>
        /// <param name="value">New value for the configuration variable</param>
        void SetConfigValue(BO.ConfigVariable variable, object? value);

        /// <summary>
        /// Resets the database: clears all entities and resets configuration values.
        /// </summary>
        void ResetDatabase();

        /// <summary>
        /// Initializes the database:
        /// resets database and then loads initial required data.
        /// </summary>
        BO.UserType Login(int id, string password);

        Config GetConfig();
        void SetConfig(Config config);
        UserType Login(int id);

        void InitializeDatabase();
    }
}
