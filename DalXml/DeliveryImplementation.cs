namespace Dal;
using DalApi;
using DO;



internal class DeliveryImplementation : IDelivery
{
    private List<Delivery> LoadList() =>
        XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);

    private void SaveList(List<Delivery> list) =>
        XMLTools.SaveListToXMLSerializer(list, Config.s_deliveries_xml);


    public void Create(Delivery item)
    {
        var list = LoadList();

        if (list.Any(d => d.Id == item.Id))
            throw new DalAlreadyExistsException($"Delivery ID {item.Id} exists");

        list.Add(item);
        SaveList(list);
    }

    public Delivery? Read(int id)
    {
        var list = LoadList();
        return list.FirstOrDefault(d => d.Id == id);
    }

    public Delivery? Read(Func<Delivery, bool> filter)
    {
        var list = LoadList();
        return list.FirstOrDefault(filter);
    }

    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        var list = LoadList();
        return filter is null ? list : list.Where(filter);
    }

    public void Update(Delivery item)
    {
        var list = LoadList();

        int index = list.FindIndex(d => d.Id == item.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Delivery ID {item.Id} not found");

        list[index] = item;
        SaveList(list);
    }

    public void Delete(int id)
    {
        var list = LoadList();

        int removed = list.RemoveAll(d => d.Id == id);
        if (removed == 0)
            throw new DalDoesNotExistException($"Delivery ID {id} not found");

        SaveList(list);
    }

    public void DeleteAll()
    {
        SaveList(new List<Delivery>());
    }
}
