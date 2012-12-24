using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    [Serializable]
    public class ScriptException : NuGetUpdateException
    {
        public ScriptException()
        {
        }

        public ScriptException(string message)
            : base(message)
        {
        }

        public ScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
