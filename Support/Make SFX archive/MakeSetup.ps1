Function Escape([string]$Value)
{
    return $Value.Replace('"', '\"')
}

if ($Args.Length -ne 4)
{
    Write-Host "Expected command line:"
    Write-Host "    powershell.exe -file MakeSetup.ps1 <target> <site> <package code> <setup title>"
    exit
}

$Header = "header.sfx"
$Payload = "ngubs.7z"

if (-not (Test-Path $Header))
{
    Write-Host "Could not find" $Header
    exit
}
if (-not (Test-Path $Payload))
{
    Write-Host "Could not find" $Payload
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

$Config = @"
;!@Install@!UTF-8!
Title="$(Escape($SetupTitle))"
RunProgram="ngubs.exe -s \"$(Escape($site))\" -p \"$(Escape($PackageCode))\" -t \"$(Escape($SetupTitle))\""
Progress="no"
;!@InstallEnd@!
"@

$Output = [System.IO.File]::Create($Target)
    
$Input = [System.IO.File]::OpenRead($Header)
$Input.CopyTo($Output)
$Input.Dispose()
    
$ConfigBytes = [System.Text.Encoding]::UTF8.GetBytes($Config)
$Output.Write($ConfigBytes, 0, $ConfigBytes.Length)
    
$Input = [System.IO.File]::OpenRead($Payload)
$Input.CopyTo($Output)
$Input.Dispose()
    
$Output.Dispose()
