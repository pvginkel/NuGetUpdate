using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class TemplateExpression
    {
        public string Expression { get; private set; }

        public string Replacement { get; set; }

        public TemplateExpression(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression = expression;
        }
    }
}
