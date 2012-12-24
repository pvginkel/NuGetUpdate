@echo off

set XSD="C:\Program Files\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\xsd.exe"

if not exist %XSD% set XSD="C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\xsd.exe"

%XSD% InstallLog-v1.xsd /classes /namespace:NuGetUpdate.Installer.InstallLogging

pause
