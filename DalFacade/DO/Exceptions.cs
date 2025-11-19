
namespace DO;



/// <summary>
/// Thrown when trying to create an entity that already exists in the data source.
/// Example: adding a courier with an existing ID.
/// </summary>
[Serializable]
public class DalAlreadyExistsException : Exception
{
public DalAlreadyExistsException(string message) : base(message) { }
}

/// <summary>
/// Thrown when trying to read, update, or delete an entity that does not exist.
/// Example: deleting an order that isn't found.
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
public DalDoesNotExistException(string message) : base(message) { }
}

/// <summary>
/// Thrown when the DAL fails to access or read a file (I/O or XML error).
/// Example: problem reading or writing Orders.xml.
/// </summary>
[Serializable]
public class DalAccessException : Exception
{
public DalAccessException(string message) : base(message) { }
}

/// <summary>
/// Thrown when invalid or corrupted data is found in the DAL.
/// Example: negative weight or invalid configuration values.
/// </summary>
[Serializable]
public class DalInvalidDataException : Exception
{
public DalInvalidDataException(string message) : base(message) { }
}
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string message)
        : base(message) { }
}