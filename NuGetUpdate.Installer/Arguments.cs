using System;
using System.Collections.Generic;
using System.Text;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer
{
    internal class Arguments : Shared.Arguments
    {
        private readonly ArgumentFlag _install = new ArgumentFlag("Install", "-i");
        private readonly ArgumentFlag _uninstall = new ArgumentFlag("Uninstall", "-r");
        private readonly ArgumentFlag _update = new ArgumentFlag("Update", "-u");
        private readonly ArgumentFlag _downloadUpdate = new ArgumentFlag("Download update", "-du");
        private readonly ArgumentFlag _redirected = new ArgumentFlag("Redirected", "-x");
        private readonly ArgumentOption<string> _package = new ArgumentOption<string>("Package code", true, "-p");
        private readonly ArgumentOption<string> _title = new ArgumentOption<string>("Setup title", false, "-t");
        private readonly ArgumentOption<string> _site = new ArgumentOption<string>("Site", false, "-s");
        private readonly ArgumentOption<string> _siteUserName = new ArgumentOption<string>("Site user name", false, "-su");
        private readonly ArgumentOption<string> _sitePassword = new ArgumentOption<string>("Site password", false, "-sp");

        public bool Install
        {
            get { return _install.IsProvided; }
        }

        public bool Uninstall
        {
            get { return _uninstall.IsProvided; }
        }

        public bool Update
        {
            get { return _update.IsProvided; }
        }

        public bool DownloadUpdate
        {
            get { return _downloadUpdate.IsProvided; }
        }

        public string Package
        {
            get { return _package.Argument; }
        }

        public string Title
        {
            get { return _title.Argument; }
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

        public bool Redirected
        {
            get { return _redirected.IsProvided; }
        }

        public Arguments()
        {
            Items.Add(_install);
            Items.Add(_uninstall);
            Items.Add(_update);
            Items.Add(_downloadUpdate);
            Items.Add(_package);
            Items.Add(_title);
            Items.Add(_site);
            Items.Add(_siteUserName);
            Items.Add(_sitePassword);
            Items.Add(_redirected);
        }

        public override void Parse(string[] args)
        {
            base.Parse(args);

            if ((Install ? 1 : 0) + (Uninstall ? 1 : 0) + (Update ? 1 : 0) + (DownloadUpdate ? 1 : 0) != 1)
                throw new NuGetUpdateException(UILabels.SpecifyModeFlag);
            if (Install && !_title.IsProvided)
                throw new NuGetUpdateException(UILabels.TitleRequiredForInstall);
            if (Install && !_site.IsProvided)
                throw new NuGetUpdateException(UILabels.SiteRequiredForInstall);
        }
    }
}
