namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;

internal class CourierImplementation : ICourier
{
    private string Path => Config.s_couriers_xml;

    private XElement LoadRoot()
    {
        // Loads XML → XElement
        return XMLTools.LoadListFromXMLElement(Path);
    }

    private void SaveRoot(XElement root)
    {
        XMLTools.SaveListToXMLElement(root, Path);
    }


    // ─────────────── CREATE ───────────────
    public void Create(Courier item)
    {
        XElement root = LoadRoot();

        // Check duplicate
        if (root.Elements("Courier").Any(e => (int)e.Element("Id")! == item.Id))
            throw new DalAlreadyExistsException($"Courier ID {item.Id} exists");

        XElement newCourier = new XElement("Courier",
            new XElement("Id", item.Id),
            new XElement("Name", item.Name),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("Password", item.Password),
            new XElement("IsActive", item.IsActive),
            new XElement("Transport", item.Transport),
            new XElement("MaxDistance", item.MaxDistance)
        );

        root.Add(newCourier);
        SaveRoot(root);
    }


    // ─────────────── READ BY ID ───────────────
    public Courier? Read(int id)
    {
        XElement root = LoadRoot();

        var elem = root.Elements("Courier")
                       .FirstOrDefault(e => (int)e.Element("Id")! == id);

        if (elem == null) return null;

        return new Courier(
            Id: (int)elem.Element("Id")!,
            Name: (string)elem.Element("Name")!,
            Phone: (string)elem.Element("Phone")!,
            Email: (string)elem.Element("Email")!,
            Password: (string)elem.Element("Password")!,
            IsActive: (bool)elem.Element("IsActive")!,
            Transport: (DeliveryTransport)Enum.Parse(typeof(DeliveryTransport), (string)elem.Element("Transport")!),
            MaxDistance: (double?)elem.Element("MaxDistance")
        );
    }

    // Add implementation for ICrud<Courier>.Read(Func<Courier, bool> filter)
    public Courier? Read(Func<Courier, bool> filter)
    {
        return ReadAll(filter).FirstOrDefault();
    }


    // ─────────────── READ ALL ───────────────
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        XElement root = LoadRoot();

        var list = root.Elements("Courier")
                       .Select(e => new Courier(
                           Id: (int)e.Element("Id")!,
                           Name: (string)e.Element("Name")!,
                           Phone: (string)e.Element("Phone")!,
                           Email: (string)e.Element("Email")!,
                           Password: (string)e.Element("Password")!,
                           IsActive: (bool)e.Element("IsActive")!,
                           Transport: (DeliveryTransport)Enum.Parse(typeof(DeliveryTransport), (string)e.Element("Transport")!),
                           MaxDistance: (double?)e.Element("MaxDistance")
                       ));

        return filter is null ? list : list.Where(filter);
    }


    // ─────────────── UPDATE ───────────────
    public void Update(Courier item)
    {
        XElement root = LoadRoot();

        var elem = root.Elements("Courier")
                       .FirstOrDefault(e => (int)e.Element("Id")! == item.Id);

        if (elem == null)
            throw new DalDoesNotExistException($"Courier ID {item.Id} not found");

        elem.Element("Name")!.SetValue(item.Name);
        elem.Element("Phone")!.SetValue(item.Phone);
        elem.Element("Email")!.SetValue(item.Email);
        elem.Element("Password")!.SetValue(item.Password);
        elem.Element("IsActive")!.SetValue(item.IsActive);
        elem.Element("Transport")!.SetValue(item.Transport);
        elem.Element("MaxDistance")!.SetValue(item.MaxDistance ?? 0.0);

        SaveRoot(root);
    }


    // ─────────────── DELETE ───────────────
    public void Delete(int id)
    {
        XElement root = LoadRoot();

        var elem = root.Elements("Courier")
                       .FirstOrDefault(e => (int)e.Element("Id")! == id);

        if (elem == null)
            throw new DalDoesNotExistException($"Courier ID {id} not found");

        elem.Remove();
        SaveRoot(root);
    }


    // ─────────────── DELETE ALL ───────────────
    public void DeleteAll()
    {
        SaveRoot(new XElement("ArrayOfCourier"));
    }
}
