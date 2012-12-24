using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class TemplateParser
    {
        private static readonly Regex ExpressionRe = new Regex("\\{\\{(.*?)\\}\\}", RegexOptions.Compiled | RegexOptions.Singleline);

        private readonly List<string> _fragments = new List<string>();
        private readonly List<TemplateExpression> _expressions = new List<TemplateExpression>();

        public IList<TemplateExpression> Expressions { get; private set; }

        public TemplateParser(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Expressions = new ReadOnlyCollection<TemplateExpression>(_expressions);

            int offset = 0;

            foreach (Match match in ExpressionRe.Matches(text))
            {
                _fragments.Add(text.Substring(offset, match.Index - offset));

                offset = match.Index + match.Length;

                _expressions.Add(new TemplateExpression(match.Groups[1].Value));
            }

            _fragments.Add(text.Substring(offset));
        }

        public string CreateTarget()
        {
            var sb = new StringBuilder();

            sb.Append(_fragments[0]);

            for (int i = 0; i < Expressions.Count; i++)
            {
                if (Expressions[i].Replacement != null)
                    sb.Append(Expressions[i].Replacement);

                sb.Append(_fragments[i + 1]);
            }

            return sb.ToString();
        }
    }
}
