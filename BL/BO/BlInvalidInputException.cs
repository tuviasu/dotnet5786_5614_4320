
namespace BO
{
    [Serializable]
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
}