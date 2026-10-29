from docx import Document
from docx.shared import Inches
import os

SRC = r"e:\HPM\Docs\screenshots"
IN  = r"e:\HPM\Docs\Polular_Pharmacy_User_Guide_base.docx"
OUT = r"e:\HPM\Docs\Polular_Pharmacy_User_Guide.docx"

MAPPING = {
    "Login screen":                    "01_login_page.png",
    "Pharmacy menu":                   "03_pharmacy_menu_open.png",
    "Masters – 2.1":                   "05_masters_medicine_filled.png",
    "Masters – 2.2":                   "06_masters_suppliers.png",
    "Masters – 2.3":                   "07_masters_stores.png",
    "Purchase Requisition – form":     "08_purchase_req_empty.png",
    "Purchase Requisition – submitted":"10_purchase_req_pending.png",
    "Admin view with Approve":         "11b_purchase_req_approved.png",
    "Receiving Challan":               "12b_challan_item_filled.png",
    "POS – cart":                      "13c_pos_cart.png",
    "POS – receipt":                   "13c_pos_cart.png",
    "Sales Returns":                   "16_sales_returns.png",
    "Live Stock":                      "14_live_stock.png",
    "Inter-Store Transfer":            "14_live_stock.png",
    "Issue to Ward":                   "14_live_stock.png",
    "Stock Adjustment":                "15_stock_adjustment.png",
    "Supplier Payment":                "17_supplier_payment.png",
    "Prescription":                    "18_prescriptions.png",
    "POS – cart auto-filled":          "13c_pos_cart.png",
}

doc = Document(IN)
replaced = 0

for para in doc.paragraphs:
    text = para.text.strip()
    if not text.startswith("[ Screenshot:"):
        continue
    matched = None
    for key, fname in MAPPING.items():
        if key.lower() in text.lower():
            matched = fname
            break
    if not matched:
        print(f"  No mapping: {text[:70].encode('ascii','replace').decode()}")
        continue
    img = os.path.join(SRC, matched)
    if not os.path.exists(img):
        print(f"  Missing: {img}")
        continue
    para.clear()
    run = para.add_run()
    run.add_picture(img, width=Inches(5.8))
    para.alignment = 1
    replaced += 1
    print(f"  OK {matched}")

doc.save(OUT)
print(f"\nDone: {replaced} images inserted -> {OUT}")
