
namespace BO
{
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
}