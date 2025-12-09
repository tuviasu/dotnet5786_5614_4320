namespace BO;

#region Exceptions for DAL errors wrapped by BL

/// <summary>
/// Thrown when an entity does not exist in DAL.
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message)
        : base(message) { }

    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Thrown when trying to create an entity that already exists.
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message)
        : base(message) { }

    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Thrown when deletion is not allowed or impossible.
/// </summary>
[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message)
        : base(message) { }

    public BlDeletionImpossibleException(string message, Exception innerException)
        : base(message, innerException) { }
}

#endregion

#region Exceptions for internal BL logic errors

/// <summary>
/// Thrown when a property with null value is used in BL.
/// </summary>
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message)
        : base(message) { }
}

/// <summary>
/// Thrown when a property contains an invalid value.
/// </summary>
[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message)
        : base(message) { }
}

/// <summary>
/// Thrown when dates provided to BL are inconsistent or invalid.
/// </summary>
[Serializable]
public class BlInvalidDateException : Exception
{
    public BlInvalidDateException(string? message)
        : base(message) { }
}

/// <summary>
/// Thrown when BL encounters unexpected internal problem.
/// </summary>
[Serializable]
public class BlGeneralException : Exception
{
    public BlGeneralException(string? message)
        : base(message) { }

    public BlGeneralException(string message, Exception innerException)
        : base(message, innerException) { }
}
[Serializable]
internal class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException()
    {
    }

    public BlInvalidAddressException(string? message) : base(message)
    {
    }

    public BlInvalidAddressException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
internal class BlInvalidInputException : Exception
{
    public BlInvalidInputException()
    {
    }

    public BlInvalidInputException(string? message) : base(message)
    {
    }

    public BlInvalidInputException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
[Serializable]
internal class BlProcessingException : Exception
{
    public BlProcessingException()
    {
    }

    public BlProcessingException(string? message) : base(message)
    {
    }

    public BlProcessingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
[Serializable]
internal class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException()
    {
    }

    public BLTemporaryNotAvailableException(string? message) : base(message)
    {
    }

    public BLTemporaryNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
[Serializable]
internal class BLIllegalActionException : Exception
{
    public BLIllegalActionException() { }

    public BLIllegalActionException(string? message) : base(message) { }

    public BLIllegalActionException(string? message, Exception? innerException) : base(message, innerException) { }
}

#endregion