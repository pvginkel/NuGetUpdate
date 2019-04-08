﻿using System;
using System.Collections.Generic;
using System.Text;

#if NGU_LIBRARY
namespace NuGetUpdate
{
    internal static class Escaping
#else
namespace NuGetUpdate.Shared
{
    public static class Escaping
#endif
    {
        public static string UriDecode(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                if (
                    value[i] == '%' &&
                    i < value.Length - 2 &&
                    IsHex(value[i + 1]) &&
                    IsHex(value[i + 2])
                ) {
                    sb.Append(
                        (char)(HexToInt(value[i + 1]) * 16 + HexToInt(value[i + 2]))
                    );

                    i += 2;
                }
                else if (value[i] == '+')
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(value[i]);
                }
            }

            return sb.ToString();
        }

        public static string ShellEncode(string arg)
        {
            if (String.IsNullOrEmpty(arg))
                return "\"\"";

            return "\"" + arg.Replace("\"", "\"\"") + "\"";
        }

        public static bool IsHex(char value)
        {
            switch (value)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;

                default:
                    return false;
            }
        }

        public static int HexToInt(char value)
        {
            switch (value)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return (int)value - (int)'0';

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return ((int)value - (int)'a') + 10;

                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return ((int)value - (int)'A') + 10;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public static string ShellEncode(string[] args)
        {
            var sb = new StringBuilder();

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (sb.Length > 0)
                        sb.Append(' ');

                    sb.Append(ShellEncode(args[i]));
                }
            }

            return sb.ToString();
        }
    }
}
