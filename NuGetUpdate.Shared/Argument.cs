using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NuGetUpdate.Shared
{
    public class Argument
    {
        public Argument(string title, bool isMandatory, string prefix, bool expectArgument, bool allowMultiple)
        {
            Title = title;
            IsMandatory = isMandatory;
            Prefix = prefix;
            ExpectArgument = expectArgument;
            AllowMultiple = allowMultiple;
        }

        public string Title { get; private set; }

        public bool IsMandatory { get; private set; }

        public string Prefix { get; private set; }

        public bool ExpectArgument { get; private set; }

        public bool IsProvided { get; internal set; }

        public bool AllowMultiple { get; private set; }

        public virtual void ParseArgument(string argument)
        {
        }
    }

    public class ArgumentFlag : Argument
    {
        public ArgumentFlag(string title, string prefix)
            : base(title, false, prefix, false, false)
        {
        }
    }

    public class ArgumentOption<T> : Argument
    {
        public ArgumentOption(string title, bool isMandatory, string prefix)
            : base(title, isMandatory, prefix, true, false)
        {
        }

        public T Argument { get; private set; }

        public override void ParseArgument(string argument)
        {
            try
            {
                Argument = (T)Convert.ChangeType(argument, typeof(T));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Illegal value for argument {0}", Prefix), ex);
            }
        }
    }

    public class ArgumentMultipleOption<T> : Argument
    {
        private readonly List<T> _arguments;

        public ArgumentMultipleOption(string title, bool isMandatory, string prefix)
            : base(title, isMandatory, prefix, true, true)
        {
            _arguments = new List<T>();

            Arguments = new ReadOnlyCollection<T>(_arguments);
        }

        public ICollection<T> Arguments { get; private set; }

        public override void ParseArgument(string argument)
        {
            try
            {
                _arguments.Add((T)Convert.ChangeType(argument, typeof(T)));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Illegal value for argument {0}", Prefix), ex);
            }
        }
    }
}
