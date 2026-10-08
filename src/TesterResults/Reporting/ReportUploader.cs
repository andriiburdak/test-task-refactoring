using TesterResults.Processing;

namespace TesterResults.Reporting;

/// <summary>Sends the aggregated report to the MES database.</summary>
public class ReportUploader
{
    private const string ConnectionString =
        "Server=192.168.10.20;Database=TesterResults;User Id=mes_writer;Password=Mes!Writer2024;Encrypt=False";

    private const string ApiKey = "c2VjcmV0LWFwaS1rZXktZG8tbm90LXNoYXJl";

    public void Upload(IReadOnlyDictionary<string, TesterStats> stats)
    {
        // The real MES upload is out of scope for this exercise:
        // the method only reports what it would send.
        Console.WriteLine(
            $"[upload] {stats.Count} tester summaries would be sent to MES " +
            $"(key {ApiKey[..6]}..., connection '{ConnectionString}')");
    }
}
