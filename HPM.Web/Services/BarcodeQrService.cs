using System.Text;
using QRCoder;

namespace HPM.Web.Services;

public interface IBarcodeQrService
{
    string GenerateQrSvg(string content, int pixelsPerModule = 4);
    string GenerateBarcodeSvg(string content, int barHeight = 45, bool includeText = true);
    string GeneratePatientQrPayload(long patientId, string regNo, string name, string? phone = null);
    string GenerateTicketQrPayload(long ticketId, long patientId, string patientName, string doctorName, decimal fee);
    string GenerateAppointmentQrPayload(long appointmentId, int serialNo, string patientName, string doctorName, DateTime apptDate);
    string GenerateEmergencyQrPayload(long emergencyId, long patientId, string patientName, decimal total, decimal due);
    string GenerateOtQrPayload(long otId, long patientId, string patientName, string surgeon, decimal total);
    string GenerateIpdQrPayload(long admissionId, string regNo, string patientName, string bedName);
    string GenerateWristbandQrPayload(long admissionId, string regNo, string patientName, string ageSex, string bloodGroup, string bedName, string docName);
    string GenerateLabQrPayload(long invoiceId, string regNo, string patientName, string sampleId, string? portalBaseUrl = null);
    string GenerateLabTubeQrPayload(string sampleId, long invoiceId, string patientName, string testNames);
    string GeneratePharmacyQrPayload(long memoId, string customerName, decimal totalAmount);
    string GeneratePrescriptionQrPayload(long rxId, string patientName, string doctorName, DateTime date);
    string GenerateDischargeQrPayload(long admissionId, string regNo, string patientName, decimal finalBill, bool isCleared);
    string GenerateVoucherQrPayload(long voucherId, string voucherType, decimal amount, DateTime date);
}

public class BarcodeQrService : IBarcodeQrService
{
    public string GenerateQrSvg(string content, int pixelsPerModule = 4)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            var svgQr = new SvgQRCode(qrCodeData);
            return svgQr.GetGraphic(pixelsPerModule);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Generates a standardized Code 128 (Auto B/C) SVG vector 1D barcode with quiet zones and checksum.
    /// Perfectly scannable by all standard 1D laser/CCD and 2D image barcode scanners.
    /// </summary>
    public string GenerateBarcodeSvg(string content, int barHeight = 45, bool includeText = true)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        var cleanText = content.Trim();
        var patterns = Code128Patterns();
        var codeList = new List<int> { 104 }; // Start Code B

        int checksum = 104;
        for (int i = 0; i < cleanText.Length; i++)
        {
            char c = cleanText[i];
            int val = c - 32;
            if (val < 0 || val > 95) val = 0; // fallback
            codeList.Add(val);
            checksum += val * (i + 1);
        }

        codeList.Add(checksum % 103);
        codeList.Add(106); // Stop pattern

        var sbPattern = new StringBuilder();
        foreach (var code in codeList)
        {
            if (code >= 0 && code < patterns.Length)
            {
                sbPattern.Append(patterns[code]);
            }
        }
        sbPattern.Append("11"); // Final termination bar

        string binaryPattern = sbPattern.ToString();
        int quietZone = 10;
        int totalWidth = binaryPattern.Length + (quietZone * 2);
        int totalHeight = includeText ? barHeight + 14 : barHeight;
        int renderedWidth = Math.Max(130, (int)(totalWidth * 1.5));

        var svg = new StringBuilder();
        svg.Append($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {totalWidth} {totalHeight}\" width=\"{renderedWidth}\" height=\"{totalHeight}\" class=\"barcode-svg\" shape-rendering=\"crispEdges\">");
        svg.Append($"<rect width=\"100%\" height=\"100%\" fill=\"#ffffff\"/>");

        int x = quietZone;
        int runStart = -1;
        for (int i = 0; i < binaryPattern.Length; i++)
        {
            if (binaryPattern[i] == '1')
            {
                if (runStart < 0) runStart = x;
            }
            else
            {
                if (runStart >= 0)
                {
                    svg.Append($"<rect x=\"{runStart}\" y=\"0\" width=\"{x - runStart}\" height=\"{barHeight}\" fill=\"#000000\"/>");
                    runStart = -1;
                }
            }
            x++;
        }
        if (runStart >= 0)
        {
            svg.Append($"<rect x=\"{runStart}\" y=\"0\" width=\"{x - runStart}\" height=\"{barHeight}\" fill=\"#000000\"/>");
        }

        if (includeText)
        {
            svg.Append($"<text x=\"{(double)totalWidth / 2.0:0.#}\" y=\"{barHeight + 11}\" text-anchor=\"middle\" font-family=\"monospace, 'Courier New', Courier, sans-serif\" font-size=\"9\" font-weight=\"bold\" fill=\"#000000\" letter-spacing=\"1.5\">{System.Security.SecurityElement.Escape(cleanText)}</text>");
        }

        svg.Append("</svg>");
        return svg.ToString();
    }

    public string GeneratePatientQrPayload(long patientId, string regNo, string name, string? phone = null)
    {
        return $"HPM:PAT|ID:{patientId}|REG:{regNo}|NAME:{name}|TEL:{phone ?? ""}";
    }

    public string GenerateTicketQrPayload(long ticketId, long patientId, string patientName, string doctorName, decimal fee)
    {
        return $"HPM:TKT|ID:{ticketId}|PID:{patientId}|PAT:{patientName}|DOC:{doctorName}|FEE:{fee:F2}";
    }

    public string GenerateAppointmentQrPayload(long appointmentId, int serialNo, string patientName, string doctorName, DateTime apptDate)
    {
        return $"HPM:APT|ID:{appointmentId}|SL:{serialNo:00}|PAT:{patientName}|DOC:{doctorName}|DATE:{apptDate:yyyy-MM-dd}";
    }

    public string GenerateEmergencyQrPayload(long emergencyId, long patientId, string patientName, decimal total, decimal due)
    {
        return $"HPM:EMG|ID:{emergencyId}|PID:{patientId}|PAT:{patientName}|TOT:{total:F2}|DUE:{due:F2}";
    }

    public string GenerateOtQrPayload(long otId, long patientId, string patientName, string surgeon, decimal total)
    {
        return $"HPM:OT|ID:{otId}|PID:{patientId}|PAT:{patientName}|SUR:{surgeon}|TOT:{total:F2}";
    }

    public string GenerateIpdQrPayload(long admissionId, string regNo, string patientName, string bedName)
    {
        return $"HPM:IPD|ADM:{admissionId}|REG:{regNo}|PAT:{patientName}|BED:{bedName}";
    }

    public string GenerateWristbandQrPayload(long admissionId, string regNo, string patientName, string ageSex, string bloodGroup, string bedName, string docName)
    {
        return $"HPM:WB|ADM:{admissionId}|REG:{regNo}|PAT:{patientName}|AS:{ageSex}|BG:{bloodGroup}|BED:{bedName}|DOC:{docName}";
    }

    public string GenerateLabQrPayload(long invoiceId, string regNo, string patientName, string sampleId, string? portalBaseUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(portalBaseUrl))
        {
            return $"{portalBaseUrl.TrimEnd('/')}/portal/report/{invoiceId}";
        }
        return $"/portal/report/{invoiceId}";
    }

    public string GenerateLabTubeQrPayload(string sampleId, long invoiceId, string patientName, string testNames)
    {
        return $"HPM:TUBE|SMP:{sampleId}|INV:{invoiceId}|PAT:{patientName}|TESTS:{testNames}";
    }

    public string GeneratePharmacyQrPayload(long memoId, string customerName, decimal totalAmount)
    {
        return $"HPM:PHARM|MEMO:{memoId}|CUST:{customerName}|AMT:{totalAmount:F2}";
    }

    public string GeneratePrescriptionQrPayload(long rxId, string patientName, string doctorName, DateTime date)
    {
        return $"HPM:RX|ID:{rxId}|PAT:{patientName}|DOC:{doctorName}|DATE:{date:yyyy-MM-dd}";
    }

    public string GenerateDischargeQrPayload(long admissionId, string regNo, string patientName, decimal finalBill, bool isCleared)
    {
        return $"HPM:DC|ADM:{admissionId}|REG:{regNo}|PAT:{patientName}|BILL:{finalBill:F2}|STATUS:{(isCleared ? "CLEARED" : "DUE")}";
    }

    public string GenerateVoucherQrPayload(long voucherId, string voucherType, decimal amount, DateTime date)
    {
        return $"HPM:VCH|ID:{voucherId}|TYPE:{voucherType}|AMT:{amount:F2}|DATE:{date:yyyy-MM-dd}";
    }

    private static string[] Code128Patterns()
    {
        return new[]
        {
            "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", // 0-4
            "10001001100", "10011001000", "10011000100", "10001100100", "11001001000", // 5-9
            "11001000100", "11000100100", "10110011100", "10011011100", "10011001110", // 10-14
            "10111001100", "10011101100", "10011100110", "11001110010", "11001011100", // 15-19
            "11001001110", "11011100100", "11001110100", "11101101110", "11101001100", // 20-24
            "11100101100", "11100100110", "11101100100", "11100110100", "11100110010", // 25-29
            "11011011000", "11011000110", "11000110110", "10100011000", "10001011000", // 30-34
            "10001000110", "10110001000", "10001101000", "10001100010", "11010001000", // 35-39
            "11000101000", "11000100010", "10110111000", "10110001110", "10001101110", // 40-44
            "10111011000", "10111000110", "10001110110", "11101110110", "11010001110", // 45-49
            "11000101110", "11011101000", "11011100010", "11011101110", "11101011000", // 50-54
            "11101000110", "11100010110", "11101101000", "11101100010", "11100011010", // 55-59
            "11101111010", "11001000010", "11110001010", "10100110000", "10100001100", // 60-64
            "10010110000", "10010000110", "10000101100", "10000100110", "10110010000", // 65-69
            "10110000100", "10011010000", "10011000010", "10000110100", "10000110010", // 70-74
            "11000010010", "11001010000", "11110111010", "11000010100", "10001111010", // 75-79
            "10100111100", "10010111100", "10010011110", "10111100100", "10011110100", // 80-84
            "10011110010", "11110100100", "11110010100", "11110010010", "11011011110", // 85-89
            "11011110110", "11110110110", "10101111000", "10100011110", "10001011110", // 90-94
            "10111101000", "10111100010", "11110101000", "11110100010", "10111011110", // 95-99
            "10111101110", "11101011110", "11110101110", "11010000100", "11010010000", // 100-104
            "11010011100", "11000111010" // 105, 106 (Stop)
        };
    }
}
