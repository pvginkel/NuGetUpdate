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
        private readonly ArgumentOption<string> _siteUserName = new ArgumentOption<string>("Site user name", false, "-su");
        private readonly ArgumentOption<string> _sitePassword = new ArgumentOption<string>("Site password", false, "-sp");
        private readonly ArgumentOption<string> _title = new ArgumentOption<string>("Setup title", true, "-t");

        public string Package
        {
            get { return _package.Argument; }
        }

        public string Site
        {
            get { return _site.Argument; }
        }

        public string SiteUserName
        {
            get { return _siteUserName.Argument; }
        }

        public string SitePassword
        {
            get { return _sitePassword.Argument; }
        }

        public string Title
        {
            get { return _title.Argument; }
        }

        public Arguments()
        {
            Items.Add(_package);
            Items.Add(_site);
            Items.Add(_siteUserName);
            Items.Add(_sitePassword);
            Items.Add(_title);
        }
    }
}
