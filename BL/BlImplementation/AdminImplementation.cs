using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

/// <summary>
/// Implementation of the <see cref="IAdmin"/> interface.
/// Provides administrative operations such as database management,
/// system clock control, and configuration handling.
/// </summary>
internal class AdminImplementation : IAdmin
{
    /// <summary>
    /// Resets the database to its initial empty state.
    /// Delegates the operation to AdminManager.ResetDB().
    /// </summary>
    /// <exception cref="BO.BlProcessingException">Thrown if the database reset operation fails.</exception>
    public void ResetDB()
    {
        try
        {
            AdminManager.ResetDB();
        }
        catch (Exception ex)
        {

            throw new BO.BlProcessingException($"Failed to reset the database. Details: {ex}");
        }
    }

    /// <summary>
    /// Initializes the database with initial seeded data.
    /// Delegates the operation to AdminManager.InitializeDB().
    /// </summary>
    /// <exception cref="BO.BlProcessingException">Thrown if the database initialization operation fails.</exception>
    public void InitializeDB()
    {
        try
        {
            AdminManager.InitializeDB();
        }
        catch (Exception ex)
        {

            throw new BO.BlProcessingException($"Failed to initialize the database. Details: {ex}");
        }
    }

    /// <summary>
    /// Retrieves the current system clock time.
    /// </summary>
    /// <returns>The current <see cref="DateTime"/> according to the system clock.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    /// <summary>
    /// Advances the system clock by one unit of the specified type.
    /// Uses a switch statement to add the correct time increment.
    /// </summary>
    /// <param name="unit">The unit of time to advance (<see cref="TimeUnit"/>: Minute, Hour, Day, Month, Year).</param>
    /// <exception cref="BO.BlProcessingException">Thrown if the clock update operation fails.</exception>
    public void ForwardClock(TimeUnit unit)
    {
        try
        {
            DateTime currentTime = AdminManager.Now;

            DateTime updatedTime = unit switch
            {
                BO.TimeUnit.MINUTE => currentTime.AddMinutes(1),
                BO.TimeUnit.HOUR => currentTime.AddHours(1),
                BO.TimeUnit.DAY => currentTime.AddDays(1),
                BO.TimeUnit.MONTH => currentTime.AddMonths(1),
                BO.TimeUnit.YEAR => currentTime.AddYears(1),
                _ => currentTime
            };

            AdminManager.UpdateClock(updatedTime);
        }
        catch (Exception ex)
        {
            throw new BO.BlProcessingException($"Failed to forward the clock. Details: {ex}");
        }
    }

    /// <summary>
    /// Retrieves the current configuration values.
    /// Delegates to AdminManager.GetConfig().
    /// </summary>
    /// <returns>A <see cref="Config"/> object containing current system configuration settings.</returns>
    public Config GetConfig()
    {
        return AdminManager.GetConfig();
    }

    /// <summary>
    /// Updates the system configuration based on the provided <see cref="BO.Config"/> object.
    /// Performs validation on time ranges, speeds, and geographical coordinates before saving.
    /// </summary>
    /// <param name="config">A <see cref="Config"/> object containing the configuration values to update.</param>
    /// <exception cref="BO.BlInvalidInputException">Thrown if any configuration value (e.g., non-positive speed, invalid coordinate range) is invalid.</exception>
    /// <exception cref="BO.BlProcessingException">Thrown if an underlying error occurs while trying to save the configuration.</exception>
    public void SetConfig(Config config)
    {
        try
        {
            // 1. Validate time ranges (must be positive)
            if (config.MaxDeliveryTime.TotalSeconds <= 0)
            {
                throw new BO.BlInvalidInputException("Maximum delivery time (MaxDeliveryTime) must be a positive duration.");
            }
            if (config.RiskRange.TotalSeconds <= 0)
            {
                throw new BO.BlInvalidInputException("Risk range (RiskRange) must be a positive duration.");
            }
            if (config.InactivityRange.TotalSeconds <= 0)
            {
                throw new BO.BlInvalidInputException("Inactivity time range (InactivityRange) must be a positive duration.");
            }
            // Ensure risk range is smaller than the maximum delivery range
            if (config.RiskRange >= config.MaxDeliveryTime)
            {
                throw new BO.BlInvalidInputException("Risk range must be strictly smaller than the Maximum Delivery Time Range.");
            }

            // 2. Validate average speeds (must be positive)
            if (config.AvgCarSpeed <= 0)
            {
                throw new BO.BlInvalidInputException("Average car speed must be a positive value.");
            }
            if (config.AvgMotorbikeSpeed <= 0)
            {
                throw new BO.BlInvalidInputException("Average motorcycle speed must be a positive value.");
            }
            if (config.AvgBicycleSpeed <= 0)
            {
                throw new BO.BlInvalidInputException("Average bike speed must be a positive value.");
            }
            if (config.AvgWalkingSpeed <= 0)
            {
                throw new BO.BlInvalidInputException("Average foot speed must be a positive value.");
            }

            // 3. Validate geographical coordinates and distance

            // Ensure coordinates are set as a pair (both set or both null).
            if (config.Latitude.HasValue != config.Longitude.HasValue)
            {
                throw new BO.BlInvalidInputException("Latitude and Longitude must both be set or both be null, they cannot be set independently.");
            }

            if (config.Latitude.HasValue) // Now we know both are either present or missing
            {
                // Validate Latitude range (-90 to 90)
                if (config.Latitude.Value < -90.0 || config.Latitude.Value > 90.0)
                {
                    throw new BO.BlInvalidInputException($"Invalid Latitude: {config.Latitude}. Latitude must be between -90 and 90.");
                }
                // Validate Longitude range (-180 to 180)
                if (config.Longitude is null)
                {
                    throw new BO.BlInvalidInputException("Longitude must be set if Latitude is set.");
                }
                if (config.Longitude.Value < -180.0 || config.Longitude.Value > 180.0)
                {
                    throw new BO.BlInvalidInputException($"Invalid Longitude: {config.Longitude}. Longitude must be between -180 and 180.");
                }
            }

            // Validate maximum air distance (if set, must be positive)
            if (config.MaxRange > 0 && config.MaxRange <= 0)
            {
                throw new BO.BlInvalidInputException("Maximum air distance must be a positive number.");
            }

            AdminManager.SetConfig(config);
        }
        catch (BO.BlInvalidInputException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlProcessingException($"Failed to set configuration. Details: {ex}");
        }
    }

    // Add missing interface implementations to fix CS0535 errors

    public void AdvanceClock(TimeUnit unit)
    {
        ForwardClock(unit);
    }

    public object? GetConfigValue(ConfigVariable variable)
    {
        // Assuming AdminManager.GetConfig() returns a Config object with properties matching ConfigVariable
        var config = AdminManager.GetConfig();
        // Use reflection or a switch to get the value by variable
        // Example using switch (replace with actual logic as needed)
        return variable switch
        {
            ConfigVariable.MaxDeliveryTime => config.MaxDeliveryTime,
            ConfigVariable.RiskRange => config.RiskRange,
            ConfigVariable.InactivityRange => config.InactivityRange,
            ConfigVariable.AvgCarSpeed => config.AvgCarSpeed,
            ConfigVariable.AvgMotorbikeSpeed => config.AvgMotorbikeSpeed,
            ConfigVariable.AvgBicycleSpeed => config.AvgBicycleSpeed,
            ConfigVariable.AvgWalkingSpeed => config.AvgWalkingSpeed,
            ConfigVariable.Latitude => config.Latitude,
            ConfigVariable.Longitude => config.Longitude,
            ConfigVariable.MaxRange => config.MaxRange,
            _ => null
        };
    }

    public void SetConfigValue(ConfigVariable variable, object? value)
    {
        var config = AdminManager.GetConfig();
        // Use switch to set the value (replace with actual logic as needed)
        switch (variable)
        {
            case ConfigVariable.MaxDeliveryTime:
                config.MaxDeliveryTime = (TimeSpan)value!;
                break;
            case ConfigVariable.RiskRange:
                config.RiskRange = (TimeSpan)value!;
                break;
            case ConfigVariable.InactivityRange:
                config.InactivityRange = (TimeSpan)value!;
                break;
            case ConfigVariable.AvgCarSpeed:
                config.AvgCarSpeed = Convert.ToDouble(value);
                break;
            case ConfigVariable.AvgMotorbikeSpeed:
                config.AvgMotorbikeSpeed = Convert.ToDouble(value);
                break;
            case ConfigVariable.AvgBicycleSpeed:
                config.AvgBicycleSpeed = Convert.ToDouble(value);
                break;
            case ConfigVariable.AvgWalkingSpeed:
                config.AvgWalkingSpeed = Convert.ToDouble(value);
                break;
            case ConfigVariable.Latitude:
                config.Latitude = (double?)value;
                break;
            case ConfigVariable.Longitude:
                config.Longitude = (double?)value;
                break;
            case ConfigVariable.MaxRange:
                config.MaxRange = (int)Convert.ToDouble(value);
                break;
            default:
                break;
        }
        SetConfig(config);
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

    public void ResetDatabase()
    {
        ResetDB();
    }

    public void InitializeDatabase()
    {
        InitializeDB();
    }
}