using TesterResults.Processing;
using TesterResults.Reporting;

namespace TesterResults;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var inputDir = args.Length > 0 ? args[0] : "data";
            var outputCsvPath = args.Length > 1 ? args[1] : Path.Combine("reports", "results.csv");

            if (!Directory.Exists(inputDir))
            {
                Console.Error.WriteLine($"[ERROR] Directory not found: '{inputDir}'");
                return;
            }

            var logFiles = Directory.GetFiles(inputDir, "*.log");
            if (logFiles.Length == 0)
            {
                Console.WriteLine($"[WARN] No .log files found in '{inputDir}'. Exiting.");
                return;
            }

            Console.WriteLine($"[INFO] Found {logFiles.Length} log file(s) in '{inputDir}'. Processing...");

            var aggregator = new StatsAggregator();
            var stats = aggregator.Aggregate(logFiles);

            ReportWriter.WriteConsole(stats);
            ReportWriter.WriteCsv(stats, outputCsvPath);

            Console.WriteLine($"\n[INFO] Report saved to: '{outputCsvPath}'");

            ReportUploader.Upload(stats);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
        }
    }
}