using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.DTOs;

namespace backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly List<Expense> _expenses = new();
    private readonly List<ActivityLog> _activities = new();
    private UserProfile _profile = new();
    private int _nextExpenseIdNum = 1018;

    public EmployeeService()
    {
        InitializeDummyData();
    }

    private void InitializeDummyData()
    {
        // 1. Seed User Profile
        _profile = new UserProfile
        {
            Name = "Himeshwar",
            Email = "himeshwar.s@firstpay.com",
            Role = "Senior Software Engineer",
            Department = "Engineering",
            EmployeeId = "FP-2024-897",
            BudgetLimit = 5000.00m,
            SpentAmount = 1600.00m, // will be dynamically computed in API if needed, but set as a default
            AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop"
        };

        // 2. Seed 17 Sample Expenses spanning Feb 2026 to July 2026
        _expenses.AddRange(new List<Expense>
        {
            new Expense
            {
                Id = "EXP-1001",
                Title = "AWS Server Hosting (July 2026)",
                Category = "Software & SaaS",
                Date = "2026-07-20",
                Description = "Monthly cloud hosting charges for production microservices and database instances.",
                Status = "Approved",
                TotalAmount = 1250.00m,
                Notes = "Auto-approved under pre-authorized engineering server budget.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-1", Name = "AWS EC2 - m5.xlarge instances", Category = "Software & SaaS", Cost = 600.00m, Quantity = 1 },
                    new ExpenseItem { Id = "ITM-2", Name = "AWS Aurora DB hosting", Category = "Software & SaaS", Cost = 450.00m, Quantity = 1 },
                    new ExpenseItem { Id = "ITM-3", Name = "AWS S3 storage fees", Category = "Software & SaaS", Cost = 200.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1002",
                Title = "Team dinner & Milestone Celebration",
                Category = "Meals & Entertainment",
                Date = "2026-07-22",
                Description = "Catering dinner for the team after successful deployment of the authentication gateway.",
                Status = "Approved",
                TotalAmount = 350.00m,
                Notes = "Receipt attached. Approved by Engineering Director.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-4", Name = "Catering & Beverages (15 pax)", Category = "Meals & Entertainment", Cost = 350.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1003",
                Title = "Flight tickets to Bengaluru Summit",
                Category = "Travel",
                Date = "2026-07-25",
                Description = "Round-trip flight booking to attend the FirstPay Annual Engineering Summit.",
                Status = "Pending",
                TotalAmount = 685.50m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-5", Name = "Air India Flight (DEL-BLR-DEL)", Category = "Travel", Cost = 550.00m, Quantity = 1 },
                    new ExpenseItem { Id = "ITM-6", Name = "Airport Cab Transfer", Category = "Travel", Cost = 135.50m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1004",
                Title = "Ergonomic Mechanical Keyboard",
                Category = "Office Supplies",
                Date = "2026-07-15",
                Description = "Purchase of keychron keyboard for workspace ergonomic enhancement.",
                Status = "Rejected",
                TotalAmount = 150.00m,
                Notes = "Rejected: Office furniture and keyboards must be routed through IT standard hardware procurement policy.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-7", Name = "Keychron K2 Keyboard", Category = "Office Supplies", Cost = 99.00m, Quantity = 1 },
                    new ExpenseItem { Id = "ITM-8", Name = "Ergonomic Mouse pad", Category = "Office Supplies", Cost = 51.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1005",
                Title = "Internet Reimbursement - June",
                Category = "Others",
                Date = "2026-06-30",
                Description = "Work from home high speed broadband connection reimbursement.",
                Status = "Paid",
                TotalAmount = 50.00m,
                Notes = "Approved and paid in June payroll cycle.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-9", Name = "Airtel Fiber Broadband monthly plan", Category = "Others", Cost = 50.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1006",
                Title = "Client Lunch meeting",
                Category = "Meals & Entertainment",
                Date = "2026-06-18",
                Description = "Business meal with prospects from FinTech Corp discussing payment gateway integration.",
                Status = "Approved",
                TotalAmount = 180.00m,
                Notes = "Receipt verified. Business justification acceptable.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-10", Name = "Business lunch at Taj Diner", Category = "Meals & Entertainment", Cost = 180.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1007",
                Title = "Udemy - GoLang Microservices Course",
                Category = "Others",
                Date = "2026-05-12",
                Description = "Online video course for backend architecture scaling upskilling.",
                Status = "Approved",
                TotalAmount = 25.00m,
                Notes = "Reimbursed under self-learning allowance budget.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-11", Name = "GoLang Microservices course license", Category = "Others", Cost = 25.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1008",
                Title = "Dell USB-C Monitor Docking Hub",
                Category = "Office Supplies",
                Date = "2026-05-24",
                Description = "Multiport adapter for workstation dual monitor display setup.",
                Status = "Approved",
                TotalAmount = 110.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-12", Name = "Dell DA310 USB-C Adapter", Category = "Office Supplies", Cost = 110.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1009",
                Title = "GitHub Copilot Individual - Q2",
                Category = "Software & SaaS",
                Date = "2026-04-01",
                Description = "AI programming assistant quarterly license subscription.",
                Status = "Paid",
                TotalAmount = 30.00m,
                Notes = "Paid. Direct corporate credit card reconciliation.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-13", Name = "GitHub Copilot subscription (April - June)", Category = "Software & SaaS", Cost = 10.00m, Quantity = 3 }
                }
            },
            new Expense
            {
                Id = "EXP-1010",
                Title = "Local Uber Rides to Client Site",
                Category = "Travel",
                Date = "2026-04-15",
                Description = "Commute fares for design reviews with API clients in Gurugram.",
                Status = "Approved",
                TotalAmount = 45.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-14", Name = "Uber Go ride - Gurugram office", Category = "Travel", Cost = 45.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1011",
                Title = "Vitreous Whiteboard for Office desk",
                Category = "Office Supplies",
                Date = "2026-03-10",
                Description = "Desktop glass dry-erase panel for software design sketches.",
                Status = "Rejected",
                TotalAmount = 60.00m,
                Notes = "Rejected: Desk whiteboards must be requested through standard physical facilities desk allocation.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-15", Name = "Desktop Glass Whiteboard", Category = "Office Supplies", Cost = 60.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1012",
                Title = "Monthly Broadband Internet - Feb",
                Category = "Others",
                Date = "2026-02-28",
                Description = "WFH monthly broadband connectivity subscription fee.",
                Status = "Paid",
                TotalAmount = 50.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-16", Name = "Broadband fiber Internet bills", Category = "Others", Cost = 50.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1013",
                Title = "IntelliJ IDEA Professional annual subscription",
                Category = "Software & SaaS",
                Date = "2026-02-14",
                Description = "Annual developer tool license fee.",
                Status = "Approved",
                TotalAmount = 249.00m,
                Notes = "Approved under software department pre-cleared tool budget.",
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-17", Name = "IntelliJ IDEA Ultimate Individual license", Category = "Software & SaaS", Cost = 249.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1014",
                Title = "Draft: Wireless Ergonomic Mouse",
                Category = "Office Supplies",
                Date = "2026-07-26",
                Description = "Logitech MX Master mouse for daily development workstation comfort.",
                Status = "Draft",
                TotalAmount = 99.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-18", Name = "Logitech MX Master 3S Mouse", Category = "Office Supplies", Cost = 99.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1015",
                Title = "Draft: Book - Designing Data-Intensive Applications",
                Category = "Others",
                Date = "2026-07-27",
                Description = "Hardcopy reference book for software engineering technical design.",
                Status = "Draft",
                TotalAmount = 45.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-19", Name = "Designing Data-Intensive Applications by Kleppmann", Category = "Others", Cost = 45.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1016",
                Title = "Local Cab Fares to Airport",
                Category = "Travel",
                Date = "2026-06-10",
                Description = "Travel cab for business travel to Delhi Airport.",
                Status = "Approved",
                TotalAmount = 75.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-20", Name = "Airport cab ride transfer", Category = "Travel", Cost = 75.00m, Quantity = 1 }
                }
            },
            new Expense
            {
                Id = "EXP-1017",
                Title = "Client Tea & Snacks Catering",
                Category = "Meals & Entertainment",
                Date = "2026-03-25",
                Description = "Catered refreshments for external clients during design discussions.",
                Status = "Approved",
                TotalAmount = 40.00m,
                Items = new List<ExpenseItem>
                {
                    new ExpenseItem { Id = "ITM-21", Name = "Beverages and Snacks", Category = "Meals & Entertainment", Cost = 40.00m, Quantity = 1 }
                }
            }
        });

        // 3. Seed Activity Logs
        _activities.AddRange(new List<ActivityLog>
        {
            new ActivityLog
            {
                Id = "ACT-1",
                Action = "Draft claim initiated: \"Book - Designing Data-Intensive Applications\"",
                Timestamp = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "info"
            },
            new ActivityLog
            {
                Id = "ACT-2",
                Action = "Draft claim initiated: \"Wireless Ergonomic Mouse\"",
                Timestamp = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "info"
            },
            new ActivityLog
            {
                Id = "ACT-3",
                Action = "Submitted expense request \"Flight tickets to Bengaluru Summit\" for $685.50",
                Timestamp = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "warning"
            },
            new ActivityLog
            {
                Id = "ACT-4",
                Action = "Expense claim \"Team dinner & Milestone Celebration\" of $350.00 approved by Manager",
                Timestamp = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "success"
            },
            new ActivityLog
            {
                Id = "ACT-5",
                Action = "Expense claim \"Ergonomic Mechanical Keyboard\" of $150.00 was rejected",
                Timestamp = DateTime.UtcNow.AddDays(-12).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "danger"
            },
            new ActivityLog
            {
                Id = "ACT-6",
                Action = "Expense claim \"AWS Server Hosting (July 2026)\" of $1250.00 approved automatically",
                Timestamp = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                StatusType = "success"
            }
        });
    }

    public Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var approvedExpenses = _expenses.Where(e => e.Status == "Approved" || e.Status == "Paid").ToList();
        var totalAmount = approvedExpenses.Sum(e => e.TotalAmount);

        // Compute monthly trends dynamically for the last 6 months (Feb to July 2026)
        var monthsList = new[] { "Feb", "Mar", "Apr", "May", "Jun", "Jul" };
        var monthlyChart = new List<ChartDataPointDto>();
        
        // Let's hardcode monthly distributions or aggregate from dates
        foreach (var m in monthsList)
        {
            decimal value = 0;
            switch (m)
            {
                case "Feb":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-02")).Sum(e => e.TotalAmount);
                    break;
                case "Mar":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-03")).Sum(e => e.TotalAmount);
                    break;
                case "Apr":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-04")).Sum(e => e.TotalAmount);
                    break;
                case "May":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-05")).Sum(e => e.TotalAmount);
                    break;
                case "Jun":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-06")).Sum(e => e.TotalAmount);
                    break;
                case "Jul":
                    value = _expenses.Where(e => (e.Status == "Approved" || e.Status == "Paid") && e.Date.StartsWith("2026-07")).Sum(e => e.TotalAmount);
                    break;
            }
            monthlyChart.Add(new ChartDataPointDto { Label = m, Value = value });
        }

        // Compute status chart counts & amounts
        var statusGroups = _expenses.GroupBy(e => e.Status)
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

        var summary = new DashboardSummaryDto
        {
            TotalExpenses = _expenses.Count,
            Draft = _expenses.Count(e => e.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase)),
            Pending = _expenses.Count(e => e.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
            Approved = _expenses.Count(e => e.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase)),
            Rejected = _expenses.Count(e => e.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase)),
            Paid = _expenses.Count(e => e.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase)),
            TotalAmount = totalAmount,
            MonthlyExpenseChartData = monthlyChart,
            StatusChartData = statusGroups,
            RecentActivities = _activities.Take(5).ToList(),
            LatestExpenses = _expenses.OrderByDescending(e => e.Id).Take(5).ToList()
        };

        return Task.FromResult(summary);
    }

    public Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        // Return sorted by date/ID descending
        return Task.FromResult<IEnumerable<Expense>>(_expenses.OrderByDescending(e => e.Id));
    }

    public Task<Expense?> GetExpenseByIdAsync(string id)
    {
        var expense = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(expense);
    }

    public Task<Expense> SaveDraftAsync(Expense expense)
    {
        if (string.IsNullOrEmpty(expense.Id))
        {
            expense.Id = $"EXP-{_nextExpenseIdNum++}";
        }
        
        expense.Status = "Draft";
        
        // Remove older record with same ID if updating, otherwise add to front
        var existing = _expenses.FirstOrDefault(e => e.Id.Equals(expense.Id, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            _expenses.Remove(existing);
        }
        _expenses.Insert(0, expense);

        AddActivityLog($"Saved draft expense requisition \"{expense.Title}\" for ${expense.TotalAmount:F2}", "info");
        return Task.FromResult(expense);
    }

    public Task<Expense> SubmitExpenseAsync(Expense expense)
    {
        if (string.IsNullOrEmpty(expense.Id))
        {
            expense.Id = $"EXP-{_nextExpenseIdNum++}";
        }
        
        expense.Status = "Pending";

        var existing = _expenses.FirstOrDefault(e => e.Id.Equals(expense.Id, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            _expenses.Remove(existing);
        }
        _expenses.Insert(0, expense);

        AddActivityLog($"Submitted expense claim \"{expense.Title}\" of ${expense.TotalAmount:F2} for manager audit", "warning");
        return Task.FromResult(expense);
    }

    public Task<bool> UpdateExpenseAsync(string id, Expense expense)
    {
        var existing = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (existing == null)
        {
            return Task.FromResult(false);
        }

        // Verify status is Draft or Rejected
        if (!existing.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase) && 
            !existing.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false); // bad request (only Draft or Rejected can be updated)
        }

        // Update fields
        existing.Title = expense.Title;
        existing.Category = expense.Category;
        existing.Date = expense.Date;
        existing.Description = expense.Description;
        existing.Items = expense.Items;
        existing.TotalAmount = expense.TotalAmount;
        existing.Notes = expense.Notes;

        AddActivityLog($"Modified pending/draft details for \"{expense.Title}\"", "info");
        return Task.FromResult(true);
    }

    public Task<bool> DeleteExpenseAsync(string id)
    {
        var existing = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (existing == null)
        {
            return Task.FromResult(false);
        }

        // Verify status is Draft
        if (!existing.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false); // bad request (only Draft can be deleted)
        }

        _expenses.Remove(existing);
        AddActivityLog($"Deleted draft claim requisition \"{existing.Title}\"", "danger");
        return Task.FromResult(true);
    }

    public Task<UserProfile> GetProfileAsync()
    {
        // Dynamically compute spentAmount based on Approved/Paid items
        var approvedTotal = _expenses.Where(e => e.Status == "Approved" || e.Status == "Paid").Sum(e => e.TotalAmount);
        _profile.SpentAmount = approvedTotal;

        return Task.FromResult(_profile);
    }

    public Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        _profile.Name = profile.Name;
        _profile.Email = profile.Email;
        _profile.BudgetLimit = profile.BudgetLimit;
        if (!string.IsNullOrEmpty(profile.AvatarUrl))
        {
            _profile.AvatarUrl = profile.AvatarUrl;
        }

        AddActivityLog("Updated user profile settings and contact details", "info");
        return Task.FromResult(true);
    }

    public Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        AddActivityLog("User changed corporate account password", "info");
        return Task.FromResult(true);
    }

    private void AddActivityLog(string action, string statusType)
    {
        var newLog = new ActivityLog
        {
            Id = $"ACT-{DateTime.UtcNow.Ticks}",
            Action = action,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            StatusType = statusType
        };
        _activities.Insert(0, newLog);
        if (_activities.Count > 30)
        {
            _activities.RemoveAt(_activities.Count - 1);
        }
    }
}
