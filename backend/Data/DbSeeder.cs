using Microsoft.EntityFrameworkCore;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace backend.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // 1. Ensure database is clean and migrations are applied (conditional on environment flag)
        var resetDb = Environment.GetEnvironmentVariable("RESET_DATABASE");
        if (!string.IsNullOrEmpty(resetDb) && resetDb.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            context.Database.EnsureDeleted();
        }
        context.Database.Migrate();

        // 2. Seed FreezeDateSettings (Conditional)
        if (!context.FreezeDateSettings.Any(f => f.Id == 1))
        {
            context.FreezeDateSettings.Add(new FreezeDateSetting { Id = 1, FreezeDay = 18 });
        }

        // 3. Seed SystemSettings (Conditional)
        if (!context.SystemSettings.Any(s => s.Id == 1))
        {
            context.SystemSettings.Add(new SystemSettings
            {
                Id = 1,
                CompanyName = "FirstPay Corporate Services",
                CompanyAddress = "Level 21, Fintech Plaza, Istanbul, Turkey",
                CorporateCurrency = "USD ($)",
                SystemMode = "Production Mode (SQL Server Live)"
            });
        }

        // 4. Seed UserCredentials (Incremental)
        var credentials = new List<UserCredential>
        {
            new() { Username = "himesh", Password = "123", DisplayName = "Himeshwar", Role = "Employee" },
            new() { Username = "ghiri", Password = "pass", DisplayName = "ghiri", Role = "Employee" },
            new() { Username = "manager", Password = "123", DisplayName = "Ishwari Rajmohan", Role = "Manager" },
            new() { Username = "manager2", Password = "123", DisplayName = "Robert Johnson", Role = "Manager" },
            new() { Username = "manager3", Password = "123", DisplayName = "Linda Martinez", Role = "Manager" },
            new() { Username = "accountant", Password = "123", DisplayName = "Accountant Office", Role = "Accountant" },
            new() { Username = "accountant2", Password = "123", DisplayName = "Finance Auditor", Role = "Accountant" },
            new() { Username = "admin", Password = "123", DisplayName = "System Admin", Role = "Admin" },
            new() { Username = "aisha", Password = "123", DisplayName = "Aisha Rahman", Role = "Employee" },
            new() { Username = "john", Password = "123", DisplayName = "John Doe", Role = "Employee" },
            new() { Username = "sarah", Password = "123", DisplayName = "Sarah Jenkins", Role = "Employee" },
            new() { Username = "michael", Password = "123", DisplayName = "Michael Brown", Role = "Employee" },
            new() { Username = "emily", Password = "123", DisplayName = "Emily Davis", Role = "Employee" },
            new() { Username = "david", Password = "123", DisplayName = "David Wilson", Role = "Employee" },
            new() { Username = "jessica", Password = "123", DisplayName = "Jessica Taylor", Role = "Employee" },
            new() { Username = "james", Password = "123", DisplayName = "James Thomas", Role = "Employee" },
            new() { Username = "sarvesh", Password = "123", DisplayName = "sarvesh", Role = "Employee" }
            

        };

        foreach (var cred in credentials)
        {
            if (!context.UserCredentials.Any(uc => uc.Username == cred.Username))
            {
                context.UserCredentials.Add(cred);
            }
        }

        // 5. Seed UserProfiles (Incremental - Managers first, then Employees)
        var managers = new List<UserProfile>
        {
            new() { EmployeeId = "FP-2024-001", Name = "Ishwari Rajmohan", Email = "ishwari.r@firstpay.com", Role = "Manager", Department = "Engineering", BudgetLimit = 50000.00m, SpentAmount = 15000.00m, AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop", ManagerId = null },
            new() { EmployeeId = "FP-2024-002", Name = "Robert Johnson", Email = "robert.j@firstpay.com", Role = "Manager", Department = "Marketing", BudgetLimit = 40000.00m, SpentAmount = 8000.00m, AvatarUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?q=80&w=256&auto=format&fit=crop", ManagerId = null },
            new() { EmployeeId = "FP-2024-003", Name = "Linda Martinez", Email = "linda.m@firstpay.com", Role = "Manager", Department = "Sales", BudgetLimit = 45000.00m, SpentAmount = 12000.00m, AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop", ManagerId = null },
            new() { EmployeeId = "FP-2024-010", Name = "Accountant Office", Email = "finance.audit@firstpay.com", Role = "Accountant", Department = "Finance", BudgetLimit = 0.00m, SpentAmount = 0.00m, AvatarUrl = "https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?q=80&w=256&auto=format&fit=crop", ManagerId = null },
            new() { EmployeeId = "FP-2024-011", Name = "Finance Auditor", Email = "auditor@firstpay.com", Role = "Accountant", Department = "Finance", BudgetLimit = 0.00m, SpentAmount = 0.00m, AvatarUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?q=80&w=256&auto=format&fit=crop", ManagerId = null },
            new() { EmployeeId = "FP-ADMIN-01", Name = "System Admin", Email = "admin.hq@firstpay.com", Role = "Admin", Department = "Operations", BudgetLimit = 0.00m, SpentAmount = 0.00m, AvatarUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop", ManagerId = null }
        };

        foreach (var manager in managers)
        {
            if (!context.UserProfiles.Any(up => up.EmployeeId == manager.EmployeeId))
            {
                context.UserProfiles.Add(manager);
            }
        }
        context.SaveChanges(); // Save changes so manager keys exist before children are checked/added

        var employees = new List<UserProfile>
        {
            new() { EmployeeId = "FP-2024-897", Name = "Himeshwar", Email = "himeshwar.s@firstpay.com", Role = "Senior Software Engineer", Department = "Engineering", BudgetLimit = 5000.00m, SpentAmount = 1600.00m, AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-001" },
            new() { EmployeeId = "FP-2024-111", Name = "ghiri", Email = "ghiri@firstpay.com", Role = "Employee", Department = "Engineering", BudgetLimit = 6000.00m, SpentAmount = 1200.00m, AvatarUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-001" },
            new() { EmployeeId = "FP-2024-912", Name = "Aisha Rahman", Email = "aisha.r@firstpay.com", Role = "Employee", Department = "Engineering", BudgetLimit = 8000.00m, SpentAmount = 2500.00m, AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-001" },
            new() { EmployeeId = "FP-2024-521", Name = "John Doe", Email = "john.d@firstpay.com", Role = "Employee", Department = "Marketing", BudgetLimit = 5000.00m, SpentAmount = 1800.00m, AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-002" },
            new() { EmployeeId = "FP-2024-340", Name = "Sarah Jenkins", Email = "sarah.j@firstpay.com", Role = "Employee", Department = "Sales", BudgetLimit = 10000.00m, SpentAmount = 4000.00m, AvatarUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-003" },
            new() { EmployeeId = "FP-2024-222", Name = "Michael Brown", Email = "michael.b@firstpay.com", Role = "Employee", Department = "Marketing", BudgetLimit = 5000.00m, SpentAmount = 500.00m, AvatarUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-002" },
            new() { EmployeeId = "FP-2024-333", Name = "Emily Davis", Email = "emily.d@firstpay.com", Role = "Employee", Department = "Sales", BudgetLimit = 8000.00m, SpentAmount = 1500.00m, AvatarUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-003" },
            new() { EmployeeId = "FP-2024-444", Name = "David Wilson", Email = "david.w@firstpay.com", Role = "Employee", Department = "Engineering", BudgetLimit = 7000.00m, SpentAmount = 900.00m, AvatarUrl = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-001" },
            new() { EmployeeId = "FP-2024-555", Name = "Jessica Taylor", Email = "jessica.t@firstpay.com", Role = "Employee", Department = "Marketing", BudgetLimit = 6000.00m, SpentAmount = 1100.00m, AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-002" },
            new() { EmployeeId = "FP-2024-666", Name = "James Thomas", Email = "james.t@firstpay.com", Role = "Employee", Department = "Sales", BudgetLimit = 9000.00m, SpentAmount = 2200.00m, AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=256&auto=format&fit=crop", ManagerId = "FP-2024-003" }
        };

        foreach (var employee in employees)
        {
            if (!context.UserProfiles.Any(up => up.EmployeeId == employee.EmployeeId))
            {
                context.UserProfiles.Add(employee);
            }
        }
        context.SaveChanges();

        // 6. Seed Expenses (using EF Entry to assign shadow property "EmployeeId")
        var expensesData = new List<(Expense Expense, string EmployeeId)>
        {
            (new Expense { Id = "EXP-1001", Title = "AWS Server Hosting (July 2026)", Category = "Software & SaaS", Date = "2026-07-20", Description = "Monthly cloud hosting charges for production microservices and database instances.", TotalAmount = 1250.00m, Status = "Approved", Notes = "Auto-approved under pre-authorized engineering server budget." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1002", Title = "Team dinner & Milestone Celebration", Category = "Meals & Entertainment", Date = "2026-07-22", Description = "Catering dinner for the team after successful deployment of the authentication gateway.", TotalAmount = 350.00m, Status = "Approved", Notes = "Receipt attached. Approved by Engineering Director." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1003", Title = "Flight tickets to Bengaluru Summit", Category = "Travel", Date = "2026-07-25", Description = "Round-trip flight booking to attend the FirstPay Annual Engineering Summit.", TotalAmount = 685.50m, Status = "Pending" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1004", Title = "Ergonomic Mechanical Keyboard", Category = "Office Supplies", Date = "2026-07-15", Description = "Purchase of keychron keyboard for workspace ergonomic enhancement.", TotalAmount = 150.00m, Status = "Rejected", Notes = "Rejected: Office furniture and keyboards must be routed through IT standard hardware procurement policy." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1005", Title = "Internet Reimbursement - June", Category = "Others", Date = "2026-06-30", Description = "Work from home high speed broadband connection reimbursement.", TotalAmount = 50.00m, Status = "Paid", Notes = "Approved and paid in June payroll cycle." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1006", Title = "Client Lunch meeting", Category = "Meals & Entertainment", Date = "2026-06-18", Description = "Business meal with prospects from FinTech Corp discussing payment gateway integration.", TotalAmount = 180.00m, Status = "Approved", Notes = "Receipt verified. Business justification acceptable." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1007", Title = "Udemy - GoLang Microservices Course", Category = "Others", Date = "2026-05-12", Description = "Online video course for backend architecture scaling upskilling.", TotalAmount = 25.00m, Status = "Approved", Notes = "Reimbursed under self-learning allowance budget." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1008", Title = "Dell USB-C Monitor Docking Hub", Category = "Office Supplies", Date = "2026-05-24", Description = "Multiport adapter for workstation dual monitor display setup.", TotalAmount = 110.00m, Status = "Approved" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1009", Title = "GitHub Copilot Individual - Q2", Category = "Software & SaaS", Date = "2026-04-01", Description = "AI programming assistant quarterly license subscription.", TotalAmount = 30.00m, Status = "Paid", Notes = "Paid. Direct corporate credit card reconciliation." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1010", Title = "Local Uber Rides to Client Site", Category = "Travel", Date = "2026-04-15", Description = "Commute fares for design reviews with API clients in Gurugram.", TotalAmount = 45.00m, Status = "Approved" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1011", Title = "Vitreous Whiteboard for Office desk", Category = "Office Supplies", Date = "2026-03-10", Description = "Desktop glass dry-erase panel for software design sketches.", TotalAmount = 60.00m, Status = "Rejected", Notes = "Rejected: Desk whiteboards must be requested through standard physical facilities desk allocation." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1012", Title = "Monthly Broadband Internet - Feb", Category = "Others", Date = "2026-02-28", Description = "WFH monthly broadband connectivity subscription fee.", TotalAmount = 50.00m, Status = "Paid" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1013", Title = "IntelliJ IDEA Professional annual subscription", Category = "Software & SaaS", Date = "2026-02-14", Description = "Annual developer tool license fee.", TotalAmount = 249.00m, Status = "Approved", Notes = "Approved under software department pre-cleared tool budget." }, "FP-2024-897"),
            (new Expense { Id = "EXP-1014", Title = "Draft: Wireless Ergonomic Mouse", Category = "Office Supplies", Date = "2026-07-26", Description = "Logitech MX Master mouse for daily development workstation comfort.", TotalAmount = 99.00m, Status = "Draft" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1015", Title = "Draft: Book - Designing Data-Intensive Applications", Category = "Others", Date = "2026-07-27", Description = "Hardcopy reference book for software engineering technical design.", TotalAmount = 45.00m, Status = "Draft" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1016", Title = "Local Cab Fares to Airport", Category = "Travel", Date = "2026-06-10", Description = "Travel cab for business travel to Delhi Airport.", TotalAmount = 75.00m, Status = "Approved" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1017", Title = "Client Tea & Snacks Catering", Category = "Meals & Entertainment", Date = "2026-03-25", Description = "Catered refreshments for external clients during design discussions.", TotalAmount = 40.00m, Status = "Approved" }, "FP-2024-897"),
            (new Expense { Id = "EXP-1018", Title = "Marketing Standees Paris", Category = "Office Supplies", Date = "2026-07-28", Description = "Standees printing for regional launch event.", TotalAmount = 220.00m, Status = "Paid", Notes = "Supplier invoice cleared.", PaymentDate = "2026-07-30T10:00:00Z" }, "FP-2024-912"),
            (new Expense { Id = "EXP-1019", Title = "Client Engagement Dinner London", Category = "Meals & Entertainment", Date = "2026-07-29", Description = "Dinner with prospective client representatives.", TotalAmount = 320.00m, Status = "Approved", Notes = "Cleared within standard guidelines." }, "FP-2024-340"),
            (new Expense { Id = "EXP-1020", Title = "Software IDE Subscription", Category = "Software & SaaS", Date = "2026-07-30", Description = "License fees for design tool suites.", TotalAmount = 150.00m, Status = "Pending" }, "FP-2024-521")
        };

        foreach (var data in expensesData)
        {
            if (!context.Expenses.Any(e => e.Id == data.Expense.Id))
            {
                var entry = context.Expenses.Add(data.Expense);
                entry.Property("EmployeeId").CurrentValue = data.EmployeeId;
            }
        }
        context.SaveChanges();

        // 7. Seed Expense Items
        var items = new List<ExpenseItem>
        {
            new() { Id = "ITM-1", ExpenseId = "EXP-1001", Name = "AWS EC2 - m5.xlarge instances", Category = "Software & SaaS", Cost = 600.00m, Quantity = 1 },
            new() { Id = "ITM-2", ExpenseId = "EXP-1001", Name = "AWS Aurora DB hosting", Category = "Software & SaaS", Cost = 450.00m, Quantity = 1 },
            new() { Id = "ITM-3", ExpenseId = "EXP-1001", Name = "AWS S3 storage fees", Category = "Software & SaaS", Cost = 200.00m, Quantity = 1 },
            new() { Id = "ITM-4", ExpenseId = "EXP-1002", Name = "Catering & Beverages (15 pax)", Category = "Meals & Entertainment", Cost = 350.00m, Quantity = 1 },
            new() { Id = "ITM-5", ExpenseId = "EXP-1003", Name = "Air India Flight (DEL-BLR-DEL)", Category = "Travel", Cost = 550.00m, Quantity = 1 },
            new() { Id = "ITM-6", ExpenseId = "EXP-1003", Name = "Airport Cab Transfer", Category = "Travel", Cost = 135.50m, Quantity = 1 },
            new() { Id = "ITM-7", ExpenseId = "EXP-1004", Name = "Keychron K2 Keyboard", Category = "Office Supplies", Cost = 99.00m, Quantity = 1 },
            new() { Id = "ITM-8", ExpenseId = "EXP-1004", Name = "Ergonomic Mouse pad", Category = "Office Supplies", Cost = 51.00m, Quantity = 1 },
            new() { Id = "ITM-9", ExpenseId = "EXP-1005", Name = "Airtel Fiber Broadband monthly plan", Category = "Others", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-10", ExpenseId = "EXP-1006", Name = "Business lunch at Taj Diner", Category = "Meals & Entertainment", Cost = 180.00m, Quantity = 1 },
            new() { Id = "ITM-11", ExpenseId = "EXP-1007", Name = "GoLang Microservices course license", Category = "Others", Cost = 25.00m, Quantity = 1 },
            new() { Id = "ITM-12", ExpenseId = "EXP-1008", Name = "Dell DA310 USB-C Adapter", Category = "Office Supplies", Cost = 110.00m, Quantity = 1 },
            new() { Id = "ITM-13", ExpenseId = "EXP-1009", Name = "GitHub Copilot subscription (April - June)", Category = "Software & SaaS", Cost = 10.00m, Quantity = 3 },
            new() { Id = "ITM-14", ExpenseId = "EXP-1010", Name = "Uber Go ride - Gurugram office", Category = "Travel", Cost = 45.00m, Quantity = 1 },
            new() { Id = "ITM-15", ExpenseId = "EXP-1011", Name = "Desktop Glass Whiteboard", Category = "Office Supplies", Cost = 60.00m, Quantity = 1 },
            new() { Id = "ITM-16", ExpenseId = "EXP-1012", Name = "Broadband fiber Internet bills", Category = "Others", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-17", ExpenseId = "EXP-1013", Name = "IntelliJ IDEA Ultimate Individual license", Category = "Software & SaaS", Cost = 249.00m, Quantity = 1 },
            new() { Id = "ITM-18", ExpenseId = "EXP-1014", Name = "Logitech MX Master 3S Mouse", Category = "Office Supplies", Cost = 99.00m, Quantity = 1 },
            new() { Id = "ITM-19", ExpenseId = "EXP-1015", Name = "Designing Data-Intensive Applications by Kleppmann", Category = "Others", Cost = 45.00m, Quantity = 1 },
            new() { Id = "ITM-20", ExpenseId = "EXP-1016", Name = "Airport cab ride transfer", Category = "Travel", Cost = 75.00m, Quantity = 1 },
            new() { Id = "ITM-21", ExpenseId = "EXP-1017", Name = "Beverages and Snacks", Category = "Meals & Entertainment", Cost = 40.00m, Quantity = 1 },
            new() { Id = "ITM-22", ExpenseId = "EXP-1018", Name = "Vinyl Roll Standees", Category = "Office Supplies", Cost = 110.00m, Quantity = 2 },
            new() { Id = "ITM-23", ExpenseId = "EXP-1002", Name = "Desserts & Pastries", Category = "Meals & Entertainment", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-24", ExpenseId = "EXP-1002", Name = "Beverages and Juices", Category = "Meals & Entertainment", Cost = 100.00m, Quantity = 1 },
            new() { Id = "ITM-25", ExpenseId = "EXP-1003", Name = "Hotel Stay (2 nights)", Category = "Travel", Cost = 150.00m, Quantity = 2 },
            new() { Id = "ITM-26", ExpenseId = "EXP-1004", Name = "Custom Wrist Rest", Category = "Office Supplies", Cost = 45.00m, Quantity = 1 },
            new() { Id = "ITM-27", ExpenseId = "EXP-1005", Name = "Router lease fee", Category = "Others", Cost = 10.00m, Quantity = 1 },
            new() { Id = "ITM-28", ExpenseId = "EXP-1006", Name = "Taxi ride Taj restaurant", Category = "Meals & Entertainment", Cost = 30.00m, Quantity = 1 },
            new() { Id = "ITM-29", ExpenseId = "EXP-1007", Name = "Companion booklet ebook", Category = "Others", Cost = 15.00m, Quantity = 1 },
            new() { Id = "ITM-30", ExpenseId = "EXP-1008", Name = "HDMI Premium Cable 3m", Category = "Office Supplies", Cost = 25.00m, Quantity = 2 },
            new() { Id = "ITM-31", ExpenseId = "EXP-1009", Name = "Copilot additional server node", Category = "Software & SaaS", Cost = 15.00m, Quantity = 1 },
            new() { Id = "ITM-32", ExpenseId = "EXP-1010", Name = "Uber ride return route", Category = "Travel", Cost = 45.00m, Quantity = 1 },
            new() { Id = "ITM-33", ExpenseId = "EXP-1011", Name = "Whiteboard dry erasers pack", Category = "Office Supplies", Cost = 10.00m, Quantity = 1 },
            new() { Id = "ITM-34", ExpenseId = "EXP-1013", Name = "IntelliJ database plugin key", Category = "Software & SaaS", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-35", ExpenseId = "EXP-1014", Name = "MX Mouse travel case", Category = "Office Supplies", Cost = 20.00m, Quantity = 1 },
            new() { Id = "ITM-36", ExpenseId = "EXP-1015", Name = "Reference documentation pdf copy", Category = "Others", Cost = 10.00m, Quantity = 1 },
            new() { Id = "ITM-37", ExpenseId = "EXP-1016", Name = "Toll gate tax", Category = "Travel", Cost = 15.00m, Quantity = 1 },
            new() { Id = "ITM-38", ExpenseId = "EXP-1017", Name = "Mineral water bottles box", Category = "Meals & Entertainment", Cost = 15.00m, Quantity = 1 },
            new() { Id = "ITM-39", ExpenseId = "EXP-1019", Name = "Restaurant Booking Charges", Category = "Meals & Entertainment", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-40", ExpenseId = "EXP-1019", Name = "Desserts & appetizers platter", Category = "Meals & Entertainment", Cost = 90.00m, Quantity = 3 },
            new() { Id = "ITM-41", ExpenseId = "EXP-1020", Name = "JetBrains IDE key", Category = "Software & SaaS", Cost = 150.00m, Quantity = 1 },
            new() { Id = "ITM-42", ExpenseId = "EXP-1001", Name = "Premium AWS Bandwidth Out", Category = "Software & SaaS", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-43", ExpenseId = "EXP-1001", Name = "Premium AWS CloudWatch metric logs", Category = "Software & SaaS", Cost = 40.00m, Quantity = 1 },
            new() { Id = "ITM-44", ExpenseId = "EXP-1002", Name = "Disposable plates & napkins", Category = "Meals & Entertainment", Cost = 25.00m, Quantity = 1 },
            new() { Id = "ITM-45", ExpenseId = "EXP-1003", Name = "Flight luggage checkin fee", Category = "Travel", Cost = 50.00m, Quantity = 1 },
            new() { Id = "ITM-46", ExpenseId = "EXP-1004", Name = "Ergonomic palm pillow", Category = "Office Supplies", Cost = 25.00m, Quantity = 1 },
            new() { Id = "ITM-47", ExpenseId = "EXP-1005", Name = "Broadband fiber installation charge", Category = "Others", Cost = 20.00m, Quantity = 1 },
            new() { Id = "ITM-48", ExpenseId = "EXP-1006", Name = "Service tips at lunch", Category = "Meals & Entertainment", Cost = 20.00m, Quantity = 1 },
            new() { Id = "ITM-49", ExpenseId = "EXP-1008", Name = "Workstation USB extension extension", Category = "Office Supplies", Cost = 15.00m, Quantity = 2 },
            new() { Id = "ITM-50", ExpenseId = "EXP-1010", Name = "Uber ride convenience charge", Category = "Travel", Cost = 5.00m, Quantity = 1 }
        };

        foreach (var item in items)
        {
            if (!context.ExpenseItems.Any(ei => ei.Id == item.Id))
            {
                context.ExpenseItems.Add(item);
            }
        }

        // 8. Seed ActivityLogs
        var logs = new List<ActivityLog>
        {
            new() { Id = "ACT-1", Action = "Draft claim initiated: \"Book - Designing Data-Intensive Applications\"", Timestamp = DateTime.UtcNow.AddMinutes(-5).ToString("o"), StatusType = "info" },
            new() { Id = "ACT-2", Action = "Draft claim initiated: \"Wireless Ergonomic Mouse\"", Timestamp = DateTime.UtcNow.AddHours(-1).ToString("o"), StatusType = "info" },
            new() { Id = "ACT-3", Action = "Submitted expense request \"Flight tickets to Bengaluru Summit\" for $685.50", Timestamp = DateTime.UtcNow.AddDays(-2).ToString("o"), StatusType = "warning" },
            new() { Id = "ACT-4", Action = "Expense claim \"Team dinner & Milestone Celebration\" of $350.00 approved by Manager", Timestamp = DateTime.UtcNow.AddDays(-5).ToString("o"), StatusType = "success" },
            new() { Id = "ACT-5", Action = "Expense claim \"Ergonomic Mechanical Keyboard\" of $150.00 was rejected", Timestamp = DateTime.UtcNow.AddDays(-12).ToString("o"), StatusType = "danger" },
            new() { Id = "ACT-6", Action = "Expense claim \"AWS Server Hosting (July 2026)\" of $1250.00 approved automatically", Timestamp = DateTime.UtcNow.AddDays(-7).ToString("o"), StatusType = "success" }
        };

        foreach (var log in logs)
        {
            if (!context.ActivityLogs.Any(al => al.Id == log.Id))
            {
                context.ActivityLogs.Add(log);
            }
        }

        // 9. Seed Approval Histories
        var history = new List<ApprovalHistory>
        {
            new() { Id = "APH-101", ExpenseId = "EXP-1001", Action = "Submitted", PerformedBy = "Himeshwar", Timestamp = "2026-07-20T09:00:00Z", Notes = "AWS server billing." },
            new() { Id = "APH-102", ExpenseId = "EXP-1001", Action = "Approved", PerformedBy = "Ishwari Rajmohan", Timestamp = "2026-07-20T14:30:00Z", Notes = "Budget cleared." },
            new() { Id = "APH-201", ExpenseId = "EXP-1002", Action = "Submitted", PerformedBy = "Himeshwar", Timestamp = "2026-07-22T10:00:00Z", Notes = "Dinner logs." },
            new() { Id = "APH-202", ExpenseId = "EXP-1002", Action = "Approved", PerformedBy = "Ishwari Rajmohan", Timestamp = "2026-07-22T15:30:00Z", Notes = "Approved by engineering lead." },
            new() { Id = "APH-301", ExpenseId = "EXP-1003", Action = "Submitted", PerformedBy = "Himeshwar", Timestamp = "2026-07-25T11:00:00Z", Notes = "Summit tickets flight." },
            new() { Id = "APH-401", ExpenseId = "EXP-1004", Action = "Submitted", PerformedBy = "Himeshwar", Timestamp = "2026-07-15T09:00:00Z", Notes = "Ergonomic hardware." },
            new() { Id = "APH-402", ExpenseId = "EXP-1004", Action = "Rejected", PerformedBy = "Ishwari Rajmohan", Timestamp = "2026-07-15T12:00:00Z", Notes = "Purchase standard equipment via IT procurement." },
            new() { Id = "APH-501", ExpenseId = "EXP-1005", Action = "Submitted", PerformedBy = "Himeshwar", Timestamp = "2026-06-30T09:00:00Z", Notes = "Internet connection." },
            new() { Id = "APH-502", ExpenseId = "EXP-1005", Action = "Approved", PerformedBy = "Ishwari Rajmohan", Timestamp = "2026-06-30T14:00:00Z", Notes = "WFH reimbursement cleared." },
            new() { Id = "APH-503", ExpenseId = "EXP-1005", Action = "Paid", PerformedBy = "Accountant Office", Timestamp = "2026-07-05T10:00:00Z", Notes = "Reimbursed in June payroll cycle." },
            new() { Id = "APH-1801", ExpenseId = "EXP-1018", Action = "Submitted", PerformedBy = "Aisha Rahman", Timestamp = "2026-07-28T09:00:00Z", Notes = "Marketing print materials." },
            new() { Id = "APH-1802", ExpenseId = "EXP-1018", Action = "Approved", PerformedBy = "Ishwari Rajmohan", Timestamp = "2026-07-28T14:00:00Z", Notes = "Approved under roadshow marketing budget." },
            new() { Id = "APH-1803", ExpenseId = "EXP-1018", Action = "Paid", PerformedBy = "Accountant Office", Timestamp = "2026-07-30T10:00:00Z", Notes = "Supplier invoice paid via bank wire." },
            new() { Id = "APH-1901", ExpenseId = "EXP-1019", Action = "Submitted", PerformedBy = "Sarah Jenkins", Timestamp = "2026-07-29T10:00:00Z", Notes = "Travel dining client." },
            new() { Id = "APH-1902", ExpenseId = "EXP-1019", Action = "Approved", PerformedBy = "Robert Johnson", Timestamp = "2026-07-29T16:00:00Z", Notes = "Approved sales hospitality." }
        };

        foreach (var hist in history)
        {
            if (!context.ApprovalHistories.Any(ah => ah.Id == hist.Id))
            {
                context.ApprovalHistories.Add(hist);
            }
        }

        context.SaveChanges();
    }
}
