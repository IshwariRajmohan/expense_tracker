namespace backend.Models;

public class ActivityLog
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty; // ISO Date String
    public string StatusType { get; set; } = "info"; // "success", "warning", "info", "danger"
}
