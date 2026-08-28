namespace BO;

public enum DeliveryTransport
{
    Car,            // For long-distance or large deliveries
    Motorcycle,     // Standard and fast city deliveries
    Bicycle,        // Short-distance, eco-friendly deliveries
    Walk,           // Very close deliveries (near the branch)
    All             // All transport types
}

public enum DeliveryType
{
    Regular,    // Standard delivery (30–60 minutes)
    Express     // Fast delivery (up to 20 minutes)
}

public enum OrderType
{
    Individual,     // Individual order
    Group,          // Order placed by a group of people
    Corporate       // Order placed by a company or organization
}



public enum OrderStatus
{
    NotDelivered,       // Open / still not closed
    Delivered,          // Closed - delivered
    CustomerRefused,    // Closed - customer refused
    Cancelled           // Closed - cancelled
}

public enum ScheduleStatus
{
    OnTime,     // On time
    InRisk,     // In risk window
    Late        // Late
}

public enum DeviceType
{
    Desktop,     // Desktop computer
    Laptop,      // Laptop / notebook computer
    Tablet,      // Tablet device
    Smartphone,  // Smartphone / mobile phone
    Headphones   // Headphones (Bluetooth or wired)
}
    

public enum DeliveryDoneType
{
    Delivered,          // Supplied
    CustomerRefused,    // Ordering customer refused to accept
    Cancelled,          // Cancelled by customer/manager
    CustomerNotFound,   // Customer not found at destination
    Failed              // Failure during assignment/route calculation
}

public enum OrderListFilterProperty
{
    Status,         // Filter by order status
    DeliveryType,   // Filter by delivery type
    OrderType       // Filter by order type
}

public enum OrderListSortProperty
{
    OrderId,
    LastDeliveryId,
    OrderType,
    Status,
    ScheduleStatus,
    TotalDeliveries,
    TimeRemaining,
    TotalHandlingTime
}

public enum ClosedDeliveryListSortProperty
{
    DeliveryId,         // Sort by delivery ID
    DeliveryDate,       // Sort by delivery date
    DeliveryTransport,  // Sort by delivery transport
    DeliveryType        // Sort by delivery type
}

public enum OpenOrderListSortProperty
{
    OrderId,        // Sort by order ID
    CustomerName,   // Sort by customer name
    OrderDate       // Sort by order date
}

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours,
    Days,
    Month,
    Years
}
