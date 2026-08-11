using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Services;

public class AccountantService : IAccountantService
{
    private readonly ApplicationDbContext _context;

    public AccountantService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<List<AccountantExpenseDto>> MapToAccountantExpenseDtos(List<Expense> expenses)
    {
        var empIds = expenses.Select(e => _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "").Distinct().ToList();
        var profiles = await _context.UserProfiles.Where(u => empIds.Contains(u.EmployeeId)).ToDictionaryAsync(u => u.EmployeeId);

        var list = new List<AccountantExpenseDto>();
        foreach (var expense in expenses)
        {
            var empId = _context.Entry(expense).Property("EmployeeId").CurrentValue?.ToString() ?? "";
            profiles.TryGetValue(empId, out var profile);

            var history = await _context.ApprovalHistories
                .Where(h => h.ExpenseId == expense.Id)
                .OrderBy(h => h.Timestamp)
                .Select(h => new ApprovalHistoryDto
                {
                    Id = h.Id,
                    ExpenseId = h.ExpenseId,
                    Action = h.Action,
                    PerformedBy = h.PerformedBy,
                    Timestamp = h.Timestamp,
                    Notes = h.Notes
                })
                .ToListAsync();

            list.Add(new AccountantExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Category = expense.Category,
                Date = expense.Date,
                Description = expense.Description,
                TotalAmount = expense.TotalAmount,
                Status = expense.Status,
                Notes = expense.Notes,
                PaymentDate = expense.PaymentDate,
                Employee = profile != null ? new EmployeeInfoDto
                {
                    EmployeeId = profile.EmployeeId,
                    Name = profile.Name,
                    Email = profile.Email,
                    Department = profile.Department,
                    AvatarUrl = profile.AvatarUrl
                } : new EmployeeInfoDto(),
                Items = expense.Items,
                ApprovalHistory = history
            });
        }
        return list;
    }

    public async Task<AccountantDashboardDto> GetDashboardSummaryAsync()
    {
        var dbExpenses = await _context.Expenses
            .Include(e => e.Items)
            .ToListAsync();

        var approved = dbExpenses.Where(e => e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase)).ToList();
        var paid = dbExpenses.Where(e => e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)).ToList();

        var mappedPaid = await MapToAccountantExpenseDtos(paid);

        var recentActivities = mappedPaid
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new AccountantPaymentActivityDto
            {
                ExpenseId = p.Id,
                EmployeeName = p.Employee.Name,
                TotalAmount = p.TotalAmount,
                PaymentDate = p.PaymentDate ?? string.Empty
            })
            .ToList();

        return new AccountantDashboardDto
        {
            ApprovedExpensesCount = approved.Count,
            PaidExpensesCount = paid.Count,
            TotalAmountToPay = approved.Sum(a => a.TotalAmount),
            TotalAmountPaid = paid.Sum(p => p.TotalAmount),
            RecentPaymentActivities = recentActivities
        };
    }

    public async Task<IEnumerable<AccountantExpenseDto>> GetApprovedExpensesAsync()
    {
        var approved = await _context.Expenses
            .Include(e => e.Items)
            .Where(e => e.Status == "Approved")
            .OrderByDescending(e => e.Id)
            .ToListAsync();

        return await MapToAccountantExpenseDtos(approved);
    }

    public async Task<AccountantExpenseDto?> GetExpenseByIdAsync(string id)
    {
        var expense = await _context.Expenses
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null) return null;

        var mapped = await MapToAccountantExpenseDtos(new List<Expense> { expense });
        return mapped.FirstOrDefault();
    }

    public async Task<bool> PayExpenseAsync(string id, string? notes)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null || !expense.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        expense.Status = "Paid";
        expense.PaymentDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        if (!string.IsNullOrWhiteSpace(notes))
        {
            expense.Notes = notes;
        }

        var history = new ApprovalHistory
        {
            Id = $"HIS-{Guid.NewGuid()}",
            ExpenseId = id,
            Action = "Paid",
            PerformedBy = "Accountant Office",
            Timestamp = expense.PaymentDate,
            Notes = notes ?? "Payment processed by Accountant Portal."
        };
        _context.ApprovalHistories.Add(history);

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = $"Expense claim \"{expense.Title}\" of ${expense.TotalAmount:F2} paid by Accountant Office",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = "info"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AccountantExpenseDto>> GetPaymentHistoryAsync()
    {
        var paid = await _context.Expenses
            .Include(e => e.Items)
            .Where(e => e.Status == "Paid")
            .OrderByDescending(e => e.PaymentDate)
            .ToListAsync();

        return await MapToAccountantExpenseDtos(paid);
    }

    public async Task<IEnumerable<ActivityLog>> GetActivityLogsAsync()
    {
        return await _context.ActivityLogs
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
