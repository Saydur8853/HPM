from docx import Document
from docx.shared import Pt, RGBColor, Inches, Cm
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml.ns import qn
from docx.oxml import OxmlElement
import datetime

doc = Document()

# ── Page margins ──────────────────────────────────────────────
section = doc.sections[0]
section.page_width  = Inches(8.5)
section.page_height = Inches(11)
section.left_margin   = Inches(1)
section.right_margin  = Inches(1)
section.top_margin    = Inches(1)
section.bottom_margin = Inches(0.75)

# ── Styles helper ─────────────────────────────────────────────
def set_font(run, name="Calibri", size=11, bold=False, color=None):
    run.font.name = name
    run.font.size = Pt(size)
    run.font.bold = bold
    if color:
        run.font.color.rgb = RGBColor(*color)

def add_heading(doc, text, level=1, color=(31, 73, 125)):
    p = doc.add_heading(text, level=level)
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    for run in p.runs:
        run.font.color.rgb = RGBColor(*color)
    return p

def add_para(doc, text, bold=False, size=11, indent=0):
    p = doc.add_paragraph()
    if indent:
        p.paragraph_format.left_indent = Inches(indent)
    run = p.add_run(text)
    set_font(run, size=size, bold=bold)
    return p

def add_step(doc, number, title, description):
    p = doc.add_paragraph(style="List Number")
    p.paragraph_format.left_indent = Inches(0.25)
    r1 = p.add_run(f"{title}: ")
    set_font(r1, bold=True, size=11, color=(31, 73, 125))
    r2 = p.add_run(description)
    set_font(r2, size=11)

def add_note(doc, text, kind="Note"):
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.25)
    colors = {"Note": (0, 112, 192), "Tip": (0, 150, 60), "Warning": (192, 0, 0)}
    r1 = p.add_run(f"💡 {kind}: ")
    set_font(r1, bold=True, size=10, color=colors.get(kind, (0, 0, 0)))
    r2 = p.add_run(text)
    set_font(r2, size=10)

def add_table_row(table, cells, header=False):
    row = table.add_row()
    for i, text in enumerate(cells):
        cell = row.cells[i]
        cell.text = text
        for run in cell.paragraphs[0].runs:
            run.font.bold = header
            run.font.size = Pt(10)
    return row

def shade_row(row, hex_color="D9E1F2"):
    for cell in row.cells:
        tc = cell._tc
        tcPr = tc.get_or_add_tcPr()
        shd = OxmlElement('w:shd')
        shd.set(qn('w:val'), 'clear')
        shd.set(qn('w:color'), 'auto')
        shd.set(qn('w:fill'), hex_color)
        tcPr.append(shd)

def add_image_placeholder(doc, label):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(4)
    p.paragraph_format.space_after  = Pt(4)
    r = p.add_run(f"[ Screenshot: {label} ]")
    set_font(r, size=9, color=(120, 120, 120))
    # dashed border via shading approximation
    pPr = p._p.get_or_add_pPr()
    pBdr = OxmlElement('w:pBdr')
    for side in ['top','left','bottom','right']:
        bdr = OxmlElement(f'w:{side}')
        bdr.set(qn('w:val'), 'dashed')
        bdr.set(qn('w:sz'), '6')
        bdr.set(qn('w:space'), '4')
        bdr.set(qn('w:color'), 'AAAAAA')
        pBdr.append(bdr)
    pPr.append(pBdr)

# ═══════════════════════════════════════════════════════════════
#  COVER PAGE
# ═══════════════════════════════════════════════════════════════
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("POLULAR HOSPITAL")
set_font(r, size=22, bold=True, color=(31, 73, 125))

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("Pharmacy Module – Step-by-Step User Guide")
set_font(r, size=16, bold=True, color=(70, 70, 70))

doc.add_paragraph()
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run(f"Version 1.0  |  {datetime.date.today().strftime('%B %Y')}")
set_font(r, size=11, color=(120, 120, 120))

p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("HPM Hospital Patient Management System")
set_font(r, size=11)

doc.add_page_break()

# ═══════════════════════════════════════════════════════════════
#  TABLE OF CONTENTS (manual)
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "Table of Contents", level=1)
toc_items = [
    ("1", "Getting Started – Login"),
    ("2", "Pharmacy Masters (Setup)"),
    ("3", "Purchase Requisition"),
    ("4", "Receiving Challan (Stock Inward)"),
    ("5", "OPD Counter POS (Dispensing & Sales)"),
    ("6", "Sales Returns (Customer Refund)"),
    ("7", "Live Stock & Expiry Monitor"),
    ("8", "Inter-Store Transfer"),
    ("9", "Issue to Ward / Department"),
    ("10", "Stock Adjustment & Audit"),
    ("11", "Supplier Payment"),
    ("12", "Doctor Prescription"),
    ("Appendix A", "User Roles & Access"),
    ("Appendix B", "Common Errors & Solutions"),
]
for num, title in toc_items:
    p = doc.add_paragraph()
    p.paragraph_format.left_indent = Inches(0.25)
    r1 = p.add_run(f"{num}.  ")
    set_font(r1, bold=True, size=11, color=(31, 73, 125))
    r2 = p.add_run(title)
    set_font(r2, size=11)

doc.add_page_break()

# ═══════════════════════════════════════════════════════════════
#  SECTION 1 – LOGIN
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "1.  Getting Started – Login", level=1)
add_para(doc, "Before using any pharmacy feature you must log in with your assigned username and password.", size=11)
doc.add_paragraph()

add_heading(doc, "Steps", level=2)
add_step(doc, 1, "Open browser", "Go to http://localhost:5180 (or the server address your IT gave you).")
add_step(doc, 2, "Enter credentials",
    "Type your Username (e.g. diagnostic) and Password.  Then click Sign In.")
add_step(doc, 3, "Open Pharmacy menu",
    "Click the Pharmacy button in the top navigation bar.  A dropdown shows three groups: "
    "Dispensary & Sales, Inventory & Stock, and Procurement & Suppliers.")

add_image_placeholder(doc, "Login screen – enter username & password, click Sign In")
add_image_placeholder(doc, "Pharmacy menu open – showing all three groups")

add_note(doc, "If you do not see the Pharmacy menu your account may not have the Pharmacy Operator role. Ask your Administrator.", "Warning")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 2 – MASTERS
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "2.  Pharmacy Masters (One-Time Setup)", level=1)
add_para(doc, "Masters must be set up before you can receive stock or sell medicines.  "
    "Navigate: Pharmacy menu → (any masters link) OR go to /pharmacy/masters.", size=11)
doc.add_paragraph()

subsections = [
    ("2.1  Add a Medicine", [
        ("Go to Pharmacy Masters", "Click the Medicines tab."),
        ("Fill Medicine Name", "Enter the brand name, e.g. Napa 500mg."),
        ("Select Dosage Type", "Choose Tablet, Syrup, Injection, etc."),
        ("Enter Pack Size", "Number of units per pack, e.g. 10 (strips of 10 tablets)."),
        ("Set Buy Rate & Sale Rate", "Purchase cost and counter sale price per unit."),
        ("Save", "Click Save Medicine.  The medicine now appears in the list below."),
    ]),
    ("2.2  Add a Supplier", [
        ("Go to Suppliers tab", "Click the Suppliers tab at the top of the page."),
        ("Fill Supplier Name", "Company name, address and contact number."),
        ("Save", "Click Save Supplier."),
    ]),
    ("2.3  Add a Store", [
        ("Go to Stores tab", "Click the Stores tab."),
        ("Enter Store Name", "E.g. 'Main Pharmacy' or 'ICU Sub-Store'."),
        ("Mark Active", "Tick the Active checkbox so it appears in drop-downs."),
        ("Save", "Click Save Store."),
    ]),
]

for title, steps in subsections:
    add_heading(doc, title, level=2)
    for i, (t, d) in enumerate(steps, 1):
        add_step(doc, i, t, d)
    add_image_placeholder(doc, f"Masters – {title}")
    doc.add_paragraph()

add_note(doc, "You must add at least one Medicine, one Supplier, and one Store before any purchasing or selling can be done.", "Warning")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 3 – PURCHASE REQUISITION
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "3.  Purchase Requisition", level=1)
add_para(doc, "The Pharmacy Operator creates a requisition to request medicines.  "
    "The Administrator then approves or rejects it.  "
    "Navigate: Pharmacy → Procurement & Suppliers → Purchase Requisition.", size=11)
doc.add_paragraph()

add_heading(doc, "3.1  Pharmacy Operator – Create a Requisition", level=2)
steps = [
    ("Open page", "Click Pharmacy menu → Purchase Requisition."),
    ("Select Store", "Choose which store needs the medicines (e.g. Main Pharmacy)."),
    ("Add Remarks (optional)", "Type a short note such as 'Monthly stock replenishment'."),
    ("Select a Medicine", "In the Medicine Items section pick a medicine from the drop-down."),
    ("Enter Quantity", "Type the number of units / packs needed."),
    ("Enter Estimated Rate (optional)", "Type the expected unit price."),
    ("Click + Add", "The item appears in the table below.  Repeat for more medicines."),
    ("Submit Requisition", "Click Submit Requisition.  Status becomes Pending."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Purchase Requisition – form with medicines added, before submit")
add_image_placeholder(doc, "Purchase Requisition – submitted, status = Pending")

doc.add_paragraph()
add_heading(doc, "3.2  Administrator – Approve or Reject", level=2)
steps = [
    ("Log in as Administrator", "Sign in with an admin account."),
    ("Open Purchase Requisition", "Pharmacy → Procurement & Suppliers → Purchase Requisition."),
    ("Find the requisition", "The list on the right shows all recent requisitions.  Click View next to the Pending one."),
    ("Review items", "Check the medicine list, quantities and estimated rates."),
    ("Add Review Note (optional)", "Type a comment in the Review Note field."),
    ("Approve or Reject", "Click ✓ Approve (green) or ✕ Reject (red).  Status updates immediately."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Purchase Requisition – Admin view with Approve / Reject buttons")
add_note(doc, "Only Administrator can approve or reject.  Pharmacy Operator can only create and view their own requisitions.")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 4 – RECEIVING CHALLAN
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "4.  Receiving Challan (Stock Inward)", level=1)
add_para(doc, "When medicines arrive from a supplier, record them here to update the stock balance.  "
    "Navigate: Pharmacy → Procurement & Suppliers → Receiving Challan (Stock In).", size=11)
doc.add_paragraph()

steps = [
    ("Select Supplier", "Choose the supplier who delivered the medicines."),
    ("Select Store", "Choose the destination store."),
    ("Enter Invoice No.", "Type the supplier's invoice / delivery challan number."),
    ("Enter Invoice Date", "Pick the date on the supplier's invoice."),
    ("Add medicine batch", "Pick Medicine, enter Batch Number, Expiry Date, Quantity, Buy Rate, Sale Rate."),
    ("Click + Add Item", "Item is added to the challan list.  Repeat for each medicine."),
    ("Review totals", "Check Discount and VAT fields if applicable."),
    ("Post Challan", "Click Post / Save.  Stock balance is updated automatically."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Receiving Challan – completed form before posting")
add_note(doc, "After posting you cannot edit the challan.  Stock increases immediately in the selected store.")
add_note(doc, "Batch number and expiry date are required for FEFO (First Expiry First Out) dispensing.", "Tip")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 5 – POS
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "5.  OPD Counter POS – Dispensing & Sales", level=1)
add_para(doc, "Sell medicines over the counter to walk-in or OPD patients.  "
    "Navigate: Pharmacy → Dispensary & Sales → OPD Counter POS.", size=11)
doc.add_paragraph()

steps = [
    ("Click + New Counter Sale", "Clears any previous draft."),
    ("Select Patient Category", "OPD / Walk-in, IPD, or Emergency."),
    ("Enter Patient Name", "Type the customer or patient name."),
    ("Enter Contact (optional)", "Mobile number for receipt SMS."),
    ("Scan or Search Medicine", "Scan a barcode OR type medicine name in the scan box.  Matching suggestions appear."),
    ("Select Medicine from list", "Click the medicine to add it to the cart."),
    ("Adjust Quantity", "Change the Qty field in the cart row if needed."),
    ("Add Discount (optional)", "Enter a discount amount or percentage."),
    ("Review total", "Check the final amount at the bottom."),
    ("Click Confirm Sale", "Payment is recorded.  A printable receipt appears."),
    ("Print Receipt", "Click Print or use Ctrl+P to print the cash memo."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "POS – cart with medicines, patient name entered")
add_image_placeholder(doc, "POS – receipt after confirming sale")
add_note(doc, "If a medicine shows Out of Stock you need to receive stock first (Section 4).", "Warning")
add_note(doc, "You can scan a prescription QR code (RX-XXXX) to auto-fill all prescribed medicines.", "Tip")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 6 – SALES RETURNS
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "6.  Sales Returns (Customer Refund)", level=1)
add_para(doc, "Process a refund when a customer returns unused medicines.  "
    "Navigate: Pharmacy → Dispensary & Sales → Sales Returns (Customer).", size=11)
doc.add_paragraph()

steps = [
    ("Search original sale", "Enter the original Cash Memo ID or patient name to find the sale."),
    ("Select the bill", "Click on the bill to load it."),
    ("Select items to return", "Tick the medicines being returned and enter the return quantity."),
    ("Confirm Return", "Click Process Return.  Stock is added back; refund amount is shown."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Sales Returns – selecting items to return")
add_note(doc, "Returned stock goes back to the same store/batch it was sold from.")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 7 – LIVE STOCK & EXPIRY
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "7.  Live Stock & Expiry Monitor", level=1)
add_para(doc, "See current stock balances across all stores and get alerts for medicines expiring soon.  "
    "Navigate: Pharmacy → Inventory & Stock → Live Stock & Expiry.", size=11)
doc.add_paragraph()

steps = [
    ("Open the page", "All medicines and their current stock are listed."),
    ("Filter by Store", "Use the store drop-down to see one store at a time."),
    ("Check Expiry column", "Medicines expiring within 90 days are highlighted in orange or red."),
    ("Export", "Click Export to download the list as Excel if needed."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Live Stock – expiry alerts highlighted in red/orange")
add_note(doc, "Review this page every week to catch near-expiry items before they cannot be sold.")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 8 – INTER-STORE TRANSFER
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "8.  Inter-Store Transfer", level=1)
add_para(doc, "Move medicines from the main pharmacy to a sub-store (e.g. ICU, OT).  "
    "Navigate: Pharmacy → Inventory & Stock → Inter-Store Transfer.", size=11)
doc.add_paragraph()

steps = [
    ("Select Source Store", "The store that will send stock (e.g. Main Pharmacy)."),
    ("Select Destination Store", "The store that will receive stock (e.g. ICU Sub-Store)."),
    ("Add medicines", "Pick medicine, batch and quantity to transfer."),
    ("Post Transfer", "Click Post.  Stock moves instantly between the two stores."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Inter-Store Transfer – source → destination")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 9 – ISSUE TO WARD
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "9.  Issue to Ward / Department", level=1)
add_para(doc, "Dispense medicines to OT, ICU or Emergency without a cash sale.  "
    "Navigate: Pharmacy → Inventory & Stock → Issue to Ward / Dept.", size=11)
doc.add_paragraph()

steps = [
    ("Select Ward / Dept", "Choose OT, ICU, Emergency, etc."),
    ("Select Store", "Choose the dispensing store."),
    ("Add medicines", "Pick medicine, batch and quantity to issue."),
    ("Confirm Issue", "Click Issue.  Stock is deducted from the store."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Issue to Ward – form completed")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 10 – STOCK ADJUSTMENT
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "10.  Stock Adjustment & Audit", level=1)
add_para(doc, "Correct stock counts after a physical audit or write off damaged / expired items.  "
    "Navigate: Pharmacy → Inventory & Stock → Stock Adjustment & Audit.", size=11)
doc.add_paragraph()

steps = [
    ("Select Store", "Choose the store being audited."),
    ("Find the medicine", "Search or scroll to the medicine needing correction."),
    ("Enter physical count", "Type the actual counted quantity."),
    ("Select Reason", "Choose Damage, Expiry, Count Correction, etc."),
    ("Save Adjustment", "Click Save.  Stock updates to the physical count."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Stock Adjustment – physical count entry")
add_note(doc, "All adjustments are logged with user name and timestamp for audit purposes.")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 11 – SUPPLIER PAYMENT
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "11.  Supplier Payment", level=1)
add_para(doc, "Record payments made to medicine suppliers.  "
    "Navigate: Pharmacy → Procurement & Suppliers → Supplier Payment.", size=11)
doc.add_paragraph()

steps = [
    ("Select Supplier", "Choose the supplier you are paying."),
    ("Enter Amount", "Type the payment amount in Taka."),
    ("Select Payment Mode", "Cash, Bank Transfer, Cheque, etc."),
    ("Enter Date", "Pick the payment date."),
    ("Save Payment", "Click Save.  The supplier ledger is updated."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Supplier Payment – form")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  SECTION 12 – DOCTOR PRESCRIPTION
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "12.  Doctor Prescription", level=1)
add_para(doc, "A doctor creates a digital prescription.  "
    "The Pharmacy Operator can scan the QR code at the POS to auto-fill the medicines.  "
    "Navigate: Pharmacy → Dispensary & Sales → Doctor Prescription.", size=11)
doc.add_paragraph()

add_heading(doc, "Doctor – Create Prescription", level=2)
steps = [
    ("Search patient", "Enter patient registration number or name."),
    ("Add medicines", "Select each prescribed medicine, dose, frequency and duration."),
    ("Submit Prescription", "Click Submit.  A QR code is generated (e.g. RX-00042)."),
    ("Print or show QR", "Give the printed QR or code to the patient."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_heading(doc, "Pharmacy Operator – Dispense from Prescription", level=2)
steps = [
    ("Open POS", "Pharmacy → OPD Counter POS."),
    ("Scan QR code", "Scan the prescription QR or type RX-00042 in the scan box."),
    ("Review auto-filled cart", "All medicines from the prescription appear in the cart."),
    ("Confirm Sale", "Adjust quantities if needed then click Confirm Sale."),
]
for i, (t, d) in enumerate(steps, 1):
    add_step(doc, i, t, d)

add_image_placeholder(doc, "Prescription – QR code generated")
add_image_placeholder(doc, "POS – cart auto-filled from prescription QR scan")
doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  APPENDIX A – USER ROLES
# ═══════════════════════════════════════════════════════════════
doc.add_page_break()
add_heading(doc, "Appendix A – User Roles & Pharmacy Access", level=1)

table = doc.add_table(rows=1, cols=3)
table.style = "Table Grid"
hdr = table.rows[0]
hdr.cells[0].text = "Role"
hdr.cells[1].text = "What they can do in Pharmacy"
hdr.cells[2].text = "Cannot do"
for cell in hdr.cells:
    for run in cell.paragraphs[0].runs:
        run.font.bold = True
        run.font.size = Pt(10)
shade_row(hdr, "BDD7EE")

roles = [
    ("Administrator", "Everything – create, approve, reject, post, adjust", "—"),
    ("Pharmacy Operator",
     "Masters, Purchase Requisition (create), Receiving Challan, POS, Stock transfers, Prescription dispensing",
     "Approve/reject requisitions"),
    ("Cashier", "POS sales, Sales returns", "Stock management, purchasing"),
    ("Doctor", "Create prescriptions, view stock", "Purchasing, stock changes"),
]
for role, can, cannot in roles:
    row = table.add_row()
    row.cells[0].text = role
    row.cells[1].text = can
    row.cells[2].text = cannot
    for cell in row.cells:
        for run in cell.paragraphs[0].runs:
            run.font.size = Pt(10)

doc.add_paragraph()

# ═══════════════════════════════════════════════════════════════
#  APPENDIX B – COMMON ERRORS
# ═══════════════════════════════════════════════════════════════
add_heading(doc, "Appendix B – Common Errors & Solutions", level=1)

table2 = doc.add_table(rows=1, cols=3)
table2.style = "Table Grid"
hdr2 = table2.rows[0]
hdr2.cells[0].text = "Error / Problem"
hdr2.cells[1].text = "Cause"
hdr2.cells[2].text = "Solution"
for cell in hdr2.cells:
    for run in cell.paragraphs[0].runs:
        run.font.bold = True
        run.font.size = Pt(10)
shade_row(hdr2, "BDD7EE")

errors = [
    ("Medicine not in drop-down", "Not added in Masters", "Add the medicine in Pharmacy Masters (Section 2)"),
    ("Out of stock at POS", "Stock not received or fully sold", "Do a Receiving Challan (Section 4)"),
    ("Cannot post challan – no supplier", "Supplier not in Masters", "Add supplier in Pharmacy Masters (Section 2)"),
    ("Purchase Requisition – Approve button not visible", "Logged in as Pharmacy Operator not Administrator", "Log in with an Administrator account"),
    ("Medicines near expiry still showing in POS", "System uses FEFO – check batch", "Do a Stock Adjustment to write off expired batch (Section 10)"),
    ("'Select a store' validation error", "No active store set up", "Add and activate a store in Masters (Section 2.3)"),
]
for err, cause, sol in errors:
    row = table2.add_row()
    row.cells[0].text = err
    row.cells[1].text = cause
    row.cells[2].text = sol
    for cell in row.cells:
        for run in cell.paragraphs[0].runs:
            run.font.size = Pt(10)

doc.add_paragraph()

# ── Footer with page numbers ──────────────────────────────────
from docx.oxml.ns import nsmap
for sec in doc.sections:
    footer = sec.footer
    fp = footer.paragraphs[0]
    fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = fp.add_run("Polular Hospital  |  Pharmacy User Guide  |  Page ")
    set_font(r, size=9, color=(130, 130, 130))
    # page number field
    fldChar1 = OxmlElement('w:fldChar'); fldChar1.set(qn('w:fldCharType'), 'begin')
    instrText = OxmlElement('w:instrText'); instrText.set(qn('xml:space'), 'preserve'); instrText.text = 'PAGE'
    fldChar2 = OxmlElement('w:fldChar'); fldChar2.set(qn('w:fldCharType'), 'end')
    run_el = OxmlElement('w:r')
    run_el.append(fldChar1); run_el.append(instrText); run_el.append(fldChar2)
    fp._p.append(run_el)

# ── Save ──────────────────────────────────────────────────────
out = r"e:\HPM\Docs\Polular_Pharmacy_User_Guide_base.docx"
doc.save(out)
print(f"Saved: {out}")
