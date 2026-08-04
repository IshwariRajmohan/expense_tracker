using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<List<AdminExpenseDto>> MapToAdminExpenseDtos(List<Expense> expenses)
    {
        var empIds = expenses.Select(e => _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "").Distinct().ToList();
        var profiles = await _context.UserProfiles.Where(u => empIds.Contains(u.EmployeeId)).ToDictionaryAsync(u => u.EmployeeId);

        var list = new List<AdminExpenseDto>();
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

            list.Add(new AdminExpenseDto
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

    public async Task<AdminDashboardDto> GetDashboardSummaryAsync()
    {
        var users = await _context.UserProfiles.ToListAsync();
        var empCount = users.Count(u => u.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase));
        var mgrCount = users.Count(u => u.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase));
        var actCount = users.Count(u => u.Role.Equals("Accountant", StringComparison.OrdinalIgnoreCase));

        var dbExpenses = await _context.Expenses.ToListAsync();
        var totalExp = dbExpenses.Count;
        var pending = dbExpenses.Count(e => e.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        var approved = dbExpenses.Count(e => e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase));
        var rejected = dbExpenses.Count(e => e.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase));
        var paid = dbExpenses.Count(e => e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase));
        var totalAmt = dbExpenses.Sum(e => e.TotalAmount);

        // Compute monthly chart data
        var monthlyChart = new List<ChartDataPointDto>();
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        foreach (var m in months)
        {
            var monthIndex = Array.IndexOf(months, m) + 1;
            var monthStr = monthIndex.ToString("D2");
            var value = dbExpenses.Where(e => (e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)) && e.Date.Contains($"-{monthStr}-")).Sum(e => e.TotalAmount);
            monthlyChart.Add(new ChartDataPointDto { Label = m, Value = value });
        }

        // Compute status breakdown
        var statuses = new[] { "Draft", "Pending", "Approved", "Rejected", "Paid" };
        var statusChart = statuses.Select(s => new StatusChartDataPointDto
        {
            Status = s,
            Count = dbExpenses.Count(e => e.Status.Equals(s, StringComparison.OrdinalIgnoreCase)),
            Amount = dbExpenses.Where(e => e.Status.Equals(s, StringComparison.OrdinalIgnoreCase)).Sum(e => e.TotalAmount)
        }).ToList();

        var activities = await _context.ActivityLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .ToListAsync();

        return new AdminDashboardDto
        {
            TotalEmployees = empCount,
            TotalManagers = mgrCount,
            TotalAccountants = actCount,
            TotalExpenses = totalExp,
            PendingCount = pending,
            ApprovedCount = approved,
            RejectedCount = rejected,
            PaidCount = paid,
            TotalExpenseAmount = totalAmt,
            MonthlyExpenseChartData = monthlyChart,
            StatusChartData = statusChart,
            RecentActivities = activities
        };
    }

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
    {
        var dbExpenses = await _context.Expenses.ToListAsync();
        var spentAmountMap = dbExpenses
            .Where(e => e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            .GroupBy(e => _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "")
            .ToDictionary(g => g.Key, g => g.Sum(e => e.TotalAmount));

        var users = await _context.UserProfiles.ToListAsync();
        return users.Select(u => {
            spentAmountMap.TryGetValue(u.EmployeeId, out var spent);
            return new AdminUserDto
            {
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                Department = u.Department,
                EmployeeId = u.EmployeeId,
                BudgetLimit = u.BudgetLimit,
                SpentAmount = spent,
                AvatarUrl = u.AvatarUrl
            };
        });
    }

    public async Task<IEnumerable<AdminExpenseDto>> GetAllExpensesAsync()
    {
        var expenses = await _context.Expenses
            .Include(e => e.Items)
            .OrderByDescending(e => e.Id)
            .ToListAsync();

        return await MapToAdminExpenseDtos(expenses);
    }

    public async Task<AdminExpenseDto?> GetExpenseByIdAsync(string id)
    {
        var expense = await _context.Expenses
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null) return null;

        var mapped = await MapToAdminExpenseDtos(new List<Expense> { expense });
        return mapped.FirstOrDefault();
    }

    public async Task<AdminReportsDto> GetReportsAsync()
    {
        var dbExpenses = await _context.Expenses.Include(e => e.Items).ToListAsync();
        var profiles = await _context.UserProfiles.ToDictionaryAsync(u => u.EmployeeId);

        var deptExpenses = dbExpenses
            .Select(e => {
                var empId = _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "";
                profiles.TryGetValue(empId, out var p);
                return new { Expense = e, Department = p?.Department ?? "Operations" };
            })
            .GroupBy(x => x.Department)
            .Select(g => new DepartmentReportDto
            {
                DepartmentName = g.Key,
                TotalAmount = g.Sum(x => x.Expense.TotalAmount),
                Count = g.Count(x => x.Expense.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || x.Expense.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
            })
            .ToList();

        var empExpenses = dbExpenses
            .Select(e => {
                var empId = _context.Entry(e).Property("EmployeeId").CurrentValue?.ToString() ?? "";
                profiles.TryGetValue(empId, out var p);
                return new { Expense = e, Profile = p };
            })
            .Where(x => x.Profile != null)
            .GroupBy(x => x.Profile!.EmployeeId)
            .Select(g => {
                var p = g.First().Profile!;
                return new EmployeeReportDto
                {
                    EmployeeId = p.EmployeeId,
                    EmployeeName = p.Name,
                    Role = p.Role,
                    Department = p.Department,
                    Count = g.Count(),
                    TotalAmount = g.Sum(x => x.Expense.TotalAmount)
                };
            })
            .ToList();

        var topSpenders = empExpenses
            .OrderByDescending(x => x.TotalAmount)
            .Take(5)
            .ToList();

        // Calculate monthly expense report
        var monthlyReport = new List<ChartDataPointDto>();
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        foreach (var m in months)
        {
            var monthIndex = Array.IndexOf(months, m) + 1;
            var monthStr = monthIndex.ToString("D2");
            var value = dbExpenses.Where(e => (e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)) && e.Date.Contains($"-{monthStr}-")).Sum(e => e.TotalAmount);
            monthlyReport.Add(new ChartDataPointDto { Label = m, Value = value });
        }

        // Calculate status wise expenses
        var statuses = new[] { "Draft", "Pending", "Approved", "Rejected", "Paid" };
        var statusWise = statuses.Select(s => new StatusChartDataPointDto
        {
            Status = s,
            Count = dbExpenses.Count(e => e.Status.Equals(s, StringComparison.OrdinalIgnoreCase)),
            Amount = dbExpenses.Where(e => e.Status.Equals(s, StringComparison.OrdinalIgnoreCase)).Sum(e => e.TotalAmount)
        }).ToList();

        return new AdminReportsDto
        {
            MonthlyExpenseReport = monthlyReport,
            DepartmentWiseExpenses = deptExpenses,
            EmployeeWiseExpenses = empExpenses,
            StatusWiseExpenses = statusWise,
            TopSpendingEmployees = topSpenders
        };
    }

    public async Task<IEnumerable<ApprovalHistoryDto>> GetGlobalWorkflowHistoryAsync()
    {
        return await _context.ApprovalHistories
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
    }

    public async Task<AdminFreezeDateDto> GetFreezeDateAsync()
    {
        var setting = await _context.FreezeDateSettings.FirstOrDefaultAsync(s => s.Id == 1);
        if (setting == null)
        {
            setting = new FreezeDateSetting { Id = 1, FreezeDay = 18 };
            _context.FreezeDateSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        bool isClosed = DateTime.UtcNow.Day > setting.FreezeDay;

        return new AdminFreezeDateDto
        {
            FreezeDay = setting.FreezeDay,
            IsClosed = isClosed,
            CurrentMonth = DateTime.UtcNow.ToString("MMMM")
        };
    }

    public async Task<bool> UpdateFreezeDateAsync(int day)
    {
        var setting = await _context.FreezeDateSettings.FirstOrDefaultAsync(s => s.Id == 1);
        if (setting == null)
        {
            setting = new FreezeDateSetting { Id = 1, FreezeDay = day };
            _context.FreezeDateSettings.Add(setting);
        }
        else
        {
            setting.FreezeDay = day;
        }

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = $"Admin updated freeze date to day {day}",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = "info"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AdminSettingsDto> GetSettingsAsync()
    {
        var settings = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == 1);
        if (settings == null)
        {
            settings = new SystemSettings
            {
                Id = 1,
                CompanyName = "FirstPay Corporate Services",
                CompanyAddress = "Level 21, Fintech Plaza, Istanbul, Turkey",
                CorporateCurrency = "USD ($)",
                SystemMode = "Production Mode (SQL Server Live)"
            };
            _context.SystemSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        var adminProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.EmployeeId == "FP-ADMIN-01");
        var profile = adminProfile != null ? new UserProfile
        {
            EmployeeId = adminProfile.EmployeeId,
            Name = adminProfile.Name,
            Email = adminProfile.Email,
            Role = adminProfile.Role,
            Department = adminProfile.Department,
            AvatarUrl = adminProfile.AvatarUrl
        } : new UserProfile
        {
            EmployeeId = "FP-ADMIN-01",
            Name = "System Admin",
            Email = "admin.hq@firstpay.com",
            Role = "Administrator",
            Department = "Operations",
            AvatarUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop"
        };

        return new AdminSettingsDto
        {
            CompanyName = settings.CompanyName,
            CompanyAddress = settings.CompanyAddress,
            CorporateCurrency = settings.CorporateCurrency,
            SystemMode = settings.SystemMode,
            AdminProfile = profile
        };
    }

    public async Task<bool> AddUserAsync(AdminUserDto user)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.EmployeeId) || string.IsNullOrWhiteSpace(user.Name))
            return false;

        var existing = await _context.UserProfiles.AnyAsync(u => u.EmployeeId == user.EmployeeId);
        if (existing) return false;

        var cred = new UserCredential
        {
            Username = user.Name.ToLower().Replace(" ", ""),
            Password = "123",
            DisplayName = user.Name,
            Role = user.Role
        };
        _context.UserCredentials.Add(cred);

        var profile = new UserProfile
        {
            EmployeeId = user.EmployeeId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Department = user.Department,
            BudgetLimit = user.BudgetLimit,
            SpentAmount = 0m,
            AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl) ? "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop" : user.AvatarUrl
        };
        _context.UserProfiles.Add(profile);

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = $"Admin created user: {user.Name} ({user.Role})",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = "info"
        });

        await _context.SaveChangesAsync();
        return true;
    }
}
