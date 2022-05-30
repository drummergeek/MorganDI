using System;

namespace MorganDI
{
    public class InvalidDependencyDefinitionException : ApplicationException
    {
        public InvalidDependencyDefinitionException(string message, Exception innerException) : base(message, innerException) { }
        public InvalidDependencyDefinitionException(string message) : base(message) { }
    }
}
