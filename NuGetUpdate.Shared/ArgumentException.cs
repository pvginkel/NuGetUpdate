using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NuGetUpdate.Shared
{
    [Serializable]
    public class ArgumentException : Exception
    {
        public ArgumentException()
        {
        }

        public ArgumentException(string message)
            : base(message)
        {
        }

        public ArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
