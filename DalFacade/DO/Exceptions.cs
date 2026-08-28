namespace DO;
[Serializable]
public class DalIdAlreadyExist(string message) : Exception(message) { };


[Serializable]
public class DalIdNotExist(string message) : Exception(message) { };

[Serializable]
public class DalItemNotExist(string message) : Exception(message) { };

[Serializable]
public class DalInvalidId(string message) : Exception(message) { };

[Serializable]
public class DalEmptyCollection(string message) : Exception(message) { };

[Serializable]
public class DalXMLFileLoadCreateException(string message, Exception innerException) : Exception(message, innerException) { };
