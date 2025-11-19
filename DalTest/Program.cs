using Dal;
using DalApi;
using DO;

namespace DalTest
{
    // ====== ENUMS FOR MENUS ======
    internal enum MainMenuOption
    {
        Exit = 0,
        Couriers = 1,
        Orders = 2,
        Deliveries = 3,
        InitializeData = 4,
        ShowAllData = 5,
        Config = 6,
        ResetDatabaseAndConfig = 7
    }

    internal enum CrudMenuOption
    {
        Exit = 0,
        Create = 1,
        Read = 2,
        ReadAll = 3,
        Update = 4,
        Delete = 5,
        DeleteAll = 6
    }

    internal enum ConfigMenuOption
    {
        Exit = 0,
        AdvanceOneMinute = 1,
        AdvanceOneHour = 2,
        AdvanceOneDay = 3,
        ShowClock = 4,
        SetClock = 5,
        ShowConfigValues = 6,
        ResetConfig = 7
    }

    internal class Program
    {
        // ====== DAL instances ======
        // ----- Stage 1 -----
        //private static ICourier s_dalCourier = new CourierImplementation();
        //private static IOrder s_dalOrder = new OrderImplementation();
        //private static IDelivery s_dalDelivery = new DeliveryImplementation();
        //private static IConfig s_dalConfig = new ConfigImplementation();


        //static readonly IDal s_dal = new DalList(); // Stage 2 
        static readonly IDal s_dal = new DalXml();  //stage 3

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
                PrintMainMenu();

                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int choiceInt))
                {
                    Console.WriteLine("Invalid choice, please enter a number.");
                    continue;
                }

                MainMenuOption choice = (MainMenuOption)choiceInt;

                switch (choice)
                {
                    case MainMenuOption.Exit:
                        exit = true;
                        break;

                    case MainMenuOption.Couriers:
                        CourierMenu();
                        break;

                    case MainMenuOption.Orders:
                        OrderMenu();
                        break;

                    case MainMenuOption.Deliveries:
                        DeliveryMenu();
                        break;

                    case MainMenuOption.InitializeData:
                        // ----- Stage 1 -----
                        //Initialization.Do(s_dalCourier, s_dalOrder, s_dalDelivery, s_dalConfig);

                        // ----- Stage 2 -----
                        Initialization.Do(s_dal);
                        Console.WriteLine("Data initialization completed successfully!");
                        //Console.ReadKey();
                        break;

                    case MainMenuOption.ShowAllData:
                        ShowAllData();
                        break;

                    case MainMenuOption.Config:
                        ConfigMenu();
                        break;

                    case MainMenuOption.ResetDatabaseAndConfig:
                        ResetAllData();
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }

        private static void PrintMainMenu()
        {
            Console.WriteLine("\n===== MAIN MENU =====");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Couriers menu");
            Console.WriteLine("2. Orders menu");
            Console.WriteLine("3. Deliveries menu");
            Console.WriteLine("4. Initialize data (Initialization.Do)");
            Console.WriteLine("5. Show ALL data (Couriers + Orders + Deliveries)");
            Console.WriteLine("6. Config menu");
            Console.WriteLine("7. Reset all data (DeleteAll + Config.Reset)");
            Console.Write("Choose option: ");
        }

        // ==================== GLOBAL RESET & SHOW ALL DATA ====================

        private static void ResetAllData()
        {
            Console.WriteLine("Resetting all DAL data and configuration...");

            // ----- Stage 1 -----
            //s_dalCourier.DeleteAll();
            //s_dalOrder.DeleteAll();
            //s_dalDelivery.DeleteAll();
            //s_dalConfig.Reset();

            // ----- Stage 2 -----
            s_dal.Courier.DeleteAll();
            s_dal.Order.DeleteAll();
            s_dal.Delivery.DeleteAll();
            s_dal.Config.Reset();

            Console.WriteLine("Reset completed.");
        }

        private static void ShowAllData()
        {
            Console.WriteLine("===== ALL DATA IN SYSTEM =====\n");

            // ----- Stage 1 -----
            //Console.WriteLine("--- Couriers ---");
            //foreach (Courier c in s_dalCourier.ReadAll())
            //    Console.WriteLine(c);
            //Console.WriteLine("\n--- Orders ---");
            //foreach (Order o in s_dalOrder.ReadAll())
            //    Console.WriteLine(o);
            //Console.WriteLine("\n--- Deliveries ---");
            //foreach (Delivery d in s_dalDelivery.ReadAll())
            //    Console.WriteLine(d);

            // ----- Stage 2 -----
            Console.WriteLine("--- Couriers ---");
            foreach (Courier c in s_dal.Courier.ReadAll())
                Console.WriteLine(c);

            Console.WriteLine("\n--- Orders ---");
            foreach (Order o in s_dal.Order.ReadAll())
                Console.WriteLine(o);

            Console.WriteLine("\n--- Deliveries ---");
            foreach (Delivery d in s_dal.Delivery.ReadAll())
                Console.WriteLine(d);
        }

        // ==================== COURIER MENU ====================

        private static void CourierMenu()
        {
            bool exit = false;
            while (!exit)
            {
                PrintCrudMenu("Courier");

                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int choiceInt))
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                CrudMenuOption choice = (CrudMenuOption)choiceInt;

                try
                {
                    switch (choice)
                    {
                        case CrudMenuOption.Exit:
                            exit = true;
                            break;

                        case CrudMenuOption.DeleteAll:
                            // ----- Stage 1 -----
                            //s_dalCourier.DeleteAll();
                            // ----- Stage 2 -----
                            s_dal.Courier.DeleteAll();
                            Console.WriteLine("All couriers deleted.");
                            break;

                        default:
                            Console.WriteLine("Other CRUD actions are unchanged.");
                            break;
                    }
                }
                catch (DalAlreadyExistsException ex)
                {
                    Console.WriteLine($"Courier creation failed: {ex.Message}");
                }
                catch (DalDoesNotExistException ex)
                {
                    Console.WriteLine($"Courier operation failed: {ex.Message}");
                }
                catch (DalAccessException ex)
                {
                    Console.WriteLine($"Data access error: {ex.Message}");
                }
                catch (DalInvalidDataException ex)
                {
                    Console.WriteLine($"Invalid data error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        // ==================== ORDER MENU ====================

        private static void OrderMenu()
        {
            bool exit = false;
            while (!exit)
            {
                PrintCrudMenu("Order");

                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int choiceInt))
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                CrudMenuOption choice = (CrudMenuOption)choiceInt;

                try
                {
                    switch (choice)
                    {
                        case CrudMenuOption.Exit:
                            exit = true;
                            break;

                        case CrudMenuOption.DeleteAll:
                            // ----- Stage 1 -----
                            //s_dalOrder.DeleteAll();
                            // ----- Stage 2 -----
                            s_dal.Order.DeleteAll();
                            Console.WriteLine("All orders deleted.");
                            break;

                        default:
                            Console.WriteLine("Other CRUD actions are unchanged.");
                            break;
                    }
                }
                catch (DalAlreadyExistsException ex)
                {
                    Console.WriteLine($"Order creation failed: {ex.Message}");
                }
                catch (DalDoesNotExistException ex)
                {
                    Console.WriteLine($"Order not found: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }

            }
        }

        // ==================== DELIVERY MENU ====================

        private static void DeliveryMenu()
        {
            bool exit = false;
            while (!exit)
            {
                PrintCrudMenu("Delivery");

                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int choiceInt))
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                CrudMenuOption choice = (CrudMenuOption)choiceInt;

                try
                {
                    switch (choice)
                    {
                        case CrudMenuOption.Exit:
                            exit = true;
                            break;

                        case CrudMenuOption.DeleteAll:
                            // ----- Stage 1 -----
                            //s_dalDelivery.DeleteAll();
                            // ----- Stage 2 -----
                            s_dal.Delivery.DeleteAll();
                            Console.WriteLine("All deliveries deleted.");
                            break;

                        default:
                            Console.WriteLine("Other CRUD actions are unchanged.");
                            break;
                    }
                }
                catch (DalDoesNotExistException ex)
                {
                    Console.WriteLine($"Delivery not found: {ex.Message}");
                }
                catch (DalInvalidDataException ex)
                {
                    Console.WriteLine($"Invalid delivery data: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }

            }
        }

        // ==================== CONFIG MENU ====================

        private static void ConfigMenu()
        {
            bool exit = false;
            while (!exit)
            {
                PrintConfigMenu();

                string? input = Console.ReadLine();
                Console.WriteLine();

                if (!int.TryParse(input, out int choiceInt))
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                ConfigMenuOption choice = (ConfigMenuOption)choiceInt;

                try
                {
                    switch (choice)
                    {
                        case ConfigMenuOption.Exit:
                            exit = true;
                            break;

                        case ConfigMenuOption.ResetConfig:
                            // ----- Stage 1 -----
                            //s_dalConfig.Reset();
                            // ----- Stage 2 -----
                            s_dal.Config.Reset();
                            Console.WriteLine("Config reset to initial values.");
                            break;

                        default:
                            Console.WriteLine("Other config options remain the same.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in config menu: {ex.Message}");
                }
            }
        }

        // ==================== MENU HELPERS ====================

        private static void PrintCrudMenu(string entityName)
        {
            Console.WriteLine($"\n--- {entityName.ToUpper()} MENU ---");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("1. Create new");
            Console.WriteLine("2. Read by Id");
            Console.WriteLine("3. Show all");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Delete ALL");
            Console.Write("Choose option: ");
        }

        private static void PrintConfigMenu()
        {
            Console.WriteLine("\n--- CONFIG MENU ---");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("6. Show current config values");
            Console.WriteLine("7. Reset all config values");
            Console.Write("Choose option: ");
        }
    }
}
