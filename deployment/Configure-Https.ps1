param(
    [string]$SiteName = "PrimeHPM",
    [Parameter(Mandatory = $true)]
    [string]$CertificateThumbprint,
    [string]$HostName = "",
    [int]$HttpsPort = 443,
    [string[]]$FirewallRemoteAddress = @("LocalSubnet")
)

$ErrorActionPreference = 'Stop'

if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Run this script as Administrator.'
}

Import-Module WebAdministration

if (-not (Test-Path "IIS:\Sites\$SiteName")) {
    throw "IIS site '$SiteName' does not exist. Run Configure-IIS.ps1 first."
}

$normalizedThumbprint = ($CertificateThumbprint -replace '[^0-9A-Fa-f]', '').ToUpperInvariant()
if ($normalizedThumbprint.Length -ne 40) {
    throw 'CertificateThumbprint must be a 40-character SHA-1 thumbprint.'
}

$certificatePath = "Cert:\LocalMachine\My\$normalizedThumbprint"
$certificate = Get-Item -LiteralPath $certificatePath -ErrorAction SilentlyContinue
if ($null -eq $certificate) {
    throw "Certificate '$normalizedThumbprint' was not found in LocalMachine\My."
}
if (-not $certificate.HasPrivateKey) {
    throw 'The selected certificate does not have a private key.'
}
if ($certificate.NotAfter -le (Get-Date)) {
    throw "The selected certificate expired on $($certificate.NotAfter.ToString('u'))."
}
if ($certificate.NotBefore -gt (Get-Date)) {
    throw "The selected certificate is not valid until $($certificate.NotBefore.ToString('u'))."
}

$binding = Get-WebBinding -Name $SiteName -Protocol https -ErrorAction SilentlyContinue |
    Where-Object {
        $parts = $_.bindingInformation.Split(':', 3)
        [int]$parts[1] -eq $HttpsPort -and $parts[2] -eq $HostName
    } |
    Select-Object -First 1

if ($null -eq $binding) {
    $sslFlags = if ([string]::IsNullOrWhiteSpace($HostName)) { 0 } else { 1 }
    New-WebBinding -Name $SiteName -Protocol https -Port $HttpsPort -HostHeader $HostName -SslFlags $sslFlags | Out-Null
    $binding = Get-WebBinding -Name $SiteName -Protocol https |
        Where-Object {
            $parts = $_.bindingInformation.Split(':', 3)
            [int]$parts[1] -eq $HttpsPort -and $parts[2] -eq $HostName
        } |
        Select-Object -First 1
}

$binding.AddSslCertificate($normalizedThumbprint, 'My')

$firewallRuleName = "PrimeHPM HTTPS $HttpsPort"
$existingRule = Get-NetFirewallRule -DisplayName $firewallRuleName -ErrorAction SilentlyContinue
if ($null -ne $existingRule) {
    Set-NetFirewallRule -DisplayName $firewallRuleName -Enabled True -Direction Inbound -Action Allow | Out-Null
    Set-NetFirewallAddressFilter -AssociatedNetFirewallRule $existingRule -RemoteAddress $FirewallRemoteAddress | Out-Null
    Set-NetFirewallPortFilter -AssociatedNetFirewallRule $existingRule -Protocol TCP -LocalPort $HttpsPort | Out-Null
}
else {
    New-NetFirewallRule `
        -DisplayName $firewallRuleName `
        -Direction Inbound `
        -Protocol TCP `
        -LocalPort $HttpsPort `
        -RemoteAddress $FirewallRemoteAddress `
        -Action Allow | Out-Null
}

Write-Host "HTTPS configured for IIS site '$SiteName'."
Write-Host "Binding: https://$($HostName):$HttpsPort (blank host means all host names)"
Write-Host "Certificate: $($certificate.Subject); expires $($certificate.NotAfter.ToString('u'))"
Write-Host "Firewall scope: $($FirewallRemoteAddress -join ', ')"

