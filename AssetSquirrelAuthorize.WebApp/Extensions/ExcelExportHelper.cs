using ClosedXML.Excel;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class ExcelExportHelper
    {
        public static byte[] BuildWorkbook(string sheetName, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<object?>> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(sheetName);

            for (var column = 0; column < headers.Count; column++)
            {
                sheet.Cell(1, column + 1).Value = headers[column];
            }

            var rowIndex = 2;
            foreach (var row in rows)
            {
                for (var column = 0; column < row.Count; column++)
                {
                    var cell = sheet.Cell(rowIndex, column + 1);
                    switch (row[column])
                    {
                        case null:
                            break;
                        case string s:
                            cell.Value = s;
                            break;
                        case bool b:
                            cell.Value = b;
                            break;
                        case DateTime dt:
                            cell.Value = dt;
                            break;
                        case DateTimeOffset dto:
                            cell.Value = dto.DateTime;
                            break;
                        case int i:
                            cell.Value = i;
                            break;
                        default:
                            cell.Value = row[column]!.ToString();
                            break;
                    }
                }

                rowIndex++;
            }

            sheet.Row(1).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
