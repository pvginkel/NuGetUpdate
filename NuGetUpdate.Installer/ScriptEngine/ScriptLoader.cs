using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public static class ScriptLoader
    {
        public static Script Load(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            var script = LoadScript(fileName);

            ValidateScript(script);

            return script;
        }

        private static Script LoadScript(string fileName)
        {
            try
            {
                var settings = new XmlReaderSettings();

                string resourceName = typeof(ScriptLoader).Namespace + ".Script-v1.xsd";

                using (var stream = typeof(ScriptLoader).Assembly.GetManifestResourceStream(resourceName))
                using (var reader = XmlReader.Create(stream))
                {
                    settings.Schemas.Add(Constants.ScriptNs, reader);
                }

                settings.ValidationType = ValidationType.Schema;

                using (var reader = XmlReader.Create(fileName, settings))
                {
                    var serializer = new XmlSerializer(typeof(Script));

                    return (Script)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new ScriptException(UILabels.InvalidScript, ex);
            }
        }

        private static void ValidateScript(Script script)
        {
            script.Setup.Visit(new ScriptValidator(ScriptValidatorMode.Setup));

            ValidateInstallPhase(script.Install, new ScriptValidator(ScriptValidatorMode.Install));
            ValidateInstallPhase(script.Uninstall, new ScriptValidator(ScriptValidatorMode.Uninstall));
            ValidateInstallPhase(script.Update, new ScriptValidator(ScriptValidatorMode.Update));
        }

        private static void ValidateInstallPhase(ContainerType container, ScriptValidator validator)
        {
            container.Visit(validator);

            if (!validator.HadProgressPage)
                throw new ScriptException(UILabels.ProgressPageIsRequired);
        }

        public static ScriptConfig LoadConfigFromNuspec(string downloadFolder, string setupTitle, string installedVersion, string restartArguments)
        {
            if (downloadFolder == null)
                throw new ArgumentNullException("downloadFolder");
            if (setupTitle == null)
                throw new ArgumentNullException("setupTitle");

            var files = Directory.GetFiles(downloadFolder, "*.nuspec");

            if (files.Length != 1)
                throw new ScriptException(UILabels.InvalidPackage);

            var document = new XmlDocument();

            document.Load(files[0]);

            if (Constants.TryGetDetails(document, out string packageCode, out string version))
                throw new ScriptException(UILabels.InvalidPackage);

            return new ScriptConfig(
                downloadFolder,
                packageCode,
                setupTitle,
                version,
                installedVersion,
                restartArguments
            );
        }
    }
}
