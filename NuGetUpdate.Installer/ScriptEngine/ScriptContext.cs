using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Expressions;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptContext : IExpressionContext
    {
        private readonly ScriptEnvironment _environment;

        private static readonly IList<Import> _expressionImports = new[]
        {
            new Import("Math", typeof(Math)),
            new Import("Convert", typeof(Convert)),
            new Import("Path", typeof(Path)),
            new Import("File", typeof(File)),
            new Import("DateTime", typeof(DateTime))
        };

        public VariableCollection Variables { get; private set; }

        public IList<Import> Imports
        {
            get { return _expressionImports; }
        }

        public object Owner
        {
            get { return _environment; }
        }

        public Type OwnerType
        {
            get { return _environment.GetType(); }
        }

        public ScriptContext(ScriptEnvironment environment)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");

            _environment = environment;

            Variables = new VariableCollection();
        }

        public Type GetVariableType(string variable, bool ignoreCase)
        {
            if (Variables.Contains(variable))
            {
                var result = Variables[variable];

                if (result.Value != null)
                    return result.Value.GetType();
                else
                    return typeof(object);
            }

            return null;
        }

        public object GetVariableValue(string variable, bool ignoreCase)
        {
            return Variables.Contains(variable) ? Variables[variable].Value : null;
        }
    }
}
