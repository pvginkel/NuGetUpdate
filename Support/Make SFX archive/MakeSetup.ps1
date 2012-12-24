if ($Args.Length -ne 4)
{
    Write-Host "Expected command line:"
    Write-Host "    powershell.exe -file MakeSetup.ps1 <target> <site> <package code> <setup title>"
    exit
}

$WinRar = "C:\Program Files\WinRar\winrar.exe"

if (-not (Test-Path $WinRar))
{
    $WinRar = "C:\Program Files (x86)\WinRar\winrar.exe"
}

if (-not (Test-Path $WinRar))
{
    Write-Host "Could not find WinRar"
    exit
}

$Source = "ngubs.exe"

if (-not (Test-Path $Source))
{
    Write-Host "Could not find ngubs.exe"
    exit
}

if (-not (Test-Path "mainicon.ico"))
{
    Write-Host "Could not find mainicon.ico"
    exit
}

$Target = $Args[0]
$Site = $Args[1]
$PackageCode = $Args[2]
$SetupTitle = $Args[3]

if ([System.IO.Path]::GetExtension($Target) -ne ".exe")
{
    $Target += ".exe"
}

$Config = "Silent=1`n"
$Config += "TempMode`n"
$Config += "Setup=ngubs.exe -s `"$Site`" -p `"$PackageCode`" -t `"$SetupTitle`""

$ConfigFileName = [System.IO.Path]::GetTempFileName()

[System.IO.File]::WriteAllText($ConfigFileName, $Config, [System.Text.Encoding]::ASCII)

$Target = [System.IO.Path]::GetFullPath($Target)

if (Test-Path $Target)
{
    Remove-Item $Target
}

Start-Process $WinRar "a -sfx `"-z$ConfigFileName`" `"-iiconmainicon.ico`" `"$Target`" `"$Source`"" -Wait

Remove-Item $ConfigFileName
