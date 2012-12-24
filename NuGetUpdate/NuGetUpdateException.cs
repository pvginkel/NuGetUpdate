using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NuGetUpdate
{
    [Serializable]
    public class NuGetUpdateException : Exception
    {
        public NuGetUpdateException()
        {
        }

        public NuGetUpdateException(string message)
            : base(message)
        {
        }

        public NuGetUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NuGetUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
