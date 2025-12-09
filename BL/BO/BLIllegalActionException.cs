
namespace BO
{
    [Serializable]
    internal class BLIllegalActionException : Exception
    {
        public BLIllegalActionException()
        {
        }

        public BLIllegalActionException(string? message) : base(message)
        {
        }

        public BLIllegalActionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}