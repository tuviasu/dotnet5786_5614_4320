namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static IDelivery? s_dalDelivery; //stage 1
    private static ICourier? s_dalCourier; //stage 1
    private static IOrder? s_dalOrder; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
    //...




    /// <summary>
    /// Initialize courier list with random realistic data
    /// </summary>
    private static void createCouriers()
    {
        string[] courierNames = { "Avi Cohen", "Dana Levi", "Yossi Bar", "Noa Regev", "Ron Azulay", "Galit Saban" };

        foreach (var name in courierNames)
        {
            int id;
            do id = s_rand.Next(200000000, 400000000);
            while (s_dalCourier!.Read(id) != null);

            string phone = "05" + s_rand.Next(0, 100000000).ToString("D8");
            string email = $"{name.Replace(" ", ".").ToLower()}@example.com";
            string password = "Password123"; // or generate random password
            bool isActive = s_rand.Next(0, 2) == 1;
            DeliveryTransport transport = DeliveryTransport.Bicycle; // or any default value
            double? maxDistance = 20.0; // or any default value

            s_dalCourier!.Create(new Courier(
                id,
                name,
                phone,
                email,
                password,
                isActive,
                transport,
                maxDistance
            ));
        }
    }

    /// <summary>
    /// Initialize order list with random realistic data
    /// </summary>
    private static void createOrders()
    {
        string[] customerNames = { "David Levi", "Maya Ron", "Eli Shahar", "Ruth Avital", "Ido Barkai" };
        string[] cities = { "Jerusalem", "Tel Aviv", "Haifa", "Eilat", "Ashdod" };

        int nextOrderId = 1; // Start from 1 or any desired initial value

        if (s_dalConfig == null)
            throw new InvalidOperationException("s_dalConfig must be initialized before calling createOrders.");

        foreach (var name in customerNames)
        {
            int id = nextOrderId++; // Use local counter for order IDs
            string address = cities[s_rand.Next(cities.Length)];
            double latitude = s_rand.NextDouble() * 180 - 90; // Random latitude (-90 to 90)
            double longitude = s_rand.NextDouble() * 360 - 180; // Random longitude (-180 to 180)
            string phone = "05" + s_rand.Next(0, 100000000).ToString("D8");
            DateTime start = new DateTime(s_dalConfig.Clock.Year - 1, 1, 1);
            int range = (s_dalConfig.Clock - start).Days;
            DateTime orderDate = start.AddDays(s_rand.Next(range));

            s_dalOrder!.Create(new Order(
                id,
                OrderType.Regular, // fixed: use a defined enum value
                latitude,
                longitude,
                name,
                address,
                phone,
                orderDate,
                DeliveryTransport.Bicycle, // or any default DeliveryTransport
                FragilityLevel.NotFragile,     // or any default FragilityLevel
                null,                     // Description
                OrderStatus.Created        // or any default OrderStatus
            ));
        }
    }

    /// <summary>
    /// Initialize delivery list with logical connections between couriers and orders
    /// </summary>
    private static void createDeliveries()
    {
        if (s_dalConfig == null)
            throw new InvalidOperationException("s_dalConfig must be initialized before calling createDeliveries.");

        var couriers = s_dalCourier!.ReadAll();
        var orders = s_dalOrder!.ReadAll();

        int nextDeliveryId = 1; // Start from 1 or any desired initial value

        foreach (var order in orders)
        {
            // only part of the orders will be already delivered
            bool delivered = s_rand.Next(0, 2) == 1;

            // choose a random courier
            var courier = couriers[s_rand.Next(couriers.Count)];

            int id = nextDeliveryId++; // Use local counter for delivery IDs
            DateTime start = new DateTime(s_dalConfig.Clock.Year - 1, 1, 1);
            int range = (s_dalConfig.Clock - start).Days;
            DateTime deliveryDate = start.AddDays(s_rand.Next(range));

            // Fix: Pass DeliveryTransport as argument 4, and set other required fields
            s_dalDelivery!.Create(new Delivery(
                id,
                order.Id,
                courier.Id,
                courier.Transport, // DeliveryTransport argument
                deliveryDate,      // StartDelivery argument
                null,              // ActualDistance
                delivered ? DeliveryCompletionType.Successful : null, // CompletionType
                delivered ? deliveryDate.AddHours(s_rand.Next(1, 5)) : null // EndDelivery
            ));
        }
    }

    /// <summary>
    /// Main method that initializes all DAL lists.
    /// </summary>
    public static void Do(
        ICourier? dalCourier,
        IOrder? dalOrder,
        IDelivery? dalDelivery,
        IConfig? dalConfig)
    {
        // ========== 1. Assign interface instances and validate ==========

        s_dalCourier = dalCourier ?? throw new NullReferenceException("DAL object cannot be null!");
        s_dalOrder = dalOrder ?? throw new NullReferenceException("DAL object cannot be null!");
        s_dalDelivery = dalDelivery ?? throw new NullReferenceException("DAL object cannot be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object cannot be null!");

        // ========== 2. Reset all lists and configuration ==========
        Console.WriteLine("Reset configuration values and list data...");
        s_dalConfig.Reset();
        s_dalCourier.DeleteAll();
        s_dalOrder.DeleteAll();
        s_dalDelivery.DeleteAll();

        // ========== 3. Initialize lists ==========
        Console.WriteLine("Initializing Couriers list...");
        createCouriers();

        Console.WriteLine("Initializing Orders list...");
        createOrders();

        Console.WriteLine("Initializing Deliveries list...");
        createDeliveries();

        Console.WriteLine("Initialization completed successfully!");
    }

}
