@echo off
setlocal EnableExtensions
title HPM Web Launcher
cd /d "%~dp0"

set "HPM_URL=http://localhost:5180"
set "HPM_SQL_SERVICE=MSSQL$HPMDB"

rem SQL Server must be running for login, reports and live data.
powershell -NoProfile -Command "$s=Get-Service -Name '%HPM_SQL_SERVICE%' -ErrorAction SilentlyContinue; if($null -eq $s){exit 2}; if($s.Status -eq 'Running'){exit 0}else{exit 1}"
set "SQL_CHECK=%ERRORLEVEL%"

if "%SQL_CHECK%"=="2" (
    echo.
    echo ERROR: SQL Server instance HPMDB was not found.
    echo Install or restore the HPMDB SQL Server instance, then try again.
    echo.
    pause
    exit /b 2
)

if not "%SQL_CHECK%"=="0" (
    net session >nul 2>&1
    if errorlevel 1 (
        echo Requesting Administrator permission to start SQL Server HPMDB...
        powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
        exit /b 0
    )

    echo Starting SQL Server HPMDB...
    powershell -NoProfile -Command "Start-Service -Name '%HPM_SQL_SERVICE%' -ErrorAction Stop; (Get-Service -Name '%HPM_SQL_SERVICE%').WaitForStatus('Running',[TimeSpan]::FromSeconds(30))"
    if errorlevel 1 (
        echo.
        echo ERROR: SQL Server HPMDB could not be started.
        echo Check SQL Server Configuration Manager or Windows Services.
        echo.
        pause
        exit /b 3
    )
)

rem Reuse the existing HPM Web server instead of starting a duplicate on port 5180.
powershell -NoProfile -Command "try{$r=Invoke-WebRequest -UseBasicParsing '%HPM_URL%' -TimeoutSec 4; if($r.StatusCode -ge 200 -and $r.StatusCode -lt 500){exit 0}}catch{}; exit 1"
if not errorlevel 1 (
    echo HPM Web is already running at %HPM_URL%
    start "" "%HPM_URL%"
    exit /b 0
)

where dotnet >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERROR: .NET SDK was not found in PATH.
    echo Install the .NET 8 SDK, then run this launcher again.
    echo.
    pause
    exit /b 4
)

if not exist "HPM.Web\HPM.Web.csproj" (
    echo.
    echo ERROR: HPM.Web\HPM.Web.csproj was not found.
    echo Keep this launcher in the HPM-Web project folder.
    echo.
    pause
    exit /b 5
)

echo Starting HPM Web at %HPM_URL% ...
echo Keep this window open while using the application.
echo Press Ctrl+C to stop HPM Web.
echo.

set "ASPNETCORE_ENVIRONMENT=Development"
dotnet run --project "HPM.Web\HPM.Web.csproj" --launch-profile http --no-build

if errorlevel 1 (
    echo.
    echo HPM Web stopped with an error. Review the message above.
    pause
)

endlocal
