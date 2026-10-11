window.hpm = {
    focusElementById: function (id) {
        const el = document.getElementById(id);
        if (el) {
            el.focus();
            if (el.select && typeof el.select === 'function') {
                el.select();
            }
        }
    },

    confirm: function (message) {
        return window.confirm(message);
    },
    prompt: function (message) {
        return window.prompt(message, 'Entry correction');
    },
    clickElementById: function (id) {
        const el = document.getElementById(id);
        if (el && !el.disabled) el.click();
    },

    attachPatientShortcuts: function (dotNetRef) {
        this.detachPatientShortcuts();
        this._patientShortcutRef = dotNetRef;
        this._patientShortcutHandler = (event) => {
            let action = null;
            const key = String(event.key || '').toLowerCase();

            if ((event.ctrlKey || event.metaKey) && key === 's') action = 'save';
            else if ((event.ctrlKey || event.metaKey) && key === 'u') action = 'update';
            else if ((event.ctrlKey || event.metaKey) && key === 'p') action = 'print';
            else if (key === 'escape') action = 'clear';

            if (!action) return;
            event.preventDefault();
            event.stopPropagation();
            dotNetRef.invokeMethodAsync('HandlePatientShortcut', action).catch(() => {});
        };
        document.addEventListener('keydown', this._patientShortcutHandler, true);

        this._patientClickListener = (event) => {
            const idDropdown = document.querySelector('.id-dropdown');
            const phoneDropdown = document.querySelector('.phone-dropdown');
            if (!idDropdown && !phoneDropdown) return;

            const target = event.target instanceof Element ? event.target : (event.target ? event.target.parentElement : null);
            const inIdCombo = !!(target && target.closest && target.closest('.id-search-combo'));
            const inPhoneCombo = !!(target && target.closest && target.closest('.phone-search-combo'));

            if (inIdCombo && idDropdown) idDropdown.style.display = '';
            if (inPhoneCombo && phoneDropdown) phoneDropdown.style.display = '';

            const outsideId = !!(idDropdown && !inIdCombo);
            const outsidePhone = !!(phoneDropdown && !inPhoneCombo);

            if (outsideId || outsidePhone) {
                if (outsideId && idDropdown) idDropdown.style.display = 'none';
                if (outsidePhone && phoneDropdown) phoneDropdown.style.display = 'none';

                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('HandlePatientClickOutside', outsideId, outsidePhone).catch(() => {});
                }
            }
        };
        document.addEventListener('pointerdown', this._patientClickListener, true);
    },

    detachPatientShortcuts: function () {
        if (this._patientShortcutHandler) {
            document.removeEventListener('keydown', this._patientShortcutHandler, true);
        }
        if (this._patientClickListener) {
            document.removeEventListener('pointerdown', this._patientClickListener, true);
        }
        this._patientShortcutHandler = null;
        this._patientClickListener = null;
        this._patientShortcutRef = null;
    },

    printPatientCard: function (elementId, title) {
        const source = document.getElementById(elementId);
        if (!source) return;

        const cardElement = source.querySelector('.patient-cr80-card') || source;
        const cardHtml = cardElement.outerHTML;
        const safeTitle = String(title || 'Patient ID Card').replace(/[<>&"']/g, '');

        const frame = document.createElement('iframe');
        frame.style.position = 'fixed';
        frame.style.right = '0';
        frame.style.bottom = '0';
        frame.style.width = '0';
        frame.style.height = '0';
        frame.style.border = '0';
        document.body.appendChild(frame);
        const printWindow = frame.contentWindow;

        printWindow.document.write(`<!doctype html>
<html>
<head>
    <meta charset="utf-8">
    <title>${safeTitle}</title>
    <style>
        @page {
            size: 85.60mm 53.98mm;
            margin: 0 !important;
        }
        @media print {
            @page {
                size: 85.60mm 53.98mm;
                margin: 0 !important;
            }
            html, body {
                width: 85.60mm !important;
                height: 53.98mm !important;
                margin: 0 !important;
                padding: 0 !important;
                overflow: hidden !important;
                -webkit-print-color-adjust: exact !important;
                print-color-adjust: exact !important;
            }
            .patient-cr80-card {
                margin: 0 !important;
                box-shadow: none !important;
                border: none !important;
            }
        }
        * {
            box-sizing: border-box !important;
            -webkit-print-color-adjust: exact !important;
            print-color-adjust: exact !important;
        }
        html, body {
            margin: 0 !important;
            padding: 0 !important;
            width: 85.60mm !important;
            height: 53.98mm !important;
            max-width: 85.60mm !important;
            max-height: 53.98mm !important;
            background: #ffffff !important;
            overflow: hidden !important;
            font-family: 'Plus Jakarta Sans', 'Hind Siliguri', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif !important;
        }
        .patient-cr80-card {
            width: 85.60mm !important;
            height: 53.98mm !important;
            max-width: 85.60mm !important;
            max-height: 53.98mm !important;
            border-radius: 3.18mm !important;
            background: #ffffff !important;
            box-sizing: border-box !important;
            position: relative !important;
            overflow: hidden !important;
            border: 1px solid #cbd5e1 !important;
            display: flex !important;
            flex-direction: column !important;
            justify-content: space-between !important;
            font-family: 'Plus Jakarta Sans', 'Hind Siliguri', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif !important;
            color: #0f172a !important;
            margin: 0 !important;
            padding: 0 !important;
        }
        .patient-cr80-card .card-header {
            height: 11.5mm !important;
            min-height: 11.5mm !important;
            max-height: 11.5mm !important;
            background: linear-gradient(135deg, #0284c7 0%, #0369a1 100%) !important;
            color: #ffffff !important;
            padding: 1.5mm 3.5mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: space-between !important;
            box-sizing: border-box !important;
            border-top-left-radius: 3.18mm !important;
            border-top-right-radius: 3.18mm !important;
        }
        .patient-cr80-card .card-brand {
            display: flex !important;
            align-items: center !important;
            gap: 2mm !important;
            max-width: 61mm !important;
            overflow: hidden !important;
        }
        .patient-cr80-card .card-logo {
            width: 8mm !important;
            height: 8mm !important;
            max-width: 8mm !important;
            max-height: 8mm !important;
            object-fit: contain !important;
            background: #ffffff !important;
            border-radius: 1.5mm !important;
            padding: 0.5mm !important;
            flex-shrink: 0 !important;
        }
        .patient-cr80-card .card-logo-fallback {
            width: 8mm !important;
            height: 8mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            background: rgba(255, 255, 255, 0.2) !important;
            border-radius: 1.5mm !important;
            font-size: 5mm !important;
            flex-shrink: 0 !important;
        }
        .patient-cr80-card .card-title-group {
            display: flex !important;
            flex-direction: column !important;
            justify-content: center !important;
            line-height: 1.15 !important;
            overflow: hidden !important;
        }
        .patient-cr80-card .card-hospital-name {
            font-size: 8pt !important;
            font-weight: 800 !important;
            color: #ffffff !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
            letter-spacing: 0.2px !important;
        }
        .patient-cr80-card .card-doc-type {
            font-size: 5pt !important;
            font-weight: 700 !important;
            color: #bae6fd !important;
            text-transform: uppercase !important;
            letter-spacing: 0.5px !important;
        }
        .patient-cr80-card .card-header-badge {
            background: #ffffff !important;
            color: #0369a1 !important;
            border-radius: 1.8mm !important;
            padding: 0.8mm 2mm !important;
            display: flex !important;
            align-items: center !important;
            gap: 1mm !important;
            font-size: 6.5pt !important;
            font-weight: 800 !important;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12) !important;
            flex-shrink: 0 !important;
        }
        .patient-cr80-card .card-header-badge .badge-label {
            font-size: 5pt !important;
            color: #64748b !important;
            font-weight: 700 !important;
            text-transform: uppercase !important;
        }
        .patient-cr80-card .card-body {
            height: 28mm !important;
            min-height: 28mm !important;
            max-height: 28mm !important;
            padding: 1.5mm 3.5mm 1mm 3.5mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: space-between !important;
            box-sizing: border-box !important;
            background: #ffffff !important;
        }
        .patient-cr80-card .card-info-col {
            flex: 1 !important;
            max-width: 58mm !important;
            display: flex !important;
            flex-direction: column !important;
            justify-content: space-around !important;
            height: 100% !important;
            overflow: hidden !important;
        }
        .patient-cr80-card .card-patient-name {
            font-size: 9.5pt !important;
            font-weight: 850 !important;
            color: #0f172a !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
            line-height: 1.2 !important;
            margin-bottom: 0.8mm !important;
        }
        .patient-cr80-card .card-grid {
            display: grid !important;
            grid-template-columns: 1fr 1fr !important;
            row-gap: 0.6mm !important;
            column-gap: 2mm !important;
        }
        .patient-cr80-card .card-field {
            display: flex !important;
            flex-direction: column !important;
            line-height: 1.15 !important;
        }
        .patient-cr80-card .card-field.span-2 {
            grid-column: span 2 !important;
            flex-direction: row !important;
            align-items: baseline !important;
            gap: 1.5mm !important;
        }
        .patient-cr80-card .field-lbl {
            font-size: 5pt !important;
            font-weight: 700 !important;
            text-transform: uppercase !important;
            color: #64748b !important;
            letter-spacing: 0.3px !important;
        }
        .patient-cr80-card .field-val {
            font-size: 7pt !important;
            font-weight: 700 !important;
            color: #1e293b !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
        }
        .patient-cr80-card .blood-highlight {
            color: #dc2626 !important;
            font-weight: 800 !important;
        }
        .patient-cr80-card .card-qr-col {
            width: 20mm !important;
            display: flex !important;
            flex-direction: column !important;
            align-items: center !important;
            justify-content: center !important;
            flex-shrink: 0 !important;
        }
        .patient-cr80-card .card-qr-box {
            width: 18mm !important;
            height: 18mm !important;
            background: #ffffff !important;
            padding: 0.5mm !important;
            border: 1px solid #e2e8f0 !important;
            border-radius: 1.5mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            box-sizing: border-box !important;
        }
        .patient-cr80-card .card-qr-box svg {
            width: 100% !important;
            height: 100% !important;
            display: block !important;
        }
        .patient-cr80-card .card-qr-sub {
            font-size: 4.5pt !important;
            font-weight: 700 !important;
            color: #64748b !important;
            text-align: center !important;
            letter-spacing: 0.4px !important;
            margin-top: 0.5mm !important;
        }
        .patient-cr80-card .card-footer {
            height: 13.5mm !important;
            min-height: 13.5mm !important;
            max-height: 13.5mm !important;
            background: #f8fafc !important;
            border-top: 1px solid #e2e8f0 !important;
            padding: 0.8mm 3mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            box-sizing: border-box !important;
            border-bottom-left-radius: 3.18mm !important;
            border-bottom-right-radius: 3.18mm !important;
        }
        .patient-cr80-card .card-barcode-wrap {
            width: 100% !important;
            max-width: 60mm !important;
            height: 100% !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
        }
        .patient-cr80-card .card-barcode-wrap svg {
            max-width: 100% !important;
            height: 11.5mm !important;
            display: block !important;
        }
        button, .btn, .no-print { display: none !important; }
    </style>
</head>
<body>
    ${cardHtml}
</body>
</html>`);
        printWindow.document.close();

        const cleanup = () => {
            try {
                if (document.body.contains(frame)) {
                    document.body.removeChild(frame);
                }
            } catch (e) {}
        };
        printWindow.onafterprint = cleanup;
        setTimeout(() => {
            printWindow.focus();
            printWindow.print();
            setTimeout(cleanup, 60000);
        }, 350);
    },

    downloadPatientCardPng: async function (containerId, fileName) {
        const source = document.getElementById(containerId);
        if (!source) return;
        const card = source.querySelector('.patient-cr80-card') || source;
        if (!window.html2canvas) {
            console.error('html2canvas not loaded');
            return;
        }
        try {
            const canvas = await window.html2canvas(card, {
                scale: 4, // 300+ DPI equivalent
                useCORS: true,
                backgroundColor: '#ffffff',
                logging: false
            });
            const link = document.createElement('a');
            link.download = (fileName || 'Patient_Card') + '.png';
            link.href = canvas.toDataURL('image/png');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (e) {
            console.error('PNG export failed:', e);
        }
    },

    downloadPatientCardPdf: async function (containerId, fileName) {
        const source = document.getElementById(containerId);
        if (!source) return;
        const card = source.querySelector('.patient-cr80-card') || source;
        if (!window.html2canvas || !window.jspdf) {
            console.error('html2canvas or jspdf not loaded');
            return;
        }
        try {
            const canvas = await window.html2canvas(card, {
                scale: 4,
                useCORS: true,
                backgroundColor: '#ffffff',
                logging: false
            });
            const imgData = canvas.toDataURL('image/png');
            const { jsPDF } = window.jspdf;
            const pdf = new jsPDF({
                orientation: 'landscape',
                unit: 'mm',
                format: [85.60, 53.98]
            });
            pdf.addImage(imgData, 'PNG', 0, 0, 85.60, 53.98);
            pdf.save((fileName || 'Patient_Card') + '.pdf');
        } catch (e) {
            console.error('PDF export failed:', e);
        }
    },

    printElement: function (elementId, title) {
        if (elementId === 'patient-print' || elementId === 'patient-card-print') {
            return this.printPatientCard(elementId, title);
        }
        const source = document.getElementById(elementId);
        if (!source) return;

        const safeTitle = String(title || 'Diagnostic Report').replace(/[<>&"']/g, '');
        const frame = document.createElement('iframe');
        frame.style.position = 'fixed';
        frame.style.right = '0';
        frame.style.bottom = '0';
        frame.style.width = '0';
        frame.style.height = '0';
        frame.style.border = '0';
        document.body.appendChild(frame);
        const printWindow = frame.contentWindow;
        printWindow.document.write(`<!doctype html><html><head><meta charset="utf-8"><title>${safeTitle}</title>
        <link rel="stylesheet" href="/app.css">
        <style>
            @page { size: A4 portrait; margin: 8mm 10mm 10mm 10mm; }
            * { box-sizing: border-box !important; print-color-adjust: exact !important; }
            html, body { 
                background: #ffffff !important; 
                margin: 0; 
                padding: 0; 
                font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; 
                font-size: 12px; 
                color: #0f172a; 
                height: 100%; 
                min-height: 100%; 
            }
            body {
                display: flex;
                flex-direction: column;
                min-height: 100%;
            }
            .print-document, .print-sheet, .clinical-report-sheet, .a4-page-sheet { 
                width: 100% !important; 
                max-width: 100% !important; 
                min-height: 260mm !important; 
                box-shadow: none !important; 
                border: none !important; 
                padding: 0 !important; 
                background: #ffffff !important; 
                display: flex !important;
                flex-direction: column !important;
                justify-content: space-between !important;
                flex: 1 0 auto !important;
                box-sizing: border-box !important;
            }
            .print-document-footer {
                margin-top: auto !important;
                page-break-inside: avoid !important;
                padding-top: 24px !important;
            }
            button, .btn, .no-print { display: none !important; }
        </style></head><body>${source.innerHTML}</body></html>`);
        printWindow.document.close();
        
        const cleanup = () => {
            try {
                if (document.body.contains(frame)) {
                    document.body.removeChild(frame);
                }
            } catch (e) {}
        };
        printWindow.onafterprint = cleanup;
        setTimeout(() => {
            printWindow.focus();
            printWindow.print();
            setTimeout(cleanup, 60000);
        }, 350);
    }
};