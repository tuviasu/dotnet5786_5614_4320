namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception? innerException) : base(message, innerException) { }
}


[Serializable]
public class BlItemAlreadyExistsException : Exception
{
    public BlItemAlreadyExistsException(string? message) : base(message) { }
    public BlItemAlreadyExistsException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlItemNotFoundException : Exception
{
    public BlItemNotFoundException(string? message) : base(message) { }
    public BlItemNotFoundException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlEmptyCollectionException : Exception
{
    public BlEmptyCollectionException(string? message) : base(message) { }
    public BlEmptyCollectionException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlInvalidIdException : Exception
{
    public BlInvalidIdException(string? message) : base(message) { }
    public BlInvalidIdException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlTemporaryNotAvailableException : Exception
{
    public BlTemporaryNotAvailableException(string? message) : base(message) { }
    public BlTemporaryNotAvailableException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
    public BlNullPropertyException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
    public BlInvalidInputException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlCannotDeleteException : Exception
{
    public BlCannotDeleteException(string? message) : base(message) { }
    public BlCannotDeleteException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlOperationNotAllowedException : Exception
{
    public BlOperationNotAllowedException(string? message) : base(message) { }
    public BlOperationNotAllowedException(string message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BlAuthenticationFailedException : Exception
{
    public BlAuthenticationFailedException(string? message) : base(message) { }
    public BlAuthenticationFailedException(string message, Exception? innerException) : base(message, innerException) { }
}
