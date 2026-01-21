namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("         MAIN MENU - BL TEST             ");
            Console.WriteLine("=========================================");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Courier menu");
            Console.WriteLine("2. Order menu");
            Console.WriteLine("3. Admin menu");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 3)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter a valid number (0-3).");
                Console.WriteLine();
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        Console.WriteLine();
                        CourierMenu();
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        OrderMenu();
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine();
                        AdminMenu();
                        Console.WriteLine();
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error - Item not found: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlItemAlreadyExistsException ex)
            {
                Console.WriteLine($"Error - Item already exists: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlInvalidIdException ex)
            {
                Console.WriteLine($"Error - Invalid data: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlTemporaryNotAvailableException ex)
            {
                Console.WriteLine($"Error - Temporarily not available: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    #region Courier Menu

    static void CourierMenu()
    {
        while (true)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("            COURIER MENU                 ");
            Console.WriteLine("=========================================");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("1. Authenticate courier");
            Console.WriteLine("2. Get couriers list");
            Console.WriteLine("3. Get courier details");
            Console.WriteLine("4. Add courier");
            Console.WriteLine("5. Update courier");
            Console.WriteLine("6. Delete courier");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 6)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter a valid number (0-6).");
                Console.WriteLine();
                continue;
            }

            if (choice == 0) return;

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine();
                        AuthenticateCourier();
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        GetCouriersList();
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine();
                        GetCourierDetails();
                        Console.WriteLine();
                        break;
                    case 4:
                        Console.WriteLine();
                        AddCourier();
                        Console.WriteLine();
                        break;
                    case 5:
                        Console.WriteLine();
                        UpdateCourier();
                        Console.WriteLine();
                        break;
                    case 6:
                        Console.WriteLine();
                        DeleteCourier();
                        Console.WriteLine();
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error - Item not found: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlItemAlreadyExistsException ex)
            {
                Console.WriteLine($"Error - Item already exists: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlInvalidIdException ex)
            {
                Console.WriteLine($"Error - Invalid data: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    static void AuthenticateCourier()
    {
        Console.Write("Enter username (or 0 to go back): ");
        string username = Console.ReadLine() ?? "";

        if (username == "0")
            return;

        if (username == "")
        {
            Console.WriteLine();
            Console.WriteLine("Username cannot be NULL.");
            AuthenticateCourier();
        }

        Console.Write("Enter password: ");
        string password = Console.ReadLine() ?? "";

        if (password == "")
        {
            Console.WriteLine();
            Console.WriteLine("Password cannot be NULL.");
            AuthenticateCourier();
        }

        string role = s_bl.Courier.AuthenticateCourier(username, password);
        Console.WriteLine($"Authentication successful! Role: {role}");
    }

    static void GetCouriersList()
    {
        Console.Write("Enter requester ID (or 0 to go back): ");

        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.Write("Filter by active status? (y/n/enter for all): ");
        string activeInput = Console.ReadLine() ?? "";
        bool? isActive = activeInput.ToLower() == "y" ? true :
                         activeInput.ToLower() == "n" ? false : null;

        Console.Write("Sort by property (or enter for default): ");
        string sortBy = Console.ReadLine()!;

        var couriers = s_bl.Courier.GetCouriersList(requesterId, isActive, sortBy);

        Console.WriteLine();
        Console.WriteLine("Couriers List:");
        Console.WriteLine("=========================================");
        foreach (var courier in couriers)
        {
            Console.WriteLine(courier);
            Console.WriteLine("-----------------------------------------");
        }
    }

    static void GetCourierDetails()
    {
        Console.Write("Enter requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.Write("Enter courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
        {
            Console.WriteLine("Invalid courier ID.");
            return;
        }

        var courier = s_bl.Courier.GetCourierDetails(requesterId, courierId);
        Console.WriteLine();
        Console.WriteLine(courier);
    }

    static void AddCourier()
    {
        Console.Write("Enter requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.Write("Courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
        {
            Console.WriteLine("Invalid courier ID.");
            return;
        }

        Console.Write("Full Name: ");
        string fullName = Console.ReadLine() ?? "";

        Console.Write("Phone: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Transport Type (Car/Motorcycle/Bicycle/Walk): ");
        if (!Enum.TryParse<BO.DeliveryTransport>(Console.ReadLine(), true, out var transport))
        {
            Console.WriteLine("Invalid transport type.");
            return;
        }

        Console.Write("Password: ");
        string password = Console.ReadLine();
        while (password == "")
        {
            Console.WriteLine();
            Console.WriteLine("Password cannot be NULL.");
            Console.Write("Password: ");
            password = Console.ReadLine()!;
        }

        Console.Write("Max Delivery Distance (km): ");
        if (!double.TryParse(Console.ReadLine(), out double maxDistance))
        {
            Console.WriteLine("Invalid distance.");
            return;
        }

        var newCourier = new BO.Courier
        {
            CourierID = courierId,
            FullName = fullName,
            Phone = phone,
            Email = email,
            TransportType = transport,
            Password = password,
            MaxDeliveryDistanceKM = maxDistance,
            IsActive = true
        };

        s_bl.Courier.AddCourier(requesterId, newCourier);
        Console.WriteLine("Courier added successfully.");
    }

    static void UpdateCourier()
    {
        Console.Write("Enter requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.Write("Enter courier ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
        {
            Console.WriteLine("Invalid courier ID.");
            return;
        }

        var courier = s_bl.Courier.GetCourierDetails(requesterId, courierId);
        Console.WriteLine("Current courier details:");
        Console.WriteLine(courier);

        Console.Write($"Full Name ({courier.FullName}): ");
        string fullName = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(fullName))
            courier.FullName = fullName;

        Console.Write($"Phone ({courier.Phone}): ");
        string phone = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(phone))
            courier.Phone = phone;

        Console.Write($"Email ({courier.Email}): ");
        string email = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(email))
            courier.Email = email;

        Console.Write($"IsActive ({courier.IsActive}): ");
        if (bool.TryParse(Console.ReadLine(), out bool isActive))
            courier.IsActive = isActive;

        s_bl.Courier.UpdateCourier(requesterId, courier);
        Console.WriteLine("Courier updated successfully.");
    }

    static void DeleteCourier()
    {
        Console.Write("Enter requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.Write("Enter courier ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
        {
            Console.WriteLine("Invalid courier ID.");
            return;
        }

        s_bl.Courier.DeleteCourier(requesterId, courierId);
        Console.WriteLine($"Courier {courierId} deleted successfully.");
    }

    #endregion

    #region Order Menu

    static void OrderMenu()
    {
        while (true)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("             ORDER MENU                  ");
            Console.WriteLine("=========================================");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("1. Get orders summary");
            Console.WriteLine("2. Get orders list");
            Console.WriteLine("3. Get order details");
            Console.WriteLine("4. Add order");
            Console.WriteLine("5. Update order");
            Console.WriteLine("6. Cancel order");
            Console.WriteLine("7. Delete order");
            Console.WriteLine("8. Complete order handling");
            Console.WriteLine("9. Select order for handling");
            Console.WriteLine("10. Get closed orders by courier");
            Console.WriteLine("11. Get open orders for courier");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 11)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter a valid number (0-11).");
                Console.WriteLine();
                continue;
            }

            if (choice == 0) return;

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine();
                        GetOrdersSummary();
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        GetOrdersList();
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine();
                        GetOrderDetails();
                        Console.WriteLine();
                        break;
                    case 4:
                        Console.WriteLine();
                        AddOrder();
                        Console.WriteLine();
                        break;
                    case 5:
                        Console.WriteLine();
                        UpdateOrder();
                        Console.WriteLine();
                        break;
                    case 6:
                        Console.WriteLine();
                        CancelOrder();
                        Console.WriteLine();
                        break;
                    case 7:
                        Console.WriteLine();
                        DeleteOrder();
                        Console.WriteLine();
                        break;
                    case 8:
                        Console.WriteLine();
                        CompleteOrderHandling();
                        Console.WriteLine();
                        break;
                    case 9:
                        Console.WriteLine();
                        SelectOrderForHandling();
                        Console.WriteLine();
                        break;
                    case 10:
                        Console.WriteLine();
                        GetClosedOrdersByCourier();
                        Console.WriteLine();
                        break;
                    case 11:
                        Console.WriteLine();
                        GetOpenOrdersForCourier();
                        Console.WriteLine();
                        break;
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error - Item not found: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlItemAlreadyExistsException ex)
            {
                Console.WriteLine($"Error - Item already exists: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (BO.BlInvalidIdException ex)
            {
                Console.WriteLine($"Error - Invalid data: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    static void GetOrdersSummary()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        var summary = s_bl.Order.GetOrdersSummary(requesterId);
        Console.WriteLine();
        Console.WriteLine("Orders Summary:");
        for (int i = 0; i < summary.Length; i++)
        {
            Console.WriteLine($"Status {i}: {summary[i]} orders");
        }
    }

    static void GetOrdersList()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Filter by property (Status/DeliveryType/OrderType or enter for none): ");
        string filterPropInput = Console.ReadLine()!;
        BO.OrderListFilterProperty? filterProperty = string.IsNullOrWhiteSpace(filterPropInput) ? null :
            Enum.TryParse<BO.OrderListFilterProperty>(filterPropInput, true, out var fp) ? fp : null;

        object? filterValue = null;
        if (filterProperty.HasValue)
        {
            Console.Write("Filter value: ");
            filterValue = Console.ReadLine();
        }

        Console.Write("Sort by (OrderId/CustomerName/OrderDate/TotalPrice or enter for default): ");
        string sortInput = Console.ReadLine()!;
        BO.OrderListSortProperty? sortProperty = string.IsNullOrWhiteSpace(sortInput) ? null :
            Enum.TryParse<BO.OrderListSortProperty>(sortInput, true, out var sp) ? sp : null;

        var orders = s_bl.Order.GetOrdersList(requesterId, filterProperty, filterValue, sortProperty);

        Console.WriteLine();
        Console.WriteLine("Orders List:");
        Console.WriteLine("=========================================");
        foreach (var order in orders)
        {
            Console.WriteLine(order);
            Console.WriteLine("-----------------------------------------");
        }
    }

    static void GetOrderDetails()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            return;
        }

        var order = s_bl.Order.GetOrderDetails(requesterId, orderId);
        Console.WriteLine();
        Console.WriteLine(order);
    }

    static void AddOrder()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Order Type (Individual/Group/Corporate): ");
        if (!Enum.TryParse<BO.OrderType>(Console.ReadLine(), true, out var orderType))
        {
            Console.WriteLine("Invalid order type.");
            return;
        }

        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";

        Console.Write("Full Address: ");
        string fullAddress = Console.ReadLine() ?? "";

        Console.Write("Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
        {
            Console.WriteLine("Invalid latitude.");
            return;
        }

        Console.Write("Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
        {
            Console.WriteLine("Invalid longitude.");
            return;
        }

        Console.Write("Customer Full Name: ");
        string customerName = Console.ReadLine() ?? "";

        Console.Write("Customer Phone: ");
        string customerPhone = Console.ReadLine() ?? "";

        Console.Write("Pizza Size (Desktop/Laptop/Tablet/Smartphone/Headphones): ");
        if (!Enum.TryParse<BO.DeviceType>(Console.ReadLine(), true, out var pizzaSize))
        {
            Console.WriteLine("Invalid pizza size.");
            return;
        }

        var newOrder = new BO.Order
        {
            OrderType = orderType,
            Description = description,
            FullAddress = fullAddress,
            Latitude = latitude,
            Longitude = longitude,
            CustomerFullName = customerName,
            CustomerPhone = customerPhone,
            PizzaSize = pizzaSize
        };

        s_bl.Order.AddOrder(requesterId, newOrder);
        Console.WriteLine("Order added successfully.");
    }

    static void UpdateOrder()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter order ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            return;
        }

        var order = s_bl.Order.GetOrderDetails(requesterId, orderId);
        Console.WriteLine("Current order details:");
        Console.WriteLine(order);

        Console.Write($"Description ({order.Description}): ");
        string description = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(description))
            order.Description = description;

        Console.Write($"Customer Full Name ({order.CustomerFullName}): ");
        string customerName = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(customerName))
            order.CustomerFullName = customerName;

        Console.Write($"Customer Phone ({order.CustomerPhone}): ");
        string customerPhone = Console.ReadLine()!;
        if (!string.IsNullOrWhiteSpace(customerPhone))
            order.CustomerPhone = customerPhone;

        s_bl.Order.UpdateOrder(requesterId, order);
        Console.WriteLine("Order updated successfully.");
    }

    static void CancelOrder()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter order ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            return;
        }

        s_bl.Order.CancelOrder(requesterId, orderId);
        Console.WriteLine($"Order {orderId} cancelled successfully.");
    }

    static void DeleteOrder()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter order ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            return;
        }

        s_bl.Order.DeleteOrder(requesterId, orderId);
        Console.WriteLine($"Order {orderId} deleted successfully.");
    }

    static void CompleteOrderHandling()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter courier ID: ");
        string courierId = Console.ReadLine() ?? "";

        Console.Write("Enter delivery ID to complete: ");
        if (!int.TryParse(Console.ReadLine(), out int deliveryId))
        {
            Console.WriteLine("Invalid delivery ID.");
            return;
        }

        s_bl.Order.CompleteOrderHandling(requesterId, courierId, deliveryId);
        Console.WriteLine("Order handling completed successfully.");
    }

    static void SelectOrderForHandling()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter courier ID: ");
        string courierId = Console.ReadLine() ?? "";

        Console.Write("Enter order ID to handle: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID.");
            return;
        }

        s_bl.Order.SelectOrderForHandling(requesterId, courierId, orderId);
        Console.WriteLine("Order selected for handling successfully.");
    }

    static void GetClosedOrdersByCourier()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter courier ID: ");
        string courierId = Console.ReadLine() ?? "";

        Console.Write("Order type filter (Individual/Group/Corporate or enter for all): ");
        string orderTypeInput = Console.ReadLine()!;
        BO.OrderType? orderTypeFilter = string.IsNullOrWhiteSpace(orderTypeInput) ? null :
            Enum.TryParse<BO.OrderType>(orderTypeInput, true, out var ot) ? ot : null;

        Console.Write("Sort by (DeliveryId/DeliveryDate/DeliveryTransport/DeliveryType or enter for default): ");
        string sortInput = Console.ReadLine()!;
        BO.ClosedDeliveryListSortProperty? sortProperty = string.IsNullOrWhiteSpace(sortInput) ? null :
            Enum.TryParse<BO.ClosedDeliveryListSortProperty>(sortInput, true, out var sp) ? sp : null;

        var closedOrders = s_bl.Order.GetClosedOrdersByCourier(requesterId, courierId, orderTypeFilter, sortProperty);

        Console.WriteLine();
        Console.WriteLine("Closed Orders:");
        Console.WriteLine("=========================================");
        foreach (var order in closedOrders)
        {
            Console.WriteLine(order);
            Console.WriteLine("-----------------------------------------");
        }
    }

    static void GetOpenOrdersForCourier()
    {
        Console.Write("Enter requester ID: ");
        string requesterId = Console.ReadLine() ?? "";

        Console.Write("Enter courier ID: ");
        string courierId = Console.ReadLine() ?? "";

        Console.Write("Order type filter (Individual/Group/Corporate or enter for all): ");
        string orderTypeInput = Console.ReadLine()!;
        BO.OrderType? orderTypeFilter = string.IsNullOrWhiteSpace(orderTypeInput) ? null :
            Enum.TryParse<BO.OrderType>(orderTypeInput, true, out var ot) ? ot : null;

        Console.Write("Sort by (OrderId/CustomerName/OrderDate or enter for default): ");
        string sortInput = Console.ReadLine()!;
        BO.OpenOrderListSortProperty? sortProperty = string.IsNullOrWhiteSpace(sortInput) ? null :
            Enum.TryParse<BO.OpenOrderListSortProperty>(sortInput, true, out var sp) ? sp : null;

        var openOrders = s_bl.Order.GetOpenOrdersForCourier(requesterId, courierId, orderTypeFilter, sortProperty);

        Console.WriteLine();
        Console.WriteLine("Open Orders:");
        Console.WriteLine("=========================================");
        foreach (var order in openOrders)
        {
            Console.WriteLine(order);
            Console.WriteLine("-----------------------------------------");
        }
    }

    #endregion

    #region Admin Menu

    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=========================================");
            Console.WriteLine("             ADMIN MENU                  ");
            Console.WriteLine("=========================================");
            Console.WriteLine("0. Back to main menu");
            Console.WriteLine("1. Reset database");
            Console.WriteLine("2. Initialize database");
            Console.WriteLine("3. Get clock");
            Console.WriteLine("4. Forward clock");
            Console.WriteLine("5. Get configuration");
            Console.WriteLine("6. Set configuration");
            Console.WriteLine("=========================================");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 6)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter a valid number (0-6).");
                Console.WriteLine();
                continue;
            }

            if (choice == 0) return;

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine();
                        ResetDB();
                        Console.WriteLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        InitializeDB();
                        Console.WriteLine();
                        break;
                    case 3:
                        Console.WriteLine();
                        GetClock();
                        Console.WriteLine();
                        break;
                    case 4:
                        Console.WriteLine();
                        ForwardClock();
                        Console.WriteLine();
                        break;
                    case 5:
                        Console.WriteLine();
                        GetConfig();
                        Console.WriteLine();
                        break;
                    case 6:
                        Console.WriteLine();
                        SetConfig();
                        Console.WriteLine();
                        break;
                }
            }
            catch (BO.BlTemporaryNotAvailableException ex)
            {
                Console.WriteLine($"Error - Temporarily not available: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    static void ResetDB()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        s_bl.Admin.ResetDB(requesterId);
        Console.WriteLine("Database reset successfully.");
    }

    static void InitializeDB()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        s_bl.Admin.InitializeDB(requesterId);
        Console.WriteLine("Database initialized successfully.");
    }

    static void GetClock()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        var clock = s_bl.Admin.GetClock(requesterId);
        Console.WriteLine($"Current system clock: {clock}");
    }

    static void ForwardClock()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        Console.WriteLine("Select time unit:");
        Console.WriteLine("0. to go back");
        Console.WriteLine("1. seconds");
        Console.WriteLine("2. Minutes");
        Console.WriteLine("3. Hours");
        Console.WriteLine("4. Days");
        Console.WriteLine("5. Years");

        Console.Write("Enter your choice: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > 5)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        if (choice == 0)
            return;

        BO.TimeUnit timeUnit = choice switch
        {
            1 => BO.TimeUnit.Month,
            2 => BO.TimeUnit.Minutes,
            3 => BO.TimeUnit.Hours,
            4 => BO.TimeUnit.Days,
            5 => BO.TimeUnit.Years,
            _ => BO.TimeUnit.Minutes
        };

        s_bl.Admin.ForwardClock(requesterId, timeUnit);
        Console.WriteLine($"Clock advanced by 1 {timeUnit}.");
        Console.WriteLine($"New clock value: {s_bl.Admin.GetClock(requesterId)}");
    }

    static void GetConfig()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        var config = s_bl.Admin.GetConfig(requesterId);
        Console.WriteLine();
        Console.WriteLine("Configuration:");
        Console.WriteLine("=========================================");
        Console.WriteLine($"Clock: {config.Clock}");
        Console.WriteLine($"Max Delivery Distance: {config.MaxDeliveryDistance}");
        Console.WriteLine($"Max Delivery Time Range: {config.MaxDeliveryTimeRange}");
        Console.WriteLine($"Risk Range: {config.RiskRange}");
        Console.WriteLine($"Inactivity Time Range: {config.InactivityTimeRange}");
    }

    static void SetConfig()
    {
        Console.Write("Enter manager requester ID: ");
        if (!int.TryParse(Console.ReadLine(), out int requesterId))
        {
            Console.WriteLine("Invalid requester ID.");
            return;
        }

        var config = s_bl.Admin.GetConfig(requesterId);
        Console.WriteLine("Current configuration:");
        Console.WriteLine($"Max Delivery Distance: {config.MaxDeliveryDistance}");

        Console.Write("Enter new Max Delivery Distance (or enter to keep current): ");
        if (int.TryParse(Console.ReadLine(), out int maxDistance))
        {
            config.MaxDeliveryDistance = maxDistance;
        }

        Console.Write($"Max Delivery Time Range ({config.MaxDeliveryTimeRange}): ");
        Console.Write("Enter new value (HH:MM:SS or enter to keep current): ");
        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan maxTimeRange))
        {
            config.MaxDeliveryTimeRange = maxTimeRange;
        }

        s_bl.Admin.SetConfig(requesterId, config);
        Console.WriteLine("Configuration updated successfully.");
    }

    #endregion
}
