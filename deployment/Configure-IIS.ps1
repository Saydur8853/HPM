param(
    [string]$SiteName = "PrimeHPM",
    [string]$PhysicalPath = "C:\inetpub\PrimeHPM",
    [int]$HttpPort = 8080
)
$ErrorActionPreference = 'Stop'
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { throw 'Run this script as Administrator.' }
Import-Module WebAdministration
$resolvedPath = [System.IO.Path]::GetFullPath($PhysicalPath)
New-Item -ItemType Directory -Path $resolvedPath -Force | Out-Null
if (-not (Test-Path "IIS:\AppPools\$SiteName")) { New-WebAppPool -Name $SiteName | Out-Null }
Set-ItemProperty "IIS:\AppPools\$SiteName" -Name managedRuntimeVersion -Value ''
if (-not (Test-Path "IIS:\Sites\$SiteName")) { New-Website -Name $SiteName -PhysicalPath $resolvedPath -Port $HttpPort -ApplicationPool $SiteName | Out-Null }
New-NetFirewallRule -DisplayName "PrimeHPM HTTP $HttpPort" -Direction Inbound -Protocol TCP -LocalPort $HttpPort -Action Allow -ErrorAction SilentlyContinue | Out-Null
Write-Host "IIS site $SiteName configured on port $HttpPort. Bind the hospital TLS certificate in IIS before production use."
