using System.Text.Json.Serialization;

namespace IQP.Infrastructure.CodeRunner;

public class TestRun
{
    public int Version { get; set; } = 3;
    [JsonPropertyName("status")]
    public TestStatus Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("tests")]
    public TestResult[]? Tests { get; set; }
}