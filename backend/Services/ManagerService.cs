using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;

namespace backend.Services;

public class ManagerService : IManagerService
{
    private static readonly List<ManagerExpenseDto> _expenses = new();
    private static readonly List<ApprovalHistoryDto> _history = new();
    private static UserProfile _profile = new UserProfile
    {
        Name = "Ishwari Rajmohan",
        Email = "ishwari.r@firstpay.com",
        Role = "Department Manager",
        Department = "Engineering",
        EmployeeId = "FP-2024-001",
        BudgetLimit = 50000.00m,
        SpentAmount = 0.00m,
        AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop"
    };
    private static readonly object _lock = new();

    static ManagerService()
    {
        InitializeDummyData();
    }

    private static void InitializeDummyData()
    {
        var himeshwar = new EmployeeInfoDto
        {
            Name = "Himeshwar",
            Email = "himeshwar.s@firstpay.com",
            EmployeeId = "FP-2024-897",
            Department = "Engineering",
            AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop"
        };

        var aisha = new EmployeeInfoDto
        {
            Name = "Aisha Rahman",
            Email = "aisha.r@firstpay.com",
            EmployeeId = "FP-2024-912",
            Department = "Engineering",
            AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?q=80&w=256&auto=format&fit=crop"
        };

        var john = new EmployeeInfoDto
        {
            Name = "John Doe",
            Email = "john.d@firstpay.com",
            EmployeeId = "FP-2024-521",
            Department = "Marketing",
            AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=256&auto=format&fit=crop"
        };

        var sarah = new EmployeeInfoDto
        {
            Name = "Sarah Jenkins",
            Email = "sarah.j@firstpay.com",
            EmployeeId = "FP-2024-340",
            Department = "Sales",
            AvatarUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?q=80&w=256&auto=format&fit=crop"
        };

        var todayStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var yesterdayStr = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var twoDaysAgoStr = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd");
        var fiveDaysAgoStr = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd");

        // 12 dummy expenses
        var list = new List<ManagerExpenseDto>
        {
            new()
            {
                Id = "EXP-2001",
                Title = "AWS Cloud Infrastructure",
                Category = "Software & SaaS",
                Date = yesterdayStr,
                Description = "Hosting fees for the staging and testing environments.",
                Status = "Pending",
                Employee = aisha,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2001", Name = "EC2 Standard Instances", Category = "Software & SaaS", Cost = 120.00m, Quantity = 2 },
                    new() { Id = "ITM-2002", Name = "S3 Storage Allocation", Category = "Software & SaaS", Cost = 45.50m, Quantity = 1 },
                    new() { Id = "ITM-2003", Name = "RDS PostgreSQL Database", Category = "Software & SaaS", Cost = 150.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2002",
                Title = "Client Appreciation Lunch",
                Category = "Meals & Entertainment",
                Date = twoDaysAgoStr,
                Description = "Business lunch with stakeholders from Veripark.",
                Status = "Pending",
                Employee = john,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2004", Name = "Steakhouse Dinner", Category = "Meals & Entertainment", Cost = 75.00m, Quantity = 3 },
                    new() { Id = "ITM-2005", Name = "Beverages & Desserts", Category = "Meals & Entertainment", Cost = 15.00m, Quantity = 3 },
                    new() { Id = "ITM-2006", Name = "Valet Parking Services", Category = "Meals & Entertainment", Cost = 20.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2003",
                Title = "Tech Conference Travel",
                Category = "Travel",
                Date = fiveDaysAgoStr,
                Description = "Travel allowance and logistics for the Tech Summit 2026.",
                Status = "Pending",
                Employee = himeshwar,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2007", Name = "Flight Tickets (Roundtrip)", Category = "Travel", Cost = 450.00m, Quantity = 1 },
                    new() { Id = "ITM-2008", Name = "Hotel Accommodations", Category = "Travel", Cost = 120.00m, Quantity = 3 },
                    new() { Id = "ITM-2009", Name = "Airport Express Cab", Category = "Travel", Cost = 45.00m, Quantity = 2 }
                }
            },
            new()
            {
                Id = "EXP-2004",
                Title = "Marketing Campaign Software",
                Category = "Software & SaaS",
                Date = yesterdayStr,
                Description = "Design and automation subscriptions for next month's sales campaign.",
                Status = "Pending",
                Employee = sarah,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2010", Name = "HubSpot Professional", Category = "Software & SaaS", Cost = 250.00m, Quantity = 1 },
                    new() { Id = "ITM-2011", Name = "Canva Pro License", Category = "Software & SaaS", Cost = 15.00m, Quantity = 5 },
                    new() { Id = "ITM-2012", Name = "Figma Design Seat", Category = "Software & SaaS", Cost = 45.00m, Quantity = 3 }
                }
            },
            new()
            {
                Id = "EXP-2005",
                Title = "Office Supplies Restocking",
                Category = "Office Supplies",
                Date = todayStr,
                Description = "Purchased accessories for the hybrid setup in Room 4B.",
                Status = "Pending",
                Employee = john,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2013", Name = "Ergonomic Keyboards", Category = "Office Supplies", Cost = 89.00m, Quantity = 2 },
                    new() { Id = "ITM-2014", Name = "Magnetic Whiteboards", Category = "Office Supplies", Cost = 120.00m, Quantity = 1 },
                    new() { Id = "ITM-2015", Name = "Height-Adjustable Standing Desk", Category = "Office Supplies", Cost = 299.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2006",
                Title = "GitHub Enterprise Subscription",
                Category = "Software & SaaS",
                Date = todayStr,
                Description = "Developer tooling and AI coding assistant seat upgrades.",
                Status = "Pending",
                Employee = aisha,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2016", Name = "GitHub Copilot Enterprise", Category = "Software & SaaS", Cost = 19.00m, Quantity = 10 },
                    new() { Id = "ITM-2017", Name = "GitHub Advanced Security Seat", Category = "Software & SaaS", Cost = 49.00m, Quantity = 5 }
                }
            },
            new()
            {
                Id = "EXP-2007",
                Title = "Sales Presentation Catering",
                Category = "Meals & Entertainment",
                Date = todayStr,
                Description = "Snacks and refreshments for the regional quarterly sales pitch.",
                Status = "Pending",
                Employee = sarah,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2018", Name = "Breakfast Pastry Platters", Category = "Meals & Entertainment", Cost = 12.50m, Quantity = 15 },
                    new() { Id = "ITM-2019", Name = "Premium Coffee Urns", Category = "Meals & Entertainment", Cost = 30.00m, Quantity = 2 },
                    new() { Id = "ITM-2020", Name = "Catering Delivery Fee", Category = "Meals & Entertainment", Cost = 15.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2008",
                Title = "DevOps Tooling Licenses",
                Category = "Software & SaaS",
                Date = yesterdayStr,
                Description = "Performance monitoring and collaboration tool monthly quotas.",
                Status = "Pending",
                Employee = himeshwar,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2021", Name = "Datadog Pro Host", Category = "Software & SaaS", Cost = 15.00m, Quantity = 8 },
                    new() { Id = "ITM-2022", Name = "Slack Upgrade Plan", Category = "Software & SaaS", Cost = 8.00m, Quantity = 12 },
                    new() { Id = "ITM-2023", Name = "Zoom Business Accounts", Category = "Software & SaaS", Cost = 20.00m, Quantity = 2 }
                }
            },
            new()
            {
                Id = "EXP-2009",
                Title = "Client Travel to Boston",
                Category = "Travel",
                Date = fiveDaysAgoStr,
                Description = "Meeting with the acquisitions team.",
                Status = "Approved",
                Notes = "Pre-approved travel budget for sales team.",
                Employee = sarah,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2024", Name = "Amtrak Express Ticket", Category = "Travel", Cost = 180.00m, Quantity = 1 },
                    new() { Id = "ITM-2025", Name = "Taxi Cab Fares", Category = "Travel", Cost = 25.00m, Quantity = 4 },
                    new() { Id = "ITM-2026", Name = "Meals Allowance", Category = "Travel", Cost = 40.00m, Quantity = 3 }
                }
            },
            new()
            {
                Id = "EXP-2010",
                Title = "Software Books & Training",
                Category = "Others",
                Date = fiveDaysAgoStr,
                Description = "Professional textbooks and corporate training licenses.",
                Status = "Approved",
                Notes = "Approved educational reimbursement.",
                Employee = himeshwar,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2027", Name = "O'Reilly Book Collection", Category = "Others", Cost = 49.00m, Quantity = 2 },
                    new() { Id = "ITM-2028", Name = "Pluralsight Annual Sub", Category = "Software & SaaS", Cost = 299.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2011",
                Title = "Team Building Activity",
                Category = "Meals & Entertainment",
                Date = fiveDaysAgoStr,
                Description = "Team outing and refreshments.",
                Status = "Rejected",
                Notes = "Budget exceeded for unofficial events.",
                Employee = john,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2029", Name = "Escape Room Admission", Category = "Meals & Entertainment", Cost = 35.00m, Quantity = 8 },
                    new() { Id = "ITM-2030", Name = "Pizza Catering Order", Category = "Meals & Entertainment", Cost = 120.00m, Quantity = 1 }
                }
            },
            new()
            {
                Id = "EXP-2012",
                Title = "Urgent Hard Drive Replacement",
                Category = "Office Supplies",
                Date = todayStr,
                Description = "Replaced SSD for crashes on corporate laptop.",
                Status = "Approved",
                Notes = "Approved replacement hardware for developer continuity.",
                Employee = aisha,
                Items = new List<ExpenseItem>
                {
                    new() { Id = "ITM-2031", Name = "NVMe SSD 2TB", Category = "Office Supplies", Cost = 169.00m, Quantity = 2 },
                    new() { Id = "ITM-2032", Name = "Hard Drive Enclosure", Category = "Office Supplies", Cost = 25.00m, Quantity = 1 }
                }
            }
        };

        foreach (var e in list)
        {
            e.TotalAmount = e.Items.Sum(item => item.Cost * item.Quantity);
            _expenses.Add(e);

            // Populate timeline
            var submissionTime = DateTime.Parse(e.Date).AddHours(9).ToString("yyyy-MM-ddTHH:mm:ssZ");
            _history.Add(new ApprovalHistoryDto
            {
                Id = $"HIS-{Guid.NewGuid()}",
                ExpenseId = e.Id,
                Action = "Submitted",
                PerformedBy = e.Employee.Name,
                Timestamp = submissionTime,
                Notes = "Claim submitted for verification."
            });

            if (e.Status == "Approved")
            {
                var processedTime = DateTime.Parse(e.Date).AddHours(14).ToString("yyyy-MM-ddTHH:mm:ssZ");
                _history.Add(new ApprovalHistoryDto
                {
                    Id = $"HIS-{Guid.NewGuid()}",
                    ExpenseId = e.Id,
                    Action = "Approved",
                    PerformedBy = "Ishwari Rajmohan",
                    Timestamp = processedTime,
                    Notes = e.Notes ?? "Approved by Manager."
                });
            }
            else if (e.Status == "Rejected")
            {
                var processedTime = DateTime.Parse(e.Date).AddHours(15).ToString("yyyy-MM-ddTHH:mm:ssZ");
                _history.Add(new ApprovalHistoryDto
                {
                    Id = $"HIS-{Guid.NewGuid()}",
                    ExpenseId = e.Id,
                    Action = "Rejected",
                    PerformedBy = "Ishwari Rajmohan",
                    Timestamp = processedTime,
                    Notes = e.Notes ?? "Reason not specified."
                });
            }
        }
    }

    public Task<ManagerDashboardDto> GetDashboardSummaryAsync()
    {
        lock (_lock)
        {
            var pending = _expenses.Where(e => e.Status == "Pending").ToList();
            
            // Check counts today in local/UTC date
            var todayStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var approvedToday = _expenses.Count(e => e.Status == "Approved" && e.Date == todayStr);
            var rejectedToday = _expenses.Count(e => e.Status == "Rejected" && e.Date == todayStr);

            var dashboard = new ManagerDashboardDto
            {
                PendingRequestsCount = pending.Count,
                ApprovedTodayCount = approvedToday,
                RejectedTodayCount = rejectedToday,
                TotalPendingAmount = pending.Sum(e => e.TotalAmount),
                RecentPendingRequests = pending.OrderByDescending(e => e.Id).Take(5).ToList()
            };

            return Task.FromResult(dashboard);
        }
    }

    public Task<IEnumerable<ManagerExpenseDto>> GetPendingExpensesAsync()
    {
        lock (_lock)
        {
            var pending = _expenses.Where(e => e.Status == "Pending").OrderByDescending(e => e.Id);
            return Task.FromResult<IEnumerable<ManagerExpenseDto>>(pending.ToList());
        }
    }

    public Task<ManagerExpenseDto?> GetExpenseByIdAsync(string id)
    {
        lock (_lock)
        {
            var exp = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exp);
        }
    }

    public Task<bool> ApproveExpenseAsync(string id, string notes)
    {
        lock (_lock)
        {
            var exp = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (exp == null || exp.Status != "Pending")
            {
                return Task.FromResult(false);
            }

            exp.Status = "Approved";
            exp.Notes = notes;
            exp.Date = DateTime.UtcNow.ToString("yyyy-MM-dd"); // move date to today to show in "Approved Today"

            _history.Add(new ApprovalHistoryDto
            {
                Id = $"HIS-{Guid.NewGuid()}",
                ExpenseId = exp.Id,
                Action = "Approved",
                PerformedBy = "Ishwari Rajmohan",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Notes = string.IsNullOrWhiteSpace(notes) ? "Expense claim approved." : notes
            });

            return Task.FromResult(true);
        }
    }

    public Task<bool> RejectExpenseAsync(string id, string reason)
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return Task.FromResult(false);
            }

            var exp = _expenses.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (exp == null || exp.Status != "Pending")
            {
                return Task.FromResult(false);
            }

            exp.Status = "Rejected";
            exp.Notes = reason;
            exp.Date = DateTime.UtcNow.ToString("yyyy-MM-dd"); // move date to today to show in "Rejected Today"

            _history.Add(new ApprovalHistoryDto
            {
                Id = $"HIS-{Guid.NewGuid()}",
                ExpenseId = exp.Id,
                Action = "Rejected",
                PerformedBy = "Ishwari Rajmohan",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Notes = reason
            });

            return Task.FromResult(true);
        }
    }

    public Task<IEnumerable<ApprovalHistoryDto>> GetExpenseHistoryAsync(string id)
    {
        lock (_lock)
        {
            var events = _history.Where(h => h.ExpenseId.Equals(id, StringComparison.OrdinalIgnoreCase))
                                 .OrderBy(h => h.Timestamp);
            return Task.FromResult<IEnumerable<ApprovalHistoryDto>>(events.ToList());
        }
    }

    public Task<IEnumerable<ApprovalHistoryDto>> GetGlobalHistoryAsync()
    {
        lock (_lock)
        {
            // Global history of approvals/rejections: find expenses that are Approved or Rejected, 
            // and fetch their history records (specifically the audit actions)
            var auditActions = _history.Where(h => h.Action == "Approved" || h.Action == "Rejected")
                                       .OrderByDescending(h => h.Timestamp);
            return Task.FromResult<IEnumerable<ApprovalHistoryDto>>(auditActions.ToList());
        }
    }

    public Task<UserProfile> GetProfileAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_profile);
        }
    }

    public Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        lock (_lock)
        {
            if (profile == null) return Task.FromResult(false);
            _profile.Name = profile.Name;
            _profile.Email = profile.Email;
            if (!string.IsNullOrWhiteSpace(profile.AvatarUrl))
            {
                _profile.AvatarUrl = profile.AvatarUrl;
            }
            return Task.FromResult(true);
        }
    }
}
