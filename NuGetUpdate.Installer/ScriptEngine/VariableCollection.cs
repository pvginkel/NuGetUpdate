using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Expressions;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class VariableCollection : KeyedCollection<string, Variable>
    {
        public VariableCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        protected override string GetKeyForItem(Variable item)
        {
            return item.Name;
        }

        public void AddOrSet(string name, object value)
        {
            Variable variable;

            if (Contains(name))
            {
                variable = this[name];
            }
            else
            {
                variable = new Variable(name);
                Add(variable);
            }

            variable.Value = value;
        }

        public T GetRequired<T>(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (!Contains(name))
                throw new ScriptException(String.Format(UILabels.ScriptVariableNotFound, name));

            var variable = this[name];

            if (variable.Value == null)
                throw new ScriptException(String.Format(UILabels.ScriptVariableNotFound, name));

            if (!(variable.Value is T))
                return (T)Convert.ChangeType(variable.Value, typeof(T));

            return (T)variable.Value;
        }

        public T GetOptional<T>(string name)
        {
            return GetOptional(name, default(T));
        }

        public T GetOptional<T>(string name, T defaultValue)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (!Contains(name))
                return defaultValue;

            var variable = this[name];

            if (variable.Value == null)
                return defaultValue;

            if (!(variable.Value is T))
                return (T)Convert.ChangeType(variable.Value, typeof(T));

            return (T)variable.Value;
        }
    }
}
