using TesterResults.Models;
using TesterResults.Parsing;

namespace TesterResults.Tests;

public class LogParserTests
{
    [Fact]
    public void ParseLine_parses_pass_record()
    {
        var record = new LogParser().ParseLine("tester-01", "2026-09-30T08:00:01;SN000123;PASS;0;31");

        Assert.Equal("tester-01", record.TesterId);
        Assert.Equal("SN000123", record.SerialNumber);
        Assert.Equal(TestResult.Pass, record.Result);
        Assert.Equal(0, record.ErrorCode);
        Assert.Equal(31, record.CycleTimeSeconds);
    }

    [Fact]
    public void ParseLine_parses_fail_record_with_error_code()
    {
        var record = new LogParser().ParseLine("tester-01", "2026-09-30T08:00:05;SN000124;FAIL;17;29");

        Assert.Equal(TestResult.Fail, record.Result);
        Assert.Equal(17, record.ErrorCode);
    }
}
