namespace Dal;
using DalApi;
using DO;
using System.Xml.Linq;
using System.Globalization;

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
                       .FirstOrDefault(e => int.TryParse(e.Element("Id")?.Value, out var iid) && iid == id);

        if (elem == null) return null;

        // Parse Id and required strings
        int idVal = int.Parse(elem.Element("Id")!.Value);
        string name = elem.Element("Name")?.Value ?? string.Empty;
        string phone = elem.Element("Phone")?.Value ?? string.Empty;

        // Optional numeric
        double? maxDistance = null;
        var mdStr = elem.Element("MaxDistance")?.Value;
        if (!string.IsNullOrWhiteSpace(mdStr)
            && double.TryParse(mdStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var md))
            maxDistance = md;

        // Optional DateTime
        DateTime startWorkingDate = default;
        if (!string.IsNullOrWhiteSpace(elem.Element("StartWorkingDate")?.Value))
            DateTime.TryParse(elem.Element("StartWorkingDate")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out startWorkingDate);

        // Enum safe parse
        Enum.TryParse<DeliveryTransport>(elem.Element("Transport")?.Value ?? "", true, out var transport);

        bool isActive = bool.TryParse(elem.Element("IsActive")?.Value, out var ia) && ia;

        return new Courier(
            Id: idVal,
            Name: name,
            Phone: phone,
            Email: elem.Element("Email")?.Value ?? string.Empty,
            Password: elem.Element("Password")?.Value ?? string.Empty,
            IsActive: isActive,
            Transport: transport,
            StartWorkingDate: startWorkingDate,
            MaxDistance: maxDistance
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
                           StartWorkingDate: e.Element("StartWorkingDate") != null
                               ? DateTime.Parse((string)e.Element("StartWorkingDate")!)
                               : default,
                           // reading
                           MaxDistance: double.TryParse(e.Element("MaxDistance")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var md) ? md : (double?)null
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
