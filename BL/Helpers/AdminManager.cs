//using BO;
using BO;
using System.Runtime.CompilerServices;

namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Configuration Variables and Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4-7
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4

    private static void ThrowIfNotManager(int requesterId)
    {
        if (!s_dal.Config.Managers.ContainsKey(requesterId))
            throw new BO.BlInvalidIdException("Access denied - manager permissions required", null);
    }

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    internal static event Action? ConfigUpdatedObservers; //stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //stage 5 - for clock update observers

    /// <summary>
    /// Method to update application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        var oldClock = s_dal.Config.Clock; //stage 4
        s_dal.Config.Clock = newClock; //stage 4
        
        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic updates:
        // - Go through all couriers/orders/deliveries to update properties that are affected by the clock update
        // - (orders become overdue, deliveries status changes, etc.)

        //stage 7
        // Fire-and-forget: periodic updates should not block the simulator thread.
        _ = Task.Run(() => AdminManager.PeriodicManagersUpdates(oldClock, newClock));

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }

    /// <summary>
    /// Method for getting current configuration variables values for any BL class that may need it
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static Config GetConfig() //stage 4
        => new Config()
        {
            Clock = s_dal.Config.Clock,
            CompanyAddress = s_dal.Config.CompanyAddress,
            Latitude = s_dal.Config.Latitude,
            Longitude = s_dal.Config.Longitude,
            MaxDeliveryDistance = s_dal.Config.MaxDeliveryDistance,
            MaxDeliveryTimeRange = s_dal.Config.MaxDeliveryTimeRange,
            RiskRange = s_dal.Config.RiskRange,
            InactivityTimeRange = s_dal.Config.InactivityTimeRange
        };

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static Config GetConfig(int requesterId)
    {
        ThrowIfNotManager(requesterId);
        return GetConfig();
    }

    /// <summary>
    /// Method for setting current configuration variables values for any BL class that may need it
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void SetConfig(BO.Config configuration) //stage 4
    {
        bool configChanged = false; // stage 5

        if (s_dal.Config.Clock != configuration.Clock) //stage 4
        {
            s_dal.Config.Clock = configuration.Clock;
            configChanged = true;
        }

        if (s_dal.Config.CompanyAddress != configuration.CompanyAddress)
        {
            s_dal.Config.CompanyAddress = configuration.CompanyAddress;
            configChanged = true;
        }

        if (s_dal.Config.Latitude != configuration.Latitude)
        {
            s_dal.Config.Latitude = configuration.Latitude;
            configChanged = true;
        }

        if (s_dal.Config.Longitude != configuration.Longitude)
        {
            s_dal.Config.Longitude = configuration.Longitude;
            configChanged = true;
        }

        if (s_dal.Config.MaxDeliveryDistance != configuration.MaxDeliveryDistance) //stage 4
        {
            s_dal.Config.MaxDeliveryDistance = configuration.MaxDeliveryDistance;
            configChanged = true;
        }

        if (s_dal.Config.MaxDeliveryTimeRange != configuration.MaxDeliveryTimeRange)
        {
            s_dal.Config.MaxDeliveryTimeRange = configuration.MaxDeliveryTimeRange;
            configChanged = true;
        }

        if (s_dal.Config.RiskRange != configuration.RiskRange)
        {
            s_dal.Config.RiskRange = configuration.RiskRange;
            configChanged = true;
        }

        if (s_dal.Config.InactivityTimeRange != configuration.InactivityTimeRange)
        {
            s_dal.Config.InactivityTimeRange = configuration.InactivityTimeRange;
            configChanged = true;
        }

        //Calling all the observers of configuration update
        if (configChanged) // stage 5
            ConfigUpdatedObservers?.Invoke(); // stage 5
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void SetConfig(int requesterId, BO.Config configuration)
    {
        ThrowIfNotManager(requesterId);
        SetConfig(configuration);
    }

    /// <summary>
    /// Periodic updates for managers after clock changes
    /// </summary>
    /// <param name="oldClock">Previous clock value</param>
    /// <param name="newClock">New clock value</param>
    private static void PeriodicManagersUpdates(DateTime oldClock, DateTime newClock)
    {
        // Add periodic update logic here
        // For example:
        // - Update order statuses based on time
        // - Check for overdue deliveries
        // - Update courier availability
        // - etc.
    }

    internal static void ResetDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            s_dal.ResetDB(); //stage 4
            AdminManager.UpdateClock(AdminManager.Now); //stage 5 - needed since we want the label on Pl to be updated
            AdminManager.GetConfig(); //stage 5 - sync DAL config to BO
        }
    }

    internal static void ResetDB(int requesterId) //stage 4-7
    {
        ThrowIfNotManager(requesterId);
        ResetDB();
    }

    internal static void InitializeDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            DalTest.Initialization.Do(); //stage 4
            AdminManager.UpdateClock(AdminManager.Now);  //stage 5 - needed since we want the label on Pl to be updated           
            AdminManager.GetConfig(); //stage 5 - sync DAL config to BO
        }
    }

    internal static void InitializeDB(int requesterId) //stage 4-7
    {
        ThrowIfNotManager(requesterId);
        InitializeDB();
    }
    #endregion Stage 4-7

    private static readonly AsyncMutex s_periodicMutex = new(); //stage 7

    #region Stage 7 base

    /// <summary>    
    /// Mutex to use from BL methods to get mutual exclusion while the simulator is running
    /// </summary>
    internal static readonly object BlMutex = new(); // BlMutex = s_dal; // This field is actually the same as s_dal - it is defined for readability of locks
    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static volatile Thread? s_thread;
    /// <summary>
    /// The Interval for clock updating
    /// in minutes by second (default value is 1, will be set on Start())    
    /// </summary>
    private static int s_interval = 1;
    /// <summary>
    /// The flag that signs whether simulator is running
    /// </summary>
    private static volatile bool s_stop = false;

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
            throw new BO.BlTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt(); //awake a sleeping thread
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }
    }

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            //stage 7
            //Add calls here to any logic simulation that was required in stage 7
            //for example: delivery assignment simulation, order processing simulation
            _ = Task.Run(() => CourierManager.SimulateCouriersActivityAsync());

            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }

    /// <summary>
    /// Simulates automatic delivery assignment and processing
    /// </summary>
    // Simulation logic is implemented inside relevant manager classes (stage 7)

    #endregion Stage 7 base
}
