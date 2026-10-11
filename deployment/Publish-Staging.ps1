param([string]$OutputPath = "$(Join-Path $PSScriptRoot '..\publish\staging')")
$ErrorActionPreference = 'Stop'
$resolvedOutput = [System.IO.Path]::GetFullPath($OutputPath)
dotnet publish (Join-Path $PSScriptRoot '..\HPM.Web\HPM.Web.csproj') -c Release -o $resolvedOutput
Write-Host "Published PrimeHPM to $resolvedOutput"
Write-Host "Set ConnectionStrings__HpmDatabase and ASPNETCORE_ENVIRONMENT=Production in IIS before starting."
