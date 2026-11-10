# PrimeHPM — Hospital Patient Management

PrimeHPM is a web-based hospital management system built with ASP.NET Core Blazor, Entity Framework Core, and SQL Server. It covers patient registration, appointments, OPD and emergency services, IPD, diagnostics, pharmacy, accounts, reporting, user management, and audit logging.

## Technology stack

- .NET 8 / ASP.NET Core Blazor Web App (Interactive Server)
- Entity Framework Core 8
- Microsoft SQL Server
- ASP.NET Core Identity
- xUnit

## Prerequisites

Install the following before starting:

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [SQL Server 2019 or later](https://www.microsoft.com/sql-server/sql-server-downloads) (Developer or Express is suitable for development)
3. [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) (optional, but recommended)
4. Git

Confirm that .NET is available:

```powershell
dotnet --version
```

The reported version should be `8.x`, or a newer SDK capable of targeting .NET 8.

## 1. Clone the repository

```powershell
git clone https://github.com/Saydur8853/HPM.git
cd HPM
```

If you already have the repository, open PowerShell in its root directory—the folder containing `HPM.Web.sln`.

## 2. Configure SQL Server

The checked-in development configuration expects a local named SQL Server instance called `HPMDB` and creates a database named `HPM_Web`. It never targets the legacy `HPM` database.

If your instance is named `HPMDB`, start it from an elevated PowerShell window when necessary:

```powershell
Start-Service 'MSSQL$HPMDB'
```

For another SQL Server instance, set the connection string in the current PowerShell session. Examples:

Windows authentication:

```powershell
$env:ConnectionStrings__HpmDatabase = 'Server=.\SQLEXPRESS;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True'
```

SQL Server authentication:

```powershell
$env:ConnectionStrings__HpmDatabase = 'Server=localhost;Database=HPM_Web;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True'
```

Do not commit real database credentials to `appsettings.json`.

## 3. Restore packages and prepare the database

```powershell
dotnet restore HPM.Web.sln
dotnet tool install --global dotnet-ef --version 8.*
dotnet ef database update --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj
```

If `dotnet-ef` is already installed, keep it on the EF Core 8 major version:

```powershell
dotnet tool update --global dotnet-ef --version 8.*
```

The application also checks and applies pending migrations during startup. Running `database update` explicitly is recommended because it reports database or permission errors immediately.

## 4. Run the application

```powershell
dotnet run --project HPM.Web\HPM.Web.csproj --launch-profile http
```

Open [http://localhost:5180](http://localhost:5180).

On a new database, visit [http://localhost:5180/setup](http://localhost:5180/setup) and provide:

- the hospital name;
- an optional hospital photo/logo;
- the App Owner user ID; and
- a strong password with at least eight characters, including uppercase, lowercase, a number, and a special character.

The setup page can be completed only once. After setup, sign in with the App Owner account and open `/app-owner`. Select licensed modules (required dependencies are added automatically), enter module prices and subscription/hosting/domain dates, then create the Hospital Owner credentials. The App Owner has full administrative access regardless of the hospital's licensed package; opening `/` redirects them to `/app-owner`. The Hospital Owner receives access to licensed modules and can create staff users only within that license; unavailable modules are hidden and direct navigation is denied.

For an existing installation, current module access is retained until an App Owner activates a package. To provision its first App Owner without changing existing users, set `InitialAppOwner__UserName` and `InitialAppOwner__Password` as environment variables before startup. The new account must change its temporary password at first sign-in. Do not put these credentials in source control.

Subscription invoices are issued and marked paid manually in `/app-owner`. Hosting, domain and subscription renewals appear as red warnings to the Hospital Owner from 15 days before their expiry. Expiry warnings do not automatically remove clinical access; the App Owner controls module activation.

### Windows quick launcher

After the project has been restored and built at least once, `RUN_HPM_WEB.cmd` can start the `HPMDB` SQL Server service, launch the app, and open it in the browser:

```powershell
.\RUN_HPM_WEB.cmd
```

The launcher is specific to the `HPMDB` named instance and uses `--no-build`. Use `dotnet run` for the first run or whenever the code has not been built.

## Development workflow

Build the complete solution:

```powershell
dotnet build HPM.Web.sln
```

Run the automated test suite:

```powershell
dotnet test HPM.Web.Tests\HPM.Web.Tests.csproj
```

Several repository tests inspect the local `HPMDB` instance and expect the legacy `HPM` database and the development `HPM_Web` database to exist. To run only the database-independent workflow and EF model tests:

```powershell
dotnet test HPM.Web.Tests\HPM.Web.Tests.csproj --filter FullyQualifiedName~WorkflowAndModelTests
```

The isolated SQL transaction tests run only when `HPM_TEST_CONNECTION` points to a disposable database whose name ends in `_AutomatedTests`:

```powershell
$env:HPM_TEST_CONNECTION = 'Server=.\HPMDB;Database=HPM_Web_AutomatedTests;Trusted_Connection=True;TrustServerCertificate=True'
dotnet test HPM.Web.Tests\HPM.Web.Tests.csproj
```

These integration tests create, migrate, and remove that dedicated test database. Never point `HPM_TEST_CONNECTION` at development or production data.

### Create a database migration

After changing the EF model:

```powershell
dotnet ef migrations add MigrationName --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj --output-dir Data\Migrations
dotnet ef database update --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj
```

Commit both migration files and the updated model snapshot.

## Useful URLs

| URL | Purpose |
| --- | --- |
| `http://localhost:5180/` | Application |
| `http://localhost:5180/setup` | One-time hospital and Superadmin setup |
| `http://localhost:5180/health` | Application and database health check |

## Troubleshooting

### Cannot connect to SQL Server

- Confirm the SQL Server service is running: `Get-Service 'MSSQL$HPMDB'`.
- Verify the server/instance name in `ConnectionStrings__HpmDatabase`.
- Confirm that the current Windows account or SQL login can create and update `HPM_Web`.
- Add `TrustServerCertificate=True` for a local development SQL Server certificate.

### `dotnet ef` is not recognized

Install the EF Core 8 CLI and restart the terminal:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
```

### Port 5180 is already in use

Stop the other process, or use another URL:

```powershell
dotnet run --project HPM.Web\HPM.Web.csproj --urls http://localhost:5190
```

### HTTPS development certificate error

The `http` launch profile does not require a development certificate. To use the `https` profile, trust the local certificate first:

```powershell
dotnet dev-certs https --trust
dotnet run --project HPM.Web\HPM.Web.csproj --launch-profile https
```

## Publishing

Create a release build:

```powershell
dotnet publish HPM.Web\HPM.Web.csproj -c Release -o .\publish\HPM.Web
```

Set `ConnectionStrings__HpmDatabase` in the hosting environment, apply migrations, and host the published output on a machine with the .NET 8 Hosting Bundle. IIS deployment and backup scripts are documented in [`deployment/README.md`](deployment/README.md).

## Additional documentation

- [`USER_MANUAL_BN.md`](USER_MANUAL_BN.md) — Bangla user manual
- [`HPM.Web/README.md`](HPM.Web/README.md) — module and architecture notes
- [`deployment/README.md`](deployment/README.md) — IIS, HTTPS, health check, and backup guidance
