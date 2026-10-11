# PrimeHPM deployment

1. Install the .NET 8 Hosting Bundle and IIS WebSocket feature on the staging server.
2. Run `Publish-Staging.ps1`, copy the published output to the IIS physical path, and run `Configure-IIS.ps1` as Administrator.
3. Set `ConnectionStrings__HpmDatabase`, `ASPNETCORE_ENVIRONMENT=Production`, and the initial administrator secret through IIS environment variables or a secret store.
4. Apply EF migrations before opening the site and verify `/health` returns HTTP 200.
5. Bind the hospital-owned TLS certificate in IIS and redirect HTTP to HTTPS.
6. Schedule `Backup-HpmWeb.ps1` in Windows Task Scheduler under a SQL-authorized service identity.
7. Logs roll daily under `logs` and retain 30 files. Grant the application-pool identity write permission only to that folder.

## HTTPS and LAN firewall

Run from an elevated PowerShell after the hospital certificate is installed in
`LocalMachine\My`:

```powershell
.\Configure-Https.ps1 `
  -SiteName PrimeHPM `
  -CertificateThumbprint 'HOSPITAL_CERTIFICATE_THUMBPRINT' `
  -HostName 'hpm.hospital.local' `
  -FirewallRemoteAddress 'LocalSubnet'
```

The script validates the certificate, creates the IIS HTTPS binding and limits
the inbound firewall rule to the supplied LAN scope. Use an explicit CIDR value
instead of `LocalSubnet` when hospital IT provides one.

## Staging readiness check

After deployment and EF migration, run:

```powershell
.\Test-StagingReadiness.ps1 `
  -BaseUrl 'https://hpm.hospital.local' `
  -RequireHttps `
  -ConcurrentClients 10 `
  -RequestsPerClient 5 `
  -ReportPath '..\deployment-reports\staging-readiness.json'
```

Supplying `-ConnectionString` additionally verifies that the connected database
is `HPM_Web` and contains EF migration history. The concurrent probe validates
availability only; reception/cashier transaction concurrency still requires the
signed UAT workflow in `UAT_CHECKLIST.md`.

Certificate selection, public DNS, server credentials and final firewall scope must be supplied by hospital IT and are intentionally not embedded in source control.
