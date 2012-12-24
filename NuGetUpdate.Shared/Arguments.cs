using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace NuGetUpdate.Shared
{
    public class Arguments
    {
        public static string[] ExtractExtra(ref string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--")
                {
                    string[] result = new string[args.Length - (i + 1)];

                    for (int j = 0; j < result.Length; j++)
                    {
                        result[j] = args[i + 1 + j];
                    }

                    Array.Resize(ref args, i);

                    return result;
                }
            }

            return new string[0];
        }

        protected Arguments()
        {
            Items = new Collection<Argument>();
        }

        public Collection<Argument> Items { get; private set; }

        public virtual void Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            var items = new Dictionary<string, Argument>();

            foreach (var item in Items)
            {
                items.Add(item.Prefix, item);
            }

            Argument current = null;

            foreach (string arg in args)
            {
                if (current != null)
                {
                    if (!current.AllowMultiple && current.IsProvided)
                        throw new ArgumentException(String.Format("Argument {0} occurs multiple times", current.Prefix));

                    current.IsProvided = true;
                    current.ParseArgument(arg);
                    current = null;
                }
                else
                {
                    Argument item;

                    if (!items.TryGetValue(arg, out item))
                        throw new ArgumentException(String.Format("Cannot process argument {0}", arg));

                    if (item.ExpectArgument)
                    {
                        current = item;
                    }
                    else
                    {
                        item.IsProvided = true;
                    }
                }
            }

            var missingArguments = new List<string>();

            foreach (var item in Items)
            {
                if (item.IsMandatory && !item.IsProvided)
                    missingArguments.Add(item.Prefix);
            }

            if (missingArguments.Count > 0)
                throw new ArgumentException(String.Format("Required missing arguments {0}", String.Join(", ", missingArguments.ToArray())));
        }

        public string GetInstructions()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Unknown or invalid arguments provided.");
            sb.AppendLine();

            int longestPrefix = int.MinValue;
            const string expectArgumentPostfix = "<argument>";

            foreach (var item in Items)
            {
                int length = item.Prefix.Length;

                if (item.ExpectArgument)
                    length += expectArgumentPostfix.Length + 1;

                longestPrefix = Math.Max(longestPrefix, length);
            }

            foreach (var item in Items)
            {
                string prefix = item.Prefix;

                if (item.ExpectArgument)
                    prefix += " " + expectArgumentPostfix;

                sb.AppendLine(String.Format(
                    "  {0,-" + longestPrefix + "} : {1}{2}",
                    prefix,
                    item.Title,
                    item.IsMandatory ? " (required)" : ""
                ));
            }

            return sb.ToString();
        }

        public void WriteInstructions()
        {
            Console.Error.WriteLine();

            var entryAssembly = Assembly.GetEntryAssembly();

            Console.Error.WriteLine(
                "{0} version {1}",
                entryAssembly.GetName().Name,
                entryAssembly.GetName().Version
            );
            Console.Error.WriteLine();
            Console.Error.WriteLine(GetInstructions());
        }
    }
}
