using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.IO;

namespace AssetSquirrelAuthorize.WebApp.Services
{
    // Draws the variable content of a return document (date, item table,
    // prepared-by/from, storage location, signature lines) directly onto the
    // existing company letterhead PDF, mirroring EquipmentHandoverPdfGenerator.
    public class EquipmentReturnPdfGenerator : IEquipmentReturnPdfGenerator
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public EquipmentReturnPdfGenerator(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;

            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }

        public byte[] Generate(EquipmentReturnDto equipmentReturn)
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

            gfx.DrawString($"Dokument zwrotu sprzętu nr {equipmentReturn.ReturnDocumentNumber}", titleFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight * 1.5;

            gfx.DrawString($"Data zwrotu: {equipmentReturn.ReturnDate:yyyy-MM-dd}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            var preparedBy = equipmentReturn.PreparedByUserName ?? equipmentReturn.PreparedByUserId ?? "-";
            gfx.DrawString($"Przyjął zwrot: {preparedBy}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            var fromParts = new List<string>();
            if (equipmentReturn.Employee is not null)
            {
                fromParts.Add($"{equipmentReturn.Employee.FirstName} {equipmentReturn.Employee.LastName}");
            }
            if (equipmentReturn.Location is not null)
            {
                fromParts.Add($"{equipmentReturn.Location.City} {equipmentReturn.Location.Street}");
            }
            var from = fromParts.Count > 0 ? string.Join(" / ", fromParts) : "-";
            gfx.DrawString($"Zwrócono od: {from}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            gfx.DrawString($"Miejsce przechowania: {equipmentReturn.StorageLocationName ?? "-"}", bodyFont, XBrushes.Black, new XPoint(margin, y));
            y += lineHeight;

            if (!string.IsNullOrWhiteSpace(equipmentReturn.Comment))
            {
                gfx.DrawString($"Uwagi: {equipmentReturn.Comment}", bodyFont, XBrushes.Black, new XPoint(margin, y));
                y += lineHeight;
            }

            y += lineHeight;

            double[] columnX = { margin, margin + 90, margin + 195, margin + 300, margin + 415 };
            gfx.DrawString("Nr inwentarzowy", headerFont, XBrushes.Black, new XPoint(columnX[0], y));
            gfx.DrawString("Producent", headerFont, XBrushes.Black, new XPoint(columnX[1], y));
            gfx.DrawString("Typ sprzętu", headerFont, XBrushes.Black, new XPoint(columnX[2], y));
            gfx.DrawString("Model", headerFont, XBrushes.Black, new XPoint(columnX[3], y));
            gfx.DrawString("Numer seryjny", headerFont, XBrushes.Black, new XPoint(columnX[4], y));
            y += 4;
            gfx.DrawLine(XPens.Black, margin, y, page.Width.Point - margin, y);
            y += lineHeight;

            foreach (var item in equipmentReturn.Items)
            {
                gfx.DrawString(item.InventoryNumber ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[0], y));
                gfx.DrawString(item.ManufacturerName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[1], y));
                gfx.DrawString(item.HardwareTypeName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[2], y));
                gfx.DrawString(item.ModelName ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[3], y));
                gfx.DrawString(item.SerialNumber ?? "-", bodyFont, XBrushes.Black, new XPoint(columnX[4], y));
                y += lineHeight;
            }

            y += lineHeight * 3;

            gfx.DrawLine(XPens.Black, margin, y, margin + 180, y);
            gfx.DrawLine(XPens.Black, page.Width.Point - margin - 180, y, page.Width.Point - margin, y);
            y += 14;
            gfx.DrawString("Podpis zdającego", bodyFont, XBrushes.Black, new XPoint(margin, y));
            gfx.DrawString("Podpis przyjmującego", bodyFont, XBrushes.Black, new XPoint(page.Width.Point - margin - 180, y));

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}
