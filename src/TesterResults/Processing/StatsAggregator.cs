using System.Collections.Concurrent;
using TesterResults.Models;
using TesterResults.Parsing;

namespace TesterResults.Processing;

public class TesterStats
{
    public string TesterId { get; set; } = "";
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }

    /// <summary>Boards that passed after a previous FAIL on this tester.</summary>
    public int PassedOnRetest { get; set; }

    public double TotalCycleTime { get; set; }
    public double AverageCycleTime => Total == 0 ? 0 : TotalCycleTime / Total;
}

/// <summary>Builds per-tester statistics out of all tester log files.</summary>
public class StatsAggregator
{
    private readonly LogParser _parser = new();
    private readonly Dictionary<string, TesterStats> _stats = new();
    private readonly Dictionary<string, HashSet<string>> _failedSerialsPerTester = new();

    public IReadOnlyDictionary<string, TesterStats> Aggregate(IEnumerable<string> logFiles)
    {
        var allRecords = new ConcurrentBag<TestRecord>();

        Parallel.ForEach(logFiles, file =>
        {
            var records = _parser.ParseFile(file);
            foreach (var record in records)
            {
                allRecords.Add(record);
            }
        });

        var sortedRecords = allRecords.OrderBy(r => r.Timestamp).ToList();

        foreach (var record in sortedRecords)
        {
            Apply(record);
        }

        return _stats;
    }

    private void Apply(TestRecord record)
    {
        if (!_stats.TryGetValue(record.TesterId, out var stats))
        {
            stats = new TesterStats { TesterId = record.TesterId };
            _stats[record.TesterId] = stats;
            _failedSerialsPerTester[record.TesterId] = new HashSet<string>();
        }

        stats.Total++;
        stats.TotalCycleTime += record.CycleTimeSeconds;

        var failedSerials = _failedSerialsPerTester[record.TesterId];

        if (record.Result == TestResult.Pass)
        {
            stats.Passed++;

            if (failedSerials.Contains(record.SerialNumber))
            {
                stats.PassedOnRetest++;
                failedSerials.Remove(record.SerialNumber);
            }
        }
        else
        {
            stats.Failed++;
            failedSerials.Add(record.SerialNumber);
        }
    }
}