using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NuGetUpdate.Installer
{
    public static class TextUtil
    {
        private static readonly Regex NewlineRe = new Regex("\r?\n", RegexOptions.Compiled);

        public static string FixNewlines(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            return NewlineRe.Replace(text, Environment.NewLine);
        }
    }
}
