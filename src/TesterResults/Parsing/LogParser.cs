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
    public List<TestRecord> ParseFile(string path)
    {
        var testerId = Path.GetFileNameWithoutExtension(path);
        var records = new List<TestRecord>();

        var reader = new StreamReader(path);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            try
            {
                records.Add(ParseLine(testerId, line));
            }
            catch (Exception)
            {
            }
        }

        return records;
    }

    public TestRecord ParseLine(string testerId, string line)
    {
        var parts = line.Split(';');

        var timestamp = DateTime.Parse(parts[0]);
        var serialNumber = parts[1].Trim();
        var result = parts[2].Trim().ToUpper() == "PASS" ? TestResult.Pass : TestResult.Fail;
        var errorCode = int.Parse(parts[3]);
        var cycleTime = double.Parse(parts[4]);

        return new TestRecord(testerId, timestamp, serialNumber, result, errorCode, cycleTime);
    }
}
