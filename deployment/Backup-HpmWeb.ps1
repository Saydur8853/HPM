param(
    [string]$SqlInstance = ".\HPMDB",
    [string]$Database = "HPM_Web",
    [string]$BackupDirectory = "C:\PrimeHPM-Backups",
    [int]$RetentionDays = 30
)
$ErrorActionPreference = 'Stop'
$backupRoot = [System.IO.Path]::GetFullPath($BackupDirectory)
if ([System.IO.Path]::GetPathRoot($backupRoot) -eq $backupRoot) { throw 'BackupDirectory cannot be a drive root.' }
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$backupFile = Join-Path $backupRoot "$Database-$stamp.bak"
$escapedFile = $backupFile.Replace("'", "''")
sqlcmd -S $SqlInstance -E -C -b -Q "BACKUP DATABASE [$Database] TO DISK=N'$escapedFile' WITH COPY_ONLY, COMPRESSION, CHECKSUM; RESTORE VERIFYONLY FROM DISK=N'$escapedFile' WITH CHECKSUM;"
if ($LASTEXITCODE -ne 0) { throw 'SQL backup or verification failed.' }
$cutoff = (Get-Date).AddDays(-$RetentionDays)
Get-ChildItem -LiteralPath $backupRoot -Filter "$Database-*.bak" -File | Where-Object LastWriteTime -lt $cutoff | Remove-Item -Force
Write-Host "Verified backup created: $backupFile"
