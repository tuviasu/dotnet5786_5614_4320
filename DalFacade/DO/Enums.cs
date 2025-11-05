namespace DO;

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

public enum  OrderType
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
public enum DeliveryCompletionType
{
    Successful,   // Delivered successfully to the customer
    Returned,     // Package was returned to the sender
    Failed        // Delivery attempt failed (no one received, address issue, etc.)
}
