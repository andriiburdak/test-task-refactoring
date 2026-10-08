namespace TesterResults.Models;

public enum TestResult
{
    Pass,
    Fail
}

/// <summary>One line of a tester log: a single test run of one PCB on one tester.</summary>
public record TestRecord(
    string TesterId,
    DateTime Timestamp,
    string SerialNumber,
    TestResult Result,
    int ErrorCode,
    double CycleTimeSeconds);
