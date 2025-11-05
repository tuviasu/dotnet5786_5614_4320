using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        // ====== DAL instances (Stage 1) ======
        private static readonly ICourier s_dalCourier = new CourierImplementation();      // stage 1
        private static readonly IOrder s_dalOrder = new OrderImplementation();            // stage 1
        private static readonly IDelivery s_dalDelivery = new DeliveryImplementation();   // stage 1
        private static readonly IConfig s_dalConfig = new ConfigImplementation();         // stage 1

        static void Main(string[] args)
        {
            try
            {
                MainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n*** Exception caught in main: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // ==================== MAIN MENU ====================

        private static void MainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n===== MAIN MENU =====");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Couriers menu");
                Console.WriteLine("2. Orders menu");
                Console.WriteLine("3. Deliveries menu");
                Console.WriteLine("4. Initialize data (Initialization.Do)");
                Console.WriteLine("5. Reset all data (DeleteAll + Config.Reset)");
                Console.Write("Choose option: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        CourierMenu();
                        break;
                    case "2":
                        OrderMenu();
                        break;
                    case "3":
                        DeliveryMenu();
                        break;
                    case "4":
                        Initialization.Do(s_dalCourier, s_dalOrder, s_dalDelivery, s_dalConfig);
                        break;
                    case "5":
                        ResetAllData();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        // ==================== GLOBAL RESET ====================

        private static void ResetAllData()
        {
            Console.WriteLine("Resetting all DAL data and configuration...");
            s_dalCourier.DeleteAll();
            s_dalOrder.DeleteAll();
            s_dalDelivery.DeleteAll();
            s_dalConfig.Reset();
            Console.WriteLine("Reset completed.");
        }

        // ==================== COURIER MENU ====================

        private static void CourierMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- COURIER MENU ---");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("1. Create new courier");
                Console.WriteLine("2. Read courier by Id");
                Console.WriteLine("3. Show all couriers");
                Console.WriteLine("4. Update courier");
                Console.WriteLine("5. Delete courier");
                Console.WriteLine("6. Delete ALL couriers");
                Console.Write("Choose option: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "0": exit = true; break;
                        case "1": CreateCourier(); break;
                        case "2": ReadCourier(); break;
                        case "3": ReadAllCouriers(); break;
                        case "4": UpdateCourier(); break;
                        case "5": DeleteCourier(); break;
                        case "6": s_dalCourier.DeleteAll(); Console.WriteLine("All couriers deleted."); break;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in courier menu: {ex.Message}");
                }
            }
        }

        private static void CreateCourier()
        {
            Console.WriteLine("Creating new courier:");

            int id = ReadInt("Enter Id (Teudat Zehut): ");
            Console.Write("Enter name: ");
            string? name = Console.ReadLine() ?? "";

            Console.Write("Enter phone (e.g. 0501234567): ");
            string? phone = Console.ReadLine() ?? "";

            Console.Write("Enter email: ");
            string? email = Console.ReadLine() ?? "";

            Console.Write("Enter password: ");
            string? password = Console.ReadLine() ?? "";

            bool isActive = ReadBool("Is active? (y/n): ");

            Console.WriteLine("Delivery transport type:");
            Console.WriteLine("0 - Bicycle");
            Console.WriteLine("1 - Motorcycle");
            Console.WriteLine("2 - Car");
            int transportInt = ReadInt("Choose transport (0/1/2): ");
            DeliveryTransport transport = (DeliveryTransport)transportInt;

            double? maxDistance = ReadDoubleNullable("Enter max distance (km) or leave empty: ");

            Courier courier = new Courier(
                id,
                name,
                phone,
                email,
                password,
                isActive,
                transport,
                maxDistance);

            s_dalCourier.Create(courier);
            Console.WriteLine("Courier created successfully.");
        }

        private static void ReadCourier()
        {
            int id = ReadInt("Enter Id of courier to read: ");
            Courier? c = s_dalCourier.Read(id);
            Console.WriteLine(c is null ? "Courier not found." : c.ToString());
        }

        private static void ReadAllCouriers()
        {
            Console.WriteLine("All couriers:");
            foreach (Courier c in s_dalCourier.ReadAll())
                Console.WriteLine(c);
        }

        private static void UpdateCourier()
        {
            int id = ReadInt("Enter Id of courier to update: ");
            Courier? c = s_dalCourier.Read(id);
            if (c is null)
            {
                Console.WriteLine("Courier not found.");
                return;
            }

            Console.WriteLine("Current courier:");
            Console.WriteLine(c);

            Console.Write("Enter new phone (empty = no change): ");
            string? phone = Console.ReadLine();
            Console.Write("Enter new email (empty = no change): ");
            string? email = Console.ReadLine();

            bool? isActive = ReadBoolNullable("Change active status? (y/n/empty = no change): ");

            Courier updated = c with
            {
                Phone = string.IsNullOrWhiteSpace(phone) ? c.Phone : phone,
                Email = string.IsNullOrWhiteSpace(email) ? c.Email : email,
                IsActive = isActive ?? c.IsActive
            };

            s_dalCourier.Update(updated);
            Console.WriteLine("Courier updated.");
        }

        private static void DeleteCourier()
        {
            int id = ReadInt("Enter Id of courier to delete: ");
            s_dalCourier.Delete(id);
            Console.WriteLine("Courier deleted.");
        }

        // ==================== ORDER MENU ====================

        private static void OrderMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- ORDER MENU ---");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("1. Create new order");
                Console.WriteLine("2. Read order by Id");
                Console.WriteLine("3. Show all orders");
                Console.WriteLine("4. Update order");
                Console.WriteLine("5. Delete order");
                Console.WriteLine("6. Delete ALL orders");
                Console.Write("Choose option: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "0": exit = true; break;
                        case "1": CreateOrder(); break;
                        case "2": ReadOrder(); break;
                        case "3": ReadAllOrders(); break;
                        case "4": UpdateOrder(); break;
                        case "5": DeleteOrder(); break;
                        case "6": s_dalOrder.DeleteAll(); Console.WriteLine("All orders deleted."); break;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in order menu: {ex.Message}");
                }
            }
        }

        private static void CreateOrder()
        {
            Console.WriteLine("Creating new order:");

            int id = s_dalConfig.NextOrderId; // running id from config

            Console.Write("Enter customer name: ");
            string customerName = Console.ReadLine() ?? "";

            Console.Write("Enter address / city: ");
            string address = Console.ReadLine() ?? "";

            double weight = ReadDouble("Enter weight (kg): ");

            DateTime orderDate = ReadDateTime("Enter order date (dd/MM/yy HH:mm:ss): ");

            Order order = new Order(
                id,
                customerName,
                address,
                weight,
                orderDate);

            s_dalOrder.Create(order);
            Console.WriteLine($"Order created with Id = {id}");
        }

        private static void ReadOrder()
        {
            int id = ReadInt("Enter Id of order to read: ");
            Order? o = s_dalOrder.Read(id);
            Console.WriteLine(o is null ? "Order not found." : o.ToString());
        }

        private static void ReadAllOrders()
        {
            Console.WriteLine("All orders:");
            foreach (Order o in s_dalOrder.ReadAll())
                Console.WriteLine(o);
        }

        private static void UpdateOrder()
        {
            int id = ReadInt("Enter Id of order to update: ");
            Order? o = s_dalOrder.Read(id);
            if (o is null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.WriteLine("Current order:");
            Console.WriteLine(o);

            Console.Write("Enter new address (empty = no change): ");
            string? address = Console.ReadLine();

            double? newWeight = ReadDoubleNullable("Enter new weight (empty = no change): ");

            Order updated = o with
            {
                CustomerAddress = string.IsNullOrWhiteSpace(address) ? o.CustomerAddress : address,
                weight = newWeight ?? o.weight
            };

            s_dalOrder.Update(updated);
            Console.WriteLine("Order updated.");
        }

        private static void DeleteOrder()
        {
            int id = ReadInt("Enter Id of order to delete: ");
            s_dalOrder.Delete(id);
            Console.WriteLine("Order deleted.");
        }

        // ==================== DELIVERY MENU ====================

        private static void DeliveryMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- DELIVERY MENU ---");
                Console.WriteLine("0. Back to main menu");
                Console.WriteLine("1. Create new delivery");
                Console.WriteLine("2. Read delivery by Id");
                Console.WriteLine("3. Show all deliveries");
                Console.WriteLine("4. Update delivery");
                Console.WriteLine("5. Delete delivery");
                Console.WriteLine("6. Delete ALL deliveries");
                Console.Write("Choose option: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "0": exit = true; break;
                        case "1": CreateDelivery(); break;
                        case "2": ReadDelivery(); break;
                        case "3": ReadAllDeliveries(); break;
                        case "4": UpdateDelivery(); break;
                        case "5": DeleteDelivery(); break;
                        case "6": s_dalDelivery.DeleteAll(); Console.WriteLine("All deliveries deleted."); break;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in delivery menu: {ex.Message}");
                }
            }
        }

        private static void CreateDelivery()
        {
            Console.WriteLine("Creating new delivery:");

            int id = s_dalConfig.NextDeliveryId;

            int courierId = ReadInt("Enter courier Id: ");
            int orderId = ReadInt("Enter order Id: ");

            // Add prompt for DeliveryTransport
            Console.WriteLine("Delivery transport type:");
            Console.WriteLine("0 - Bicycle");
            Console.WriteLine("1 - Motorcycle");
            Console.WriteLine("2 - Car");
            int transportInt = ReadInt("Choose transport (0/1/2): ");
            DeliveryTransport transport = (DeliveryTransport)transportInt;

            DateTime deliveryDate = ReadDateTime("Enter delivery date (dd/MM/yy HH:mm:ss): ");

            bool delivered = ReadBool("Is delivered? (y/n): ");

            // Use object initializer to set properties (fixes CS1729 and IDE0090)
            Delivery d = new Delivery
            {
                Id = id,
                CourierId = courierId,
                OrderedId = orderId,
                DeliveryType = transport,
                StartDelivery = deliveryDate,
                ActualDistance = null,
                CompletionType = null,
                EndDelivery = null,
                status = delivered ? "Delivered" : "Pending"
            };

            s_dalDelivery.Create(d);
            Console.WriteLine($"Delivery created with Id = {id}");
        }

        private static void ReadDelivery()
        {
            int id = ReadInt("Enter Id of delivery to read: ");
            Delivery? d = s_dalDelivery.Read(id);
            Console.WriteLine(d is null ? "Delivery not found." : d.ToString());
        }

        private static void ReadAllDeliveries()
        {
            Console.WriteLine("All deliveries:");
            foreach (Delivery d in s_dalDelivery.ReadAll())
                Console.WriteLine(d);
        }

        private static void UpdateDelivery()
        {
            int id = ReadInt("Enter Id of delivery to update: ");
            Delivery? d = s_dalDelivery.Read(id);
            if (d is null)
            {
                Console.WriteLine("Delivery not found.");
                return;
            }

            Console.WriteLine("Current delivery:");
            Console.WriteLine(d);

            bool? delivered = ReadBoolNullable("Change delivered status? (y/n/empty = no change): ");

            Delivery updated = d with
            {
                status = delivered.HasValue
                    ? (delivered.Value ? "Delivered" : "Pending")
                    : d.status
            };

            s_dalDelivery.Update(updated);
            Console.WriteLine("Delivery updated.");
        }

        private static void DeleteDelivery()
        {
            int id = ReadInt("Enter Id of delivery to delete: ");
            s_dalDelivery.Delete(id);
            Console.WriteLine("Delivery deleted.");
        }

        // ==================== HELPER INPUT METHODS ====================

        private static int ReadInt(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int value))
                throw new FormatException("Input is not a valid integer.");
            return value;
        }

        private static double ReadDouble(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double value))
                throw new FormatException("Input is not a valid double.");
            return value;
        }

        private static double? ReadDoubleNullable(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (!double.TryParse(input, out double value))
                throw new FormatException("Input is not a valid double.");
            return value;
        }

        private static DateTime ReadDateTime(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (!DateTime.TryParse(input, out DateTime dt))
                throw new FormatException("Date/time is invalid.");
            return dt;
        }

        private static bool ReadBool(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            return input != null && (input.Equals("y", StringComparison.OrdinalIgnoreCase)
                                     || input.Equals("yes", StringComparison.OrdinalIgnoreCase));
        }

        private static bool? ReadBoolNullable(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return input.Equals("y", StringComparison.OrdinalIgnoreCase)
                || input.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
