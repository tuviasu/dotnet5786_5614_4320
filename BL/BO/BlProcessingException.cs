
namespace BO
{
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
}