#region Using directives
using System;
using System.IO;
using System.Text;
using UAManagedCore;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.Store;
#endregion

/*
 * CSV export for the AgeOvenProcessLog table (DATA LOG screen button).
 * Reference implementation for FactoryTalk Optix >= 1.4 - verify the
 * Store.Query signature against your version's docs before first build.
 * Writes AgeOvenProcessLog_<yyyyMMdd_HHmmss>.csv under the project's
 * ApplicationData CSVExport folder.
 */
public class CsvExportLogic : BaseNetLogic
{
    [ExportMethod]
    public void ExportProcessLog()
    {
        var store = Project.Current.Get<Store>("DataStores/AgeOvenLogs");
        if (store == null)
        {
            Log.Error("CsvExportLogic", "DataStores/AgeOvenLogs not found");
            return;
        }

        store.Query("SELECT * FROM ProcessLog ORDER BY Timestamp DESC LIMIT 86400",
                    out string[] header, out object[,] rows);
        if (header == null || rows == null)
        {
            Log.Warning("CsvExportLogic", "ProcessLog query returned no data");
            return;
        }

        var dir = Path.Combine(
            new ResourceUri("%APPLICATIONDATA%/CSVExport").Uri, "");
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir,
            $"AgeOvenProcessLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", header));
        for (int r = 0; r < rows.GetLength(0); r++)
        {
            for (int c = 0; c < rows.GetLength(1); c++)
            {
                if (c > 0) sb.Append(',');
                var v = rows[r, c];
                var s = v?.ToString() ?? "";
                // quote fields containing separators
                sb.Append(s.IndexOfAny(new[] { ',', '"', '\n' }) >= 0
                    ? "\"" + s.Replace("\"", "\"\"") + "\"" : s);
            }
            sb.AppendLine();
        }

        File.WriteAllText(file, sb.ToString());
        Log.Info("CsvExportLogic", $"Exported {rows.GetLength(0)} rows to {file}");
    }
}
