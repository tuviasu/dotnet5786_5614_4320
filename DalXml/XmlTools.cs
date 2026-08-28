namespace Dal;

using DO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{
    const string s_xmlDir = @"..\xml\";
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }

    #region SaveLoadWithXMLSerializer
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlSerializer(typeof(List<T>)).Serialize(file, list);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException("Error creating XML file: " + xmlFilePath, ex);
        }
    }
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException("Error loading XML file: " + xmlFilePath, ex);
        }
    }
    #endregion

    #region SaveLoadWithXElement
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException("Error saving XML file: " + xmlFilePath, ex);
        }
    }
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException("Error loading XML file: " + xmlFilePath, ex);
        }
    }
    #endregion

    #region XmlConfig
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
        return nextId;
    }
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return num;
    }
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
        return dt;
    }
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static double? GetConfigDoubleVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        return root.ToDoubleNullable(elemName);
    }
    public static void SetConfigDoubleVal(string xmlFileName, string elemName, double? elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        if (elemVal.HasValue)
            root.Element(elemName)?.SetValue(elemVal.Value.ToString());
        else
            root.Element(elemName)?.SetValue("");
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }
    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        string val = (string?)root.Element(elemName) ?? "";
        return TimeSpan.TryParse(val, out var result) ? result : TimeSpan.Zero;
    }
    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal.ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    public static string GetConfigStringVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        return (string?)root.Element(elemName) ?? string.Empty;
    }

    public static void SetConfigStringVal(string xmlFileName, string elemName, string elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal ?? string.Empty);
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    public static Dictionary<int, string> GetManagers(string xmlFileName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        // Backward compatibility: old files might have ManagerID/ManagerPassword
        var managersElem = root.Element("Managers");
        if (managersElem is null)
        {
            int? legacyId = root.ToIntNullable("ManagerID");
            string legacyPwd = (string?)root.Element("ManagerPassword") ?? string.Empty;

            var fallback = new Dictionary<int, string>();
            if (legacyId.HasValue)
                fallback[legacyId.Value] = legacyPwd;

            return fallback;
        }

        return managersElem.Elements("Manager")
            .Select(m => new
            {
                Id = (int?)m.Element("Id"),
                Password = (string?)m.Element("Password") ?? string.Empty
            })
            .Where(x => x.Id.HasValue)
            .ToDictionary(x => x.Id!.Value, x => x.Password);
    }

    public static void SetManagers(string xmlFileName, Dictionary<int, string> managers)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        root.Element("Managers")?.Remove();

        var managersElem = new XElement("Managers",
            managers.Select(kvp =>
                new XElement("Manager",
                    new XElement("Id", kvp.Key),
                    new XElement("Password", kvp.Value ?? string.Empty)
                )));

        root.Add(managersElem);

        // Remove legacy nodes if present
        root.Element("ManagerID")?.Remove();
        root.Element("ManagerPassword")?.Remove();

        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    public static Dictionary<int, string> GetCouriers(string xmlFileName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        var couriersElem = root.Element("Couriers");
        if (couriersElem is null)
            return new Dictionary<int, string>();

        return couriersElem.Elements("Courier")
            .Select(c => new
            {
                Id = (int?)c.Element("Id"),
                Password = (string?)c.Element("Password") ?? string.Empty
            })
            .Where(x => x.Id.HasValue)
            .ToDictionary(x => x.Id!.Value, x => x.Password);
    }

    public static void SetCouriers(string xmlFileName, Dictionary<int, string> couriers)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);

        root.Element("Couriers")?.Remove();

        var couriersElem = new XElement("Couriers",
            couriers.Select(kvp =>
                new XElement("Courier",
                    new XElement("Id", kvp.Key),
                    new XElement("Password", kvp.Value ?? string.Empty)
                )));

        root.Add(couriersElem);
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    #endregion

    #region ExtensionFuctions
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;

    public static XElement? getDelivery<T>(this XElement? element)
    {
        return element?.Element("Delivery") as XElement;
    }
    #endregion

}