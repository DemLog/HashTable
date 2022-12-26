using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;

namespace DynamicStructures.Test.Tester.classes
{
    public static class SaveManager
    {
        public static void SaveTable<TResult>(FileInfo file, TestResult<TResult>[] results, string title1, string title2,
            bool isGraphic = true)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using ExcelPackage package = new(file);
            string name = results[0].AlgorithmName;
            var ws = package.Workbook.Worksheets
                .FirstOrDefault(ws => ws.Name == name);

            if (ws == null) ws = package.Workbook.Worksheets.Add(name);
            else
            {
                ws.Cells.Clear();
                ws.Drawings.Clear();
            }

            ws.Cells[1, 1].Value = title1;
            ws.Cells[1, 2].Value = title2;

            for (int i = 2; i < results.Length + 2; i++)
            {
                ws.Cells[i, 1].Value = results[i - 2].ID;
                ws.Cells[i, 2].Value = results[i - 2].Result;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            /*var table = ws.PivotTables.Add(dataRange, dataRange, results[0].AlgorithmName);
            var field = table.RowFields.Add(table.Fields[""]);*/
            if (isGraphic)
            {
                var rangeX = ws.Cells[$"A2:A{results.Length + 1}"];
                var rangeY = ws.Cells[$"B2:B{results.Length + 1}"];
                AddChart(ws, rangeX, rangeY, name, title1, title2);
            }
            package.Save();
        }

        private static void AddChart(ExcelWorksheet ws, ExcelRange rangeX, ExcelRange rangeY, string name, string titleX, string titleY)
        {
            var chart = ws.Drawings.AddChart("FindingsChart", eChartType.XYScatterLinesNoMarkers);

            chart.Title.Text = name;
            chart.SetPosition(7, 0, 5, 0);
            chart.SetSize(800, 400);

            var serie = chart
                .Series
                .Add(rangeY, rangeX);
            serie.Header = name;
            chart.XAxis.AddTitle(titleX);
            chart.YAxis.AddTitle(titleY);
            chart.StyleManager.SetChartStyle(ePresetChartStyle.ScatterChartStyle3);
        }
    }
}