import os
import sys
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.dml.color import RGBColor
from pptx.enum.text import PP_ALIGN, MSO_ANCHOR
from pptx.enum.shapes import MSO_SHAPE

def create_presentation():
    prs = Presentation()
    prs.slide_width = Inches(13.333)
    prs.slide_height = Inches(7.5)
    blank_layout = prs.slide_layouts[6] # Blank slide

    # Color Palette
    BG_DARK = RGBColor(15, 23, 42)      # #0f172a
    BG_LIGHT = RGBColor(248, 250, 252)  # #f8fafc
    CARD_BG = RGBColor(255, 255, 255)   # #ffffff
    PRIMARY = RGBColor(2, 132, 199)     # #0284c7 (Medical Blue)
    PRIMARY_DARK = RGBColor(3, 105, 161)# #0369a1
    ACCENT = RGBColor(16, 185, 129)     # #10b981 (Emerald Green)
    WARNING = RGBColor(245, 158, 11)    # #f59e0b (Amber)
    TEXT_MAIN = RGBColor(15, 23, 42)    # #0f172a
    TEXT_MUTED = RGBColor(71, 85, 105)  # #475569
    TEXT_LIGHT = RGBColor(255, 255, 255)# #ffffff
    BORDER_COLOR = RGBColor(226, 232, 240) # #e2e8f0

    screenshot_dir = "d:/Shamim Sir/HPM-Web/manual_assets/screenshots"

    def add_header(slide, title, category, subtitle=""):
        # Top Header Bar
        header_box = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(1.1))
        header_box.fill.solid()
        header_box.fill.fore_color.rgb = BG_DARK
        header_box.line.color.rgb = PRIMARY
        header_box.line.width = Pt(0)

        # Bottom Accent Line on Header
        line = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(1.08), Inches(13.333), Inches(0.04))
        line.fill.solid()
        line.fill.fore_color.rgb = PRIMARY
        line.line.fill.background()

        # Category Pill
        cat_shape = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(0.2), Inches(2.2), Inches(0.32))
        cat_shape.fill.solid()
        cat_shape.fill.fore_color.rgb = PRIMARY
        cat_shape.line.fill.background()
        tf_cat = cat_shape.text_frame
        tf_cat.word_wrap = True
        p_cat = tf_cat.paragraphs[0]
        p_cat.text = category.upper()
        p_cat.font.size = Pt(10)
        p_cat.font.bold = True
        p_cat.font.color.rgb = TEXT_LIGHT
        p_cat.alignment = PP_ALIGN.CENTER

        # Title Text
        tb = slide.shapes.add_textbox(Inches(3.2), Inches(0.12), Inches(9.3), Inches(0.85))
        tf = tb.text_frame
        tf.word_wrap = True
        p = tf.paragraphs[0]
        p.text = title
        p.font.size = Pt(20)
        p.font.bold = True
        p.font.color.rgb = TEXT_LIGHT

        if subtitle:
            p2 = tf.add_paragraph()
            p2.text = subtitle
            p2.font.size = Pt(11)
            p2.font.color.rgb = RGBColor(148, 163, 184)

    def add_content_slide(title, category, subtitle, purpose, steps, tips, screenshot_filename, role="All Staff"):
        slide = prs.slides.add_slide(blank_layout)

        # Background
        bg = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(7.5))
        bg.fill.solid()
        bg.fill.fore_color.rgb = BG_LIGHT
        bg.line.fill.background()

        add_header(slide, title, category, subtitle)

        # Left Info Card
        left_card = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(1.3), Inches(4.9), Inches(5.8))
        left_card.fill.solid()
        left_card.fill.fore_color.rgb = CARD_BG
        left_card.line.color.rgb = BORDER_COLOR
        left_card.line.width = Pt(1)

        tf = left_card.text_frame
        tf.word_wrap = True
        tf.margin_left = Inches(0.25)
        tf.margin_right = Inches(0.25)
        tf.margin_top = Inches(0.25)
        tf.margin_bottom = Inches(0.25)

        # Role Badge
        p_role = tf.paragraphs[0]
        p_role.text = f"👤 দ্বায়িত্বপ্রাপ্ত রোল: {role}"
        p_role.font.size = Pt(11)
        p_role.font.bold = True
        p_role.font.color.rgb = PRIMARY_DARK
        p_role.space_after = Pt(8)

        # Purpose Section
        p_phead = tf.add_paragraph()
        p_phead.text = "🎯 মডিউলের উদ্দেশ্য ও কাজ:"
        p_phead.font.size = Pt(12)
        p_phead.font.bold = True
        p_phead.font.color.rgb = TEXT_MAIN
        p_phead.space_after = Pt(3)

        p_pbody = tf.add_paragraph()
        p_pbody.text = purpose
        p_pbody.font.size = Pt(10.5)
        p_pbody.font.color.rgb = TEXT_MUTED
        p_pbody.space_after = Pt(10)

        # Steps Section
        p_shead = tf.add_paragraph()
        p_shead.text = "📝 ধাপসমূহ (Step-by-Step Guide):"
        p_shead.font.size = Pt(12)
        p_shead.font.bold = True
        p_shead.font.color.rgb = TEXT_MAIN
        p_shead.space_after = Pt(4)

        for idx, step in enumerate(steps, 1):
            p_step = tf.add_paragraph()
            p_step.text = f"{idx}. {step}"
            p_step.font.size = Pt(10)
            p_step.font.color.rgb = TEXT_MUTED
            p_step.space_after = Pt(4)

        # Tips Section
        if tips:
            p_the = tf.add_paragraph()
            p_the.text = "💡 গুরুত্বপূর্ণ সতর্কতা ও টিপস:"
            p_the.font.size = Pt(11)
            p_the.font.bold = True
            p_the.font.color.rgb = ACCENT
            p_the.space_before = Pt(6)
            p_the.space_after = Pt(2)

            p_tbody = tf.add_paragraph()
            p_tbody.text = tips
            p_tbody.font.size = Pt(9.5)
            p_tbody.font.color.rgb = TEXT_MUTED

        # Right Screenshot Frame
        img_path = os.path.join(screenshot_dir, screenshot_filename)
        if os.path.exists(img_path):
            img_card = slide.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(5.9), Inches(1.3), Inches(6.6), Inches(5.8))
            img_card.fill.solid()
            img_card.fill.fore_color.rgb = CARD_BG
            img_card.line.color.rgb = BORDER_COLOR
            img_card.line.width = Pt(1)

            # Insert Picture
            slide.shapes.add_picture(img_path, Inches(6.0), Inches(1.4), Inches(6.4), Inches(5.6))

    # ==================== SLIDE 1: Title Slide ====================
    s1 = prs.slides.add_slide(blank_layout)
    bg1 = s1.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(7.5))
    bg1.fill.solid()
    bg1.fill.fore_color.rgb = BG_DARK
    bg1.line.fill.background()

    # Title Card
    tbox = s1.shapes.add_textbox(Inches(1.2), Inches(1.5), Inches(11), Inches(4.5))
    ttf = tbox.text_frame
    ttf.word_wrap = True

    p = ttf.paragraphs[0]
    p.text = "🏥 PrimeHPM Web"
    p.font.size = Pt(44)
    p.font.bold = True
    p.font.color.rgb = RGBColor(56, 189, 248) # Cyan Light
    p.space_after = Pt(10)

    p2 = ttf.add_paragraph()
    p2.text = "হাসপাতাল ও ফার্মেসি ম্যানেজমেন্ট সিস্টেম — পূর্ণাঙ্গ ইউজার ম্যানুয়াল"
    p2.font.size = Pt(24)
    p2.font.bold = True
    p2.font.color.rgb = TEXT_LIGHT
    p2.space_after = Pt(16)

    p3 = ttf.add_paragraph()
    p3.text = "Enterprise Hospital & Pharmacy Operations Manual with Live Screenshots & Step-by-Step Workflows"
    p3.font.size = Pt(14)
    p3.font.color.rgb = RGBColor(148, 163, 184)
    p3.space_after = Pt(30)

    p4 = ttf.add_paragraph()
    p4.text = "প্রযুক্তি: ASP.NET Core 8 Blazor Interactive Server | MS SQL Server | Entity Framework Core 8"
    p4.font.size = Pt(12)
    p4.font.color.rgb = ACCENT

    # ==================== SLIDE 2: Operational Roles Matrix ====================
    s2 = prs.slides.add_slide(blank_layout)
    bg2 = s2.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(7.5))
    bg2.fill.solid()
    bg2.fill.fore_color.rgb = BG_LIGHT
    bg2.line.fill.background()
    add_header(s2, "ব্যবহারকারী রোল ও অ্যাক্সেস কন্ট্রোল ম্যাট্রিক্স", "SYSTEM ROLES", "Role-Based Access Control (RBAC) Architecture")

    # 10 Role Cards in Grid
    roles_data = [
        ("Administrator", "সিস্টেমের পূর্ণ নিয়ন্ত্রণ, ইউজার তৈরি, রোল নির্ধারণ, মাস্টার ডাটা, অডিট লগ ও রিপোর্টস।", PRIMARY),
        ("Reception", "নতুন রোগী নিবন্ধন (PR), ডক্টর অ্যাপয়েন্টমেন্ট শিডিউলিং, ওপিডি টিকিট প্রদান ও ইনপেশেন্ট ভর্তি।", PRIMARY_DARK),
        ("Cashier", "ওপিডি টিকিট ফি, আইপিডি এডভান্স কালেকশন, ইমার্জেন্সি ও ওটি বিলিং, ডায়াগনস্টিক পেমেন্ট ও ভাউচার।", ACCENT),
        ("Emergency Operator", "জরুরি বিভাগে আসা রোগী এন্ট্রি, ইনস্ট্যান্ট প্রসিডিউর সার্ভিস ও ইমার্জেন্সি মেমো তৈরি।", WARNING),
        ("OT Operator", "অপারেশন থিয়েটার শিডিউলিং, সার্জন/অ্যানেস্থেসিয়া চার্জ এন্ট্রি ও ওটি ক্যাশ মেমো জেনারেশন।", PRIMARY),
        ("IPD Operator", "ইনপেশেন্ট রোগী ভর্তি, বেড অ্যালটমেন্ট, বেড ট্রান্সফার, ডেইলি সার্ভিস ও ডিসচার্জ প্রসেসিং।", PRIMARY_DARK),
        ("Nurse", "আইপিডি রোগীর দৈনিক নার্সিং সার্ভিস, ভাইটালস এন্ট্রি ও মেডিসিন অ্যাডমিনিস্ট্রেশন চার্ট।", ACCENT),
        ("Doctor", "প্রেসক্রিপশন রাইটিং, ওপিডি কনসালটেশন, আইপিডি রাউন্ড নোটস, মেডিকেল ডিসচার্জ ও ল্যাব রেজাল্ট ভিউ।", WARNING),
        ("Lab Technician", "ল্যাব স্যাম্পল কালেকশন, ওয়ার্কলিস্ট প্রসেসিং, টেস্ট রেজাল্ট এন্ট্রি ও নরমাল রেঞ্জ কম্প্যারিজন।", PRIMARY),
        ("Pharmacy Operator", "কাউন্টার পিওএস সেলস, ওষুধ ডিসপেনসিং, স্টক ইনওয়ার্ড (পারচেজ), মেয়াদোত্তীর্ণ ও স্টক অডিট।", PRIMARY_DARK),
    ]

    for i, (rname, rdesc, rcolor) in enumerate(roles_data):
        row = i // 5
        col = i % 5
        x = Inches(0.8 + col * 2.38)
        y = Inches(1.4 + row * 2.8)
        rcard = s2.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, x, y, Inches(2.25), Inches(2.6))
        rcard.fill.solid()
        rcard.fill.fore_color.rgb = CARD_BG
        rcard.line.color.rgb = rcolor
        rcard.line.width = Pt(1.5)

        rtf = rcard.text_frame
        rtf.word_wrap = True
        rtf.margin_left = Inches(0.15)
        rtf.margin_right = Inches(0.15)
        rtf.margin_top = Inches(0.15)

        rp1 = rtf.paragraphs[0]
        rp1.text = rname
        rp1.font.size = Pt(12)
        rp1.font.bold = True
        rp1.font.color.rgb = rcolor
        rp1.space_after = Pt(6)

        rp2 = rtf.add_paragraph()
        rp2.text = rdesc
        rp2.font.size = Pt(9.5)
        rp2.font.color.rgb = TEXT_MUTED

    # ==================== SLIDE 3: System Login ====================
    add_content_slide(
        title="সিস্টেম লগইন ও পাসওয়ার্ড নিরাপত্তা",
        category="AUTHENTICATION",
        subtitle="Secure User Authentication & Session Management",
        purpose="হাসপাতালের প্রত্যেক কর্মকর্তা/কর্মচারী তার নির্ধারিত ইউজার আইডি ও পাসওয়ার্ড দিয়ে সিস্টেমে লগইন করবেন।",
        steps=[
            "ব্রাউজারে http://localhost:5180/login লিংকে প্রবেশ করুন।",
            "ইউজারনেম এবং গোপন পাসওয়ার্ড সঠিকভাবে টাইপ করুন।",
            "Sign In বাটনে ক্লিক করুন।",
            "প্রথমবার লগইনে স্বয়ংক্রিয়ভাবে পাসওয়ার্ড চেঞ্জ পেজ আসবে; নতুন শক্তিশালী পাসওয়ার্ড সেট করে নিন।"
        ],
        tips="নিরাপত্তার স্বার্থে কাজ শেষে অবশ্যই উপরের ডান পাশের প্রোফাইল আইকন থেকে Logout করুন।",
        screenshot_filename="01_login_screen.png",
        role="All Hospital Staff"
    )

    # ==================== SLIDE 4: Dashboard ====================
    add_content_slide(
        title="রিয়েল-টাইম এক্সিকিউটিভ ড্যাশবোর্ড",
        category="OVERVIEW",
        subtitle="Live Hospital Operations & Revenue Intelligence",
        purpose="হাসপাতালের সামগ্রিক অবস্থা এক নজরে পর্যবেক্ষণ ও দ্রুত নেভিগেশন সুবিধা প্রদান করে।",
        steps=[
            "বর্তমানে ভর্তি থাকা মোট রোগী (Admitted Patients) সংখ্যা দেখা।",
            "লাইভ বেড অকুপেন্সি পারসেন্টেজ (%) মনিটর করা।",
            "আজকের ওপিডি টিকিট সংখ্যা এবং আজকের মোট ক্যাশ কালেকশন দেখা।",
            "কুইক শর্টকাট কার্ডে ক্লিক করে সরাসরি প্রয়োজনীয় মডিউলে প্রবেশ করা।"
        ],
        tips="ড্যাশবোর্ডের ডাটা লাইভ আপডেট হয়, ফলে ব্যবস্থাপনা কর্তৃপক্ষ তাৎক্ষণিক সিদ্ধান্ত নিতে পারেন।",
        screenshot_filename="02_dashboard_overview.png",
        role="Administrator, Management, Superadmin"
    )

    # ==================== SLIDE 5: Patient Registration ====================
    add_content_slide(
        title="পেশেন্ট রেজিস্ট্রেশন (Patient Registration - PR)",
        category="OPD / RECEPTION",
        subtitle="Master Patient Index & Unique Medical Record",
        purpose="হাসপাতালে আগত প্রত্যেক নতুন রোগীর জন্য আজীবন স্থায়ী একটি ইউনিক রেজিস্ট্রেশন আইডি (MRN) তৈরি করা।",
        steps=[
            "+ New Registration বাটনে ক্লিক করুন।",
            "রোগীর নাম, মোবাইল নম্বর, বয়স, জেন্ডার ও রক্তের গ্রুপ ইনপুট দিন।",
            "বর্তমান ঠিকানা ও অভিভাবকের মোবাইল নম্বর যুক্ত করুন।",
            "Save বাটনে ক্লিক করলে স্বয়ংক্রিয়ভাবে REG-2026-XXXX আইডি তৈরি হবে।"
        ],
        tips="মোবাইল নম্বর দিয়ে পূর্বে নিবন্ধিত রোগী দ্রুত সার্চ করে পুনরাবৃত্তি এড়ানো যায়।",
        screenshot_filename="03_patient_registration.png",
        role="Reception, OPD Operator"
    )

    # ==================== SLIDE 6: Doctor Appointments ====================
    add_content_slide(
        title="ডক্টর অ্যাপয়েন্টমেন্ট ও সিরিয়াল বুকিং (AP)",
        category="OPD / SCHEDULING",
        subtitle="Consultant Appointment Scheduling & Queue Booking",
        purpose="বিশেষজ্ঞ ডাক্তারদের শিডিউল অনুযায়ী আউটডোর রোগীদের অগ্রিম বা তাৎক্ষণিক সিরিয়াল বুকিং।",
        steps=[
            "ক্লিনিক্যাল ডিপার্টমেন্ট ও কনসালট্যান্ট চিকিৎসক সিলেক্ট করুন।",
            "অ্যাপয়েন্টমেন্টের তারিখ নির্বাচন করলে ওই দিনের অবশিষ্ট স্লট দেখা যাবে।",
            "রোগীর রেজিস্ট্রেশন আইডি সার্চ করে সিরিয়াল কনফার্ম করুন।",
            "রোগীকে অ্যাপয়েন্টমেন্ট কনফার্মেশন ও সিরিয়াল নম্বর স্লিপ প্রদান করুন।"
        ],
        tips="একই সময়ে ডক্টরের সর্বোচ্চ রোগী দেখার ক্যাপাসিটি স্বয়ংক্রিয়ভাবে নিয়ন্ত্রণ করা হয়।",
        screenshot_filename="04_doctor_appointments.png",
        role="Reception, Call Center"
    )

    # ==================== SLIDE 7: Consultation Tickets ====================
    add_content_slide(
        title="কনসালটেশন টিকিট ও ফি কালেকশন (TK)",
        category="OPD BILLING",
        subtitle="OPD Consultation Fee Collection & Token Slip",
        purpose="আউটডোরে ডাক্তার দেখানোর জন্য টিকিট ইস্যু ও কনসালটেশন ফি গ্রহণ করা।",
        steps=[
            "রোগীর রেজিস্ট্রেশন নম্বর বা মোবাইল দিয়ে সার্চ করুন।",
            "ডক্টর ও টিকিট ক্যাটাগরি (General / Specialist / Follow-up) সিলেক্ট করুন।",
            "সিস্টেমে ডক্টরের নির্ধারিত ফি প্রদর্শিত হলে ক্যাশ গ্রহণ করুন।",
            "Generate & Print Ticket চাপলে প্রিন্টারে বারকোডযুক্ত টিকিট বের হবে।"
        ],
        tips="ফলো-আপ রোগীদের ক্ষেত্রে নির্দিষ্ট দিনের মধ্যে স্বয়ংক্রিয়ভাবে ডিসকাউন্টেড রেট কার্যকর হয়।",
        screenshot_filename="05_consultation_tickets.png",
        role="Cashier, Reception"
    )

    # ==================== SLIDE 8: Doctor Consultation Queue ====================
    add_content_slide(
        title="ডক্টর কনসালটেশন কিউ ও প্রেসক্রিপশন (Doctor Queue)",
        category="CLINICAL OPD",
        subtitle="Doctor EMR Screen, Patient Queue & Clinical Diagnosis",
        purpose="ডাক্তারের চেম্বারে অপেক্ষমাণ রোগীদের ক্রমানুসারে ডাকা, হিস্ট্রি ভিউ ও ক্লিনিক্যাল পরামর্শ প্রদান।",
        steps=[
            "ডাক্তার তার ব্যক্তিগত অ্যাকাউন্টে লগইন করে আজকের কিউ লিস্ট দেখবেন।",
            "রোগীকে চেম্বারে ডাকার জন্য 'Call Next Patient' চাপুন।",
            "রোগীর পূর্ববর্তী ভিজিট, ল্যাব রিপোর্ট ও হিস্ট্রি স্ক্রিনে পর্যালোচনা করুন।",
            "প্রেসক্রিপশন লিখে কনসালটেশন সম্পন্ন (Completed) মার্ক করুন।"
        ],
        tips="ফার্মেসি ও ল্যাব মডিউল এই প্রেসক্রিপশন সরাসরি দেখতে পায়, তাই ভুল হওয়ার সুযোগ নেই।",
        screenshot_filename="06_doctor_queue.png",
        role="Doctor, Medical Officer"
    )

    # ==================== SLIDE 9: Emergency Services ====================
    add_content_slide(
        title="জরুরি বিভাগ ও ইনস্ট্যান্ট প্রসিডিউর (EM)",
        category="EMERGENCY CARE",
        subtitle="Instant Emergency Care Billing & Procedure Tracking",
        purpose="জরুরি বিভাগে আসা ক্রিটিক্যাল রোগীদের তাৎক্ষণিক চিকিৎসা, ড্রেসিং, অক্সিজেন, নেবুলাইজেশন ইত্যাদির দ্রুত বিলিং।",
        steps=[
            "জরুরি রোগীর নাম বা রেজি নং দিয়ে নতুন মেমো খুলুন।",
            "প্রদত্ত সার্ভিসেস (যেমন: Dressing, Nebulization, ECG, IV Cannula) সিলেক্ট করুন।",
            "জরুরি মেডিক্যাল ড্রাগস ও কনজ্যুমাবেল যুক্ত করুন।",
            "টাকা রিসিভ করে ইমার্জেন্সি ক্যাশ মেমো প্রিন্ট দিন।"
        ],
        tips="জরুরি অবস্থায় রোগী তাৎক্ষণিক ভর্তি প্রয়োজন হলে এখান থেকেই IPD Admission-এ রেফার করা যায়।",
        screenshot_filename="07_emergency_services.png",
        role="Emergency Operator, Cashier"
    )

    # ==================== SLIDE 10: OT Cash Memos ====================
    add_content_slide(
        title="ওটি সার্জারি ও ক্যাশ মেমো (OT Cash Memo)",
        category="OPERATION THEATRE",
        subtitle="OT Surgery, Surgeon Fee, Anesthesia & OT Drugs Billing",
        purpose="অপারেশন থিয়েটারের সার্জারি প্যাকেজ, সার্জন ফি, অ্যানেস্থেশিয়া ফি ও ওটি ড্রাগসের সমন্বিত মেমো প্রস্তুত।",
        steps=[
            "অপারেশন থিয়েটার শিডিউল ও রোগীর ইনফো সিলেক্ট করুন।",
            "সার্জারির ধরন, প্রধান সার্জন ও অ্যানেস্থেসিওলজিস্ট নির্বাচন করুন।",
            "ওটি চার্জ ও ব্যবহৃত বিশেষ সার্জিক্যাল সামগ্রী যুক্ত করুন।",
            "মেমো সেভ করে প্রিন্ট ও একাউন্টসে পোস্ট করুন।"
        ],
        tips="সার্জন ও অ্যানেস্থেশিয়া ফি আলাদা লেজারে স্বয়ংক্রিয়ভাবে ক্রেডিট হয়ে থাকে।",
        screenshot_filename="08_ot_cash_memos.png",
        role="OT Operator, Cashier"
    )

    # ==================== SLIDE 11: Bed Occupancy Board ====================
    add_content_slide(
        title="লাইভ বেড অকুপেন্সি বোর্ড (Bed Occupancy Board - BO)",
        category="IPD MANAGEMENT",
        subtitle="Visual Bed Status: Vacant, Occupied & Maintenance",
        purpose="হাসপাতালের সমস্ত ওয়ার্ড, আইসিইউ, সিসিইউ ও কেবিনের লাইভ অকুপেন্সি স্ট্যাটাস কালার-কোডেড দেখা।",
        steps=[
            "সবুজ (Green): খালি বেড — নতুন রোগীর জন্য প্রস্তুত।",
            "লাল/নীল (Red/Blue): অকুপাইড বেড — রোগীর নাম ও ভর্তির তারিখসহ বিস্তারিত।",
            "হলুদ (Yellow): ডিসচার্জ পরবর্তী স্যানিটাইজেশন ও ক্লিনিং চলছে।",
            "যেকোনো বেডে ক্লিক করে রোগীর ফাইল বা নতুন এডমিশন ফর্ম খোলা যায়।"
        ],
        tips="রিয়েল-টাইম সিগন্যালআর আপডেটের কারণে কোনো পেজ রিফ্রেশ ছাড়াই বেড স্ট্যাটাস আপডেট হয়।",
        screenshot_filename="09_ipd_bed_occupancy.png",
        role="IPD Operator, Reception, Nurse, Management"
    )

    # ==================== SLIDE 12: IPD Admissions ====================
    add_content_slide(
        title="ইনপেশেন্ট রোগী ভর্তি ও বেড বরাদ্দ (IPD Admissions - AD)",
        category="INPATIENT CARE",
        subtitle="IPD Patient Admission, Ward/Cabin Allocation & Guardian Details",
        purpose="হাসপাতালে রোগী ইনডোরে ভর্তি করা এবং নির্দিষ্ট বেড ও কেয়ার টিম অ্যাসাইন করা।",
        steps=[
            "রোগীর রেজিস্ট্রেশন আইডি দিয়ে সার্চ করুন।",
            "অ্যাডমিটিং কনসালট্যান্ট ও রেফারেল ডক্টর নির্বাচন করুন।",
            "ওয়ার্ড/কেবিন ক্যাটাগরি এবং খালি থাকা নির্দিষ্ট বেড সিলেক্ট করুন।",
            "রোগের ডায়াগনোসিস ও অভিভাবকের তথ্য প্রদান করে 'Admit Patient' কনফার্ম করুন।"
        ],
        tips="ভর্তির সাথে সাথে একটি ইউনিক IPD Tracking No জেনারেট হয় এবং বেডটি Occupied হয়ে যায়।",
        screenshot_filename="10_ipd_admissions.png",
        role="IPD Operator, Reception"
    )

    # ==================== SLIDE 13: IPD Advance Receipts ====================
    add_content_slide(
        title="আইপিডি অগ্রিম জমা ও মানি রিসিট (Advance Receipts - AR)",
        category="IPD FINANCE",
        subtitle="Inpatient Running Bill Advance Deposit & Ledger Update",
        purpose="চিকিৎসাধীন রোগীর চলমান চিকিৎসা খরচের বিপরীতে অগ্রিম টাকা জমা নেওয়া ও স্লিপ প্রদান।",
        steps=[
            "রোগীর IPD No বা বেড নম্বর দিয়ে সার্চ করুন।",
            "জমার পরিমাণ (Amount) এবং পেমেন্ট মেথড (Cash / Card / MFS) সিলেক্ট করুন।",
            "টাকা গ্রহণ করে 'Save & Print Advance Receipt' চাপুন।",
            "সিস্টেমে রোগীর মোট জমা ও বকেয়া স্ট্যাটাস সাথে সাথে আপডেট হবে।"
        ],
        tips="ফাইনাল ডিসচার্জের সময় এই সবকটি এডভান্স রিসিট স্বয়ংক্রিয়ভাবে মোট বিল থেকে মাইনাস হবে।",
        screenshot_filename="11_ipd_advance_receipts.png",
        role="Cashier, Accounts"
    )

    # ==================== SLIDE 14: Daily Patient Services ====================
    add_content_slide(
        title="আইপিডি দৈনিক সার্ভিস ও নার্সিং কেয়ার (Daily Services - DS)",
        category="NURSING & CLINICAL",
        subtitle="Daily Doctor Round Fees, Special Nursing & Procedure Posting",
        purpose="ভর্তি থাকা রোগীর দৈনিক ডক্টর ভিজিট ফি, নার্সিং সার্ভিস ও অতিরিক্ত টেস্টের বিল এন্ট্রি করা।",
        steps=[
            "রোগীর আইপিডি ফাইল সিলেক্ট করুন।",
            "তারিখ অনুযায়ী প্রদত্ত সার্ভিস (যেমন: Consultant Round, RMO Visit, Physiotherapy) যোগ করুন।",
            "পরিমাণ (Quantity) ও রেট কনফার্ম করুন।",
            "পোস্ট করলে তা রোগীর রানিং বিলে যুক্ত হয়ে যাবে।"
        ],
        tips="প্রতিদিনের চার্জ প্রতিদিন পোস্ট করলে ডিসচার্জের সময় কোনো চার্জ বাদ পড়ার ঝুঁকি থাকে না।",
        screenshot_filename="12_ipd_patient_services.png",
        role="Nurse, IPD Billing Operator"
    )

    # ==================== SLIDE 15: Medical Discharge Orders ====================
    add_content_slide(
        title="মেডিকেল ডিসচার্জ ক্লিয়ারেন্স অর্ডার",
        category="CLINICAL DISCHARGE",
        subtitle="Physician Discharge Advice, Notes & Medical Clearance",
        purpose="চিকিৎসক কর্তৃক রোগীর সুস্থতা সাপেক্ষে আনুষ্ঠানিকভাবে মেডিকেল ডিসচার্জ অর্ডার অনুমোদন।",
        steps=[
            "ডাক্তার তার আইপিডি রাউন্ড লিস্ট থেকে সুস্থ রোগীকে নির্বাচন করবেন।",
            "ডিসচার্জ সামারি, ফলো-আপ পরামর্শ ও ওষুধ নির্দেশিকা লিখবেন।",
            "'Approve Medical Discharge' চাপলে বিলিং কাউন্টারে ক্লিয়ারেন্স নোটিফিকেশন যাবে।",
            "মেডিকেল ডিসচার্জ ছাড়া ক্যাশিয়ার ফাইনাল বিল ক্লোজ করতে পারবে না।"
        ],
        tips="এটি হাসপাতালের ক্লিনিক্যাল ও ফিন্যান্সিয়াল ডিসচার্জের মধ্যে নিখুঁত সমন্বয় বজায় রাখে।",
        screenshot_filename="13_ipd_medical_orders.png",
        role="Doctor, Consultant, Medical Officer"
    )

    # ==================== SLIDE 16: Bed Turnover & Cleaning ====================
    add_content_slide(
        title="বেড টার্নওভার ও স্যানিটাইজেশন (Bed Turnover)",
        category="HOUSEKEEPING",
        subtitle="Post-Discharge Bed Cleaning, Disinfection & Ready Status",
        purpose="রোগী ডিসচার্জের পর বেড স্যানিটাইজেশন, ক্লিনিং ও পরবর্তী ভর্তির জন্য প্রস্তুতকরণ।",
        steps=[
            "ডিসচার্জ হওয়া বেডটি স্বয়ংক্রিয়ভাবে 'Cleaning Pending' তালিকায় আসে।",
            "হাউসকিপিং স্টাফ বেড পরিষ্কার ও লিনেন পরিবর্তন সম্পন্ন করবেন।",
            "'Mark as Sanitized & Ready' বাটনে ক্লিক করবেন।",
            "বেড অকুপেন্সি বোর্ডে বেডটি তাৎক্ষণিক 'Green / Vacant' হয়ে যাবে।"
        ],
        tips="সংক্রমণ প্রতিরোধ এবং হাসপাতালের হাইজিন স্ট্যান্ডার্ড নিশ্চিত করতে এটি অত্যন্ত কার্যকর।",
        screenshot_filename="14_ipd_bed_turnover.png",
        role="Housekeeping Supervisor, Ward In-charge"
    )

    # ==================== SLIDE 17: IPD Final Discharge Billing ====================
    add_content_slide(
        title="আইপিডি ফাইনাল ডিসচার্জ ও বিল সেটেলমেন্ট (DC)",
        category="IPD BILLING",
        subtitle="Automated Bed Days, Net Advance Adjustment & Final Release Invoice",
        purpose="রোগীর অবস্থানকালের সম্পূর্ণ বিল সমন্বয়, পূর্বের অগ্রিম বাদ দেওয়া ও ফাইনাল মানি রিসিট প্রদান।",
        steps=[
            "রোগীর IPD No সিলেক্ট করুন।",
            "সিস্টেম স্বয়ংক্রিয়ভাবে বেড রেন্ট, ডক্টর ফি, সার্ভিস, ফার্মেসি বিল ও এডভান্স সমন্বয় করবে।",
            "অনুমোদিত কোনো ডিসকাউন্ট থাকলে তা ইনপুট দিন।",
            "বকেয়া টাকা আদায় করে 'Complete Settlement & Print Final Bill' চাপুন।"
        ],
        tips="ফাইনাল বিল সম্পন্ন হওয়ার সাথে সাথে রোগীর ফাইল ক্লোজ ও গেট পাস স্লিপ প্রিন্ট হয়।",
        screenshot_filename="15_ipd_discharge_billing.png",
        role="Cashier, Accounts In-charge"
    )

    # ==================== SLIDE 18: Diagnostic Billing ====================
    add_content_slide(
        title="ডায়াগনস্টিক টেস্ট বুকিং ও বিলিং (DX)",
        category="DIAGNOSTIC & LAB",
        subtitle="Lab Test Selection, Barcode Token & Invoice Generation",
        purpose="প্রেসক্রাইব করা প্যাথলজি ও রেডিওলজি টেস্ট বুকিং ও ফি কালেকশন স্লিপ তৈরি।",
        steps=[
            "রোগীর রেজি নং সার্চ করুন বা নতুন এন্ট্রি দিন।",
            "ডায়াগনস্টিক টেস্টসমূহ (যেমন: CBC, Lipid Profile, X-Ray, USG) সিলেক্ট করুন।",
            "রেফারেল ডক্টর নির্বাচন করুন।",
            "টাকা গ্রহণ করে ইনভয়েস ও স্যাম্পল বারকোড স্লিপ রোগীকে দিন।"
        ],
        tips="ইনভয়েসের ওপর একটি ইউনিক বারকোড থাকে যা ল্যাব টেকনিশিয়ান স্ক্যান করতে পারেন।",
        screenshot_filename="16_diagnostic_billing.png",
        role="Cashier, Diagnostic Reception"
    )

    # ==================== SLIDE 19: Diagnostic Worklist ====================
    add_content_slide(
        title="ল্যাব স্যাম্পল কালেকশন ওয়ার্কলিস্ট (LW)",
        category="LABORATORY",
        subtitle="Phlebotomy Sample Collection, Barcode Scanning & Lab Queuing",
        purpose="ল্যাবরেটরিতে রক্তের নমুনা বা স্যাম্পল গ্রহণ ও প্রসেসিং স্ট্যাটাস পরিচালনা।",
        steps=[
            "রোগীর মানি রিসিট বা বারকোড স্ক্যান করুন।",
            "স্যাম্পল গ্রহণ করে 'Sample Collected' বাটনে ক্লিক করুন।",
            "টেস্টিং শুরু হলে স্ট্যাটাস 'In-Process' আপডেট করুন।",
            "জরুরি (Emergency/Stat) স্যাম্পলগুলোকে ওয়ার্কলিস্টে অগ্রাধিকার দেওয়া হয়।"
        ],
        tips="স্যাম্পল ট্র্যাকিংয়ের ফলে কোনো টেস্ট ড্রপ হওয়া বা নমুনা হারানোর ঝুঁকি থাকে না।",
        screenshot_filename="17_diagnostic_worklist.png",
        role="Lab Technician, Phlebotomist"
    )

    # ==================== SLIDE 20: Diagnostic Results ====================
    add_content_slide(
        title="ল্যাব রেজাল্ট এন্ট্রি ও ভেরিফিকেশন (LR)",
        category="PATHOLOGY REPORT",
        subtitle="Parameter Observed Values, Normal Range Alerts & QR Verified Report",
        purpose="টেস্টের প্রাপ্ত ফলাফল এন্ট্রি, রেফারেন্স রেঞ্জ কম্প্যারিজন ও প্যাথলজিস্ট ভেরিফিকেশন।",
        steps=[
            "ওয়ার্কলিস্ট থেকে কমপ্লিট হওয়া টেস্ট সিলেক্ট করুন।",
            "প্রতিটি প্যারামিটারের প্রাপ্ত মান (Observed Value) ইনপুট দিন।",
            "রেফারেন্স সীমার বাইরে থাকলে সিস্টেম স্বয়ংক্রিয় লাল (High/Low) এলার্ট দেবে।",
            "কনসালট্যান্ট প্যাথলজিস্ট রিপোর্ট ভেরিফাই করলে প্রিন্ট ও অনলাইন কিউআর কোড অ্যাক্টিভ হবে।"
        ],
        tips="রিপোর্টে থাকা সিকিউর কিউআর কোড স্ক্যান করে রোগী যেকোনো স্থান থেকে অনলাইন রিপোর্ট দেখতে পারেন।",
        screenshot_filename="18_diagnostic_results.png",
        role="Lab Technician, Pathologist, Doctor"
    )

    # ==================== SLIDE 21: Pharmacy POS Sales ====================
    add_content_slide(
        title="ফার্মেসি কাউন্টার পিওএস সেলস (Pharmacy POS)",
        category="PHARMACY POS",
        subtitle="Fast Barcode/Generic Drug Search, FIFO Batch Allocation & Instant Memo",
        purpose="আউটডোর ও ইনডোর রোগীদের কাছে দ্রুত ওষুধ বিক্রয় ও ক্যাশ মেমো প্রিন্ট।",
        steps=[
            "বারকোড স্ক্যানার দিয়ে ড্রাগ স্ক্যান করুন অথবা নাম দিয়ে সার্চ করুন।",
            "সিস্টেম স্বয়ংক্রিয়ভাবে প্রথম মেয়াদ শেষ হতে যাওয়া ব্যাচ (FIFO) সিলেক্ট করবে।",
            "প্রেসক্রিপশন আইডি দিলে সম্পূর্ণ প্রেসক্রাইবড ওষুধ এক ক্লিকে কার্টে যোগ হয়।",
            "ক্যাশ/কার্ডে বিল কালেকশন করে ক্যাশ মেমো প্রিন্ট দিন।"
        ],
        tips="মেয়াদোত্তীর্ণ কোনো ওষুধ সিস্টেমে বিক্রয় করা সম্পূর্ণ নিষিদ্ধ ও ব্লক করা থাকে।",
        screenshot_filename="19_pharmacy_pos.png",
        role="Pharmacy Operator, Pharmacist"
    )

    # ==================== SLIDE 22: Pharmacy Purchases ====================
    add_content_slide(
        title="স্টক ইনওয়ার্ড ও পারচেজ চালান (Stock Inward - PI)",
        category="PHARMACY INVENTORY",
        subtitle="Supplier Purchase Challan, Batch Tracking, Purchase Rate & MRP",
        purpose="ফার্মাসিউটিক্যাল কোম্পানি থেকে নতুন ওষুধ ক্রয় করে স্টকে অন্তর্ভুক্ত করা।",
        steps=[
            "সাপ্লায়ার কোম্পানি ও ইনভয়েস/চালান নম্বর এন্ট্রি দিন।",
            "ওষুধের নাম, ব্যাচ নম্বর (Batch No) ও এক্সপায়ারি ডেট (Expiry Date) দিন।",
            "ক্রয়মূল্য (Purchase Rate), বিক্রয়মূল্য (MRP) ও পরিমাণ ইনপুট করুন।",
            "চালান কনফার্ম করলে স্বয়ংক্রিয়ভাবে স্টকে কোয়ান্টিটি যোগ হবে ও অ্যাকাউন্টসে পেয়াবল তৈরি হবে।"
        ],
        tips="পারচেজ রেটের চেয়ে বিক্রয়মূল্য কম দিলে সিস্টেম স্বয়ংক্রিয়ভাবে সতর্ক করবে।",
        screenshot_filename="20_pharmacy_purchases.png",
        role="Pharmacy In-charge, Store Keeper"
    )

    # ==================== SLIDE 23: Pharmacy Stock & Expiry ====================
    add_content_slide(
        title="লাইভ মেডিসিন স্টক ও এক্সপায়ারি মনিটরিং (ST)",
        category="PHARMACY STOCK",
        subtitle="Live Stock Balance, Reorder Level Alerts & Expiry Risk Matrix",
        purpose="ফার্মেসির বর্তমান মজুদ, লো স্টক অ্যালার্ট ও আসন্ন মেয়াদোত্তীর্ণ ওষুধের পূর্বাভাস দেখা।",
        steps=[
            "জেনেরিক বা ব্র্যান্ড অনুযায়ী যেকোনো ওষুধের তাৎক্ষণিক স্টক সার্চ করুন।",
            "রি-অর্ডার লেভেলের নিচে নেমে যাওয়া ওষুধের তালিকা দেখে নতুন অর্ডারের সিদ্ধান্ত নিন।",
            "আগামী ৩০, ৬০ ও ৯০ দিনের মধ্যে এক্সপায়ার হতে যাওয়া ব্যাচ ফিল্টার করুন।",
            "কোম্পানিতে রিটার্নযোগ্য ওষুধের তালিকা এক্সেল/প্রিন্ট নিন।"
        ],
        tips="ফার্মেসির অপচয় ও আর্থিক ক্ষতি রোধে নিয়মিত এক্সপায়ারি রিপোর্ট মনিটর করা জরুরি।",
        screenshot_filename="21_pharmacy_stock.png",
        role="Pharmacist, Store In-charge"
    )

    # ==================== SLIDE 24: Pharmacy Stock Control ====================
    add_content_slide(
        title="স্টক কন্ট্রোল ও ফিজিক্যাল অডিট (Stock Control - SC)",
        category="STOCK AUDIT",
        subtitle="Physical Stock Count, Variance Analysis & Inventory Reconciliation",
        purpose="সফটওয়্যার মজুদের সাথে কাউন্টারের বাস্তব ফিজিক্যাল ওষুধের পরিমাণ মিলিয়ে ভ্যারিয়েন্স সমন্বয়।",
        steps=[
            "অডিট করার জন্য ড্রাগ বা র‍্যাক সিলেক্ট করুন।",
            "গণনাকৃত ফিজিক্যাল কোয়ান্টিটি ইনপুট দিন।",
            "সিস্টেম স্বয়ংক্রিয়ভাবে ঘাটতি বা বাড়তি (Variance) প্রদর্শন করবে।",
            "যৌক্তিক কারণ ও অনুমোদিত কর্তৃপক্ষের নোটে স্টক অ্যাডজাস্টমেন্ট পোস্ট করুন।"
        ],
        tips="প্রতি মাসের শেষে ফিজিক্যাল অডিট সম্পন্ন করে ইনভেন্টরি অডিট রিপোর্ট প্রিন্ট করুন।",
        screenshot_filename="22_pharmacy_stock_control.png",
        role="Store Auditor, Pharmacy Manager"
    )

    # ==================== SLIDE 25: Accounts & General Ledger ====================
    add_content_slide(
        title="ফিন্যান্স, অ্যাকাউন্টস ও ভাউচার পোস্টিং (Accounts)",
        category="FINANCE & ACCOUNTS",
        subtitle="Automated Sub-ledger Posting, Expense Vouchers & Daily Shift Closing",
        purpose="হাসপাতালের সমস্ত আর্থিক লেনদেন, বিবিধ ব্যয় ও ক্যাশিয়ার শিফট হ্যান্ডওভার পরিচালনা।",
        steps=[
            "ওপিডি, আইপিডি, ফার্মেসি ও ল্যাবের প্রতিটি বিলের জন্য রিয়েল-টাইম ডেবিট-ক্রেডিট দেখা।",
            "হাসপাতালের দৈনন্দিন খরচের জন্য Debit/Credit Voucher এন্ট্রি দিন।",
            "কাউন্টার ক্যাশিয়ার শিফট শেষে ক্যাশ ব্যালেন্স মিলিয়ে 'Shift Handover' সম্পন্ন করবেন।",
            "ফাইনান্সিয়াল লেজার ও ট্রায়াল ব্যালেন্স রিভিউ করুন।"
        ],
        tips="ডাবল-এন্ট্রি অ্যাকাউন্টিং নিয়মে প্রতিটি লেনদেন স্বয়ংক্রিয়ভাবে অডিটেবল থাকে।",
        screenshot_filename="25_accounts_ledger.png",
        role="Accountant, Finance Manager"
    )

    # ==================== SLIDE 26: Hospital Reports & Analytics ====================
    add_content_slide(
        title="অ্যাডভান্সড রিপোর্ট ও অ্যানালিটিক্স (Reports)",
        category="MANAGEMENT BI",
        subtitle="Daily Cash Summary, Department Revenue, Doctor Consultations & Dues",
        purpose="হাসপাতালের আয়-ব্যয়, রোগী পরিসংখ্যান ও পারফরম্যান্স পর্যালোচনার পূর্ণাঙ্গ রিপোর্টস।",
        steps=[
            "কাঙ্ক্ষিত তারিখ ও ফিল্টার রেঞ্জ নির্বাচন করুন।",
            "Daily Cash Collection Summary: কোন কাউন্টারে কত টাকা জমা হয়েছে তার স্টেটমেন্ট।",
            "Department Revenue Breakdown: ওপিডি, আইপিডি, প্যাথলজি ও ফার্মেসির আয়ের অনুপাত।",
            "ডক্টর কনসালটেশন ও রেফারেল রিপোর্ট এবং বকেয়া (Due Collection) তালিকা প্রিন্ট/এক্সপোর্ট করুন।"
        ],
        tips="রিপোর্টগুলো এক ক্লিকে প্রিন্ট বা এক্সেল শিটে এক্সপোর্ট করা যায়।",
        screenshot_filename="26_reports_analytics.png",
        role="Administrator, Management, Auditor"
    )

    # ==================== SLIDE 27: Master Configurations ====================
    add_content_slide(
        title="মাস্টার কনফিগারেশন ও ডাটা সেটআপ",
        category="SYSTEM CONFIG",
        subtitle="Doctors, Beds, Diagnostic Parameters & Drug Generic Masters",
        purpose="হাসপাতালের পরিষেবা, চিকিৎসক, বেড এবং ক্লিনিক্যাল প্যারামিটার সমূহের প্রাথমিক ও নিয়মিত সেটআপ।",
        steps=[
            "Doctors Master (/masters): নতুন কনসালট্যান্ট যুক্ত, ভিজিট ফি ও ডিপার্টমেন্ট নির্ধারণ।",
            "IPD Masters (/masters/ipd): নতুন ওয়ার্ড, কেবিন ক্যাটাগরি, বেড নম্বর ও দৈনিক ভাড়া সেটআপ।",
            "Diagnostic Masters (/diagnostic/masters): টেস্ট গ্রুপ, প্যারামিটার ইউনিট ও রেফারেন্স রেঞ্জ সেটআপ।",
            "Pharmacy Masters (/pharmacy/masters): নতুন মেডিসিন, ড্রাগ গ্রুপ ও ম্যানুফ্যাকচারার এন্ট্রি।"
        ],
        tips="যেকোনো নতুন সার্ভিস বা বেড যুক্ত করলে তা সাথে সাথে সংশ্লিষ্ট বিলিং মডিউলে কার্যকর হয়।",
        screenshot_filename="27_clinical_masters.png",
        role="System Administrator"
    )

    # ==================== SLIDE 28: User Admin & Permissions ====================
    add_content_slide(
        title="ইউজার অ্যাডমিন ও রোল পারমিশন (User Admin)",
        category="SECURITY & ACCESS",
        subtitle="Staff Account Provisioning, Role Assignments & Password Control",
        purpose="হাসপাতালের কর্মকর্তা-কর্মচারীদের জন্য ইউজার অ্যাকাউন্ট তৈরি ও কাজের পরিধি নিয়ন্ত্রণ।",
        steps=[
            "+ Create New User বাটনে ক্লিক করুন।",
            "স্টাফের ইউজারনেম, ডিসপ্লে নাম, ইমেইল ও প্রাথমিক পাসওয়ার্ড প্রদান করুন।",
            "১০টি নির্ধারিত রোলের মধ্য থেকে প্রযোজ্য এক বা একাধিক রোল সিলেক্ট করুন।",
            "যেকোনো ইউজারের স্ট্যাটাস সাময়িক নিষ্ক্রিয় (Deactivate) বা পাসওয়ার্ড রিসেট করুন।"
        ],
        tips="একজন স্টাফকে কেবল তার কাজের সাথে সংশ্লিষ্ট মডিউলের পারমিশন প্রদান করা নিরাপদ ও কাম্য।",
        screenshot_filename="31_user_admin.png",
        role="System Administrator"
    )

    # ==================== SLIDE 29: Real-time Audit Trail ====================
    add_content_slide(
        title="রিয়েল-টাইম সিস্টেম অডিট ট্রেইল (Audit Logs)",
        category="SECURITY AUDIT",
        subtitle="Forensic Activity Logging, Modification Tracing & IP Tracking",
        purpose="সিস্টেমের সর্বোচ্চ স্বচ্ছতা ও নিরাপত্তা নিশ্চিত করতে প্রতি সেকেন্ডের অ্যাকশন রেকর্ড করা।",
        steps=[
            "কে, কখন, কোন কম্পিউটার (IP Address) থেকে সিস্টেমে লগইন বা কাজ করেছেন তা দেখা।",
            "কোন ইনভয়েস বা ডাটা এন্ট্রি হয়েছে, এডিট হয়েছে বা ক্যান্সেল হয়েছে তার তালিকা।",
            "Old Values ও New Values এর তুলনামূলক পরিবর্তন দেখা।",
            "সন্দেহজনক যেকোনো কার্যকলাপ তাৎক্ষণিক সনাক্ত করা।"
        ],
        tips="অডিট লগ ডাটাবেজে স্থায়ীভাবে সংরক্ষিত থাকে এবং এটি কোনো সাধারণ ইউজার মুছে ফেলতে পারে না।",
        screenshot_filename="32_audit_logs.png",
        role="Administrator, Compliance Auditor"
    )

    # ==================== SLIDE 30: Hospital Settings & Branding ====================
    add_content_slide(
        title="হসপিটাল প্রোফাইল, লোগো ও প্রিন্ট হেডার সেটিংস",
        category="HOSPITAL BRANDING",
        subtitle="Hospital Name, Logo, Address, Hotlines & Custom Receipt Notes",
        purpose="মানি রিসিট, প্রেসক্রিপশন ও ল্যাব রিপোর্টের হেডার-ফুটার এবং ব্র্যান্ডিং কাস্টমাইজেশন।",
        steps=[
            "হাসপাতালের অফিসিয়াল নাম, ঠিকানা, সিটি ও লাইসেন্স নম্বর হালনাগাদ করুন।",
            "জরুরি হটলাইন নম্বর, হেল্পলাইন ও সাপোর্ট ইমেইল ইনপুট দিন।",
            "হাসপাতালের অফিশিয়াল লোগো আপলোড করুন।",
            "রিসিট ফুটারে কাস্টম পলিসি বা ধন্যবাদ নোট যুক্ত করে 'Save Hospital Profile' চাপুন।"
        ],
        tips="এখানে দেওয়া লোগো এবং নাম সমস্ত প্রিন্টেড রিসিট ও ল্যাব রিপোর্টে স্বয়ংক্রিয়ভাবে প্রতিফলিত হবে।",
        screenshot_filename="33_hospital_settings.png",
        role="Administrator"
    )

    # ==================== SLIDE 31: Operational Summary & Best Practices ====================
    s_last = prs.slides.add_slide(blank_layout)
    bg_last = s_last.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(7.5))
    bg_last.fill.solid()
    bg_last.fill.fore_color.rgb = BG_DARK
    bg_last.line.fill.background()
    add_header(s_last, "দৈনন্দিন কাজের সেরা অনুশীলন ও চেকলিস্ট (Best Practices)", "DAILY CHECKLIST", "Hospital Operational Workflow & Maintenance Checklist")

    # 3 Summary Cards
    cards_info = [
        ("🌅 সকালের শিফট শুরুর চেকলিস্ট", [
            "১. সুপারএডমিন/আইটি টিম ডাটাবেজ ব্যাকআপ ও সিস্টেম হেলথ চেক করবেন।",
            "২. ওপিডি কাউন্টারে প্রিন্টার ও রোল পেপার প্রস্তুত রাখা।",
            "৩. ডক্টর শিডিউল ও রোস্টার আপডেট আছে কি না নিশ্চিত করা।",
            "৪. ফার্মেসিতে রানিং কাউন্টার ক্যাশ ওপেন করা।"
        ], PRIMARY),
        ("☀️ সারাদিনের রুটিন অপারেশন", [
            "১. রোগীর সঠিক মোবাইল নম্বর দিয়ে রেজিস্ট্রেশন সার্চ/এন্ট্রি।",
            "২. আইপিডি পেশেন্টের দৈনিক নার্সিং ও ডক্টর সার্ভিস নিয়মিত পোস্টিং।",
            "৩. ল্যাব স্যাম্পল সংগ্রহের সাথে সাথে ওয়ার্কলিস্টে স্ট্যাটাস আপডেট।",
            "৪. ফার্মেসিতে সঠিক ব্যাচ ও বারকোড স্ক্যান করে ডিসপেনসিং।"
        ], ACCENT),
        ("🌙 রাতের শিফট ক্লোজিং ও হ্যান্ডওভার", [
            "১. প্রতিটি ক্যাশ কাউন্টারের ফিজিক্যাল ক্যাশ মিলিয়ে শিফট ক্লোজ।",
            "২. আইপিডি ডিসচার্জ বিলের এডভান্স ও ফাইনাল সেটেলমেন্ট মিলকরণ।",
            "৩. ডেইলি ক্যাশ কালেকশন সামারি রিপোর্ট প্রিন্ট ও সাইন করা।",
            "৪. ব্যবহারের পর সব কম্পিউটার থেকে সিকিউর লগআউট সম্পন্ন করা।"
        ], WARNING),
    ]

    for i, (chead, csteps, ccolor) in enumerate(cards_info):
        x = Inches(0.8 + i * 3.95)
        y = Inches(1.5)
        card = s_last.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, x, y, Inches(3.8), Inches(5.4))
        card.fill.solid()
        card.fill.fore_color.rgb = CARD_BG
        card.line.color.rgb = ccolor
        card.line.width = Pt(2)

        ctf = card.text_frame
        ctf.word_wrap = True
        ctf.margin_left = Inches(0.2)
        ctf.margin_right = Inches(0.2)
        ctf.margin_top = Inches(0.25)

        cp1 = ctf.paragraphs[0]
        cp1.text = chead
        cp1.font.size = Pt(13)
        cp1.font.bold = True
        cp1.font.color.rgb = ccolor
        cp1.space_after = Pt(14)

        for step in csteps:
            cp_step = ctf.add_paragraph()
            cp_step.text = step
            cp_step.font.size = Pt(10.5)
            cp_step.font.color.rgb = TEXT_MAIN
            cp_step.space_after = Pt(8)

    output_pptx = "d:/Shamim Sir/HPM-Web/PrimeHPM_Hospital_User_Manual.pptx"
    prs.save(output_pptx)
    print(f"Presentation saved successfully to: {output_pptx}")

if __name__ == "__main__":
    create_presentation()
