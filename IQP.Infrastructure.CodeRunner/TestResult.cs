using System.Text.Json.Serialization;

namespace IQP.Infrastructure.CodeRunner;

public class TestResult
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public TestStatus Status { get; set; }
    public int? TaskId { get; set; }
    public string? Message { get; set; }
    public string? Output { get; set; }
    [JsonPropertyName("test_code")]
    public string? TestCode { get; set; }
}