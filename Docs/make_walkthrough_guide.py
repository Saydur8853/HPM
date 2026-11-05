"""
Builds Polular_Pharmacy_Walkthrough.docx from the walkthrough screenshots.
Run: python e:\HPM\Docs\make_walkthrough_guide.py
"""
from docx import Document
from docx.shared import Pt, RGBColor, Inches
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.oxml.ns import qn
from docx.oxml import OxmlElement
import os, datetime

SHOTS = r"e:\HPM\Docs\walkthrough"
OUT   = r"e:\HPM\Docs\Polular_Pharmacy_Walkthrough.docx"

doc = Document()

# margins
s = doc.sections[0]
s.left_margin = s.right_margin = Inches(1)
s.top_margin = Inches(0.9); s.bottom_margin = Inches(0.75)

def h(text, level=1, color=(31,73,125)):
    p = doc.add_heading(text, level=level)
    for r in p.runs: r.font.color.rgb = RGBColor(*color)
    return p

def para(text, bold=False, size=11):
    p = doc.add_paragraph()
    r = p.add_run(text); r.font.size = Pt(size); r.font.bold = bold
    return p

def img(fname, caption="", width=6.0):
    path = os.path.join(SHOTS, fname)
    if not os.path.exists(path):
        para(f"[Missing: {fname}]", size=9)
        return
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run = p.add_run()
    run.add_picture(path, width=Inches(width))
    if caption:
        cp = doc.add_paragraph(caption)
        cp.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for r in cp.runs: r.font.size = Pt(9); r.font.color.rgb = RGBColor(100,100,100)

def step(num, title, desc):
    p = doc.add_paragraph(style="List Number")
    p.paragraph_format.left_indent = Inches(0.2)
    r1 = p.add_run(f"{title}: "); r1.font.bold = True; r1.font.size = Pt(11)
    r1.font.color.rgb = RGBColor(31,73,125)
    r2 = p.add_run(desc); r2.font.size = Pt(11)

def note(text):
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.3)
    r1 = p.add_run("💡 Note: "); r1.font.bold = True; r1.font.size = Pt(10)
    r1.font.color.rgb = RGBColor(0,112,192)
    r2 = p.add_run(text); r2.font.size = Pt(10)

# ── Cover ─────────────────────────────────────────────────────────────────────
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("POLULAR HOSPITAL")
r.font.size = Pt(24); r.font.bold = True; r.font.color.rgb = RGBColor(31,73,125)

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("Pharmacy Module — Complete Walkthrough Guide")
r.font.size = Pt(16); r.font.bold = True

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run(f"With Live Screenshots  |  {datetime.date.today().strftime('%d %B %Y')}")
r.font.size = Pt(11); r.font.color.rgb = RGBColor(120,120,120)

doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 1 — Login", 1)
para("Open your browser and go to the hospital system URL. Enter your username and password.")
step(1, "Open browser", "Navigate to http://localhost:5180")
step(2, "Enter credentials", "Type your Username and Password then click Sign In.")
img("01_login.png", "Login screen")
doc.add_paragraph()
img("02_dashboard.png", "Dashboard after login")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 2 — Open the Pharmacy Menu", 1)
para("The Pharmacy menu is in the top navigation bar. Click it to see all pharmacy modules grouped by function.")
img("03_pharmacy_menu.png", "Pharmacy menu — three groups: Dispensary & Sales, Inventory & Stock, Procurement & Suppliers")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 3 — Pharmacy Masters (One-Time Setup)", 1)
para("Before any stock or sales can be done, set up the master data: medicines, suppliers and stores.")

h("3A  Add a Medicine", 2)
step(1, "Go to Masters", "Pharmacy → any masters link, or navigate to /pharmacy/masters")
step(2, "Click Medicines tab", "Shows the medicine list and add form.")
step(3, "Fill Medicine Name", "e.g. Napa 500mg")
step(4, "Select Dosage Type", "Tablet, Syrup, Injection, etc.")
step(5, "Enter Buy Rate & Sale Rate", "Purchase cost and counter selling price per unit.")
step(6, "Click Save", "Medicine is added and appears in the list.")
img("05_masters_medicine_form.png", "Medicine add form — filled in")
img("06_masters_medicine_saved.png", "Medicine saved — appears in list below")

h("3B  Add a Supplier", 2)
step(1, "Click Suppliers tab", "Switch to the Suppliers section.")
step(2, "Enter Name, Address, Contact", "Fill supplier details.")
step(3, "Save", "Click Save Supplier.")
img("07_masters_supplier_form.png", "Supplier form filled")
img("08_masters_supplier_saved.png", "Supplier saved")

h("3C  Add a Store", 2)
step(1, "Click Stores tab", "Switch to Stores section.")
step(2, "Enter Store Name", "e.g. Main Pharmacy")
step(3, "Tick Active", "Makes it available in all dropdowns.")
step(4, "Save", "Click Save Store.")
img("09_masters_store_form.png", "Store form filled")
img("10_masters_store_saved.png", "Store saved and active")
note("You must complete Steps 3A, 3B and 3C before any purchasing or selling.")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 4 — Purchase Requisition", 1)
para("The Pharmacy Operator requests medicines from the Administrator before purchasing.")
step(1, "Open", "Pharmacy → Procurement & Suppliers → Purchase Requisition")
step(2, "Select Store", "Choose which store needs the medicines.")
step(3, "Select Medicine", "Pick from the dropdown.")
step(4, "Enter Quantity & Est. Rate", "How many units needed and expected price.")
step(5, "Click + Add", "Item added to the list. Repeat for more medicines.")
step(6, "Submit Requisition", "Click Submit. Status becomes Pending.")
img("11_requisition_empty.png", "Purchase Requisition — empty form")
img("12_requisition_item_added.png", "Medicine item added to requisition")
img("13_requisition_submitted_pending.png", "Requisition submitted — Status: Pending")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 5 — Approve the Requisition (Administrator)", 1)
para("The Administrator reviews and approves or rejects the requisition.")
step(1, "Log in as Administrator", "Use an admin account.")
step(2, "Open Purchase Requisition", "Same page — Pharmacy → Purchase Requisition.")
step(3, "Click View", "Click View next to a Pending requisition in the list.")
step(4, "Review items", "Check medicines, quantities and rates.")
step(5, "Approve or Reject", "Click ✓ Approve or ✕ Reject. Status updates immediately.")
img("14_requisition_admin_view.png", "Admin view — Approve and Reject buttons visible")
img("15_requisition_approved.png", "Requisition approved — Status: Approved")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 6 — Receiving Challan (Stock Inward)", 1)
para("When medicines arrive from the supplier, record them to update stock balance.")
step(1, "Open", "Pharmacy → Procurement & Suppliers → Receiving Challan (Stock In)")
step(2, "Select Supplier & Store", "Choose who supplied the medicines and which store receives them.")
step(3, "Enter Invoice No. & Date", "From the supplier's delivery note.")
step(4, "Add medicine item", "Pick medicine, enter batch number, expiry date, quantity, buy rate and sale rate.")
step(5, "Click + Add Item", "Item added to challan. Repeat for each medicine.")
step(6, "Post Challan", "Click Post. Stock is updated immediately.")
img("16_challan_empty.png", "Receiving Challan — empty")
img("17_challan_header_filled.png", "Header filled — supplier, store, invoice details")
img("18_challan_item_filled.png", "Medicine item details filled in")
img("19_challan_item_added.png", "Item added to challan list")
img("20_challan_posted.png", "Challan posted — stock updated")
note("After posting you cannot edit the challan. Stock increases immediately.")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 7 — Live Stock & Expiry Monitor", 1)
para("Check current stock levels across all stores and see medicines expiring soon.")
step(1, "Open", "Pharmacy → Inventory & Stock → Live Stock & Expiry")
step(2, "Filter by store", "Use the store dropdown to view one store at a time.")
step(3, "Check Expiry column", "Near-expiry items are highlighted in orange/red.")
img("21_live_stock.png", "Live Stock — current balances with expiry alerts")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 8 — OPD Counter POS (Sell Medicines)", 1)
para("Sell medicines over the counter to walk-in or OPD patients.")
step(1, "Open", "Pharmacy → Dispensary & Sales → OPD Counter POS")
step(2, "Select Store", "Choose the dispensing store (e.g. Main Pharmacy).")
step(3, "Enter Customer Name", "Type the patient or customer name.")
step(4, "Search Medicine", "Type medicine name — matching medicines appear as suggestions.")
step(5, "Select medicine & set Qty", "Click the suggestion, adjust quantity if needed.")
step(6, "Click + Add", "Medicine added to cart with stock and total shown.")
step(7, "Complete Sale & Print Receipt", "Click the button. Receipt is generated.")
img("22_pos_empty.png", "POS — empty, ready for new sale")
img("23_pos_medicine_search.png", "POS — medicine search with suggestions")
img("24_pos_cart.png", "POS — medicine in cart with total")
img("25_pos_receipt.png", "POS — sale completed, receipt ready to print")
note("Medicines only appear in POS if stock has been received via Receiving Challan first.")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 9 — Sales Returns (Customer Refund)", 1)
para("Process a refund when a customer returns unused medicines.")
step(1, "Open", "Pharmacy → Dispensary & Sales → Sales Returns (Customer)")
step(2, "Search original bill", "Enter Cash Memo ID or patient name.")
step(3, "Select items to return", "Tick medicines being returned and enter return qty.")
step(4, "Process Return", "Click Process Return. Stock restored, refund amount shown.")
img("26_sales_returns.png", "Sales Returns page")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 10 — Stock Control (Inter-Store Transfer & Issue)", 1)
para("Transfer medicines between stores or issue to wards/departments.")
img("27_stock_control.png", "Stock Control — transfer and issue options")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 11 — Stock Adjustment & Audit", 1)
para("Correct stock counts after a physical audit or write off damaged/expired items.")
step(1, "Open", "Pharmacy → Inventory & Stock → Stock Adjustment & Audit")
step(2, "Select store & medicine", "Find the medicine needing correction.")
step(3, "Enter physical count", "Type the actual quantity after counting.")
step(4, "Select reason", "Damage, Expiry, Count Correction, etc.")
step(5, "Save", "Stock updated. All adjustments logged with timestamp.")
img("28_stock_adjustment.png", "Stock Adjustment — physical count entry")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 12 — Supplier Payment", 1)
para("Record payments made to medicine suppliers.")
step(1, "Open", "Pharmacy → Procurement & Suppliers → Supplier Payment")
step(2, "Select Supplier", "Choose the supplier being paid.")
step(3, "Enter Amount & Payment Mode", "Cash, Bank Transfer, Cheque, etc.")
step(4, "Save", "Supplier ledger updated.")
img("29_supplier_payment.png", "Supplier Payment form")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 13 — Doctor Prescription", 1)
para("Doctors create digital prescriptions. Pharmacy Operators scan the QR to auto-fill the POS cart.")
step(1, "Doctor creates Rx", "Pharmacy → Doctor Prescription → search patient → add medicines → Submit.")
step(2, "QR code generated", "System generates QR code e.g. RX-00001.")
step(3, "Operator scans at POS", "Scan the QR at POS → all medicines auto-fill the cart.")
step(4, "Complete sale", "Adjust quantities if needed then click Complete Sale.")
img("30_prescriptions.png", "Doctor Prescriptions page")
doc.add_page_break()

# ═══════════════════════════════════════════════════════════════════════════════
h("Step 14 — IPD Medicine Billing", 1)
para("Bill medicines dispensed to admitted (IPD) patients.")
img("31_ipd_medicine_billing.png", "IPD Medicine Billing page")

# ── Footer ────────────────────────────────────────────────────────────────────
for sec in doc.sections:
    fp = sec.footer.paragraphs[0]
    fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = fp.add_run("Polular Hospital  |  Pharmacy Walkthrough Guide  |  Page ")
    r.font.size = Pt(9); r.font.color.rgb = RGBColor(130,130,130)
    for tag, txt in [('begin',''), ('end','')]:
        fc = OxmlElement('w:fldChar'); fc.set(qn('w:fldCharType'), tag)
        it = OxmlElement('w:instrText'); it.set(qn('xml:space'),'preserve'); it.text='PAGE'
        re = OxmlElement('w:r')
        if tag == 'begin': re.append(fc); re.append(it)
        else: re.append(fc)
        fp._p.append(re)

doc.save(OUT)
print(f"Saved: {OUT}")
