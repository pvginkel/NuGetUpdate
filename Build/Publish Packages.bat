@echo off

pushd "%~dp0"

set NUGET="%~dp0\..\.nuget\NuGet.exe"

pushd "%~dp0\Distrib\Packages"

for %%f in (*.nupkg) do %NUGET% push %%f

pause

popd
popd
