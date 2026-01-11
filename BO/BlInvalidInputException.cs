using System;
using System.Runtime.Serialization;

namespace BO
{
    // Change the access modifier from 'internal' to 'public'
    public class BlInvalidInputException : Exception, ISerializable
    {
        public BlInvalidInputException() { }
        public BlInvalidInputException(string message) : base(message) { }
        public BlInvalidInputException(string message, Exception inner) : base(message, inner) { }
        protected BlInvalidInputException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}