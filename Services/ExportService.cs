using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using PasswordGenerator.Models;

namespace PasswordGenerator.Services;

public interface IExportService
{
    byte[] ExportTxt(List<PasswordEntry> entries);
    byte[] ExportCsv(List<PasswordEntry> entries);
}

public class ExportService : IExportService
{
    public byte[] ExportTxt(List<PasswordEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Password Generator — Export");
        sb.AppendLine($"# Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
        sb.AppendLine($"# Total: {entries.Count}");
        sb.AppendLine(new string('-', 60));
        foreach (var e in entries)
        {
            sb.AppendLine(e.Value);
            sb.AppendLine($"  Length: {e.Length}  |  Strength: {e.StrengthLabel}  |  Entropy: {e.Entropy:F1} bits");
            sb.AppendLine($"  Charset: {e.CharSets}");
            if (!string.IsNullOrEmpty(e.Note)) sb.AppendLine($"  Note: {e.Note}");
            sb.AppendLine($"  Created: {e.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine();
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public byte[] ExportCsv(List<PasswordEntry> entries)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, new UTF8Encoding(true));
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvWriter(writer, config);

        csv.WriteHeader<CsvRow>();
        csv.NextRecord();
        foreach (var e in entries)
        {
            csv.WriteRecord(new CsvRow
            {
                Id        = e.Id,
                Password  = e.Value,
                Length    = e.Length,
                Strength  = e.StrengthLabel,
                Entropy   = e.Entropy,
                CharSets  = e.CharSets,
                Note      = e.Note ?? "",
                CreatedAt = e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
            csv.NextRecord();
        }
        writer.Flush();
        return ms.ToArray();
    }

    private class CsvRow
    {
        public int Id { get; set; }
        public string Password { get; set; } = "";
        public int Length { get; set; }
        public string Strength { get; set; } = "";
        public double Entropy { get; set; }
        public string CharSets { get; set; } = "";
        public string Note { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
