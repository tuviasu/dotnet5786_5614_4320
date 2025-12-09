using BlApi;
using BO;

namespace BlTest
{
    internal class Program
    {
        // BL instance
        static readonly IBl s_bl = Factory.Get();

        static void Main(string[] args)
        {
            MainMenu();
        }

        // ========================= MAIN MENU =========================
        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n===== MAIN MENU =====");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Courier");
                Console.WriteLine("3. Order");
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                switch (choice)
                {
                    case 1: AdminMenu(); break;
                    case 2: CourierMenu(); break;
                    case 3: OrderMenu(); break;
                    case 0: return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        // ========================= ADMIN MENU =========================
        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n===== ADMIN MENU =====");
                Console.WriteLine("1. Get Clock");
                Console.WriteLine("2. Advance Clock");
                Console.WriteLine("3. Get Config Value");
                Console.WriteLine("4. Set Config Value");
                Console.WriteLine("5. Reset Database");
                Console.WriteLine("6. Initialize Database");
                Console.WriteLine("0. Back");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine(s_bl.admin.GetClock());
                            break;

                        case 2:
                            Console.WriteLine("Enter TimeUnit (Minute/Hour/Day/Month/Year):");
                            if (Enum.TryParse<TimeUnit>(Console.ReadLine(), true, out var unit))
                                s_bl.admin.AdvanceClock(unit);
                            else Console.WriteLine("Invalid unit.");
                            break;

                        case 3:
                            Console.Write("Enter ConfigVariable: ");
                            if (Enum.TryParse<BO.ConfigVariable>(Console.ReadLine(), out var variable3))
                            {
                                if (variable3 == BO.ConfigVariable.NextOrderId ||
                                    variable3 == BO.ConfigVariable.NextDeliveryId)
                                {
                                    Console.WriteLine("ID generators cannot be viewed.");
                                    break;
                                }

                                var value = s_bl.admin.GetConfigValue(variable3);
                                Console.WriteLine($"Value: {value}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid ConfigVariable.");
                            }
                            break;


                        case 4:
                            Console.Write("Enter ConfigVariable: ");
                            if (Enum.TryParse<BO.ConfigVariable>(Console.ReadLine(), out var variable4))
                            {
                                if (variable4 == BO.ConfigVariable.NextOrderId ||
                                    variable4 == BO.ConfigVariable.NextDeliveryId)
                                {
                                    Console.WriteLine("ID generators cannot be modified.");
                                    break;
                                }

                                Console.Write("Enter new value: ");
                                var newValue = Console.ReadLine();
                                s_bl.admin.SetConfigValue(variable4, newValue);
                                Console.WriteLine("Value updated.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid ConfigVariable.");
                            }
                            break;


                        case 5:
                            s_bl.admin.ResetDatabase();
                            Console.WriteLine("Database reset.");
                            break;

                        case 6:
                            s_bl.admin.InitializeDatabase();
                            Console.WriteLine("Database initialized.");
                            break;

                        case 0: return;

                        default: Console.WriteLine("Invalid option."); break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
        }

        // ========================= COURIER MENU =========================
        static void CourierMenu()
        {
            while (true)
            {
                Console.WriteLine("\n===== COURIER MENU =====");
                Console.WriteLine("1. Read All");
                Console.WriteLine("2. Read");
                Console.WriteLine("3. Create");
                Console.WriteLine("4. Update");
                Console.WriteLine("5. Delete");
                Console.WriteLine("0. Back");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid option.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1: // Read All
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int req1);

                            foreach (var c in s_bl.Courier.ReadAll(req1))
                                Console.WriteLine(c);
                            break;

                        case 2: // Read
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int req2);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int id2);

                            Console.WriteLine(s_bl.Courier.Read(req2, id2));
                            break;

                        case 3: // Create
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int req3);

                            BO.Courier newC = new();

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int ncid);

                            newC.Id = ncid;

                            Console.Write("Courier Name: ");
                            newC.Name = Console.ReadLine() ?? "";

                            Console.Write("Phone: ");
                            newC.Phone = Console.ReadLine() ?? "";

                            s_bl.Courier.Create(req3, newC);
                            Console.WriteLine("Courier created.");
                            break;

                        case 4: // Update
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int req4);

                            BO.Courier upC = new();

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int upid);

                            upC.Id = upid;

                            Console.Write("New Name: ");
                            upC.Name = Console.ReadLine() ?? "";

                            Console.Write("New Phone: ");
                            upC.Phone = Console.ReadLine() ?? "";

                            s_bl.Courier.Update(req4, upC);
                            Console.WriteLine("Updated.");
                            break;

                        case 5: // Delete
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int req5);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int delId);

                            s_bl.Courier.Delete(req5, delId);
                            Console.WriteLine("Deleted.");
                            break;

                        case 0:
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
        }


      
        // ========================= ORDER MENU =========================
        static void OrderMenu()
        {
            while (true)
            {
                Console.WriteLine("\n===== ORDER MENU =====");
                Console.WriteLine("1. Read All");
                Console.WriteLine("2. Read");
                Console.WriteLine("3. Create");
                Console.WriteLine("4. Update");
                Console.WriteLine("5. Cancel");
                Console.WriteLine("6. Delete");
                Console.WriteLine("7. Take Order (Assign to Courier)");
                Console.WriteLine("8. Complete Treatment");
                Console.WriteLine("9. Read Closed Deliveries");
                Console.WriteLine("10. Read Open Orders");
                Console.WriteLine("11. Orders Summary");
                Console.WriteLine("0. Back");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        // ---------------------------------------------------------
                        // 1. READ ALL ORDERS
                        // ---------------------------------------------------------
                        case 1:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r1);
                            foreach (var o in s_bl.order.ReadAll(r1))
                                Console.WriteLine(o);
                            break;

                        // ---------------------------------------------------------
                        // 2. READ ORDER
                        // ---------------------------------------------------------
                        case 2:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r2);

                            Console.Write("Order ID: ");
                            int.TryParse(Console.ReadLine(), out int oid2);

                            Console.WriteLine(s_bl.order.Read(r2, oid2));
                            break;

                        // ---------------------------------------------------------
                        // 3. CREATE ORDER (FULL)
                        // ---------------------------------------------------------
                        case 3:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r3);

                            BO.Order newO = new()
                            {
                                // Customer
                                CustomerName = Console.ReadLine() ?? "",
                                FullAddressForDelivery = Console.ReadLine() ?? "",
                                CustomerPhone = Console.ReadLine() ?? "",
                                // Location
                                Latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : 0,
                                Longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : 0,
                                // Order Type
                                Type = (int.TryParse(Console.ReadLine(), out int tOpt) && tOpt == 2) ? BO.OrderType.Express :
                                       (tOpt == 3) ? BO.OrderType.International : BO.OrderType.Regular,
                                // Weight
                                weight = (int.TryParse(Console.ReadLine(), out int wOpt) && wOpt == 2) ? BO.WeightCategory.Medium :
                                         (wOpt == 3) ? BO.WeightCategory.Heavy : BO.WeightCategory.Light,
                                // Fragility
                                Fragility = (int.TryParse(Console.ReadLine(), out int fOpt) && fOpt == 2) ? BO.FragilityLevel.SlightlyFragile :
                                            (fOpt == 3) ? BO.FragilityLevel.Fragile :
                                            (fOpt == 4) ? BO.FragilityLevel.VeryFragile :
                                            (fOpt == 5) ? BO.FragilityLevel.ExtremelyFragile : BO.FragilityLevel.NotFragile,
                                Description = Console.ReadLine() ?? "",
                                OrderDate = s_bl.admin.GetClock()
                            };

                            s_bl.order.Create(r3, newO);
                            Console.WriteLine("Order created.");
                            break;

                        // ---------------------------------------------------------
                        // 4. UPDATE ORDER
                        // ---------------------------------------------------------
                        case 4:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r4);

                            Console.Write("Order ID: ");
                            int.TryParse(Console.ReadLine(), out int uId);

                            BO.Order upd = new() { Id = uId };

                            Console.Write("New Customer Name (leave blank to skip): ");
                            string newName = Console.ReadLine() ?? "";
                            if (!string.IsNullOrWhiteSpace(newName))
                                upd.CustomerName = newName;

                            Console.Write("New Description (leave blank to skip): ");
                            string newDesc = Console.ReadLine() ?? "";
                            if (!string.IsNullOrWhiteSpace(newDesc))
                                upd.Description = newDesc;

                            s_bl.order.Update(r4, upd);
                            Console.WriteLine("Order updated.");
                            break;

                        // ---------------------------------------------------------
                        // 5. CANCEL ORDER
                        // ---------------------------------------------------------
                        case 5:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r5);

                            Console.Write("Order ID: ");
                            int.TryParse(Console.ReadLine(), out int cancelId);

                            s_bl.order.Cancel(r5, cancelId);
                            Console.WriteLine("Order canceled.");
                            break;

                        // ---------------------------------------------------------
                        // 6. DELETE ORDER
                        // ---------------------------------------------------------
                        case 6:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r6);

                            Console.Write("Order ID: ");
                            int.TryParse(Console.ReadLine(), out int delId);

                            s_bl.order.Delete(r6, delId);
                            Console.WriteLine("Order deleted.");
                            break;

                        // ---------------------------------------------------------
                        // 7. TAKE ORDER (Assign to Courier)
                        // ---------------------------------------------------------
                        case 7:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r7);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int cId7);

                            Console.Write("Order ID: ");
                            int.TryParse(Console.ReadLine(), out int takeId);

                            s_bl.order.TakeOrder(r7, cId7, takeId);
                            Console.WriteLine("Order assigned.");
                            break;

                        // ---------------------------------------------------------
                        // 8. COMPLETE TREATMENT
                        // ---------------------------------------------------------
                        case 8:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r8);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int cId8);

                            Console.Write("Delivery ID: ");
                            int.TryParse(Console.ReadLine(), out int dId8);

                            s_bl.order.CompleteTreatment(r8, cId8, dId8);
                            Console.WriteLine("Delivery completed.");
                            break;

                        // ---------------------------------------------------------
                        // 9. READ CLOSED DELIVERIES
                        // ---------------------------------------------------------
                        case 9:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r9);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int cId9);

                            foreach (var cd in s_bl.order.ReadClosedDeliveries(r9, cId9))
                                Console.WriteLine(cd);
                            break;

                        // ---------------------------------------------------------
                        // 10. READ OPEN ORDERS
                        // ---------------------------------------------------------
                        case 10:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r10);

                            Console.Write("Courier ID: ");
                            int.TryParse(Console.ReadLine(), out int cId10);

                            foreach (var o in s_bl.order.ReadOpenOrders(r10, cId10))
                                Console.WriteLine(o);
                            break;

                        // ---------------------------------------------------------
                        // 11. ORDERS SUMMARY
                        // ---------------------------------------------------------
                        case 11:
                            Console.Write("Requester ID: ");
                            int.TryParse(Console.ReadLine(), out int r11);

                            int[] summary = s_bl.order.GetOrdersSummary(r11);

                            Console.WriteLine("\nOrder Summary:");
                            for (int i = 0; i < summary.Length; i++)
                                Console.WriteLine($"Status {i}: {summary[i]}");
                            break;

                        // ---------------------------------------------------------
                        case 0:
                            return;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
        }

        // ========================= EXCEPTION PRINTER =========================
        static void PrintException(Exception ex)
        {
            Console.WriteLine($"\nERROR: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.GetType().Name}");
                Console.WriteLine($"Inner Message: {ex.InnerException.Message}");
            }
        }
    }
}
    