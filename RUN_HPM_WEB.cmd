@echo off
setlocal EnableExtensions
title HPM Web Server (Port 5180)
cd /d "%~dp0"

echo ======================================================================
echo                     HPM WEB LAUNCHER ^& MANAGER
echo ======================================================================
echo.

set "HPM_URL=http://localhost:5180"
set "HPM_PORT=5180"
set "HPM_SQL_SERVICE=MSSQL$HPMDB"

rem 1. Stop any currently running HPM.Web instances or processes on port 5180
echo [1/3] Stopping any existing HPM Web or port %HPM_PORT% processes...
powershell -NoProfile -Command "Get-Process -Name 'HPM.Web' -ErrorAction SilentlyContinue | Stop-Process -Force; Get-NetTCPConnection -LocalPort %HPM_PORT% -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }" >nul 2>&1
timeout /t 1 /nobreak >nul

rem 2. Check and ensure SQL Server is running
echo [2/3] Checking SQL Server (%HPM_SQL_SERVICE%)...
powershell -NoProfile -Command "$s=Get-Service -Name '%HPM_SQL_SERVICE%' -ErrorAction SilentlyContinue; if($null -eq $s){exit 2}; if($s.Status -eq 'Running'){exit 0}else{exit 1}"
set "SQL_CHECK=%ERRORLEVEL%"

if "%SQL_CHECK%"=="2" (
    echo WARNING: SQL Service '%HPM_SQL_SERVICE%' not found as a named service. Proceeding with connection check...
) else if not "%SQL_CHECK%"=="0" (
    echo Starting SQL Server service %HPM_SQL_SERVICE%...
    powershell -NoProfile -Command "Start-Service -Name '%HPM_SQL_SERVICE%' -ErrorAction SilentlyContinue; (Get-Service -Name '%HPM_SQL_SERVICE%').WaitForStatus('Running',[TimeSpan]::FromSeconds(15))" >nul 2>&1
)

rem 3. Start HPM Web with Hot Reload (UI/CSS/Razor changes apply in ~1s without rebuilding!)
echo [3/3] Starting HPM Web with Hot Reload at %HPM_URL% ...
echo.
echo ======================================================================
echo  Hot Reload is ACTIVE!
echo  - UI / CSS / Razor changes apply in ~1-2 seconds automatically.
echo  - You do NOT need to close and re-run this window after UI edits!
echo  - Database migrations apply automatically on startup/restart.
echo  - Press Ctrl+C to stop HPM Web.
echo ======================================================================
echo.

set "ASPNETCORE_ENVIRONMENT=Development"
set "RESTORE_FLAG="
if exist "HPM.Web\obj\project.assets.json" set "RESTORE_FLAG=--no-restore"

dotnet watch --non-interactive %RESTORE_FLAG% run --project "HPM.Web\HPM.Web.csproj" --launch-profile http

if errorlevel 1 (
    echo.
    echo ======================================================================
    echo ERROR: HPM Web stopped unexpectedly. Review the error messages above.
    echo ======================================================================
    echo.
    pause
)

endlocal
