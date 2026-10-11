# PrimeHPM Web — OPD & Emergency

This is the first web release of the recovered PrimeHPM desktop system. It is a separate .NET 8 Blazor Web App and does not modify or replace the legacy WinForms application.

## Included modules

1. Patient Registration
2. Doctor Appointment
3. Consultation Ticket
4. OPD & Emergency Service
5. OT Cash Memo
6. IPD Patient Admission and Bed Allocation
7. IPD Bed Occupancy and Patient Transfer
8. IPD Money Receipt and Advance Collection
9. IPD Daily Patient Services and Discharge Billing
10. Diagnostic Billing, Lab Worklist and Results
11. Pharmacy Medicine, Generic, Dosage Form and Supplier Masters
12. Complete Pharmacy: prescriptions, purchase/POS, customer and supplier returns, supplier payment, batch stock, adjustment/disposal and reconciliation
13. Accounts, General Ledger, Vouchers, Income Statement and Cashier Handover

OPD, Emergency, Diagnostic, OT, IPD receipt and Pharmacy transactions now create balanced journals in the same database transaction. Manual vouchers use pending/approval control; posted vouchers and IPD receipts are corrected only through reasoned immutable reversals. Cashier day closing and financial period locks prevent late posting, while the Accounts screen provides printable vouchers and daily Central Cash vs General Ledger reconciliation.

Pharmacy purchase, sale, purchase-return and supplier-payment posting now updates stock/sub-ledgers and creates a balanced accounts voucher inside the same database transaction.

Customer sales returns calculate a proportional discount-aware refund, reverse cash/accounts/IPD charges, and optionally restore a verified batch. Stock-control transactions are immutable and the reconciliation report compares the captured opening baseline plus controlled movements with current batch balances.

Every released module supports create, edit, protected delete and printable documents. The Reports area provides date-range OPD operational/collection reporting. Master Setup provides CRUD screens for doctors, departments, ticket types, service categories, services, IPD departments, bed categories and beds/cabins.

ASP.NET Core Identity protects the application. Operational access is assigned through Administrator, Reception, Cashier, Emergency Operator, OT Operator, Doctor, Lab Technician and Pharmacy Operator roles. Administrators can create/disable users, reset passwords, assign roles and review the audit log. Temporary and administrator-reset passwords must be changed at the next sign-in. Legacy entity creates, updates and deletes are recorded automatically in the same database transaction as the business change.

## Technology

- .NET 8 Blazor Web App with Interactive Server rendering
- Entity Framework Core 8
- SQL Server
- EF migrations that create a new database from an empty server/database

The EF mappings retain the original table and column names, including `Registration.Patient_ID`, `Doctor_Appointment.Appt_SLNO`, `OPD_Ticket`, `Cash_Memo_DInvestigation`, `Cash_Memo_Sub_DInvestigation`, `Cash_Memo_OT`, `Admission`, `Admission_Ext`, `Bed_Cabin`, `Bed_Category`, `Bed_Cabin_Transection`, `Patient_Transaction`, `Patient_Package_Transaction`, `Money_Receipt`, `Patient_Payment`, `Central_Cash_Collection`, `Service`, `Generic_Group`, `Medicine_Type`, `Supplier_Pharmacy`, `Medicine_Store`, `Medicine_Store_Balance`, `Medicine_Return`, `Medicine_Return_Sub`, `Supplier_Payment` and `Supplier_Ledger`.

## Local setup

Prerequisites: .NET 8 SDK, SQL Server, and the EF command-line tool.

```powershell
dotnet tool install --global dotnet-ef --version 8.*
dotnet restore HPM.Web.sln
dotnet ef database update --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj
dotnet run --project HPM.Web\HPM.Web.csproj --launch-profile http
```

The checked-in development connection creates/uses `HPM_Web` on the local `HPMDB` SQL Server instance. It deliberately does not point to the recovered legacy `HPM` database.

No bootstrap credential is stored in source control. On an empty database, configure a one-time administrator through environment variables before the first run; the account must change its password after signing in. The former `Admin@12345` default is explicitly rejected.

```powershell
$env:InitialAdmin__UserName = "admin"
$env:InitialAdmin__Password = "use-a-unique-strong-temporary-password"
$env:InitialAdmin__Email = "admin@example.local"
$env:InitialAdmin__DisplayName = "System Administrator"
```

After the account has been created, remove the `InitialAdmin__*` variables from the host configuration.

For another server or deployment environment, set the connection string without changing source control:

```powershell
$env:ConnectionStrings__HpmDatabase = "Server=SERVER;Database=HPM_Web;User ID=USER;Password=PASSWORD;TrustServerCertificate=True"
```

The first migration includes a minimal Duty Doctor, Emergency and Medicine departments, consultation ticket type, and sample Emergency/OT service masters so all five workflows can be tested immediately. These are reference records, not patient data.

## Create another migration

```powershell
dotnet ef migrations add MigrationName --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj --output-dir Data\Migrations
dotnet ef database update --project HPM.Web\HPM.Web.csproj --startup-project HPM.Web\HPM.Web.csproj
```

## Automated tests

Run the fast workflow and EF-model regression suite:

```powershell
dotnet test HPM.Web.Tests\HPM.Web.Tests.csproj
```

To include the destructive isolated SQL migration/atomicity test, provide a dedicated database whose name ends with `_AutomatedTests`. The test creates, migrates and removes only that database:

```powershell
$env:HPM_TEST_CONNECTION = "Server=.\HPMDB;Database=HPM_Web_AutomatedTests;Trusted_Connection=True;TrustServerCertificate=True"
dotnet test HPM.Web.Tests\HPM.Web.Tests.csproj
```

## Publish

```powershell
dotnet publish HPM.Web\HPM.Web.csproj -c Release -o .\publish\HPM.Web
```

Configure `ConnectionStrings__HpmDatabase` in the hosting environment, apply migrations, then host the published folder in IIS, Azure App Service, a Windows service, or a container-capable .NET 8 host.

## Current boundary

This release covers the secured five-module OPD & Emergency workflow, IPD admission/bed transfer/receipts/services/discharge, diagnostic laboratory workflow, pharmacy purchase/stock/POS, and accounts. It also includes consolidated reporting, master-data administration, users/roles, audit logging, health monitoring, structured logs and repeatable IIS/backup scripts. Live legacy import and production rollout remain gated by hospital mapping approval and infrastructure credentials.

Phase 3 testing and migration work is tracked in the repository-level `IMPLEMENTATION_TASKS.md`, `UAT_CHECKLIST.md` and `LEGACY_DATA_PROFILE.md` files. The reusable `migration/ProfileLegacy.sql` script profiles the source database without modifying it.
