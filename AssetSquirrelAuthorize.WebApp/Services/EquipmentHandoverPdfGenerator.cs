using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.IO;

namespace AssetSquirrelAuthorize.WebApp.Services
{
    // Draws the variable content of a handover document (date, item table,
    // prepared-by/for, signature lines) directly onto the existing company
    // letterhead PDF, rather than recreating the letterhead from scratch.
    public class EquipmentHandoverPdfGenerator : IEquipmentHandoverPdfGenerator
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public EquipmentHandoverPdfGenerator(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;

            // PdfSharp 6.x no longer resolves OS fonts by default -- without
            // this, XFont construction throws for every font family, even
            // ones actually installed on the (Windows) deployment target.
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        public byte[] Generate(EquipmentHandoverDto handover)
        {
            var templatePath = Path.Combine(webHostEnvironment.WebRootPath, "Templates", "DRUK_FIRMOWY.pdf");

            using var document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
            var page = document.Pages[0];
            using var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            var headerFont = new XFont("Arial", 10, XFontStyleEx.Bold);
            var bodyFont = new XFont("Arial", 10, XFontStyleEx.Regular);

            const double margin = 40;
            const double lineHeight = 18;
            double y = 150;

            gfx.DrawString($"Dokument wydania sprzętu nr {handover.HandoverDocumentNumber}", titleFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight * 1.5;

            gfx.DrawString($"Data wydania: {handover.HandoverDate:yyyy-MM-dd}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            var preparedBy = handover.PreparedByUserName ?? handover.PreparedByUserId ?? "-";
            gfx.DrawString($"Sporządził: {preparedBy}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            var recipientParts = new List<string>();
            if (handover.ToEmployee is not null)
            {
                recipientParts.Add($"{handover.ToEmployee.FirstName} {handover.ToEmployee.LastName}");
            }
            if (handover.ToLocation is not null)
            {
                recipientParts.Add($"{handover.ToLocation.City} {handover.ToLocation.Street}");
            }
            var recipient = recipientParts.Count > 0 ? string.Join(" / ", recipientParts) : "-";
            gfx.DrawString($"Wydano dla: {recipient}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            if (!string.IsNullOrWhiteSpace(handover.Comment))
            {
                gfx.DrawString($"Uwagi: {handover.Comment}", bodyFont, XBrushes.Black, new XPoint(margin, y));
                y += lineHeight;
            }

            y += lineHeight;

            double[] columnX = { margin, margin + 120, margin + 240, margin + 380 };
            gfx.DrawString("Producent", headerFont, XBrushes.Black, new XPoint(columnX[0], y));
            gfx.DrawString("Typ sprzętu", headerFont, XBrushes.Black, new XPoint(columnX[1], y));
            gfx.DrawString("Model", headerFont, XBrushes.Black, new XPoint(columnX[2], y));
            gfx.DrawString("Numer seryjny", headerFont, XBrushes.Black, new XPoint(columnX[3], y));
            y += 4;
            gfx.DrawLine(XPens.Black, margin, y, page.Width.Point - margin, y);
            y += lineHeight;

            foreach (var item in handover.EquipmentHandoverDetails)
            {
                gfx.DrawString(item.ManufacturerName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[0], y));
                gfx.DrawString(item.HardwareTypeName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[1], y));
                gfx.DrawString(item.ModelName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[2], y));
                gfx.DrawString(item.SerialNumber ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[3], y));
                y += lineHeight;
            }

            y += lineHeight * 3;

            gfx.DrawLine(XPens.Black, margin, y, margin + 180, y);
            gfx.DrawLine(XPens.Black, page.Width.Point - margin - 180, y, page.Width.Point - margin, y);
            y += 14;
            gfx.DrawString("Podpis wydającego", bodyFont, XBrushes.Black, new XPoint(margin, y));
            gfx.DrawString("Podpis odbierającego", bodyFont, XBrushes.Black, new XPoint(page.Width.Point - margin - 180, y));

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}
