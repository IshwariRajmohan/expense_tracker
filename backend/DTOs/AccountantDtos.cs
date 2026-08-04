using System.Collections.Generic;
using backend.Models;

namespace backend.DTOs;

public class AccountantDashboardDto
{
    public int ApprovedExpensesCount { get; set; }
    public int PaidExpensesCount { get; set; }
    public decimal TotalAmountToPay { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public List<AccountantPaymentActivityDto> RecentPaymentActivities { get; set; } = new();
}

public class AccountantPaymentActivityDto
{
    public string ExpenseId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PaymentDate { get; set; } = string.Empty;
}

public class AccountantExpenseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ExpenseItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Approved"; // "Approved" or "Paid"
    public string? Notes { get; set; }
    public string? PaymentDate { get; set; }
    public EmployeeInfoDto Employee { get; set; } = new();
    public List<ApprovalHistoryDto> ApprovalHistory { get; set; } = new();
}

public class PayExpenseRequestDto
{
    public string? Notes { get; set; }
}
