using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Services;

public class ManagerService : IManagerService
{
    private readonly ApplicationDbContext _context;

    public ManagerService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<UserProfile> GetCurrentManagerProfileAsync()
    {
        var username = CurrentUserState.Username;
        var credential = await _context.UserCredentials.FirstOrDefaultAsync(c => c.Username.ToLower() == username.ToLower());
        
        UserProfile? profile = null;
        if (credential != null)
        {
            profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Name == credential.DisplayName);
        }

        if (profile == null)
        {
            profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Role == "Manager" || u.EmployeeId == "FP-2024-001");
        }

        if (profile == null)
        {
            profile = new UserProfile
            {
                EmployeeId = "FP-2024-001",
                Name = "Ishwari Rajmohan",
                Email = "ishwari.r@firstpay.com",
                Role = "Manager",
                Department = "Engineering",
                BudgetLimit = 50000.00m,
                SpentAmount = 15000.00m,
                AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop"
            };
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }
        return profile;
    }

    private async Task<List<ManagerExpenseDto>> MapToManagerExpenseDtos(List<Expense> expenses)
    {
        var empIds = expenses.Select(e => _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "").Distinct().ToList();
        var profiles = await _context.UserProfiles.Where(u => empIds.Contains(u.EmployeeId)).ToDictionaryAsync(u => u.EmployeeId);

        var list = new List<ManagerExpenseDto>();
        foreach (var expense in expenses)
        {
            var empId = _context.Entry(expense).Property("EmployeeId").CurrentValue?.ToString() ?? "";
            profiles.TryGetValue(empId, out var profile);
            list.Add(new ManagerExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Category = expense.Category,
                Date = expense.Date,
                Description = expense.Description,
                TotalAmount = expense.TotalAmount,
                Status = expense.Status,
                Notes = expense.Notes,
                Employee = profile != null ? new EmployeeInfoDto
                {
                    EmployeeId = profile.EmployeeId,
                    Name = profile.Name,
                    Email = profile.Email,
                    Department = profile.Department,
                    AvatarUrl = profile.AvatarUrl
                } : new EmployeeInfoDto(),
                Items = expense.Items
            });
        }
        return list;
    }

    public async Task<ManagerDashboardDto> GetDashboardSummaryAsync()
    {
        var manager = await GetCurrentManagerProfileAsync();
        var reportees = await _context.UserProfiles.Where(u => u.ManagerId == manager.EmployeeId).Select(u => u.EmployeeId).ToListAsync();

        var dbExpenses = await _context.Expenses
            .Include(e => e.Items)
            .Where(e => reportees.Contains(EF.Property<string>(e, "EmployeeId")))
            .ToListAsync();

        var pendingExpenses = dbExpenses.Where(e => e.Status == "Pending").ToList();

        var todayStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var approvedToday = dbExpenses.Count(e => e.Status == "Approved" && e.Date == todayStr);
        var rejectedToday = dbExpenses.Count(e => e.Status == "Rejected" && e.Date == todayStr);

        var mappedPending = await MapToManagerExpenseDtos(pendingExpenses);

        return new ManagerDashboardDto
        {
            PendingRequestsCount = pendingExpenses.Count,
            ApprovedTodayCount = approvedToday,
            RejectedTodayCount = rejectedToday,
            TotalPendingAmount = pendingExpenses.Sum(e => e.TotalAmount),
            RecentPendingRequests = mappedPending.OrderByDescending(e => e.Id).Take(5).ToList()
        };
    }

    public async Task<IEnumerable<ManagerExpenseDto>> GetPendingExpensesAsync()
    {
        var manager = await GetCurrentManagerProfileAsync();
        var reportees = await _context.UserProfiles.Where(u => u.ManagerId == manager.EmployeeId).Select(u => u.EmployeeId).ToListAsync();

        var pendingExpenses = await _context.Expenses
            .Include(e => e.Items)
            .Where(e => e.Status == "Pending" && reportees.Contains(EF.Property<string>(e, "EmployeeId")))
            .OrderByDescending(e => e.Id)
            .ToListAsync();

        return await MapToManagerExpenseDtos(pendingExpenses);
    }

    public async Task<ManagerExpenseDto?> GetExpenseByIdAsync(string id)
    {
        var expense = await _context.Expenses
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null) return null;

        var mapped = await MapToManagerExpenseDtos(new List<Expense> { expense });
        return mapped.FirstOrDefault();
    }

    public async Task<bool> ApproveExpenseAsync(string id, string notes)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null || expense.Status != "Pending") return false;

        var manager = await GetCurrentManagerProfileAsync();

        expense.Status = "Approved";
        expense.Notes = notes;
        expense.Date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var history = new ApprovalHistory
        {
            Id = $"HIS-{Guid.NewGuid()}",
            ExpenseId = id,
            Action = "Approved",
            PerformedBy = manager.Name,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Notes = string.IsNullOrWhiteSpace(notes) ? "Expense claim approved." : notes
        };
        _context.ApprovalHistories.Add(history);

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = $"Expense claim \"{expense.Title}\" of ${expense.TotalAmount:F2} approved by Manager",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = "success"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectExpenseAsync(string id, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return false;

        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null || expense.Status != "Pending") return false;

        var manager = await GetCurrentManagerProfileAsync();

        expense.Status = "Rejected";
        expense.Notes = reason;
        expense.Date = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var history = new ApprovalHistory
        {
            Id = $"HIS-{Guid.NewGuid()}",
            ExpenseId = id,
            Action = "Rejected",
            PerformedBy = manager.Name,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Notes = reason
        };
        _context.ApprovalHistories.Add(history);

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = $"Expense claim \"{expense.Title}\" of ${expense.TotalAmount:F2} was rejected by Manager",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = "danger"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ApprovalHistoryDto>> GetExpenseHistoryAsync(string id)
    {
        var history = await _context.ApprovalHistories
            .Where(h => h.ExpenseId == id)
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

        return history;
    }

    public async Task<IEnumerable<ApprovalHistoryDto>> GetGlobalHistoryAsync()
    {
        var manager = await GetCurrentManagerProfileAsync();
        var reportees = await _context.UserProfiles.Where(u => u.ManagerId == manager.EmployeeId).Select(u => u.EmployeeId).ToListAsync();

        var history = await _context.ApprovalHistories
            .Where(h => (h.Action == "Approved" || h.Action == "Rejected") &&
                        _context.Expenses.Any(e => e.Id == h.ExpenseId && reportees.Contains(EF.Property<string>(e, "EmployeeId"))))
            .OrderByDescending(h => h.Timestamp)
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

        return history;
    }

    public async Task<UserProfile> GetProfileAsync()
    {
        return await GetCurrentManagerProfileAsync();
    }

    public async Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        if (profile == null) return false;
        var existing = await GetCurrentManagerProfileAsync();

        existing.Name = profile.Name;
        existing.Email = profile.Email;
        if (!string.IsNullOrWhiteSpace(profile.AvatarUrl))
        {
            existing.AvatarUrl = profile.AvatarUrl;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
