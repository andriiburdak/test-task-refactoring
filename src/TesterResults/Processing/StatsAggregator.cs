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
    public double AverageCycleTime => Passed == 0 ? 0 : TotalCycleTime / Passed;
}

/// <summary>Builds per-tester statistics out of all tester log files.</summary>
public class StatsAggregator
{
    private readonly LogParser _parser = new();
    private readonly Dictionary<string, TesterStats> _stats = new();
    private readonly HashSet<string> _seenSerials = new();

    public IReadOnlyDictionary<string, TesterStats> Aggregate(IEnumerable<string> logFiles)
    {
        Parallel.ForEach(logFiles, file =>
        {
            foreach (var record in _parser.ParseFile(file))
                Apply(record);
        });

        return _stats;
    }

    private void Apply(TestRecord record)
    {
        if (!_stats.TryGetValue(record.TesterId, out var stats))
        {
            stats = new TesterStats { TesterId = record.TesterId };
            _stats[record.TesterId] = stats;
        }

        stats.Total++;
        stats.TotalCycleTime += record.CycleTimeSeconds;

        if (record.Result == TestResult.Pass)
        {
            stats.Passed++;
            if (_seenSerials.Contains(record.SerialNumber))
                stats.PassedOnRetest++;
        }
        else
        {
            stats.Failed++;
        }

        _seenSerials.Add(record.SerialNumber);
    }
}
