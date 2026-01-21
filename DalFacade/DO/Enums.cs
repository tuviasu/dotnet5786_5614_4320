namespace DO
{
    public enum DeliveryTransport
    {
        Car,          // For long-distance or large deliveries
        Motorcycle,   // Standard and fast city deliveries
        Bicycle,      // Short-distance, eco-friendly deliveries
        Walk          // Very close deliveries (near the branch)
    }

    public enum DeliveryType
    {
        Regular,    // Standard delivery (30–60 minutes)
        Express     // Fast delivery (up to 20 minutes)
    }

    public enum OrderType
    {
        Individual,  // Order placed by a single person
        Group,       // Order placed by a group of people
        Corporate,   // Order placed by a company or organization


    }

    public enum DeviceType
    {
        Desktop,         // Desktop computer
        Laptop,          // Laptop / notebook computer
        Tablet,          // Tablet device
        Smartphone,      // Smartphone / mobile phone
        Headphones       // Headphones (Bluetooth or wired)
    }

    public enum ProcessResult
    {
        Completed,          // Supplied
        CustomerRefused,    // Ordering customer refused to accept
        Cancelled,          // Cancelled by customer/manager
        CustomerNotFound,   // Customer not found at destination
        Failed              // Failure during assignment/route calculation
    }

    public enum  Main_Menu
    {
        exit,
        courier_menu,
        order_menu,
        delivery_menu,
        initialization,
        print_data_base,
        config_menu,
        reset
    }

    public enum Courier_Menu
    {
        back_to_main_menu,
        add_courier,
        get_courier,
        get_all_couriers,
        update_courier,
        delete_courier,
        delete_all_couriers        
    } 

    public enum Order_Menu
    {
        back_to_main_menu,
        add_order,
        get_order,
        get_all_orders,
        update_order,
        delete_order,
        delete_all_orders
    }

    public enum Delivery_Menu
    {
        back_to_main_menu,
        add_delivery,
        get_delivery,
        get_all_deliveries,
        update_delivery,
        delete_delivery,
        delete_all_deliveries
    }
    public enum Config_Menu
    {
        back_to_main_menu,
        clock_add_minute,
        clock_add_hour,
        get_clock,
        set_new_config,
        get_config,
        reset
    }
}
