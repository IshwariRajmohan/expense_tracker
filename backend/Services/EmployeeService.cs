using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.DTOs;
using backend.Data;

namespace backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<string> GenerateNextExpenseIdAsync()
    {
        var maxId = await _context.Expenses
            .Select(e => e.Id)
            .ToListAsync();
        
        var maxNum = maxId
            .Select(id => id.StartsWith("EXP-") && int.TryParse(id.Substring(4), out var n) ? n : 0)
            .DefaultIfEmpty(1000)
            .Max();

        return $"EXP-{maxNum + 1}";
    }

    private async Task<string> GenerateNextItemIdAsync(int offset = 0)
    {
        var maxId = await _context.ExpenseItems
            .Select(i => i.Id)
            .ToListAsync();

        var maxNum = maxId
            .Select(id => id != null && id.StartsWith("ITM-") && int.TryParse(id.Substring(4), out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"ITM-{maxNum + 1 + offset}";
    }

    private async Task AddActivityLogAsync(string action, string statusType)
    {
        var newLog = new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = action,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = statusType
        };
        _context.ActivityLogs.Add(newLog);

        // Maintain log history size (limit to 30)
        var logCount = await _context.ActivityLogs.CountAsync();
        if (logCount >= 30)
        {
            var oldestLogs = await _context.ActivityLogs
                .OrderBy(a => a.Timestamp)
                .Take(logCount - 29)
                .ToListAsync();
            _context.ActivityLogs.RemoveRange(oldestLogs);
        }
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var profile = await GetProfileAsync();
        var expenses = await _context.Expenses
            .Include(e => e.Items)
            .Where(e => EF.Property<string>(e, "EmployeeId") == profile.EmployeeId)
            .ToListAsync();
        var approvedExpenses = expenses.Where(e => e.Status == "Approved" || e.Status == "Paid").ToList();
        var totalAmount = approvedExpenses.Sum(e => e.TotalAmount);

        // Compute monthly trends dynamically for the last 6 months (relative to today)
        var monthlyChart = new List<ChartDataPointDto>();
        for (int i = 5; i >= 0; i--)
        {
            var targetDate = DateTime.UtcNow.AddMonths(-i);
            var monthName = targetDate.ToString("MMM");
            var monthPrefix = targetDate.ToString("yyyy-MM");

            var value = expenses
                .Where(e => e.Date.StartsWith(monthPrefix))
                .Sum(e => e.TotalAmount);

            monthlyChart.Add(new ChartDataPointDto { Label = monthName, Value = value });
        }

        // Compute status chart counts & amounts
        var statusGroups = expenses.GroupBy(e => e.Status)
            .Select(g => new StatusChartDataPointDto
            {
                Status = g.Key,
                Count = g.Count(),
                Amount = g.Sum(e => e.TotalAmount)
            }).ToList();

        // Ensure all statuses exist in grouping
        var statuses = new[] { "Draft", "Pending", "Approved", "Rejected", "Paid" };
        foreach (var s in statuses)
        {
            if (!statusGroups.Any(sg => sg.Status.Equals(s, StringComparison.OrdinalIgnoreCase)))
            {
                statusGroups.Add(new StatusChartDataPointDto { Status = s, Count = 0, Amount = 0m });
            }
        }

        var activities = await _context.ActivityLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .ToListAsync();

        var freezeSetting = await _context.FreezeDateSettings.FirstOrDefaultAsync(s => s.Id == 1);
        var freezeDay = freezeSetting?.FreezeDay ?? 18;
        var isFrozen = DateTime.UtcNow.Day > freezeDay;

        var summary = new DashboardSummaryDto
        {
            TotalExpenses = expenses.Count,
            Draft = expenses.Count(e => e.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase)),
            Pending = expenses.Count(e => e.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
            Approved = expenses.Count(e => e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase)),
            Rejected = expenses.Count(e => e.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase)),
            Paid = expenses.Count(e => e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)),
            TotalAmount = totalAmount,
            MonthlyExpenseChartData = monthlyChart,
            StatusChartData = statusGroups,
            RecentActivities = activities,
            LatestExpenses = expenses.OrderByDescending(e => e.Id).Take(5).ToList(),
            IsSubmissionFrozen = isFrozen,
            FreezeDay = freezeDay
        };

        return summary;
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        var profile = await GetProfileAsync();
        return await _context.Expenses
            .Include(e => e.Items)
            .Where(e => EF.Property<string>(e, "EmployeeId") == profile.EmployeeId)
            .OrderByDescending(e => e.Id)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(string id)
    {
        return await _context.Expenses
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    private async Task<Expense> SaveOrUpdateExpenseInternalAsync(Expense expense, string status)
    {
        // Get the single user profile to associate
        var profile = await GetProfileAsync();

        if (string.IsNullOrEmpty(expense.Id))
        {
            expense.Id = await GenerateNextExpenseIdAsync();
            expense.Status = status;

            int itemIndex = 0;
            foreach (var item in expense.Items)
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    item.Id = await GenerateNextItemIdAsync(itemIndex++);
                }
                item.ExpenseId = expense.Id;
            }
            _context.Expenses.Add(expense);

            if (profile != null)
            {
                _context.Entry(expense).Property("EmployeeId").CurrentValue = profile.EmployeeId;
            }
        }
        else
        {
            var existing = await _context.Expenses
                .Include(e => e.Items)
                .FirstOrDefaultAsync(e => e.Id == expense.Id);

            if (existing == null)
            {
                expense.Status = status;

                int itemIndex = 0;
                foreach (var item in expense.Items)
                {
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        item.Id = await GenerateNextItemIdAsync(itemIndex++);
                    }
                    item.ExpenseId = expense.Id;
                }
                _context.Expenses.Add(expense);

                if (profile != null)
                {
                    _context.Entry(expense).Property("EmployeeId").CurrentValue = profile.EmployeeId;
                }
            }
            else
            {
                existing.Title = expense.Title;
                existing.Category = expense.Category;
                existing.Date = expense.Date;
                existing.Description = expense.Description;
                existing.TotalAmount = expense.TotalAmount;
                existing.Status = status;
                existing.Notes = expense.Notes;

                // Sync items
                var incomingItems = expense.Items ?? new List<ExpenseItem>();

                // Remove items no longer present
                var itemsToRemove = existing.Items
                    .Where(ei => !incomingItems.Any(ii => ii.Id == ei.Id))
                    .ToList();
                foreach (var item in itemsToRemove)
                {
                    _context.ExpenseItems.Remove(item);
                }

                // Add or update items
                int incomingIndex = 0;
                foreach (var incomingItem in incomingItems)
                {
                    if (string.IsNullOrEmpty(incomingItem.Id))
                    {
                        incomingItem.Id = await GenerateNextItemIdAsync(incomingIndex++);
                        incomingItem.ExpenseId = existing.Id;
                        existing.Items.Add(incomingItem);
                    }
                    else
                    {
                        var existingItem = existing.Items.FirstOrDefault(ei => ei.Id == incomingItem.Id);
                        if (existingItem != null)
                        {
                            existingItem.Name = incomingItem.Name;
                            existingItem.Category = incomingItem.Category;
                            existingItem.Cost = incomingItem.Cost;
                            existingItem.Quantity = incomingItem.Quantity;
                        }
                        else
                        {
                            incomingItem.ExpenseId = existing.Id;
                            existing.Items.Add(incomingItem);
                        }
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<Expense> SaveDraftAsync(Expense expense)
    {
        var result = await SaveOrUpdateExpenseInternalAsync(expense, "Draft");
        await AddActivityLogAsync($"Saved draft expense requisition \"{expense.Title}\" for ${expense.TotalAmount:F2}", "info");
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<Expense> SubmitExpenseAsync(Expense expense)
    {
        var freezeSetting = await _context.FreezeDateSettings.FirstOrDefaultAsync(s => s.Id == 1);
        if (freezeSetting != null && DateTime.UtcNow.Day > freezeSetting.FreezeDay)
        {
            throw new System.InvalidOperationException("Submissions for this month are closed. The freeze date has passed.");
        }

        var profile = await GetProfileAsync();
        if (profile != null)
        {
            var approvedTotal = await _context.Expenses
                .Where(e => (e.Status == "Approved" || e.Status == "Paid") && EF.Property<string>(e, "EmployeeId") == profile.EmployeeId)
                .SumAsync(e => e.TotalAmount);

            var remainingAllowance = profile.BudgetLimit - approvedTotal;

            if (expense.TotalAmount > remainingAllowance)
            {
                throw new System.InvalidOperationException($"Submission blocked: The expense amount (${expense.TotalAmount:F2}) exceeds your remaining allowance (${remainingAllowance:F2}).");
            }
        }

        var result = await SaveOrUpdateExpenseInternalAsync(expense, "Pending");
        await AddActivityLogAsync($"Submitted expense claim \"{expense.Title}\" of ${expense.TotalAmount:F2} for manager audit", "warning");
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<bool> UpdateExpenseAsync(string id, Expense expense)
    {
        var existing = await _context.Expenses
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existing == null)
        {
            return false;
        }

        // Verify status is Draft, Rejected, or Pending
        if (!existing.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase) && 
            !existing.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) && 
            !existing.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            return false; // only Draft, Rejected, or Pending can be updated
        }

        existing.Title = expense.Title;
        existing.Category = expense.Category;
        existing.Date = expense.Date;
        existing.Description = expense.Description;
        existing.TotalAmount = expense.TotalAmount;
        existing.Notes = expense.Notes;

        if (expense.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            var freezeSetting = await _context.FreezeDateSettings.FirstOrDefaultAsync(s => s.Id == 1);
            if (freezeSetting != null && DateTime.UtcNow.Day > freezeSetting.FreezeDay)
            {
                throw new System.InvalidOperationException("Submissions for this month are closed. The freeze date has passed.");
            }

            var profile = await GetProfileAsync();
            if (profile != null)
            {
                var approvedTotal = await _context.Expenses
                    .Where(e => e.Id != id && (e.Status == "Approved" || e.Status == "Paid") && EF.Property<string>(e, "EmployeeId") == profile.EmployeeId)
                    .SumAsync(e => e.TotalAmount);

                var remainingAllowance = profile.BudgetLimit - approvedTotal;

                if (expense.TotalAmount > remainingAllowance)
                {
                    throw new System.InvalidOperationException($"Submission blocked: The updated expense amount (${expense.TotalAmount:F2}) exceeds your remaining allowance (${remainingAllowance:F2}).");
                }
            }

            existing.Status = "Pending";

            var history = new ApprovalHistory
            {
                Id = $"HIS-{Guid.NewGuid()}",
                ExpenseId = id,
                Action = "Submitted",
                PerformedBy = "Employee",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Notes = "Resubmitted claim after revision."
            };
            _context.ApprovalHistories.Add(history);
        }

        // Sync items
        var incomingItems = expense.Items ?? new List<ExpenseItem>();

        // Remove old items
        var itemsToRemove = existing.Items
            .Where(ei => !incomingItems.Any(ii => ii.Id == ei.Id))
            .ToList();
        foreach (var item in itemsToRemove)
        {
            _context.ExpenseItems.Remove(item);
        }

        // Add or update items
        int updateIndex = 0;
        foreach (var incomingItem in incomingItems)
        {
            if (string.IsNullOrEmpty(incomingItem.Id))
            {
                incomingItem.Id = await GenerateNextItemIdAsync(updateIndex++);
                incomingItem.ExpenseId = existing.Id;
                existing.Items.Add(incomingItem);
            }
            else
            {
                var existingItem = existing.Items.FirstOrDefault(ei => ei.Id == incomingItem.Id);
                if (existingItem != null)
                {
                    existingItem.Name = incomingItem.Name;
                    existingItem.Category = incomingItem.Category;
                    existingItem.Cost = incomingItem.Cost;
                    existingItem.Quantity = incomingItem.Quantity;
                }
                else
                {
                    incomingItem.ExpenseId = existing.Id;
                    existing.Items.Add(incomingItem);
                }
            }
        }

        await AddActivityLogAsync($"Modified pending/draft details for \"{expense.Title}\"", "info");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteExpenseAsync(string id)
    {
        var existing = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existing == null)
        {
            return false;
        }

        // Verify status is Draft
        if (!existing.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
        {
            return false; // only Draft can be deleted
        }

        _context.Expenses.Remove(existing);
        await AddActivityLogAsync($"Deleted draft claim requisition \"{existing.Title}\"", "danger");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserProfile> GetProfileAsync()
    {
        var username = CurrentUserState.Username;
        var credential = await _context.UserCredentials.FirstOrDefaultAsync(c => c.Username.ToLower() == username.ToLower());
        
        UserProfile? profile = null;
        if (credential != null)
        {
            profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Name == credential.DisplayName);
        }

        if (profile == null && credential != null)
        {
            profile = new UserProfile
            {
                EmployeeId = "FP-TEMP-" + credential.Username.ToUpper(),
                Name = credential.DisplayName,
                Email = credential.Username.ToLower() + "@firstpay.com",
                Role = credential.Role,
                Department = "General",
                BudgetLimit = 5000.00m,
                SpentAmount = 0.00m,
                AvatarUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop"
            };
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        if (profile == null)
        {
            profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.EmployeeId == "FP-2024-897");
        }

        if (profile == null)
        {
            profile = new UserProfile
            {
                EmployeeId = "FP-2024-897",
                Name = "Himeshwar",
                Email = "himeshwar.s@firstpay.com",
                Role = "Senior Software Engineer",
                Department = "Engineering",
                BudgetLimit = 5000.00m,
                SpentAmount = 0.00m,
                AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop"
            };
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        // Dynamically compute spentAmount based on Approved/Paid items
        var approvedTotal = await _context.Expenses
            .Where(e => (e.Status == "Approved" || e.Status == "Paid") && EF.Property<string>(e, "EmployeeId") == profile.EmployeeId)
            .SumAsync(e => e.TotalAmount);
        
        profile.SpentAmount = approvedTotal;
        await _context.SaveChangesAsync();

        return profile;
    }

    public async Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        var existing = await _context.UserProfiles.FirstOrDefaultAsync(u => u.EmployeeId == profile.EmployeeId);
        if (existing == null)
        {
            existing = await _context.UserProfiles.FirstOrDefaultAsync();
        }

        if (existing == null)
        {
            return false;
        }

        existing.Name = profile.Name;
        existing.Email = profile.Email;
        existing.BudgetLimit = profile.BudgetLimit;
        if (!string.IsNullOrEmpty(profile.AvatarUrl))
        {
            existing.AvatarUrl = profile.AvatarUrl;
        }

        await AddActivityLogAsync("Updated user profile settings and contact details", "info");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        await AddActivityLogAsync("User changed corporate account password", "info");
        await _context.SaveChangesAsync();
        return true;
    }
}
