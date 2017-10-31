# NuGetUpdate

LGPL 3.0.

[Download from NuGet](http://nuget.org/packages/NuGetUpdate).

## Introduction

NuGetUpdate is a system to create setups using NuGet as a distribution platform.

NuGetUpdate works as follows:

* You package your application as a NuGet package;
* Using the tools provided in NuGetUpdate, you create a bootstrapping setup. This setup will download the latest version of the NuGet package from https://nuget.org (or any other NuGet compatible website) and installs that on the client PC;
* By adding a dependency on NuGetUpdate in your application, you can implement automatic updates with a few lines of code.

## Creating a deployment package

NuGetUpdate uses NuGet to distribute your application. To prepare your application for distribution, you need to package your application as a NuGet package.

The NuGetUpdate installer basically downloads your NuGet package and extracts the contents of the `Tools` directory into the `Bin` folder of the installation path. To prepare a package to use the `Tools` directory to store files in, you need to create a `.nuspec` file with roughly the following declaration:

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>ConvertAllToMp3</id>
    <version>$version$</version>
    <authors>Pieter van Ginkel</authors>
    <description>Convert all WAV files in a directory to MP3 using the LAME MP3 encoder.</description>
    <projectUrl>http://github.com/pvginkel/ConvertAllToMp3</projectUrl>
  </metadata>
  <files>
    <file src="..\ConvertAllToMp3\bin\Release\**" target="Tools" exclude="..\ConvertAllToMp3\bin\Release\**.pdb;..\ConvertAllToMp3\bin\Release\**.xml" />
    <file src="..\ConvertAllToMp3\NguScript.xml" target="Tools" />
    <file src="..\packages\NuGetUpdate.*\Tools\Installer\ngu.exe" target="Tools" />
  </files>
</package>
```
The above example is taken from the [ConvertAllToMp3]([https://github.com/pvginkel/ConvertAllToMp3) application which uses NuGetUpdate. In this example, we add the following items, all with the `Tools` directory set as the `target`:

* The compilation output excluding `.pdb` and `.xml` files;
* The installation script named `NguScript.xml`;
* The `ngu.exe` file part of the `NuGetUpdate` package. This application is the installer. The self extracting setup is just a bootstrap application to download the NuGet package. Actual installation of the application is handled by the `ngu.exe` application part of the NuGet package.

NuGetUpdate uses a scripting language to script the setup. See the [NuGetUpdate script language](https://github.com/pvginkel/NuGetUpdate/wiki/NuGetUpdate-script-language) Wiki page for more information on this.

## Automatic updates

Automatic updates in .NET applications can be implemented by adding a reference to the `NuGetUpdate` NuGet package. In your application, you can implement automatic updates as by adding a few lines of code to your application.

The following example shows how your entry point into your application could be implemented to support automatic updates:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertAllToMp3
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (PerformUpdate(args))
                return;

            Application.Run(new MainForm());
        }

        private static bool PerformUpdate(string[] args)
        {
#if !DEBUG
            try
            {
                const string packageCode = "ConvertAllToMP3";

                bool updateAvailable = NuGetUpdate.Update.IsUpdateAvailable(packageCode);
                if (updateAvailable)
                {
                    NuGetUpdate.Update.StartUpdate(packageCode, args);
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions.
            }
#endif

            return false;
        }
    }
}
```

There are two method calls important to implement automatic updates:

* `NuGetUpdate.Update.IsUpdateAvailable` calls into NuGet and figures out whether a new version of the application is available. This method takes the package code of your application and returns a `bool` indicating whether updates are available;
* `NuGetUpdate.Update.StartUpdate` initiates the update process. This method takes the package code and the restart arguments. For more information on the restart arguments, see the section on `Config.RestartAguments` in the [NuGetUpdate script language](https://github.com/pvginkel/NuGetUpdate/wiki/NuGetUpdate-script-language) Wiki page.

Make sure that once you initiate the automatic update process, you immediately exit the application so it can be updated.

The above implementation contains two extra components:

* The update process itself is only executed in release mode using the `#if !DEBUG` preprocess statement as to not hinder development;
* All exceptions from the update process are suppressed to ensure the application doesn't fail when e.g. https://www.nuget.org/ cannot be reached.

## Creating a setup

The initial setup of NuGetUpdate applications is done using a bootstrap application. The purpose of this application is to download the latest version of the application from the NuGet website and run the installer from that package.

The files required to build such a setup can be found in the [Make SFX archive](https://github.com/pvginkel/NuGetUpdate/tree/master/Support/Make%20SFX%20archive) folder. This folder contains the following files:

* `Make Demo Setup.bat` demonstrates how a setup can be created for the demo NuGetUpdate application part of the project;
* `MakeSetup.ps1` is a PowerShell script that automates the creation of a setup;
* `header.sfx` is a customized 7-Zip self extractor. This is the silent version of the self executing archive application and includes the NuGetUpdate icon and a manifest to ensure the setup doesn't require administrative privileges to run;
* `ngubs.7z` is a 7-Zip compressed version of the `ngubs.exe` bootstrap application.

The `ngubs.exe` application takes a few arguments needed to start the setup. These arguments are embedded into the self extracting archive in the form of a 7-Zip configuration section. To have this configuration section embedded into the self extracting archive, the `MakeSetup.ps1` script expects these arguments on the command line.

The `Make Demo Setup.bat` demonstrates how this is done:

```bash
@echo off

pushd "%~dp0"
powershell -file .\MakeSetup.ps1 "Setup.exe" "https://nuget.org/api/v2" "NuGetUpdate.Demo" "NuGet Update Demo"
popd

pause
```

The PowerShell script takes the following positional arguments:

* The name of the setup executable to create;
* The URL to the NuGet website to use. The example above points to the public NuGet website, but a corporate NuGet installation could be used instead;
* The package code of the NuGet package to install;
* The title of the setup.

Note that it is not necessary to recreate the setup executable every time the application is redeployed or even when a new version of NuGetUpdate is rolled out. The code contained in the NuGetUpdate bootstrap application is very minimal and all actual work is delegated to the `ngu.exe` application packages with your NuGet package.

## Bugs

Bugs should be reported through GitHub at
[http://github.com/pvginkel/NuGetUpdate/issues](http://github.com/pvginkel/NuGetUpdate/issues).

## License

NuGetUpdate is licensed under the LGPL 3.0.
