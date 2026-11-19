using Microsoft.AspNetCore.Mvc;

namespace Level1;

public class CustomProblemDetails : ProblemDetails
{
    public string TraceId { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}
