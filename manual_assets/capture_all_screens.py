import asyncio
import os
import sys
from playwright.async_api import async_playwright

PAGES_TO_CAPTURE = [
    ("01_login_screen", "/login", "System Login Screen"),
    ("02_dashboard_overview", "/", "Hospital Live Dashboard"),
    ("03_patient_registration", "/patients", "Patient Registration (PR)"),
    ("04_doctor_appointments", "/appointments", "Doctor Appointments (AP)"),
    ("05_consultation_tickets", "/tickets", "Consultation Tickets (TK)"),
    ("06_doctor_queue", "/doctor-queue", "Doctor Queue"),
    ("07_emergency_services", "/emergency-services", "Emergency Services (EM)"),
    ("08_ot_cash_memos", "/ot-cash-memos", "OT Cash Memo (OT)"),
    ("09_ipd_bed_occupancy", "/ipd/bed-occupancy", "Bed Occupancy Board (BO)"),
    ("10_ipd_admissions", "/ipd/admissions", "IPD Admissions (AD)"),
    ("11_ipd_advance_receipts", "/ipd/advance-receipts", "Advance Receipts (AR)"),
    ("12_ipd_patient_services", "/ipd/patient-services", "Daily Patient Services (DS)"),
    ("13_ipd_medical_orders", "/ipd/medical-discharge-orders", "Medical Discharge Orders"),
    ("14_ipd_bed_turnover", "/ipd/bed-turnover", "Bed Turnover & Sanitization"),
    ("15_ipd_discharge_billing", "/ipd/discharge-billing", "IPD Final Discharge Billing (DC)"),
    ("16_diagnostic_billing", "/diagnostic/billing", "Diagnostic Test Billing (DX)"),
    ("17_diagnostic_worklist", "/diagnostic/worklist", "Diagnostic Worklist (LW)"),
    ("18_diagnostic_results", "/diagnostic/results", "Lab Results Entry (LR)"),
    ("19_pharmacy_pos", "/pharmacy/pos", "Pharmacy POS Sales (POS)"),
    ("20_pharmacy_purchases", "/pharmacy/purchases", "Stock Inward Purchases (PI)"),
    ("21_pharmacy_stock", "/pharmacy/stock", "Pharmacy Stock & Expiry (ST)"),
    ("22_pharmacy_stock_control", "/pharmacy/stock-control", "Stock Control & Audit (SC)"),
    ("23_pharmacy_prescriptions", "/pharmacy/prescriptions", "Prescription Dispensing Queue"),
    ("24_pharmacy_returns", "/pharmacy/sales-returns", "Pharmacy Sales Returns"),
    ("25_accounts_ledger", "/accounts", "Accounts & General Ledger"),
    ("26_reports_analytics", "/reports", "Hospital Reports & Analytics"),
    ("27_clinical_masters", "/masters", "Doctor & Department Masters"),
    ("28_ipd_masters", "/masters/ipd", "Ward & Bed Masters"),
    ("29_diagnostic_masters", "/diagnostic/masters", "Diagnostic & Lab Test Masters"),
    ("30_pharmacy_masters", "/pharmacy/masters", "Pharmacy Drug Masters"),
    ("31_user_admin", "/admin/users", "User Admin & Permissions"),
    ("32_audit_logs", "/admin/audit", "Audit Trail Logs"),
    ("33_hospital_settings", "/hospital-settings", "Hospital Profile & Settings")
]

async def capture_all():
    output_dir = "d:/Shamim Sir/HPM-Web/manual_assets/screenshots"
    os.makedirs(output_dir, exist_ok=True)

    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            viewport={"width": 1400, "height": 900},
            device_scale_factor=1.25
        )
        page = await context.new_page()

        # 1. Capture Login Screen
        print("Capturing 01_login_screen...", flush=True)
        await page.goto("http://localhost:5180/login", wait_until="domcontentloaded")
        await asyncio.sleep(1.0)
        await page.screenshot(path=os.path.join(output_dir, "01_login_screen.png"))

        # 2. Perform Login
        print("Logging in as admin...", flush=True)
        await page.fill('input[name="UserName"]', "admin")
        await page.fill('input[name="Password"]', "Admin@PrimeHpm2026!")
        await page.click('button[type="submit"]', no_wait_after=True)
        await asyncio.sleep(2.0)
        print("Logged in. Current URL:", page.url, flush=True)

        # 3. Capture all pages
        for idx, (name, route, label) in enumerate(PAGES_TO_CAPTURE[1:], 2):
            url = f"http://localhost:5180{route}"
            print(f"[{idx}/{len(PAGES_TO_CAPTURE)}] Capturing {name} ({route})...", flush=True)
            try:
                await page.goto(url, wait_until="domcontentloaded", timeout=10000)
                await asyncio.sleep(1.2)  # Wait for Blazor Server data rendering
                file_path = os.path.join(output_dir, f"{name}.png")
                await page.screenshot(path=file_path)
                print(f"    -> Saved {file_path}", flush=True)
            except Exception as e:
                print(f"    -> Error on {name}: {e}", flush=True)

        await browser.close()
        print("ALL SCREENSHOTS CAPTURED SUCCESSFULLY!", flush=True)

if __name__ == "__main__":
    asyncio.run(capture_all())
