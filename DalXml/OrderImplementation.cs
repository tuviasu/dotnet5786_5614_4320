
namespace Dal;
using DalApi;

using DO;


internal class OrderImplementation : IOrder
{
    // Load full list from XML
    private List<Order> LoadList() =>
        XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

    // Save full list to XML
    private void SaveList(List<Order> list) =>
        XMLTools.SaveListToXMLSerializer(list, Config.s_orders_xml);


    // ─────────────── CREATE ORDER ───────────────
    public void Create(Order item)
    {
        var list = LoadList();

        if (list.Any(o => o.Id == item.Id))
            throw new DalAlreadyExistsException($"Order with ID {item.Id} already exists");

        list.Add(item);
        SaveList(list);
    }


    // ─────────────── READ BY ID ───────────────
    public Order? Read(int id)
    {
        var list = LoadList();
        return list.FirstOrDefault(o => o.Id == id);
    }


    // ─────────────── READ WITH FILTER ───────────────
    public Order? Read(Func<Order, bool> filter)
    {
        var list = LoadList();
        return list.FirstOrDefault(filter);
    }


    // ─────────────── READ ALL ───────────────
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        var list = LoadList();
        return filter is null ? list : list.Where(filter);
    }


    // ─────────────── UPDATE ───────────────
    public void Update(Order item)
    {
        var list = LoadList();

        int index = list.FindIndex(o => o.Id == item.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Order ID {item.Id} does not exist");

        list[index] = item;
        SaveList(list);
    }


    // ─────────────── DELETE ───────────────
    public void Delete(int id)
    {
        var list = LoadList();

        int removed = list.RemoveAll(o => o.Id == id);
        if (removed == 0)
            throw new DalDoesNotExistException($"Order ID {id} not found");

        SaveList(list);
    }


    // ─────────────── DELETE ALL ───────────────
    public void DeleteAll()
    {
        SaveList(new List<Order>());
    }
}
