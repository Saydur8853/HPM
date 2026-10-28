window.hpm = {
    updateFavicon: function (href) {
        if (!href) return;
        try {
            var existingLink = document.getElementById('app-favicon') || document.querySelector("link[rel*='icon']");
            var newLink = document.createElement('link');
            newLink.id = 'app-favicon';
            newLink.rel = 'icon';
            newLink.href = href;
            if (existingLink && existingLink.parentNode) {
                existingLink.parentNode.replaceChild(newLink, existingLink);
            } else {
                document.head.appendChild(newLink);
            }
        } catch (e) {
            console.error('Failed to update favicon:', e);
        }
    },

    _activeWebcamStream: null,

    startWebcam: async function (videoElementId) {
        try {
            var video = document.getElementById(videoElementId);
            if (!video) {
                // Allow Blazor conditional DOM render up to 2 seconds
                for (var i = 0; i < 20; i++) {
                    await new Promise(function(resolve) { setTimeout(resolve, 100); });
                    video = document.getElementById(videoElementId);
                    if (video) break;
                }
            }
            if (!video) return { success: false, error: 'Video element not found.' };
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                return { success: false, error: 'Camera access is not supported by your browser or environment.' };
            }
            if (window.hpm._activeWebcamStream) {
                window.hpm.stopWebcam(videoElementId);
            }
            var stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    width: { ideal: 640 },
                    height: { ideal: 640 },
                    facingMode: 'user'
                },
                audio: false
            });
            window.hpm._activeWebcamStream = stream;
            video.srcObject = stream;
            try {
                await video.play();
            } catch (playErr) {
                console.warn('video.play() warning:', playErr);
            }

            // Ensure video width & height metadata are ready before declaring success
            await new Promise(function (resolve) {
                if (video.videoWidth > 0 && video.videoHeight > 0) {
                    return resolve();
                }
                var maxWait = 25; // 2.5s
                var timer = setInterval(function () {
                    maxWait--;
                    if ((video.videoWidth > 0 && video.videoHeight > 0) || maxWait <= 0) {
                        clearInterval(timer);
                        resolve();
                    }
                }, 100);
            });

            return { success: true };
        } catch (err) {
            console.error('Camera access error:', err);
            return { success: false, error: err.message || 'Camera permission denied or camera unavailable.' };
        }
    },

    captureWebcam: async function (videoElementId, maxDim) {
        try {
            maxDim = maxDim || 360;
            var video = document.getElementById(videoElementId);
            if (!video) {
                return { success: false, error: 'Video element not found.' };
            }

            // If dimensions are not ready, wait briefly for frame to arrive
            if (!video.videoWidth || !video.videoHeight) {
                await new Promise(function (resolve) {
                    var waitCount = 20; // 2s
                    var interval = setInterval(function () {
                        waitCount--;
                        if ((video.videoWidth > 0 && video.videoHeight > 0) || waitCount <= 0) {
                            clearInterval(interval);
                            resolve();
                        }
                    }, 100);
                });
            }

            if (!video.videoWidth || !video.videoHeight) {
                return { success: false, error: 'Video stream is not ready yet. Please wait a moment.' };
            }

            var canvas = document.createElement('canvas');
            var vw = video.videoWidth;
            var vh = video.videoHeight;
            var minDim = Math.min(vw, vh);
            var sx = (vw - minDim) / 2;
            var sy = (vh - minDim) / 2;

            var targetDim = Math.min(minDim, maxDim);
            canvas.width = targetDim;
            canvas.height = targetDim;
            var ctx = canvas.getContext('2d');
            ctx.drawImage(video, sx, sy, minDim, minDim, 0, 0, targetDim, targetDim);
            var dataUrl = canvas.toDataURL('image/jpeg', 0.82);
            window.hpm.stopWebcam(videoElementId);
            return { success: true, dataUrl: dataUrl };
        } catch (err) {
            console.error('Webcam capture error:', err);
            return { success: false, error: err.message || 'Failed to capture frame.' };
        }
    },

    stopWebcam: function (videoElementId) {
        try {
            if (window.hpm._activeWebcamStream) {
                window.hpm._activeWebcamStream.getTracks().forEach(function (track) {
                    try { track.stop(); } catch (e) {}
                });
                window.hpm._activeWebcamStream = null;
            }
            var video = document.getElementById(videoElementId);
            if (video) {
                video.srcObject = null;
            }
            return true;
        } catch (err) {
            return false;
        }
    },

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

    printPatientWristband: function (elementId, title) {
        const source = document.getElementById(elementId);
        if (!source) return;

        const bandElement = source.querySelector('.patient-wristband-strap') || source;
        const bandHtml = bandElement.outerHTML;
        const safeTitle = String(title || 'Patient ID Wristband').replace(/[<>&"']/g, '');

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
            size: 280mm 25.4mm landscape;
            margin: 0 !important;
        }
        @media print {
            html, body {
                width: 280mm !important;
                height: 25.4mm !important;
                margin: 0 !important;
                padding: 0 !important;
                overflow: hidden !important;
                -webkit-print-color-adjust: exact !important;
                print-color-adjust: exact !important;
            }
            .patient-wristband-strap {
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
            width: 280mm !important;
            height: 25.4mm !important;
            background: #ffffff !important;
            overflow: hidden !important;
            font-family: 'Plus Jakarta Sans', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif !important;
        }
        .patient-wristband-strap {
            width: 280mm !important;
            height: 25.4mm !important;
            background: #ffffff !important;
            display: flex !important;
            align-items: center !important;
            position: relative !important;
            overflow: hidden !important;
            box-sizing: border-box !important;
            border-top: 1px dashed #cbd5e1 !important;
            border-bottom: 1px dashed #cbd5e1 !important;
        }
        .wristband-fastener-tab {
            width: 20mm !important;
            height: 100% !important;
            display: flex !important;
            flex-direction: column !important;
            align-items: center !important;
            justify-content: center !important;
            gap: 1.5mm !important;
            background: #f8fafc !important;
            border-right: 1px dashed #cbd5e1 !important;
            flex-shrink: 0 !important;
        }
        .wristband-fastener-tab .snap-hole {
            width: 3.5mm !important;
            height: 3.5mm !important;
            border-radius: 50% !important;
            border: 1px solid #94a3b8 !important;
            background: #ffffff !important;
        }
        .wristband-fastener-tab .fastener-label {
            font-size: 4pt !important;
            font-weight: 800 !important;
            color: #94a3b8 !important;
            letter-spacing: 0.3px !important;
        }
        .wristband-print-zone {
            width: 140mm !important;
            height: 24mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: space-between !important;
            padding: 0.8mm 2.5mm !important;
            border-right: 1px dashed #cbd5e1 !important;
            flex-shrink: 0 !important;
            background: #ffffff !important;
        }
        .wristband-qr-box {
            width: 22mm !important;
            height: 22mm !important;
            display: flex !important;
            flex-direction: column !important;
            align-items: center !important;
            justify-content: center !important;
            flex-shrink: 0 !important;
        }
        .wristband-qr-box svg {
            width: 19mm !important;
            height: 19mm !important;
            display: block !important;
        }
        .wristband-qr-label {
            font-size: 3.5pt !important;
            font-weight: 800 !important;
            color: #64748b !important;
            line-height: 1 !important;
        }
        .wristband-info-col {
            flex: 1 !important;
            display: flex !important;
            flex-direction: column !important;
            justify-content: space-around !important;
            padding: 0 2mm !important;
            height: 100% !important;
            overflow: hidden !important;
            line-height: 1.15 !important;
        }
        .wristband-hospital {
            font-size: 5.5pt !important;
            font-weight: 800 !important;
            color: #475569 !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
            text-transform: uppercase !important;
        }
        .wristband-patient-name {
            font-size: 8.5pt !important;
            font-weight: 850 !important;
            color: #0f172a !important;
            white-space: nowrap !important;
            overflow: hidden !important;
            text-overflow: ellipsis !important;
        }
        .wristband-id-badge {
            font-size: 7pt !important;
            font-weight: 800 !important;
            color: #0369a1 !important;
        }
        .wristband-meta-row, .wristband-contact-row {
            font-size: 5.5pt !important;
            color: #334155 !important;
            font-weight: 650 !important;
            white-space: nowrap !important;
            display: flex !important;
            gap: 1.5mm !important;
        }
        .blood-pill {
            color: #dc2626 !important;
            font-weight: 800 !important;
        }
        .wristband-barcode-zone {
            width: 52mm !important;
            height: 22mm !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            padding: 0 1mm !important;
            flex-shrink: 0 !important;
        }
        .wristband-barcode-zone svg {
            max-width: 100% !important;
            height: 20mm !important;
            display: block !important;
        }
        .wristband-tail-zone {
            flex: 1 !important;
            height: 100% !important;
            display: flex !important;
            align-items: center !important;
            justify-content: space-around !important;
            padding: 0 5mm !important;
            background: #f8fafc !important;
        }
        .wristband-size-marker {
            font-size: 4.5pt !important;
            font-weight: 750 !important;
            color: #94a3b8 !important;
            letter-spacing: 0.5px !important;
        }
        .wristband-clip-hole {
            width: 3.5mm !important;
            height: 3.5mm !important;
            border-radius: 50% !important;
            border: 1px dashed #94a3b8 !important;
            background: #ffffff !important;
        }
    </style>
</head>
<body>
    ${bandHtml}
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

        setTimeout(() => {
            try {
                printWindow.focus();
                printWindow.print();
            } catch (e) {
                console.error(e);
            } finally {
                setTimeout(cleanup, 2000);
            }
        }, 350);
    },

    downloadPatientWristbandPng: async function (containerId, fileName) {
        const source = document.getElementById(containerId);
        if (!source) return;
        const band = source.querySelector('.patient-wristband-strap') || source;
        if (!window.html2canvas) {
            console.error('html2canvas not loaded');
            return;
        }
        try {
            const canvas = await window.html2canvas(band, {
                scale: 3,
                useCORS: true,
                backgroundColor: '#ffffff',
                logging: false
            });
            const link = document.createElement('a');
            link.download = (fileName || 'Patient_Wristband') + '.png';
            link.href = canvas.toDataURL('image/png');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (e) {
            console.error('PNG export failed:', e);
        }
    },

    downloadPatientWristbandPdf: async function (containerId, fileName) {
        const source = document.getElementById(containerId);
        if (!source) return;
        const band = source.querySelector('.patient-wristband-strap') || source;
        if (!window.html2canvas || !window.jspdf) {
            console.error('html2canvas or jspdf not loaded');
            return;
        }
        try {
            const canvas = await window.html2canvas(band, {
                scale: 3,
                useCORS: true,
                backgroundColor: '#ffffff',
                logging: false,
                windowWidth: 1000
            });
            const imgData = canvas.toDataURL('image/png');
            const { jsPDF } = window.jspdf;
            const pdf = new jsPDF({
                orientation: 'landscape',
                unit: 'mm',
                format: [280, 25.4]
            });
            pdf.addImage(imgData, 'PNG', 0, 0, 280, 25.4);
            pdf.save((fileName || 'Patient_Wristband') + '.pdf');
        } catch (e) {
            console.error('Wristband PDF export failed:', e);
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
        const isThermal = elementId === 'pharmacy-thermal-print' || (source && source.classList.contains('pos-receipt-80mm'));
        printWindow.document.write(`<!doctype html><html><head><meta charset="utf-8"><title>${safeTitle}</title>
        <link rel="stylesheet" href="/app.css">
        <style>
            ${isThermal ? `
            @page { size: 80mm auto; margin: 3mm 2mm; }
            * { box-sizing: border-box !important; -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }
            html, body {
                width: 76mm !important;
                max-width: 76mm !important;
                margin: 0 auto !important;
                padding: 0 !important;
                font-family: 'Courier New', Courier, monospace, system-ui;
                font-size: 11px !important;
                color: #000000 !important;
                background: #ffffff !important;
            }
            .pos-receipt-80mm {
                width: 100% !important;
                max-width: 100% !important;
                padding: 0 !important;
                box-shadow: none !important;
                border: none !important;
            }
            .pos-receipt-80mm table { width: 100% !important; font-size: 10.5px !important; }
            ` : `
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
            `}
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
    },

    downloadFile: function (fileName, contentType, base64Data) {
        try {
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: contentType });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            setTimeout(() => URL.revokeObjectURL(url), 2000);
        } catch (e) {
            console.error('File download failed:', e);
        }
    },

    printDoctorList: function (title, subtitle, metaText, tableHtml) {
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
    <title>${title || 'Doctors Directory'}</title>
    <style>
        @page { size: A4 landscape; margin: 8mm 10mm; }
        * { box-sizing: border-box !important; print-color-adjust: exact !important; -webkit-print-color-adjust: exact !important; }
        body {
            font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
            margin: 0;
            padding: 0;
            color: #0f172a;
            font-size: 9pt;
            background: #ffffff;
        }
        .print-header {
            text-align: center;
            border-bottom: 2px solid #0284c7;
            padding-bottom: 6px;
            margin-bottom: 8px;
        }
        .print-title {
            font-size: 15pt;
            font-weight: 800;
            color: #0f172a;
            margin: 0;
        }
        .print-sub {
            font-size: 10pt;
            color: #0369a1;
            font-weight: 700;
            margin: 2px 0 0;
        }
        .print-meta-bar {
            display: flex;
            justify-content: space-between;
            font-size: 8pt;
            color: #64748b;
            margin-bottom: 8px;
            font-weight: 600;
            border-bottom: 1px solid #e2e8f0;
            padding-bottom: 4px;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 8pt;
        }
        th {
            background: #f1f5f9 !important;
            color: #1e293b;
            font-weight: 750;
            text-align: left;
            padding: 5px 6px;
            border: 1px solid #cbd5e1;
            text-transform: uppercase;
            font-size: 7.5pt;
            letter-spacing: 0.3px;
        }
        td {
            padding: 4px 6px;
            border: 1px solid #e2e8f0;
            vertical-align: middle;
        }
        tr:nth-child(even) td {
            background: #f8fafc !important;
        }
        .text-center { text-align: center; }
        .badge-perm { color: #0369a1; font-weight: 700; }
        .badge-res { color: #7e22ce; font-weight: 700; }
        .badge-cont { color: #92400e; font-weight: 700; }
        .badge-active { color: #166534; font-weight: 750; }
        .badge-inactive { color: #dc2626; font-weight: 750; }
        .priority-star { color: #854d0e; font-weight: 800; }
        .print-footer {
            margin-top: 10px;
            font-size: 7.5pt;
            color: #94a3b8;
            display: flex;
            justify-content: space-between;
            border-top: 1px solid #e2e8f0;
            padding-top: 4px;
        }
    </style>
</head>
<body>
    <div class="print-header">
        <h1 class="print-title">${title || 'Delta Medical College & Hospital'}</h1>
        <p class="print-sub">${subtitle || 'Doctors Directory & Priority Schedule'}</p>
    </div>
    <div class="print-meta-bar">
        <span>${metaText || ''}</span>
        <span>Generated: ${new Date().toLocaleString()}</span>
    </div>
    ${tableHtml}
    <div class="print-footer">
        <span>Delta Medical College Hospital Information System</span>
        <span>Page 1</span>
    </div>
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

    initDoctorInfiniteScroll: function (containerId, listId, dotNetRef) {
        this._doctorScrollDotNetRef = dotNetRef;
        this._doctorSearchContainerId = containerId;
        this._doctorSuggestionsListId = listId;

        if (!this._doctorOutsideClickHandler) {
            this._doctorOutsideClickHandler = (event) => {
                const container = document.getElementById(this._doctorSearchContainerId);
                const list = document.getElementById(this._doctorSuggestionsListId);
                if (!container || !list) return;
                const target = event.target instanceof Element ? event.target : (event.target ? event.target.parentElement : null);
                if (target && container.contains(target)) {
                    return;
                }
                if (this._doctorScrollDotNetRef) {
                    this._doctorScrollDotNetRef.invokeMethodAsync('CloseDoctorSuggestionsFromJs').catch(() => {});
                }
            };
            document.addEventListener('pointerdown', this._doctorOutsideClickHandler, true);
        }

        const listEl = document.getElementById(listId);
        if (listEl && !listEl._hpmScrollAttached) {
            listEl._hpmScrollAttached = true;
            listEl.addEventListener('scroll', () => {
                if (listEl._hpmLoadingMore) return;
                const remaining = listEl.scrollHeight - listEl.scrollTop - listEl.clientHeight;
                if (remaining <= 70) {
                    listEl._hpmLoadingMore = true;
                    if (this._doctorScrollDotNetRef) {
                        this._doctorScrollDotNetRef.invokeMethodAsync('LoadMoreDoctorsFromJs')
                            .catch(() => {})
                            .finally(() => {
                                listEl._hpmLoadingMore = false;
                            });
                    } else {
                        listEl._hpmLoadingMore = false;
                    }
                }
            }, { passive: true });
        }
    },

    resetDoctorScrollTop: function (listId) {
        const listEl = document.getElementById(listId);
        if (listEl) {
            listEl.scrollTop = 0;
            listEl._hpmLoadingMore = false;
        }
    },

    disposeDoctorInfiniteScroll: function () {
        if (this._doctorOutsideClickHandler) {
            document.removeEventListener('pointerdown', this._doctorOutsideClickHandler, true);
            this._doctorOutsideClickHandler = null;
        }
        this._doctorScrollDotNetRef = null;
    },

    initTableAutoScroll: function (elementId, dotNetRef, methodName) {
        var attach = function () {
            var el = document.getElementById(elementId);
            if (!el) {
                setTimeout(function () {
                    var retryEl = document.getElementById(elementId);
                    if (retryEl && dotNetRef) {
                        window.hpm.initTableAutoScroll(elementId, dotNetRef, methodName);
                    }
                }, 150);
                return;
            }

            if (el._hpmTableScrollHandler) {
                el.removeEventListener('scroll', el._hpmTableScrollHandler);
            }

            el._hpmTableScrollLoading = false;
            el._hpmTableScrollHandler = function () {
                if (el._hpmTableScrollLoading) return;
                var remaining = el.scrollHeight - el.scrollTop - el.clientHeight;
                if (remaining <= 120) {
                    el._hpmTableScrollLoading = true;
                    dotNetRef.invokeMethodAsync(methodName || 'LoadNextPageFromScroll')
                        .then(function () {
                            setTimeout(function () {
                                el._hpmTableScrollLoading = false;
                            }, 250);
                        })
                        .catch(function () {
                            el._hpmTableScrollLoading = false;
                        });
                }
            };

            el.addEventListener('scroll', el._hpmTableScrollHandler, { passive: true });
        };

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', attach, { once: true });
        } else {
            attach();
        }
    },

    disposeTableAutoScroll: function (elementId) {
        var el = document.getElementById(elementId);
        if (el && el._hpmTableScrollHandler) {
            el.removeEventListener('scroll', el._hpmTableScrollHandler);
            el._hpmTableScrollHandler = null;
            el._hpmTableScrollLoading = false;
        }
    },

    scrollElementToTop: function (elementId) {
        var el = document.getElementById(elementId);
        if (el) {
            el.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }
};

// Global auto-case formatter for forms (Title Case for Names & Address, Lowercase for Email)
(function () {
    function formatTitleCase(val) {
        if (!val) return val;
        return val.replace(/(^|[\s\-\.\/,])([a-z])/g, function (match, sep, char) {
            return sep + char.toUpperCase();
        });
    }

    document.addEventListener('input', function (e) {
        const el = e.target;
        if (!el || !(el instanceof HTMLInputElement || el instanceof HTMLTextAreaElement)) return;
        const autocase = el.getAttribute('data-autocase');
        if (!autocase) return;

        if (autocase === 'title') {
            const val = el.value;
            if (!val) return;
            const formatted = formatTitleCase(val);
            if (formatted !== val) {
                const start = el.selectionStart;
                const end = el.selectionEnd;
                el.value = formatted;
                if (start !== null && end !== null) {
                    el.setSelectionRange(start, end);
                }
                el.dispatchEvent(new Event('change', { bubbles: true }));
            }
        } else if (autocase === 'email') {
            const val = el.value;
            if (!val) return;
            const formatted = val.toLowerCase();
            if (formatted !== val) {
                const start = el.selectionStart;
                const end = el.selectionEnd;
                el.value = formatted;
                if (start !== null && end !== null) {
                    el.setSelectionRange(start, end);
                }
                el.dispatchEvent(new Event('change', { bubbles: true }));
            }
        }
    }, true);
})();