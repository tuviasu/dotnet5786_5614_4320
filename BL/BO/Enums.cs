namespace BO


{ 
public enum OrderStatus
{
    /// <summary>
    /// Order was created.
    /// </summary>
    Created,
    /// <summary>
    /// Order is being shipped.
    /// </summary>
    Shipping,
    /// <summary>
    /// Order was delivered.
    /// </summary>
    Delivered,
    /// <summary>
    /// Order was cancelled.
    /// </summary>
    Cancelled
}

public enum DeliveryTransport
{
    /// <summary>
    /// difalt choise no  specifik transport
    /// </summary>
    None,                   
    /// Delivery by bicycle.
    /// </summary>
    Bicycle,
    /// <summary>
    /// Delivery by motorcycle.
    /// </summary>
    Motorcycle,
    /// <summary>
    /// Delivery by car.
    /// </summary>
    Car,
    /// <summary>
    /// Delivery by foot
    /// 
    Foot,
    /// <summary>
    /// Delivery by drone.
    /// </summary>
    Drone,

};

public enum OrderType
{   /// <summary>
    /// Regular order.
    /// </summary>
    Regular,
    /// <summary>
    /// Express order.
    /// </summary>
    Express,
    /// <summary>
    /// International order.
    /// </summary>
    International
}

public enum WeightCategory
{
    /// <summary>
    /// Light weight category.
    /// </summary>
    Light,
    /// <summary>
    /// Medium weight category.
    /// </summary>
    Medium,
    /// <summary>
    /// Heavy weight category.
    /// </summary>
    Heavy
}

public enum PriorityLevel
{
    /// <summary>
    /// Low priority level.
    /// </summary>
    Low,
    /// <summary>
    /// Medium priority level.
    /// </summary>
    Medium,
    /// <summary>
    /// High priority level.
    /// </summary>
    High
}

// chatGPT generated enum for fragility levels
// give me enum in C# for order specifics about fragility

public enum FragilityLevel
{
    /// <summary>
    /// The item is not fragile and can be handled normally.
    /// </summary>
    NotFragile = 0,

    /// <summary>
    /// The item has some delicate parts; handle with moderate care.
    /// </summary>
    SlightlyFragile = 1,

    /// <summary>
    /// The item is fragile and should be handled carefully.
    /// </summary>
    Fragile = 2,

    /// <summary>
    /// The item is very fragile and requires special packaging and handling.
    /// </summary>
    VeryFragile = 3,

    /// <summary>
    /// The item is extremely delicate (e.g., glass, fine art); handle with maximum care.
    /// </summary>
    ExtremelyFragile = 4
}
public enum ScheduleStatus
{
    Pending,      // Scheduled but not yet started
    InProgress,   // Currently being executed
    Completed,    // Successfully completed
    Cancelled     // Cancelled before completion
}

public enum DeliveryCompletionType
{
    Successful,   // Delivered successfully to the customer
    Returned,     // Package was returned to the sender
    Failed,        // Delivery attempt failed (no one received, address issue, etc.)
        Cancelled
    }
public enum CourierListSortBy
{
    Id,
    Name,
    DeliveriesCount,
    ActiveStatus
}
    // Filtering options for OrderInList
    public enum OrderInListFilterBy
    {
        Status,
        CustomerId,
        CourierId,
        OnTimeStatus
    }

    // Sorting options for OrderInList
    public enum OrderInListSortBy
    {
        Id,
        Status,
        CustomerName,
        CreatedDate,
        OnTimeStatus
    }

    // Filtering options for ClosedDeliveryInList
    public enum ClosedDeliveryFilterBy
    {
        DeliveryStatus,
        OnTimeStatus
    }

    // Sorting options for ClosedDeliveryInList
    public enum ClosedDeliverySortBy
    {
        DeliveryStatus,
        EndTime,
        OnTimeStatus
    }

    // Filtering options for OpenOrderInList
    public enum OpenOrderFilterBy
    {
        Status,
        Type,
        OnTimeStatus
    }

    // Sorting options for OpenOrderInList
    public enum OpenOrderSortBy
    {
        Distance,
        Status,
        OnTimeStatus
    }
    /// <summary>
    /// Units of time for advancing the system clock.
    /// </summary>
    public enum TimeUnit
    {
        MINUTE,
        HOUR,
        DAY,
        MONTH,
        YEAR
    }

    /// <summary>
    /// Enum representing each configuration variable name.
    /// </summary>
    public enum ConfigVariable
    {
        Clock,
        ManagerId,
        ManagerPassword,
        CompanyAddress,
        Latitude,
        Longitude,
        MaxRange,
        AvgCarSpeed,
        AvgMotorbikeSpeed,
        AvgBicycleSpeed,
        AvgWalkingSpeed,
        MaxDeliveryTime,
        RiskRange,
        InactivityRange,
        NextOrderId,
        NextDeliveryId
    }

    /// <summary>
    /// Represents the type of distance calculation (e.g., Driving, Walking).
    /// </summary>
    public enum DistanceType
    {
        Driving,
        Walking
    }
    public enum CourierFieldFilter
    {
        Transport,
        IsActive
    }
    /// <summary>
    /// Represents the type of user that logged into the system.
    /// Used by the presentation layer to decide which main screen to open.
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// System administrator.
        /// Has access to the main management screen.
        /// </summary>
        Admin,

        /// <summary>
        /// Courier user.
        /// Has access to the courier screen.
        /// </summary>
        Courier
    }

}


