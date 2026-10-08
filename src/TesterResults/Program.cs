using TesterResults.Processing;
using TesterResults.Reporting;

var dataDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data");
var files = Directory.GetFiles(dataDir, "*.log");

Console.WriteLine($"Found {files.Length} tester logs in {Path.GetFullPath(dataDir)}");

var aggregator = new StatsAggregator();
var stats = aggregator.Aggregate(files);

ReportWriter.WriteConsole(stats);
new ReportUploader().Upload(stats);
