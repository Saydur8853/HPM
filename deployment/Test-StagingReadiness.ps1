param(
    [Parameter(Mandatory = $true)]
    [string]$BaseUrl,
    [string]$ConnectionString = "",
    [string]$ExpectedDatabase = "HPM_Web",
    [int]$ConcurrentClients = 10,
    [int]$RequestsPerClient = 5,
    [switch]$RequireHttps,
    [string]$ReportPath = ""
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Net.Http

if ($ConcurrentClients -lt 1 -or $ConcurrentClients -gt 100) {
    throw 'ConcurrentClients must be between 1 and 100.'
}
if ($RequestsPerClient -lt 1 -or $RequestsPerClient -gt 100) {
    throw 'RequestsPerClient must be between 1 and 100.'
}

$baseUri = [Uri]$BaseUrl
if ($baseUri.Scheme -notin @('http', 'https')) {
    throw 'BaseUrl must use HTTP or HTTPS.'
}
if ($RequireHttps -and $baseUri.Scheme -ne 'https') {
    throw 'HTTPS is required but BaseUrl is not HTTPS.'
}

$healthUri = [Uri]::new($baseUri, '/health')
$checks = [System.Collections.Generic.List[object]]::new()

function Add-Check {
    param([string]$Name, [bool]$Passed, [string]$Detail)
    $checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    })
}

try {
    $healthResponse = Invoke-WebRequest -Uri $healthUri -UseBasicParsing -TimeoutSec 30
    $healthJson = $healthResponse.Content | ConvertFrom-Json
    Add-Check 'Application health endpoint' ($healthResponse.StatusCode -eq 200 -and $healthJson.database -eq $true) `
        "HTTP $($healthResponse.StatusCode); database=$($healthJson.database); status=$($healthJson.status)"
}
catch {
    Add-Check 'Application health endpoint' $false $_.Exception.Message
}

if ($baseUri.Scheme -eq 'https') {
    try {
        $request = [System.Net.HttpWebRequest]::Create($healthUri)
        $request.Method = 'HEAD'
        $request.Timeout = 30000
        $response = $request.GetResponse()
        $certificate = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
            $request.ServicePoint.Certificate)
        $daysRemaining = [math]::Floor(($certificate.NotAfter - (Get-Date)).TotalDays)
        Add-Check 'TLS certificate' ($daysRemaining -gt 0) `
            "Subject=$($certificate.Subject); expires=$($certificate.NotAfter.ToString('u')); daysRemaining=$daysRemaining"
        $response.Dispose()
    }
    catch {
        Add-Check 'TLS certificate' $false $_.Exception.Message
    }
}
elseif ($RequireHttps) {
    Add-Check 'TLS certificate' $false 'HTTPS was required but is not configured.'
}

if (-not [string]::IsNullOrWhiteSpace($ConnectionString)) {
    try {
        $connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = @'
SELECT
    DB_NAME() AS DatabaseName,
    CASE WHEN OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL THEN 0 ELSE 1 END AS HasMigrationHistory;
'@
        $reader = $command.ExecuteReader()
        [void]$reader.Read()
        $databaseName = [string]$reader['DatabaseName']
        $hasHistory = [int]$reader['HasMigrationHistory'] -eq 1
        $reader.Close()
        $connection.Close()
        Add-Check 'SQL database identity' ($databaseName -eq $ExpectedDatabase) `
            "Connected database=$databaseName; expected=$ExpectedDatabase"
        Add-Check 'EF migration history' $hasHistory `
            "__EFMigrationsHistory present=$hasHistory"
    }
    catch {
        Add-Check 'SQL connectivity' $false $_.Exception.Message
    }
}
else {
    Add-Check 'SQL connectivity' $true 'Skipped: no ConnectionString supplied; /health still validates application database access.'
}

$totalRequests = $ConcurrentClients * $RequestsPerClient
$client = [System.Net.Http.HttpClient]::new()
$client.Timeout = [TimeSpan]::FromSeconds(30)
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$tasks = [System.Collections.Generic.List[System.Threading.Tasks.Task[System.Net.Http.HttpResponseMessage]]]::new()
for ($i = 0; $i -lt $totalRequests; $i++) {
    $tasks.Add($client.GetAsync($healthUri))
}

$responses = [System.Threading.Tasks.Task]::WhenAll($tasks).GetAwaiter().GetResult()
$stopwatch.Stop()
$successCount = @($responses | Where-Object { [int]$_.StatusCode -eq 200 }).Count
foreach ($response in $responses) {
    $response.Dispose()
}
$client.Dispose()

Add-Check 'Concurrent health requests' ($successCount -eq $totalRequests) `
    "$successCount/$totalRequests succeeded in $([math]::Round($stopwatch.Elapsed.TotalSeconds, 2)) seconds"

$passed = @($checks | Where-Object { -not $_.Passed }).Count -eq 0
$result = [pscustomobject]@{
    GeneratedAt = (Get-Date).ToString('o')
    BaseUrl = $baseUri.AbsoluteUri
    Passed = $passed
    ConcurrentClients = $ConcurrentClients
    RequestsPerClient = $RequestsPerClient
    Checks = $checks
}

$result | ConvertTo-Json -Depth 5

if (-not [string]::IsNullOrWhiteSpace($ReportPath)) {
    $resolvedReportPath = [System.IO.Path]::GetFullPath($ReportPath)
    $reportDirectory = Split-Path -Parent $resolvedReportPath
    if (-not (Test-Path -LiteralPath $reportDirectory)) {
        New-Item -ItemType Directory -Path $reportDirectory -Force | Out-Null
    }
    $result | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedReportPath -Encoding UTF8
    Write-Host "Readiness report: $resolvedReportPath"
}

if (-not $passed) {
    exit 1
}
