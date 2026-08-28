namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class OrderImplementation : IOrder
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        if (item == null)
        {
            throw new DalItemNotExist("The order item cannot be null.");
        }

        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);
        if (Orders.Exists(it => it.OrderID == item.OrderID))
            throw new DalItemNotExist("Order with ID " + item.OrderID + " already exists.");
        Orders.Add(item);
        XMLTools.SaveListToXMLSerializer(Orders, Config.OrdersFileName);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);
        if (Orders.RemoveAll(it => it.OrderID == id) == 0)
            throw new DalItemNotExist("Order with ID " + id + " does not exist.");
        XMLTools.SaveListToXMLSerializer(Orders, Config.OrdersFileName);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Order>(), Config.OrdersFileName);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {
        if (id <= 0)
        {
            throw new DalItemNotExist("The order ID must be a positive integer.");
        }

        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);

        foreach (var order in Orders)
        {
            if (order.OrderID == id)
            {
                return order;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter)
    {
        if (filter == null)
            throw new DalItemNotExist("The filter function cannot be null.");

        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);

        foreach (var order in Orders)
        {
            if (filter(order))
            {
                return order;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);

        if (filter == null)
            return Orders;
        else
            return Orders.FindAll(order => filter(order));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.OrdersFileName);
        if (Orders.RemoveAll(it => it.OrderID == item.OrderID) == 0)
            throw new DalItemNotExist("Order with ID " + item.OrderID + " does not exist.");
        Orders.Add(item);
        XMLTools.SaveListToXMLSerializer(Orders, Config.OrdersFileName);
    }
}
