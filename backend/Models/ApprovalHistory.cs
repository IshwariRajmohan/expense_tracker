namespace backend.Models;

public class ApprovalHistory
{
    public string Id { get; set; } = string.Empty;
    public string ExpenseId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // 'Submitted', 'Approved', 'Rejected', 'Paid'
    public string PerformedBy { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty; // ISO Date String
    public string? Notes { get; set; }
}
