using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace NuGetUpdate.Shared
{
    public static class Enum<T>
    {
        public static string Format(object value, string format)
        {
            return Enum.Format(typeof(T), value, format);
        }

        public static string GetName(object value)
        {
            return Enum.GetName(typeof(T), value);
        }

        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(T));
        }

        public static Type GetUnderlyingType()
        {
            return Enum.GetUnderlyingType(typeof(T));
        }

        public static ICollection<T> GetValues()
        {
            var result = new List<T>();

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                result.Add((T)value);
            }

            return result;
        }

        public static bool IsDefined(object value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T Parse(string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static bool TryParse(string value, out T enumValue)
        {
            return TryParse(value, out enumValue, false);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static bool TryParse(string value, out T enumValue, bool ignoreCase)
        {
            var comparisonType = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

            foreach (string name in Enum<T>.GetNames())
            {
                if (String.Equals(name, value, comparisonType))
                {
                    enumValue = Enum<T>.Parse(value, ignoreCase);

                    return true;
                }
            }

            enumValue = default(T);

            return false;
        }

        public static T ToObject(byte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToObject(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToObject(long value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToObject(object value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        [CLSCompliant(false)]
        public static T ToObject(sbyte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToObject(short value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        [CLSCompliant(false)]
        public static T ToObject(uint value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        [CLSCompliant(false)]
        public static T ToObject(ulong value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        [CLSCompliant(false)]
        public static T ToObject(ushort value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
