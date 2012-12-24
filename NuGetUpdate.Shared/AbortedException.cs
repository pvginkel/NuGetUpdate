using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NuGetUpdate.Shared
{
    [Serializable]
    public class AbortedException : Exception
    {
        public AbortedException()
        {
        }

        public AbortedException(string message)
            : base(message)
        {
        }

        public AbortedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AbortedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
