namespace DalTest;
using Dal;
using DalApi;
using DO;
using System.Diagnostics;

/// <summary>
/// 
/// Main program class for the DAL test console application.
/// 
/// </summary>

internal class Program
{
    //private static IDelivery? s_dalDelivery = new DeliveryImplementation();   // stage 1
    //private static ICourier? s_dalCourier = new CourierImplementation();      // stage 1
    //private static IOrder? s_dalOrder = new OrderImplementation();            // stage 1
    //private static IConfig? s_dalConfig = new ConfigImplementation();         // stage 1

    //static readonly IDal s_dal = new DalList();                               // stage 2
    //static readonly IDal s_dal = new DalXml();                                // stage 3    
    static readonly IDal s_dal = Factory.Get;

    static void Main(string[] args)
    {
        try
        {

            while (true)
            {
                int choice;
                Console.WriteLine("MAIN MENU:\n"
                                  + "0. Exit\n"
                                  + "1. Courier menu\n"
                                  + "2. Order menu\n"
                                  + "3. Delivery menu\n"
                                  + "4. initialization\n"
                                  + "5. print all\n"
                                  + "6. Config menu\n"
                                  + "7. reset");
                Console.Write("Enter your choice: ");
                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 7)
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter valid number.");
                    Console.WriteLine();
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        Console.WriteLine();
                        switch (CourierMenu())
                        {
                            case 0:
                                Console.WriteLine();
                                break;
                            case 1:
                                s_dal?.Courier.Create(ReadCourierFromUser());
                                break;
                            case 2:
                                PrintCourier();
                                break;
                            case 3:
                                PrintAllCouriers();
                                break;
                            case 4:
                                UpdateCourier();
                                break;
                            case 5:
                                deleteCourier();
                                break;
                            case 6:
                                s_dal?.Courier.DeleteAll();
                                Console.WriteLine("All couriers deleted.");
                                Console.WriteLine();
                                break;
                        }
                        break;
                    case 2:
                        Console.WriteLine();
                        switch (OrderMenu())
                        {
                            case 0:
                                Console.WriteLine();
                                break;
                            case 1:
                                s_dal?.Order.Create(ReadOrderFromUser());
                                break;
                            case 2:
                                PrintOrder();
                                break;
                            case 3:
                                PrintAllOrders();
                                break;
                            case 4:
                                UpdateOrder();
                                break;
                            case 5:
                                deleteOrder();
                                break;
                            case 6:
                                s_dal?.Order.DeleteAll();
                                Console.WriteLine("All orders deleted.");
                                Console.WriteLine();
                                break;
                        }
                        break;
                    case 3:
                        Console.WriteLine();
                        switch (DeliveryMenu())
                        {
                            case 0:
                                Console.WriteLine();
                                break;
                            case 1:
                                s_dal?.Delivery.Create(ReadDeliveryFromUser());
                                Console.WriteLine();
                                break;
                            case 2:
                                PrintDelivery();
                                Console.WriteLine();
                                break;
                            case 3:
                                PrintAllDeliveries();
                                Console.WriteLine();
                                break;
                            case 4:
                                UpdateDelivery();
                                Console.WriteLine();
                                break;
                            case 5:
                                DeleteDelivery();
                                Console.WriteLine();
                                break;
                            case 6:
                                s_dal?.Delivery.DeleteAll();
                                Console.WriteLine();
                                Console.WriteLine("All deliveries deleted.");
                                Console.WriteLine();
                                break;
                        }
                        break;
                    case 4:
                        Initialization.Do();
                        var orders = s_dal.Order.ReadAll().ToList();
                        var deliveries = s_dal.Delivery.ReadAll().ToList();

                        Console.WriteLine("\n--- ORDERS ENUM DISTRIBUTION ---");
                        Console.WriteLine("Individual: " +
                            orders.Count(o => o.OrderType == OrderType.Individual));
                        Console.WriteLine("Group: " +
                            orders.Count(o => o.OrderType == OrderType.Group));
                        Console.WriteLine("Corporate: " +
                            orders.Count(o => o.OrderType == OrderType.Corporate));

                        Console.WriteLine("\n--- DELIVERIES ENUM DISTRIBUTION ---");
                        Console.WriteLine("Completed: " +
                            deliveries.Count(d => d.DeliveryDoneType == ProcessResult.Completed));
                        Console.WriteLine("Failed: " +
                            deliveries.Count(d => d.DeliveryDoneType == ProcessResult.Failed));
                        Console.WriteLine("CustomerNotFound: " +
                            deliveries.Count(d => d.DeliveryDoneType == ProcessResult.CustomerNotFound));
                        Console.WriteLine("Cancelled: " +
                            deliveries.Count(d => d.DeliveryDoneType == ProcessResult.Cancelled));
                        Console.WriteLine("CustomerRefused: " +
                            deliveries.Count(d => d.DeliveryDoneType == ProcessResult.CustomerRefused));

                        break;
                    case 5:
                        Console.WriteLine();
                        Console.WriteLine("=========================================");
                        Console.WriteLine("=               All data:               =");
                        Console.WriteLine("=========================================");
                        Console.WriteLine();
                        Console.WriteLine("=========================================");
                        Console.WriteLine("                COURIERS:                ");
                        Console.WriteLine("=========================================");
                        Console.WriteLine();
                        PrintAllCouriers();
                        Console.WriteLine();
                        Console.WriteLine("=========================================");
                        Console.WriteLine("                 ORDERS:                  ");
                        Console.WriteLine("=========================================");
                        Console.WriteLine();
                        PrintAllOrders();
                        Console.WriteLine();
                        Console.WriteLine("=========================================");
                        Console.WriteLine("               DELIVERIES:               ");
                        Console.WriteLine("=========================================");
                        Console.WriteLine();
                        PrintAllDeliveries();
                        break;
                    case 6:
                        Console.WriteLine();
                        switch (ConfigMenu())
                        {
                            case 0:
                                Console.WriteLine();
                                break;
                            case 1:
                                AdvanceByMinute();
                                Console.WriteLine();
                                Console.WriteLine("Minute advanced.");
                                Console.WriteLine();
                                break;
                            case 2:
                                AdvanceByHour();
                                Console.WriteLine();
                                Console.WriteLine("Hour advanced.");
                                Console.WriteLine();
                                break;
                            case 3:
                                AdvanceByDay();
                                Console.WriteLine();
                                Console.WriteLine("Day advanced.");
                                Console.WriteLine();
                                break;
                            case 4:
                                Console.WriteLine(s_dal?.Config.Clock);
                                break;
                            case 5:
                                Console.WriteLine();
                                SetValue();
                                Console.WriteLine();
                                break;
                            case 6:
                                Console.WriteLine();
                                GetValue();
                                Console.WriteLine();
                                break;
                            case 7:
                                Console.WriteLine();
                                s_dal?.Config.Reset();
                                Console.WriteLine("Config values reset successfully.");
                                Console.WriteLine();
                                break;
                        }
                        break;
                    case 7:
                        Console.WriteLine();
                        s_dal?.Courier.DeleteAll();
                        s_dal?.Order.DeleteAll();
                        s_dal?.Delivery.DeleteAll();
                        s_dal?.Config.Reset();
                        Console.WriteLine("Reset complete.");
                        Console.WriteLine();
                        break;
                    default:
                        break;
                }

            }
        }
        catch (DalIdAlreadyExist ex)
        {
            Console.WriteLine("Item already exists: " + ex.Message);
        }
        catch (DalIdNotExist ex)
        {
            Console.WriteLine("Item does not exist: " + ex.Message);
        }
        catch (DalItemNotExist ex)
        {
            Console.WriteLine("Item does not exist: " + ex.Message);
        }
        catch (DalInvalidId ex)
        {
            Console.WriteLine("Invalid ID: " + ex.Message);
        }
        catch (DalEmptyCollection ex)
        {
            Console.WriteLine("The list is empty: " + ex.Message);
        }
        catch (DalXMLFileLoadCreateException ex)
        {
            Console.WriteLine("XML file error: " + ex.Message);
        }

        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private static int CourierMenu()
    {
        while (true)
        {
            int choice1;
            Console.WriteLine("COURIER MENU:\n"
                            + "0. Back to main menu\n"
                            + "1. Add courier\n"
                            + "2. Get courier by ID\n"
                            + "3. Get all couriers\n"
                            + "4. Update courier\n"
                            + "5. Delete courier\n"
                            + "6. Delete all couriers");
            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 0 || choice1 > 6)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter valid number.");
                Console.WriteLine();
                continue;
            }
            else
            {
                return choice1;
            }
        }

    }
    private static DO.Courier ReadCourierFromUser()
    {
        Console.WriteLine("Enter Courier fields:\n"
                          + "ID: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        Console.Write("FullName: ");
        string fullName = Console.ReadLine() ?? "";

        Console.Write("Phone: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("StartWorkInCompany (yyyy-MM-dd): ");
        DateTime start = DateTime.TryParse(Console.ReadLine(), out var dt) ? dt.Date : DateTime.Today;

        Console.Write("TransportType (car/motorcycle/bicycle/walk): ");
        DeliveryTransport transport = Console.ReadLine()?.ToLower() switch
        {
            "car" => DeliveryTransport.Car,
            "motorcycle" => DeliveryTransport.Motorcycle,
            "bicycle" => DeliveryTransport.Bicycle,
            "walk" => DeliveryTransport.Walk,
            _ => DeliveryTransport.Walk // default value
        };

        Console.Write("Password: ");
        string pwd = Console.ReadLine() ?? "";

        Console.Write("IsActive (true/false): ");
        bool isActive = bool.TryParse(Console.ReadLine(), out var b) ? b : true;

        Console.Write("MaxDeliveryDistanceKm: ");
        double maxKm = double.TryParse(Console.ReadLine(), out var km) ? km : 10.0;

        return new DO.Courier
        {
            CourierID = id,
            FullName = fullName,
            Phone = phone,
            Email = email,
            StartWorkInCompany = start,
            TransportType = transport,
            Password = pwd,
            IsActive = isActive,
            MaxDeliveryDistanceKm = maxKm
        };
    }

    private static void PrintCourier()
    {
        Console.WriteLine("Enter Courier ID to print: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        var courier = s_dal?.Courier.Read(id);
        if (courier is null)
        {
            throw new DalItemNotExist("Courier with ID " + id + " does not exist.");
        }
        Console.WriteLine($"Courier ID: {courier.CourierID}");
        Console.WriteLine($"Full Name: {courier.FullName}");
        Console.WriteLine($"Phone: {courier.Phone}");
        Console.WriteLine($"Email: {courier.Email}");
        Console.WriteLine($"Start Work In Company: {courier.StartWorkInCompany:dd-MM-yyyy}");
        Console.WriteLine($"Transport Type: {courier.TransportType}");
        Console.WriteLine($"Password: {courier.Password}");
        Console.WriteLine($"Is Active: {courier.IsActive}");
        Console.WriteLine($"Max Delivery Distance (Km): {courier.MaxDeliveryDistanceKm}");
    }

    private static void PrintAllCouriers()
    {
        var couriers = s_dal?.Courier.ReadAll();
        if (couriers == null || couriers.Count() == 0)
        {
            Console.WriteLine("No couriers available.");
            return;
        }

        foreach (var courier in couriers)
        {
            Console.WriteLine($"Courier ID: {courier.CourierID}");
            Console.WriteLine($"Full Name: {courier.FullName}");
            Console.WriteLine($"Phone: {courier.Phone}");
            Console.WriteLine($"Email: {courier.Email}");
            Console.WriteLine($"Start Work: {courier.StartWorkInCompany:yyyy-MM-dd}");
            Console.WriteLine($"Transport: {courier.TransportType}");
            Console.WriteLine($"Is Active: {courier.IsActive}");
            Console.WriteLine($"Max Delivery Distance (Km): {(courier.MaxDeliveryDistanceKm.HasValue ? courier.MaxDeliveryDistanceKm.Value.ToString("F1") : "N/A")}");
            Console.WriteLine("-----------------------");
        }
        Console.WriteLine();
    }

    private static void UpdateCourier()
    {
        Console.WriteLine("Enter Courier ID to update: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;
        var existingCourier = s_dal?.Courier.Read(id);

        if (existingCourier is null)
        {
            throw new DalItemNotExist("Courier with ID " + id + " does not exist.");
        }

        Console.WriteLine("Enter new details for the courier (leave blank to keep existing value):");
        Console.Write($"FullName ({existingCourier.FullName}): ");
        string fullName = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(fullName))
            existingCourier = existingCourier with { FullName = fullName };

        Console.Write($"Phone ({existingCourier.Phone}): ");
        string phone = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(phone))
            existingCourier = existingCourier with { Phone = phone };

        Console.Write($"Email ({existingCourier.Email}): ");
        string email = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(email))
            existingCourier = existingCourier with { Email = email };

        Console.Write($"TransportType ({existingCourier.TransportType}): ");
        string transportInput = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(transportInput) && Enum.TryParse<DeliveryTransport>(transportInput, true, out var transport))
        {
            existingCourier = existingCourier with { TransportType = transport };
        }

        Console.Write($"Password ({existingCourier.Password}): ");
        string pwd = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(pwd))
            existingCourier = existingCourier with { Password = pwd };

        Console.Write($"IsActive ({existingCourier.IsActive}): ");
        string isActiveInput = Console.ReadLine()!;

        if (bool.TryParse(isActiveInput, out var isActive))
            existingCourier = existingCourier with { IsActive = isActive };

        Console.Write($"MaxDeliveryDistanceKm ({existingCourier.MaxDeliveryDistanceKm}): ");
        string maxKmInput = Console.ReadLine()!;

        if (double.TryParse(maxKmInput, out var maxKm))
            existingCourier = existingCourier with { MaxDeliveryDistanceKm = maxKm };

        s_dal?.Courier.Update(existingCourier);
        Console.WriteLine("Courier updated successfully.");
    }

    private static void deleteCourier()
    {
        Console.WriteLine("Enter Courier ID to delete: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;
        try
        {
            s_dal?.Courier.Delete(id);
            Console.WriteLine($"Courier with ID {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting courier: " + ex.Message);
        }
    }

    private static int OrderMenu()
    {
        while (true)
        {
            int choice2;
            Console.WriteLine("ORDER MENU:\n"
                            + "0. Back to main menu\n"
                            + "1. Add order\n"
                            + "2. Get order by ID\n"
                            + "3. Get all orders\n"
                            + "4. Update order\n"
                            + "5. Delete order\n"
                            + "6. Delete all orders");
            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 0 || choice2 > 6)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter valid number.");
                Console.WriteLine();
                continue;
            }
            else
            {
                return choice2;
            }
        }
    }

    public static DO.Order ReadOrderFromUser()
    {
        Console.WriteLine("Enter Order fields:\n"
                          + "ID: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";

        Console.Write("Full address: ");
        string fullAddress = Console.ReadLine() ?? "";

        Console.Write("Latitude: ");
        double latitude = double.TryParse(Console.ReadLine(), out var lat) ? lat : 0.0;

        Console.Write("Longitude: ");
        double longitude = double.TryParse(Console.ReadLine(), out var lon) ? lon : 0.0;

        Console.Write("Costumer name: ");
        string costumerName = Console.ReadLine() ?? "";

        Console.Write("Costumer phone: ");
        string custumerPhone = Console.ReadLine() ?? "";

        Console.Write("Order type: ");
        OrderType orderType = Console.ReadLine()?.ToLower() switch
        {
            "single" => OrderType.Individual,
            "group" => OrderType.Group,
            "scheduled" => OrderType.Corporate,
            _ => OrderType.Individual
        };

        Console.Write("Pizza size: ");
        DeviceType pizzaSize = Console.ReadLine()?.ToLower() switch
        {
            "personal" => DeviceType.Desktop,
            "medium" => DeviceType.Laptop,
            "large" => DeviceType.Tablet,
            "family" => DeviceType.Smartphone,
            "party" => DeviceType.Headphones,
            _ => DeviceType.Desktop
        };

        Console.Write("Order opening time: ");
        DateTime orderOpeningTime = DateTime.TryParse(Console.ReadLine(), out var dt) ? dt : DateTime.Now;

        return new DO.Order
        {
            OrderID = id,
            Description = description,
            FullAddress = fullAddress,
            Latitude = latitude,
            Longitude = longitude,
            CustomerName = costumerName,
            CustomerPhone = custumerPhone,
            OrderType = orderType,
            PizzaSize = pizzaSize,
            OrderOpeningTime = orderOpeningTime
        };
    }

    public static void PrintOrder()
    {
        Console.WriteLine("Enter Order ID to print: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;
        var order = s_dal?.Order.Read(id);

        if (order is null)
        {
            Console.WriteLine($"Order with ID {id} not found.");
            return;
        }

        Console.WriteLine($"Order ID: {order.OrderID}");
        Console.WriteLine($"Description: {order.Description}");
        Console.WriteLine($"Full Address: {order.FullAddress}");
        Console.WriteLine($"Latitude: {order.Latitude}");
        Console.WriteLine($"Longitude: {order.Longitude}");
        Console.WriteLine($"Customer Name: {order.CustomerName}");
        Console.WriteLine($"Customer Phone: {order.CustomerPhone}");
        Console.WriteLine($"Order Type: {order.OrderType}");
        Console.WriteLine($"Pizza Size: {order.PizzaSize}");
        Console.WriteLine($"Order Opening Time: {order.OrderOpeningTime}");
        Console.WriteLine();
    }

    public static void PrintAllOrders()
    {
        var orders = s_dal?.Order.ReadAll();
        if (orders == null || orders.Count() == 0)
        {
            Console.WriteLine("No orders available.");
            return;
        }
        foreach (var order in orders)
        {
            Console.WriteLine($"Order ID: {order.OrderID}");
            Console.WriteLine($"Description: {order.Description}");
            Console.WriteLine($"Full Address: {order.FullAddress}");
            Console.WriteLine($"Latitude: {order.Latitude}");
            Console.WriteLine($"Longitude: {order.Longitude}");
            Console.WriteLine($"Customer Name: {order.CustomerName}");
            Console.WriteLine($"Customer Phone: {order.CustomerPhone}");
            Console.WriteLine($"Order Type: {order.OrderType}");
            Console.WriteLine($"Pizza Size: {order.PizzaSize}");
            Console.WriteLine($"Order Opening Time: {order.OrderOpeningTime}");
            Console.WriteLine("-----------------------");
        }
        Console.WriteLine();
    }

    public static void UpdateOrder()
    {
        Console.WriteLine("Enter Order ID to update: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        var existingOrder = s_dal?.Order.Read(id);
        if (existingOrder is null)
        {
            throw new DalItemNotExist("This item does not exist.");
        }

        Console.WriteLine("Enter new details for the order (leave blank to keep existing value):");
        Console.Write($"Description ({existingOrder.Description}): ");
        string description = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(description))
            existingOrder = existingOrder with { Description = description };

        Console.Write($"Full Address ({existingOrder.FullAddress}): ");
        string fullAddress = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(fullAddress))
            existingOrder = existingOrder with { FullAddress = fullAddress };

        Console.Write($"Latitude ({existingOrder.Latitude}): ");
        string latitudeInput = Console.ReadLine()!;

        if (double.TryParse(latitudeInput, out var latitude))
            existingOrder = existingOrder with { Latitude = latitude };

        Console.Write($"Longitude ({existingOrder.Longitude}): ");
        string longitudeInput = Console.ReadLine()!;

        if (double.TryParse(longitudeInput, out var longitude))
            existingOrder = existingOrder with { Longitude = longitude };

        Console.Write($"Customer Name ({existingOrder.CustomerName}): ");
        string customerName = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(customerName))
            existingOrder = existingOrder with { CustomerName = customerName };

        Console.Write($"Customer Phone ({existingOrder.CustomerPhone}): ");
        string customerPhone = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(customerPhone))
            existingOrder = existingOrder with { CustomerPhone = customerPhone };

        Console.Write($"Order Type ({existingOrder.OrderType}): ");
        string orderTypeInput = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(orderTypeInput) && Enum.TryParse<OrderType>(orderTypeInput, true, out var orderType))
        {
            existingOrder = existingOrder with { OrderType = orderType };
        }

        Console.Write($"Pizza Size ({existingOrder.PizzaSize}): ");
        string pizzaSizeInput = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(pizzaSizeInput) && Enum.TryParse<DeviceType>(pizzaSizeInput, true, out var pizzaSize))
        {
            existingOrder = existingOrder with { PizzaSize = pizzaSize };
        }

        s_dal?.Order.Update(existingOrder);
        Console.WriteLine("Order updated successfully.");
    }

    public static void deleteOrder()
    {
        Console.WriteLine("Enter Order ID to delete: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;
        try
        {
            s_dal?.Order.Delete(id);
            Console.WriteLine($"Order with ID {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting order: " + ex.Message);
        }
    }

    private static int DeliveryMenu()
    {
        while (true)
        {
            int choice3;
            Console.WriteLine("DELIVERY MENU:\n"
                            + "0. Back to main menu\n"
                            + "1. Add delivery\n"
                            + "2. Get delivery by ID\n"
                            + "3. Get all deliveries\n"
                            + "4. Update delivery\n"
                            + "5. Delete delivery\n"
                            + "6. Delete all deliveries");
            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out choice3) || choice3 < 0 || choice3 > 6)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter valid number.");
                Console.WriteLine();
                continue;
            }
            else
            {
                return choice3;
            }
        }
    }

    private static DO.Delivery ReadDeliveryFromUser()
    {
        Console.WriteLine("Enter Delivery fields:");

        Console.Write("DeliveryID: ");
        int deliveryId = int.TryParse(Console.ReadLine(), out var dId) ? dId : 0;

        Console.Write("OrderID: ");
        int orderId = int.TryParse(Console.ReadLine(), out var oId) ? oId : 0;

        Console.Write("CourierID: ");
        int courierId = int.TryParse(Console.ReadLine(), out var cId) ? cId : 0;

        Console.Write("Delivery type: ");
        DeliveryType deliveryType = Console.ReadLine()?.ToLower() switch
        {
            "regular" => DeliveryType.Regular,
            "express" => DeliveryType.Express,
            _ => DeliveryType.Regular
        };

        Console.Write("DeliveryStartTime (DD.MM.YYYY HH:mm): ");
        DateTime startTime = DateTime.TryParse(Console.ReadLine(), out var st) ? st : DateTime.Now;

        Console.Write("DeliveryDistance (km): ");
        double distance = double.TryParse(Console.ReadLine(), out var dist) ? dist : 0.0;

        Console.Write("ProcessResult: ");
        ProcessResult doneType = Console.ReadLine()?.ToLower() switch
        {
            "delivered" => ProcessResult.Completed,
            "customerrefused" => ProcessResult.CustomerRefused,
            "refused" => ProcessResult.CustomerRefused,
            "cancelled" => ProcessResult.Cancelled,
            "canceled" => ProcessResult.Cancelled,
            "customernotfound" => ProcessResult.CustomerNotFound,
            "notfound" => ProcessResult.CustomerNotFound,
            "failed" => ProcessResult.Failed,
            _ => ProcessResult.Completed
        };

        Console.Write("DeliveryDoneTime (DD-MM-YYYY HH:mm or empty): ");
        string doneTimeInput = Console.ReadLine() ?? "";
        DateTime? doneTime = string.IsNullOrWhiteSpace(doneTimeInput) ? null :
                             DateTime.TryParse(doneTimeInput, out var dt) ? dt : null;

        return new DO.Delivery
        {
            DeliveryID = deliveryId,
            OrderID = orderId,
            CourierID = courierId,
            DeliveryType = deliveryType,
            DeliveryStartTime = startTime,
            DeliveryDistance = distance,
            DeliveryDoneType = doneType,
            DeliveryDoneTime = doneTime
        };
    }

    private static void PrintDelivery()
    {
        Console.WriteLine("Enter Delivery ID to print: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        var delivery = s_dal?.Delivery.Read(id);
        if (delivery is null)
        {
            throw new DalItemNotExist("Delivery with ID " + id + " does not exist.");
        }

        Console.WriteLine($"Delivery ID: {delivery.DeliveryID}");
        Console.WriteLine($"Order ID: {delivery.OrderID}");
        Console.WriteLine($"Courier ID: {delivery.CourierID}");
        Console.WriteLine($"Delivery Type: {delivery.DeliveryType}");
        Console.WriteLine($"Delivery Start Time: {delivery.DeliveryStartTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Delivery Distance (Km): {delivery.DeliveryDistance}");
        Console.WriteLine($"Delivery Done Type: {delivery.DeliveryDoneType}");
        Console.WriteLine($"Delivery Done Time: {(delivery.DeliveryDoneTime.HasValue ? delivery.DeliveryDoneTime.Value.ToString("yyyy-MM-dd HH:mm") : "—")}");
    }

    private static void PrintAllDeliveries()
    {
        var deliveries = s_dal?.Delivery.ReadAll();
        if (deliveries == null || deliveries.Count() == 0)
        {
            Console.WriteLine("No deliveries available.");
            Console.WriteLine();
            return;
        }

        foreach (var delivery in deliveries)
        {
            Console.WriteLine($"Delivery ID: {delivery.DeliveryID}");
            Console.WriteLine($"Order ID: {delivery.OrderID}");
            Console.WriteLine($"Courier ID: {delivery.CourierID}");
            Console.WriteLine($"Delivery Type: {delivery.DeliveryType}");
            Console.WriteLine($"Delivery Start Time: {delivery.DeliveryStartTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Delivery Distance (Km): {delivery.DeliveryDistance}");
            Console.WriteLine($"Delivery Done Type: {delivery.DeliveryDoneType}");
            Console.WriteLine($"Delivery Done Time: {(delivery.DeliveryDoneTime.HasValue ? delivery.DeliveryDoneTime.Value.ToString("yyyy-MM-dd HH:mm") : "—")}");
            Console.WriteLine("-----------------------");
        }
        Console.WriteLine();
    }

    private static void UpdateDelivery()
    {
        Console.WriteLine("Enter Delivery ID to update: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

        var existingDelivery = s_dal?.Delivery.Read(id);
        if (existingDelivery is null)
        {
            throw new DalItemNotExist("Delivery with ID " + id + " does not exist.");
        }

        Console.WriteLine("Enter new details for the delivery (leave blank to keep existing value):");
        Console.Write($"OrderID ({existingDelivery.OrderID}): ");
        string orderIdInput = Console.ReadLine()!;

        if (int.TryParse(orderIdInput, out var orderId))
            existingDelivery = existingDelivery with { OrderID = orderId };

        Console.Write($"CourierID ({existingDelivery.CourierID}): ");
        string courierIdInput = Console.ReadLine()!;

        if (int.TryParse(courierIdInput, out var courierId))
            existingDelivery = existingDelivery with { CourierID = courierId };

        Console.Write($"DeliveryType ({existingDelivery.DeliveryType}): ");
        string deliveryTypeInput = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(deliveryTypeInput) && Enum.TryParse<DeliveryType>(deliveryTypeInput, true, out var deliveryType))
            existingDelivery = existingDelivery with { DeliveryType = deliveryType };

        Console.Write($"DeliveryStartTime ({existingDelivery.DeliveryStartTime:yyyy-MM-dd HH:mm}): ");
        string startTimeInput = Console.ReadLine()!;

        if (DateTime.TryParse(startTimeInput, out var startTime))
            existingDelivery = existingDelivery with { DeliveryStartTime = startTime };

        Console.Write($"DeliveryDistance ({existingDelivery.DeliveryDistance}): ");
        string distanceInput = Console.ReadLine()!;

        if (double.TryParse(distanceInput, out var distance))
            existingDelivery = existingDelivery with { DeliveryDistance = distance };

        Console.Write($"ProcessResult ({existingDelivery.DeliveryDoneType}): ");
        string doneTypeInput = Console.ReadLine()!;

        if (!string.IsNullOrWhiteSpace(doneTypeInput) && Enum.TryParse<ProcessResult>(doneTypeInput, true, out var doneType))
            existingDelivery = existingDelivery with { DeliveryDoneType = doneType };

        Console.Write($"DeliveryDoneTime ({(existingDelivery.DeliveryDoneTime.HasValue ? existingDelivery.DeliveryDoneTime.Value.ToString("yyyy-MM-dd HH:mm") : "—")}): ");
        string doneTimeInput = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(doneTimeInput))
        {
            existingDelivery = existingDelivery with { DeliveryDoneTime = null };
        }
        else if (DateTime.TryParse(doneTimeInput, out var doneTime))
        {
            existingDelivery = existingDelivery with { DeliveryDoneTime = doneTime };
        }

        s_dal?.Delivery.Update(existingDelivery);
        Console.WriteLine("Delivery updated successfully.");
    }

    private static void DeleteDelivery()
    {
        Console.WriteLine("Enter Delivery ID to delete: ");
        int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;
        try
        {
            s_dal?.Delivery.Delete(id);
            Console.WriteLine($"Delivery with ID {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting delivery: " + ex.Message);
        }
    }

    private static int ConfigMenu()
    {
        while (true)
        {
            int choice4;
            Console.WriteLine("CONFIG MENU:\n"
                            + "0. Exit to main menu\n"
                            + "1. Advance system clock by minute\n"
                            + "2. Advance system clock by hour\n"
                            + "3. Advance system clock by day\n"
                            + "4. Show clock current value\n"
                            + "5. Set new config value\n"
                            + "6. Get current config value\n"
                            + "7. Reset all config values");
            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out choice4) || choice4 < 0 || choice4 > 7)
            {
                Console.WriteLine();
                Console.WriteLine("Enter valid number");
                Console.WriteLine();
                continue;
            }

            else
            {
                return choice4;
            }
        }
    }

    private static void AdvanceBySecond()
    {
        s_dal!.Config.Clock = s_dal.Config.Clock.AddSeconds(1);
    }

    private static void AdvanceByMinute()
    {
        s_dal!.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
    }

    private static void AdvanceByHour()
    {
        s_dal!.Config.Clock = s_dal.Config.Clock.AddHours(1);
    }

    private static void AdvanceByDay()
    {
        s_dal!.Config.Clock = s_dal.Config.Clock.AddDays(1);
    }

    private static void AdvanceByYear()
    {
        s_dal!.Config.Clock = s_dal.Config.Clock.AddYears(1);
    }

    private static void SetValue()
    {
        if (s_dal == null)
        {
            Console.WriteLine("DAL instance is not available.");
            return;
        }

        Console.WriteLine("Enter new config values:");

        while (true)
        {
            Console.Write("Clock (YYYY-MM-DD HH:MM:SS): ");
            string? newValue = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newValue) && DateTime.TryParse(newValue, out var clock))
            {
                s_dal.Config.Clock = clock;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid date/time.");
        }

        while (true)
        {
            Console.Write("Max Delivery Distance (Km's): ");
            if (int.TryParse(Console.ReadLine(), out var maxDeliveryDistance))
            {
                s_dal.Config.MaxDeliveryDistance = maxDeliveryDistance;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        while (true)
        {
            Console.Write("Max Delivery Time Range (HH:MM:SS): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out var maxDeliveryTimeRange))
            {
                s_dal.Config.MaxDeliveryTimeRange = maxDeliveryTimeRange;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid time span.");
        }

        while (true)
        {
            Console.Write("Risk Range (HH:MM:SS): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out var riskRange))
            {
                s_dal.Config.RiskRange = riskRange;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid time span.");
        }

        while (true)
        {
            Console.Write("Inactivity Time Range (HH:MM:SS): ");
            if (TimeSpan.TryParse(Console.ReadLine(), out var inactivityTimeRange))
            {
                s_dal.Config.InactivityTimeRange = inactivityTimeRange;
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid time span.");
        }

        Console.WriteLine("Config values updated successfully.");
        Console.WriteLine();
    }

    private static void GetValue()
    {
        if (s_dal != null)
        {
            Console.WriteLine("Current Configuration:");
            Console.WriteLine();
            Console.WriteLine($"Clock: {s_dal.Config.Clock}");
            Console.WriteLine($"Max Delivery Distance: {s_dal.Config.MaxDeliveryDistance}");
            Console.WriteLine($"Max Delivery Time Range: {s_dal.Config.MaxDeliveryTimeRange}");
            Console.WriteLine($"Risk Range: {s_dal.Config.RiskRange}");
            Console.WriteLine($"Inactivity Time Range: {s_dal.Config.InactivityTimeRange}");
        }
        else
        {
            Console.WriteLine("DAL instance is not available.");
        }
    }
}


