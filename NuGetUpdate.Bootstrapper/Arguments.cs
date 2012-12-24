using System;
using System.Collections.Generic;
using System.Text;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Bootstrapper
{
    internal class Arguments : Shared.Arguments
    {
        private readonly ArgumentOption<string> _package = new ArgumentOption<string>("Package", true, "-p");
        private readonly ArgumentOption<string> _site = new ArgumentOption<string>("Site", true, "-s");
        private readonly ArgumentOption<string> _title = new ArgumentOption<string>("Setup title", true, "-t");

        public string Package
        {
            get { return _package.Argument; }
        }

        public string Site
        {
            get { return _site.Argument; }
        }

        public string Title
        {
            get { return _title.Argument; }
        }

        public Arguments()
        {
            Items.Add(_package);
            Items.Add(_site);
            Items.Add(_title);
        }
    }
}
