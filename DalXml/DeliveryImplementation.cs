//namespace Dal;

//using DalApi;
//using DO;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;

///// <summary>
///// XML-backed implementation of <see cref="IDelivery"/> that persists <see cref="Delivery"/>
///// entities to the <c>deliveries.xml</c> file using <see cref="XMLTools"/>.
///// </summary>
///// <remarks>
///// - All members are internal and intended for DAL use only.
///// - Operations are not thread-safe; callers must synchronize access if used concurrently.
///// - Methods throw <see cref="DalDoesNotExistException"/> when the requested entity is not found.
///// </remarks>
//internal class DeliveryImplementation : IDelivery
//{
//    // Helper: convert Delivery -> XElement
//    private static XElement DeliveryToXElement(Delivery item)
//        => new XElement("Delivery",
//               new XElement("Id", item.DeliveryID),
//               new XElement("Order_Id", item.OrderID),
//               new XElement("Courier_Id", item.CourierID),
//               new XElement("Delivery_Type", item.DeliveryType.ToString()),
//               new XElement("Start_Work", item.DeliveryStartTime.ToString("o")),
//               item.DeliveryDistance is not null ? new XElement("Actual_Distance", item.DeliveryDistance) : null,
//               item.DeliveryDoneType is not null ? new XElement("End_Type", item.DeliveryDoneType.ToString()) : null,
//               item.DeliveryDoneTime is not null ? new XElement("End_Time", item.DeliveryDoneTime.Value.ToString("o")) : null
//           );

//    // Helper: convert XElement -> Delivery
//    private static Delivery XElementToDelivery(XElement e)
//    {
//        int id = e.ToIntNullable("Id") ?? 0;
//        int orderId = e.ToIntNullable("Order_Id") ?? 0;
//        int courierId = e.ToIntNullable("Courier_Id") ?? 0;
//        DeliveryType? deliveryType = e.ToEnumNullable<DeliveryType>("Delivery_Type");
//        DateTime startWork = e.ToDateTimeNullable("Start_Work") ?? DateTime.MinValue;
//        double? actualDistance = e.ToDoubleNullable("Actual_Distance");
//        DeliveryDoneType? endType = e.ToEnumNullable<DeliveryDoneType>("End_Type");
//        DateTime? endTime = e.ToDateTimeNullable("End_Time");


//        return new Delivery(
//            DeliveryID: id,
//            OrderID: orderId,
//            CourierID: courierId,
//            DeliveryType: deliveryType,
//            DeliveryStartTime: startWork,
//            DeliveryDistance: actualDistance,
//            DeliveryDoneType: endType,
//            DeliveryDoneTime: endTime
//        );
//    }

//    /// <summary>
//    /// Adds a new delivery to the XML store.
//    /// </summary>
//    /// <param name="item">The delivery to create. A copy with a generated Id will be stored.</param>
//    public void Create(Delivery item)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        int nextId = Config.NextDeliveryID;
//        // Add element with DAL-assigned id
//        var element = DeliveryToXElement(item with { DeliveryID = nextId });
//        root.Add(element);
//        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
//    }

//    /// <summary>
//    /// Reads a delivery by its identifier.
//    /// </summary>
//    /// <param name="id">The Id of the delivery to retrieve.</param>
//    /// <returns>The matching <see cref="Delivery"/>.</returns>
//    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified Id exists.</exception>
//    public Delivery Read(int id)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        var elem = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("DeliveryID") == id);
//        if (elem == null)
//            throw new DalItemNotExist($"Delivery with ID {id} does not exist");
//        return XElementToDelivery(elem);
//    }

//    /// <summary>
//    /// Retrieves deliveries from the XML store.
//    /// </summary>
//    /// <param name="filter">
//    /// An optional predicate used to filter the results. If <c>null</c>, all deliveries are returned.

///// </param>
///// <returns>An <see cref="IEnumerable{Delivery}"/> of the filtered (or all) deliveries.</returns>
//    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        var list = root.Elements("Delivery").Select(XElementToDelivery);
//        if (filter != null) list = list.Where(filter);
//        return list;
//    }

//    /// <summary>
//    /// Updates an existing delivery in the XML store.
//    /// </summary>
//    /// <param name="item">The updated delivery. The <see cref="Delivery.Id"/> identifies the entity to replace.</param>
//    /// <exception cref="DalDoesNotExistException">Thrown if the delivery to update does not exist.</exception>
//    public void Update(Delivery item)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        var existing = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("DeliveryID") == item.DeliveryID);
//        if (existing == null)
//            throw new DalItemNotExist($"Delivery with ID {item.DeliveryID} does not exist");

//        existing.Remove();
//        root.Add(DeliveryToXElement(item));
//        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
//    }

//    /// <summary>
//    /// Deletes the delivery with the specified Id from the XML store.
//    /// </summary>
//    /// <param name="id">The Id of the delivery to delete.</param>
//    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified Id exists.</exception>
//    public void Delete(int id)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        var elem = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("DeliveryID") == id);
//        if (elem == null)
//            throw new DalItemNotExist($"Delivery with ID {id} does not exist");
//        elem.Remove();
//        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
//    }

//    /// <summary>
//    /// Removes all deliveries from the XML store.
//    /// </summary>
//    public void DeleteAll()
//    {
//        var root = new XElement("deliveries.xml");
//        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
//    }

//    /// <summary>
//    /// Reads a single delivery that satisfies the given predicate.
//    /// </summary>
//    /// <param name="filter">Predicate to locate the delivery.</param>
//    /// <returns>The matching <see cref="Delivery"/>.</returns>
//    /// <exception cref="Dal.DoesNotExistException">Thrown if no delivery satisfies the predicate.</exception>
//    public Delivery Read(Func<Delivery, bool> filter)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
//        var delivery = root.Elements("Delivery").Select(XElementToDelivery).FirstOrDefault(filter);
//        if (delivery == null)
//            throw new DalItemNotExist($"No Delivery entity satisfies the filter");
//        return delivery;
//    }
//}

namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

/// <summary>
/// XML-backed implementation of <see cref="IDelivery"/> that persists <see cref="Delivery"/>
/// entities to the <c>deliveries.xml</c> file using <see cref="XMLTools"/>.
/// </summary>
/// <remarks>
/// - All members are internal and intended for DAL use only.
/// - Operations are not thread-safe; callers must synchronize access if used concurrently.
/// - Methods throw <see cref="DalDoesNotExistException"/> when the requested entity is not found.
/// </remarks>
internal class DeliveryImplementation : IDelivery
{
    // Helper: convert Delivery -> XElement
    private static XElement DeliveryToXElement(Delivery item)
        => new XElement("Delivery",
               new XElement("Id", item.DeliveryID),
               new XElement("Order_Id", item.OrderID),
               new XElement("Courier_Id", item.CourierID),
               new XElement("Delivery_Type", item.DeliveryType?.ToString() ?? ""),
               new XElement("Start_Work", item.DeliveryStartTime.ToString("o")),
               item.DeliveryDistance is not null ? new XElement("Actual_Distance", item.DeliveryDistance) : null,
               item.DeliveryDoneType is not null ? new XElement("End_Type", item.DeliveryDoneType.ToString()) : null,
               item.DeliveryDoneTime is not null ? new XElement("End_Time", item.DeliveryDoneTime.Value.ToString("o")) : null
           );

    // Helper: convert XElement -> Delivery
    private static Delivery XElementToDelivery(XElement e)
    {
        int id = e.ToIntNullable("Id") ?? 0;
        int orderId = e.ToIntNullable("Order_Id") ?? 0;
        int courierId = e.ToIntNullable("Courier_Id") ?? 0;
        DeliveryType? deliveryType = e.ToEnumNullable<DeliveryType>("Delivery_Type");
        DateTime startWork = e.ToDateTimeNullable("Start_Work") ?? DateTime.MinValue;
        double? actualDistance = e.ToDoubleNullable("Actual_Distance");
        ProcessResult? endType = e.ToEnumNullable<ProcessResult>("End_Type");
        DateTime? endTime = e.ToDateTimeNullable("End_Time");


        return new Delivery(
            DeliveryID: id,
            OrderID: orderId,
            CourierID: courierId,
            DeliveryType: deliveryType,
            DeliveryStartTime: startWork,
            DeliveryDistance: actualDistance,
            DeliveryDoneType: endType,
            DeliveryDoneTime: endTime
        );
    }

    /// <summary>
    /// Adds a new delivery to the XML store.
    /// </summary>
    /// <param name="item">The delivery to create. A copy with a generated Id will be stored.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        int nextId = Config.NextDeliveryID;
        // Add element with DAL-assigned id
        var element = DeliveryToXElement(item with { DeliveryID = nextId });
        root.Add(element);
        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
    }

    /// <summary>
    /// Reads a delivery by its identifier.
    /// </summary>
    /// <param name="id">The Id of the delivery to retrieve.</param>
    /// <returns>The matching <see cref="Delivery"/>.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified Id exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery Read(int id)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        // *** תיקון: שינוי מ-"DeliveryID" ל-"Id" ***
        var elem = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("Id") == id);
        if (elem == null)
            throw new DalItemNotExist($"Delivery with ID {id} does not exist");
        return XElementToDelivery(elem);
    }

    /// <summary>
    /// Retrieves deliveries from the XML store.
    /// </summary>
    /// <param name="filter">
    /// An optional predicate used to filter the results. If <c>null</c>, all deliveries are returned.

    /// </param>
    /// <returns>An <see cref="IEnumerable{Delivery}"/> of the filtered (or all) deliveries.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        var list = root.Elements("Delivery").Select(XElementToDelivery);
        if (filter != null) list = list.Where(filter);
        return list;
    }

    /// <summary>
    /// Updates an existing delivery in the XML store.
    /// </summary>
    /// <param name="item">The updated delivery. The <see cref="Delivery.Id"/> identifies the entity to replace.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the delivery to update does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        // *** תיקון: שינוי מ-"DeliveryID" ל-"Id" ***
        var existing = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("Id") == item.DeliveryID);
        if (existing == null)
            throw new DalItemNotExist($"Delivery with ID {item.DeliveryID} does not exist");

        existing.Remove();
        root.Add(DeliveryToXElement(item));
        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
    }

    /// <summary>
    /// Deletes the delivery with the specified Id from the XML store.
    /// </summary>
    /// <param name="id">The Id of the delivery to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified Id exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        // *** תיקון: שינוי מ-"DeliveryID" ל-"Id" ***
        var elem = root.Elements("Delivery").FirstOrDefault(x => x.ToIntNullable("Id") == id);
        if (elem == null)
            throw new DalItemNotExist($"Delivery with ID {id} does not exist");
        elem.Remove();
        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
    }

    /// <summary>
    /// Removes all deliveries from the XML store.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        var root = new XElement("deliveries.xml");
        XMLTools.SaveListToXMLElement(root, "deliveries.xml");
    }

    /// <summary>
    /// Reads a single delivery that satisfies the given predicate.
    /// </summary>
    /// <param name="filter">Predicate to locate the delivery.</param>
    /// <returns>The matching <see cref="Delivery"/>.</returns>
    /// <exception cref="Dal.DoesNotExistException">Thrown if no delivery satisfies the predicate.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery Read(Func<Delivery, bool> filter)
    {
        XElement root = XMLTools.LoadListFromXMLElement("deliveries.xml");
        var delivery = root.Elements("Delivery").Select(XElementToDelivery).FirstOrDefault(filter);
        if (delivery == null)
            throw new DalItemNotExist($"No Delivery entity satisfies the filter");
        return delivery;
    }
}