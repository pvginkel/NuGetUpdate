@echo off

set XSD="C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe"

if not exist %XSD% set XSD="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe"

%XSD% Script-v1.xsd /classes /namespace:NuGetUpdate.Installer.ScriptEngine

pause
