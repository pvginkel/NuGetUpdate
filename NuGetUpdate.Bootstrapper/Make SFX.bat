@echo off

set SOURCEDIR=bin\Release
set WINRAR="C:\Program Files\WinRAR\WinRar.exe"

if not exist %WINRAR% set WINRAR="C:\Program Files (x86)\WinRAR\WinRar.exe"

pushd %SOURCEDIR%

%WINRAR% a -r -X*.pdb -X*.xml -X*.manifest -X*.vshost.exe "%~dp0ngubs" *

popd
