@echo off

pushd "%~dp0"
powershell -file .\MakeSetup.ps1 "Setup.exe" "C:\Projects\NuGetUpdate\Build\Distrib\Packages" "NuGetUpdate.Demo" "NuGet Update Demo"
popd

pause
