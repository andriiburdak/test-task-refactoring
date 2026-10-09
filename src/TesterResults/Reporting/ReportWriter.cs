using System.Globalization;
using TesterResults.Processing;

namespace TesterResults.Reporting;

public static class ReportWriter
{
    public static void WriteConsole(IReadOnlyDictionary<string, TesterStats> stats)
    {
        Console.WriteLine();
        Console.WriteLine($"{"Tester",-12}{"Total",8}{"Pass",8}{"Fail",8}{"Retest",8}{"AvgCT",10}");
        Console.WriteLine(new string('-', 54));

        foreach (var s in stats.Values.OrderBy(s => s.TesterId))
        {
            var avgCTStr = s.AverageCycleTime.ToString("F2", CultureInfo.InvariantCulture);
            Console.WriteLine(
                $"{s.TesterId,-12}{s.Total,8}{s.Passed,8}{s.Failed,8}{s.PassedOnRetest,8}{avgCTStr,10}");
        }

        var failed = stats.Values.Sum(s => s.Failed);
        var retest = stats.Values.Sum(s => s.PassedOnRetest);
        var prt = failed == 0 ? 0 : 100.0 * retest / failed;

        Console.WriteLine();
        Console.WriteLine($"Total records: {stats.Values.Sum(s => s.Total)}");
        Console.WriteLine($"PRT (pass on retest): {prt.ToString("F1", CultureInfo.InvariantCulture)}%");
    }

    public static void WriteCsv(IReadOnlyDictionary<string, TesterStats> stats, string csvPath)
    {
        var directory = Path.GetDirectoryName(csvPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(csvPath);

        writer.WriteLine("tester,всього,pass,fail,pass on retest,середній час циклу");

        foreach (var s in stats.Values.OrderBy(s => s.TesterId))
        {
            var avgCycleTimeStr = s.AverageCycleTime.ToString("F2", CultureInfo.InvariantCulture);
            writer.WriteLine($"{s.TesterId},{s.Total},{s.Passed},{s.Failed},{s.PassedOnRetest},{avgCycleTimeStr}");
        }
    }
}