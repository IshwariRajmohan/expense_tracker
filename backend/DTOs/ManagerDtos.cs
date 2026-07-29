using System.Collections.Generic;
using backend.Models;

namespace backend.DTOs;

public class EmployeeInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

public class ManagerExpenseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ExpenseItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public EmployeeInfoDto Employee { get; set; } = new();
}

public class ApprovalHistoryDto
{
    public string Id { get; set; } = string.Empty;
    public string ExpenseId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "Submitted", "Approved", "Rejected"
    public string PerformedBy { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class ManagerDashboardDto
{
    public int PendingRequestsCount { get; set; }
    public int ApprovedTodayCount { get; set; }
    public int RejectedTodayCount { get; set; }
    public decimal TotalPendingAmount { get; set; }
    public List<ManagerExpenseDto> RecentPendingRequests { get; set; } = new();
}
