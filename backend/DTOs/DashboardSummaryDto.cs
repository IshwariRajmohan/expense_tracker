using System.Collections.Generic;
using backend.Models;

namespace backend.DTOs;

public class DashboardSummaryDto
{
    public int TotalExpenses { get; set; }
    public int Draft { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Paid { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ChartDataPointDto> MonthlyExpenseChartData { get; set; } = new();
    public List<StatusChartDataPointDto> StatusChartData { get; set; } = new();
    public List<ActivityLog> RecentActivities { get; set; } = new();
    public List<Expense> LatestExpenses { get; set; } = new();
    public bool IsSubmissionFrozen { get; set; }
    public int FreezeDay { get; set; }
}

public class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty; // e.g. "Jul", "Jun"
    public decimal Value { get; set; } // monthly total value
}

public class StatusChartDataPointDto
{
    public string Status { get; set; } = string.Empty; // e.g. "Approved", "Pending"
    public int Count { get; set; }
    public decimal Amount { get; set; }
}
