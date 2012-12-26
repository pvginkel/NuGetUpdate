@echo off

pushd "%~dp0"
powershell -file .\MakeSetup.ps1 "Setup.exe" "http://nuget.org/api/v2" "NuGetUpdate.Demo" "NuGet Update Demo"
popd

pause
