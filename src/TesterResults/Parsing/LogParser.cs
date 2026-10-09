using System.Globalization;
using TesterResults.Models;

namespace TesterResults.Parsing;

/// <summary>
/// Reads tester log files. One record per line:
///   timestamp;serialNumber;result;errorCode;cycleTimeSeconds
/// Example:
///   2026-09-30T08:00:01;SN000123;PASS;0;31.2
/// Lines starting with '#' are comments.
/// </summary>
public class LogParser
{
    public List<TestRecord> ParseDirectory(string directoryPath)
    {
        var allRecords = new List<TestRecord>();

        if (!Directory.Exists(directoryPath))
        {
            Console.Error.WriteLine($"[WARN] Directory not found: '{directoryPath}'");
            return allRecords;
        }

        var files = Directory.GetFiles(directoryPath, "*.log");
        foreach (var file in files)
        {
            allRecords.AddRange(ParseFile(file));
        }

        return allRecords;
    }

    public List<TestRecord> ParseFile(string path)
    {
        var testerId = Path.GetFileNameWithoutExtension(path);
        var records = new List<TestRecord>();

        using var reader = new StreamReader(path);
        string? line;
        int lineNumber = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            try
            {
                records.Add(ParseLine(testerId, line));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[WARN] Failed to parse line {lineNumber} in '{Path.GetFileName(path)}': {ex.Message}");
            }
        }

        return records;
    }

    public TestRecord ParseLine(string testerId, string line)
    {
        var parts = line.Split(';');

        var timestamp = DateTime.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
        var serialNumber = parts[1].Trim();
        var result = parts[2].Trim().ToUpper() == "PASS" ? TestResult.Pass : TestResult.Fail;
        var errorCode = int.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
        var cycleTime = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);

        return new TestRecord(testerId, timestamp, serialNumber, result, errorCode, cycleTime);
    }
}