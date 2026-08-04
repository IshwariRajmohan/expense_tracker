using System.Collections.Generic;
using backend.Models;

namespace backend.DTOs;

public class AdminDashboardDto
{
    public int TotalEmployees { get; set; }
    public int TotalManagers { get; set; }
    public int TotalAccountants { get; set; }
    public int TotalExpenses { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PaidCount { get; set; }
    public decimal TotalExpenseAmount { get; set; }
    public List<ChartDataPointDto> MonthlyExpenseChartData { get; set; } = new();
    public List<StatusChartDataPointDto> StatusChartData { get; set; } = new();
    public List<ActivityLog> RecentActivities { get; set; } = new();
}

public class AdminUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Employee", "Manager", "Accountant", "Admin"
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public decimal BudgetLimit { get; set; }
    public decimal SpentAmount { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}

public class AdminExpenseDto
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
    public string? PaymentDate { get; set; }
    public EmployeeInfoDto Employee { get; set; } = new();
    public List<ApprovalHistoryDto> ApprovalHistory { get; set; } = new();
}

public class DepartmentReportDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class EmployeeReportDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class AdminReportsDto
{
    public List<ChartDataPointDto> MonthlyExpenseReport { get; set; } = new();
    public List<DepartmentReportDto> DepartmentWiseExpenses { get; set; } = new();
    public List<EmployeeReportDto> EmployeeWiseExpenses { get; set; } = new();
    public List<StatusChartDataPointDto> StatusWiseExpenses { get; set; } = new();
    public List<EmployeeReportDto> TopSpendingEmployees { get; set; } = new();
}

public class AdminFreezeDateDto
{
    public int FreezeDay { get; set; } = 18;
    public bool IsClosed { get; set; } // calculated dynamically
    public string CurrentMonth { get; set; } = string.Empty;
}

public class UpdateFreezeDateRequest
{
    public int Day { get; set; }
}

public class AdminSettingsDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CorporateCurrency { get; set; } = string.Empty;
    public string SystemMode { get; set; } = string.Empty;
    public UserProfile AdminProfile { get; set; } = new();
}
