# HPM - Hospital Patient Management (Web Edition)

Modern, secure, and performant web-based Hospital Management System built with **ASP.NET Core Blazor (InteractiveServer)**, **Entity Framework Core**, and **SQL Server**.

---

## 🌟 Key Features
- **Patient Management & Fast Search:** Instant search by Patient ID or Contact Number, live suggestions, and keyboard navigation.
- **CR80 ID Card Printing & Export:**
  - Standard CR80 ISO 7810 card size (85.60 mm × 53.98 mm, 3.18 mm corner radius).
  - High-resolution **PNG download (300+ DPI)** via `html2canvas`.
  - **PDF card download** via client-side `jspdf`.
  - Margin-free direct card printing via isolated iframe.
  - Shortcut `Ctrl + P` opens card export & print preview directly.
- **Billing & Services:** OPD Tickets, Doctor Appointments, Emergency Services, IPD Admission, Diagnostic Billing.
- **Reports & Records:** Barcode and QR code generation, patient document tracking, and audit logging.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB or full SQL Server instance

### Run Locally
```bash
# Restore and run
dotnet restore
dotnet run --project HPM.Web/HPM.Web.csproj
```
The application will launch on `http://localhost:5180`.

---
© 2026 HPM Team.
