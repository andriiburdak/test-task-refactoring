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
            Console.WriteLine(
                $"{s.TesterId,-12}{s.Total,8}{s.Passed,8}{s.Failed,8}{s.PassedOnRetest,8}{s.AverageCycleTime,10:F2}");
        }

        var failed = stats.Values.Sum(s => s.Failed);
        var retest = stats.Values.Sum(s => s.PassedOnRetest);
        var prt = failed == 0 ? 0 : 100.0 * retest / failed;

        Console.WriteLine();
        Console.WriteLine($"Total records: {stats.Values.Sum(s => s.Total)}");
        Console.WriteLine($"PRT (pass on retest): {prt:F1}%");
    }
}
