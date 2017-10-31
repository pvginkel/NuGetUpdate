@echo off

pushd "%~dp0"

set NUGET="%~dp0\Support\NuGet.exe"

pushd "%~dp0\Distrib\Packages"

for %%f in (*.nupkg) do %NUGET% push -source https://www.nuget.org %%f

pause

popd
popd
