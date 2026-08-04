-- =========================================================================
-- 1. Create the Database
-- =========================================================================
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ExpenseTrackerDb')
BEGIN
    CREATE DATABASE ExpenseTrackerDb;
END
GO

USE ExpenseTrackerDb;
GO

-- =========================================================================
-- 2. Drop existing tables if they exist (in dependency order)
-- =========================================================================
IF OBJECT_ID('dbo.ExpenseItems', 'U') IS NOT NULL DROP TABLE dbo.ExpenseItems;
IF OBJECT_ID('dbo.Expenses', 'U') IS NOT NULL DROP TABLE dbo.Expenses;
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NOT NULL DROP TABLE dbo.UserProfiles;
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NOT NULL DROP TABLE dbo.ActivityLogs;
GO

-- =========================================================================
-- 3. Create Tables
-- =========================================================================

-- Create UserProfiles table
CREATE TABLE dbo.UserProfiles (
    EmployeeId VARCHAR(50) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    BudgetLimit DECIMAL(18, 2) NOT NULL,
    SpentAmount DECIMAL(18, 2) NOT NULL,
    AvatarUrl NVARCHAR(500) NOT NULL
);

-- Create Expenses table
CREATE TABLE dbo.Expenses (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    EmployeeId VARCHAR(50) NULL FOREIGN KEY REFERENCES dbo.UserProfiles(EmployeeId) ON DELETE SET NULL,
    Title NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Date VARCHAR(10) NOT NULL, -- YYYY-MM-DD format matching backend.Models.Expense
    Description NVARCHAR(MAX) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    Notes NVARCHAR(MAX) NULL
);

-- Create ExpenseItems table
CREATE TABLE dbo.ExpenseItems (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    ExpenseId VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES dbo.Expenses(Id) ON DELETE CASCADE,
    Name NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Cost DECIMAL(18, 2) NOT NULL,
    Quantity INT NOT NULL
);

-- Create ActivityLogs table
CREATE TABLE dbo.ActivityLogs (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    Action NVARCHAR(MAX) NOT NULL,
    Timestamp VARCHAR(30) NOT NULL, -- ISO UTC Date String format
    StatusType NVARCHAR(50) NOT NULL DEFAULT 'info'
);
GO

-- =========================================================================
-- 4. Insert Dummy / Seed Data
-- =========================================================================

-- Insert UserProfile
INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl)
VALUES (
    'FP-2024-897', 
    N'Himeshwar', 
    N'himeshwar.s@firstpay.com', 
    N'Senior Software Engineer', 
    N'Engineering', 
    5000.00, 
    1600.00, 
    N'https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop'
);

-- Insert Expenses (linked to employee 'FP-2024-897')
INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes)
VALUES 
('EXP-1001', 'FP-2024-897', N'AWS Server Hosting (July 2026)', N'Software & SaaS', '2026-07-20', N'Monthly cloud hosting charges for production microservices and database instances.', 1250.00, N'Approved', N'Auto-approved under pre-authorized engineering server budget.'),
('EXP-1002', 'FP-2024-897', N'Team dinner & Milestone Celebration', N'Meals & Entertainment', '2026-07-22', N'Catering dinner for the team after successful deployment of the authentication gateway.', 350.00, N'Approved', N'Receipt attached. Approved by Engineering Director.'),
('EXP-1003', 'FP-2024-897', N'Flight tickets to Bengaluru Summit', N'Travel', '2026-07-25', N'Round-trip flight booking to attend the FirstPay Annual Engineering Summit.', 685.50, N'Pending', NULL),
('EXP-1004', 'FP-2024-897', N'Ergonomic Mechanical Keyboard', N'Office Supplies', '2026-07-15', N'Purchase of keychron keyboard for workspace ergonomic enhancement.', 150.00, N'Rejected', N'Rejected: Office furniture and keyboards must be routed through IT standard hardware procurement policy.'),
('EXP-1005', 'FP-2024-897', N'Internet Reimbursement - June', N'Others', '2026-06-30', N'Work from home high speed broadband connection reimbursement.', 50.00, N'Paid', N'Approved and paid in June payroll cycle.'),
('EXP-1006', 'FP-2024-897', N'Client Lunch meeting', N'Meals & Entertainment', '2026-06-18', N'Business meal with prospects from FinTech Corp discussing payment gateway integration.', 180.00, N'Approved', N'Receipt verified. Business justification acceptable.'),
('EXP-1007', 'FP-2024-897', N'Udemy - GoLang Microservices Course', N'Others', '2026-05-12', N'Online video course for backend architecture scaling upskilling.', 25.00, N'Approved', N'Reimbursed under self-learning allowance budget.'),
('EXP-1008', 'FP-2024-897', N'Dell USB-C Monitor Docking Hub', N'Office Supplies', '2026-05-24', N'Multiport adapter for workstation dual monitor display setup.', 110.00, N'Approved', NULL),
('EXP-1009', 'FP-2024-897', N'GitHub Copilot Individual - Q2', N'Software & SaaS', '2026-04-01', N'AI programming assistant quarterly license subscription.', 30.00, N'Paid', N'Paid. Direct corporate credit card reconciliation.'),
('EXP-1010', 'FP-2024-897', N'Local Uber Rides to Client Site', N'Travel', '2026-04-15', N'Commute fares for design reviews with API clients in Gurugram.', 45.00, N'Approved', NULL),
('EXP-1011', 'FP-2024-897', N'Vitreous Whiteboard for Office desk', N'Office Supplies', '2026-03-10', N'Desktop glass dry-erase panel for software design sketches.', 60.00, N'Rejected', N'Rejected: Desk whiteboards must be requested through standard physical facilities desk allocation.'),
('EXP-1012', 'FP-2024-897', N'Monthly Broadband Internet - Feb', N'Others', '2026-02-28', N'WFH monthly broadband connectivity subscription fee.', 50.00, N'Paid', NULL),
('EXP-1013', 'FP-2024-897', N'IntelliJ IDEA Professional annual subscription', N'Software & SaaS', '2026-02-14', N'Annual developer tool license fee.', 249.00, N'Approved', N'Approved under software department pre-cleared tool budget.'),
('EXP-1014', 'FP-2024-897', N'Draft: Wireless Ergonomic Mouse', N'Office Supplies', '2026-07-26', N'Logitech MX Master mouse for daily development workstation comfort.', 99.00, N'Draft', NULL),
('EXP-1015', 'FP-2024-897', N'Draft: Book - Designing Data-Intensive Applications', N'Others', '2026-07-27', N'Hardcopy reference book for software engineering technical design.', 45.00, N'Draft', NULL),
('EXP-1016', 'FP-2024-897', N'Local Cab Fares to Airport', N'Travel', '2026-06-10', N'Travel cab for business travel to Delhi Airport.', 75.00, N'Approved', NULL),
('EXP-1017', 'FP-2024-897', N'Client Tea & Snacks Catering', N'Meals & Entertainment', '2026-03-25', N'Catered refreshments for external clients during design discussions.', 40.00, N'Approved', NULL);

-- Insert ExpenseItems (linked to corresponding Expense IDs)
INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity)
VALUES
('ITM-1', 'EXP-1001', N'AWS EC2 - m5.xlarge instances', N'Software & SaaS', 600.00, 1),
('ITM-2', 'EXP-1001', N'AWS Aurora DB hosting', N'Software & SaaS', 450.00, 1),
('ITM-3', 'EXP-1001', N'AWS S3 storage fees', N'Software & SaaS', 200.00, 1),
('ITM-4', 'EXP-1002', N'Catering & Beverages (15 pax)', N'Meals & Entertainment', 350.00, 1),
('ITM-5', 'EXP-1003', N'Air India Flight (DEL-BLR-DEL)', N'Travel', 550.00, 1),
('ITM-6', 'EXP-1003', N'Airport Cab Transfer', N'Travel', 135.50, 1),
('ITM-7', 'EXP-1004', N'Keychron K2 Keyboard', N'Office Supplies', 99.00, 1),
('ITM-8', 'EXP-1004', N'Ergonomic Mouse pad', N'Office Supplies', 51.00, 1),
('ITM-9', 'EXP-1005', N'Airtel Fiber Broadband monthly plan', N'Others', 50.00, 1),
('ITM-10', 'EXP-1006', N'Business lunch at Taj Diner', N'Meals & Entertainment', 180.00, 1),
('ITM-11', 'EXP-1007', N'GoLang Microservices course license', N'Others', 25.00, 1),
('ITM-12', 'EXP-1008', N'Dell DA310 USB-C Adapter', N'Office Supplies', 110.00, 1),
('ITM-13', 'EXP-1009', N'GitHub Copilot subscription (April - June)', N'Software & SaaS', 10.00, 3),
('ITM-14', 'EXP-1010', N'Uber Go ride - Gurugram office', N'Travel', 45.00, 1),
('ITM-15', 'EXP-1011', N'Desktop Glass Whiteboard', N'Office Supplies', 60.00, 1),
('ITM-16', 'EXP-1012', N'Broadband fiber Internet bills', N'Others', 50.00, 1),
('ITM-17', 'EXP-1013', N'IntelliJ IDEA Ultimate Individual license', N'Software & SaaS', 249.00, 1),
('ITM-18', 'EXP-1014', N'Logitech MX Master 3S Mouse', N'Office Supplies', 99.00, 1),
('ITM-19', 'EXP-1015', N'Designing Data-Intensive Applications by Kleppmann', N'Others', 45.00, 1),
('ITM-20', 'EXP-1016', N'Airport cab ride transfer', N'Travel', 75.00, 1),
('ITM-21', 'EXP-1017', N'Beverages and Snacks', N'Meals & Entertainment', 40.00, 1);

-- Insert ActivityLogs (relative to current UTC execution time)
INSERT INTO dbo.ActivityLogs (Id, Action, Timestamp, StatusType)
VALUES
('ACT-1', N'Draft claim initiated: "Book - Designing Data-Intensive Applications"', CONVERT(VARCHAR(19), DATEADD(MINUTE, -5, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-2', N'Draft claim initiated: "Wireless Ergonomic Mouse"', CONVERT(VARCHAR(19), DATEADD(HOUR, -1, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-3', N'Submitted expense request "Flight tickets to Bengaluru Summit" for $685.50', CONVERT(VARCHAR(19), DATEADD(DAY, -2, GETUTCDATE()), 126) + 'Z', 'warning'),
('ACT-4', N'Expense claim "Team dinner & Milestone Celebration" of $350.00 approved by Manager', CONVERT(VARCHAR(19), DATEADD(DAY, -5, GETUTCDATE()), 126) + 'Z', 'success'),
('ACT-5', N'Expense claim "Ergonomic Mechanical Keyboard" of $150.00 was rejected', CONVERT(VARCHAR(19), DATEADD(DAY, -12, GETUTCDATE()), 126) + 'Z', 'danger'),
('ACT-6', N'Expense claim "AWS Server Hosting (July 2026)" of $1250.00 approved automatically', CONVERT(VARCHAR(19), DATEADD(DAY, -7, GETUTCDATE()), 126) + 'Z', 'success');
GO
-- =========================================================================
-- 1. Create the Database
-- =========================================================================
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ExpenseTrackerDb')
BEGIN
    CREATE DATABASE ExpenseTrackerDb;
END
GO

USE ExpenseTrackerDb;
GO

-- =========================================================================
-- 2. Drop existing tables if they exist (in dependency order)
-- =========================================================================
IF OBJECT_ID('dbo.ExpenseItems', 'U') IS NOT NULL DROP TABLE dbo.ExpenseItems;
IF OBJECT_ID('dbo.Expenses', 'U') IS NOT NULL DROP TABLE dbo.Expenses;
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NOT NULL DROP TABLE dbo.UserProfiles;
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NOT NULL DROP TABLE dbo.ActivityLogs;
GO

-- =========================================================================
-- 3. Create Tables
-- =========================================================================

-- Create UserProfiles table
CREATE TABLE dbo.UserProfiles (
    EmployeeId VARCHAR(50) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    BudgetLimit DECIMAL(18, 2) NOT NULL,
    SpentAmount DECIMAL(18, 2) NOT NULL,
    AvatarUrl NVARCHAR(500) NOT NULL
);

-- Create Expenses table
CREATE TABLE dbo.Expenses (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    EmployeeId VARCHAR(50) NULL FOREIGN KEY REFERENCES dbo.UserProfiles(EmployeeId) ON DELETE SET NULL,
    Title NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Date VARCHAR(10) NOT NULL, -- YYYY-MM-DD format matching backend.Models.Expense
    Description NVARCHAR(MAX) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    Notes NVARCHAR(MAX) NULL
);

-- Create ExpenseItems table
CREATE TABLE dbo.ExpenseItems (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    ExpenseId VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES dbo.Expenses(Id) ON DELETE CASCADE,
    Name NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Cost DECIMAL(18, 2) NOT NULL,
    Quantity INT NOT NULL
);

-- Create ActivityLogs table
CREATE TABLE dbo.ActivityLogs (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    Action NVARCHAR(MAX) NOT NULL,
    Timestamp VARCHAR(30) NOT NULL, -- ISO UTC Date String format
    StatusType NVARCHAR(50) NOT NULL DEFAULT 'info'
);
GO

-- =========================================================================
-- 4. Insert Dummy / Seed Data
-- =========================================================================

-- Insert UserProfile
INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl)
VALUES (
    'FP-2024-897', 
    N'Himeshwar', 
    N'himeshwar.s@firstpay.com', 
    N'Senior Software Engineer', 
    N'Engineering', 
    5000.00, 
    1600.00, 
    N'https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop'
);

-- Insert Expenses (linked to employee 'FP-2024-897')
INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes)
VALUES 
('EXP-1001', 'FP-2024-897', N'AWS Server Hosting (July 2026)', N'Software & SaaS', '2026-07-20', N'Monthly cloud hosting charges for production microservices and database instances.', 1250.00, N'Approved', N'Auto-approved under pre-authorized engineering server budget.'),
('EXP-1002', 'FP-2024-897', N'Team dinner & Milestone Celebration', N'Meals & Entertainment', '2026-07-22', N'Catering dinner for the team after successful deployment of the authentication gateway.', 350.00, N'Approved', N'Receipt attached. Approved by Engineering Director.'),
('EXP-1003', 'FP-2024-897', N'Flight tickets to Bengaluru Summit', N'Travel', '2026-07-25', N'Round-trip flight booking to attend the FirstPay Annual Engineering Summit.', 685.50, N'Pending', NULL),
('EXP-1004', 'FP-2024-897', N'Ergonomic Mechanical Keyboard', N'Office Supplies', '2026-07-15', N'Purchase of keychron keyboard for workspace ergonomic enhancement.', 150.00, N'Rejected', N'Rejected: Office furniture and keyboards must be routed through IT standard hardware procurement policy.'),
('EXP-1005', 'FP-2024-897', N'Internet Reimbursement - June', N'Others', '2026-06-30', N'Work from home high speed broadband connection reimbursement.', 50.00, N'Paid', N'Approved and paid in June payroll cycle.'),
('EXP-1006', 'FP-2024-897', N'Client Lunch meeting', N'Meals & Entertainment', '2026-06-18', N'Business meal with prospects from FinTech Corp discussing payment gateway integration.', 180.00, N'Approved', N'Receipt verified. Business justification acceptable.'),
('EXP-1007', 'FP-2024-897', N'Udemy - GoLang Microservices Course', N'Others', '2026-05-12', N'Online video course for backend architecture scaling upskilling.', 25.00, N'Approved', N'Reimbursed under self-learning allowance budget.'),
('EXP-1008', 'FP-2024-897', N'Dell USB-C Monitor Docking Hub', N'Office Supplies', '2026-05-24', N'Multiport adapter for workstation dual monitor display setup.', 110.00, N'Approved', NULL),
('EXP-1009', 'FP-2024-897', N'GitHub Copilot Individual - Q2', N'Software & SaaS', '2026-04-01', N'AI programming assistant quarterly license subscription.', 30.00, N'Paid', N'Paid. Direct corporate credit card reconciliation.'),
('EXP-1010', 'FP-2024-897', N'Local Uber Rides to Client Site', N'Travel', '2026-04-15', N'Commute fares for design reviews with API clients in Gurugram.', 45.00, N'Approved', NULL),
('EXP-1011', 'FP-2024-897', N'Vitreous Whiteboard for Office desk', N'Office Supplies', '2026-03-10', N'Desktop glass dry-erase panel for software design sketches.', 60.00, N'Rejected', N'Rejected: Desk whiteboards must be requested through standard physical facilities desk allocation.'),
('EXP-1012', 'FP-2024-897', N'Monthly Broadband Internet - Feb', N'Others', '2026-02-28', N'WFH monthly broadband connectivity subscription fee.', 50.00, N'Paid', NULL),
('EXP-1013', 'FP-2024-897', N'IntelliJ IDEA Professional annual subscription', N'Software & SaaS', '2026-02-14', N'Annual developer tool license fee.', 249.00, N'Approved', N'Approved under software department pre-cleared tool budget.'),
('EXP-1014', 'FP-2024-897', N'Draft: Wireless Ergonomic Mouse', N'Office Supplies', '2026-07-26', N'Logitech MX Master mouse for daily development workstation comfort.', 99.00, N'Draft', NULL),
('EXP-1015', 'FP-2024-897', N'Draft: Book - Designing Data-Intensive Applications', N'Others', '2026-07-27', N'Hardcopy reference book for software engineering technical design.', 45.00, N'Draft', NULL),
('EXP-1016', 'FP-2024-897', N'Local Cab Fares to Airport', N'Travel', '2026-06-10', N'Travel cab for business travel to Delhi Airport.', 75.00, N'Approved', NULL),
('EXP-1017', 'FP-2024-897', N'Client Tea & Snacks Catering', N'Meals & Entertainment', '2026-03-25', N'Catered refreshments for external clients during design discussions.', 40.00, N'Approved', NULL);

-- Insert ExpenseItems (linked to corresponding Expense IDs)
INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity)
VALUES
('ITM-1', 'EXP-1001', N'AWS EC2 - m5.xlarge instances', N'Software & SaaS', 600.00, 1),
('ITM-2', 'EXP-1001', N'AWS Aurora DB hosting', N'Software & SaaS', 450.00, 1),
('ITM-3', 'EXP-1001', N'AWS S3 storage fees', N'Software & SaaS', 200.00, 1),
('ITM-4', 'EXP-1002', N'Catering & Beverages (15 pax)', N'Meals & Entertainment', 350.00, 1),
('ITM-5', 'EXP-1003', N'Air India Flight (DEL-BLR-DEL)', N'Travel', 550.00, 1),
('ITM-6', 'EXP-1003', N'Airport Cab Transfer', N'Travel', 135.50, 1),
('ITM-7', 'EXP-1004', N'Keychron K2 Keyboard', N'Office Supplies', 99.00, 1),
('ITM-8', 'EXP-1004', N'Ergonomic Mouse pad', N'Office Supplies', 51.00, 1),
('ITM-9', 'EXP-1005', N'Airtel Fiber Broadband monthly plan', N'Others', 50.00, 1),
('ITM-10', 'EXP-1006', N'Business lunch at Taj Diner', N'Meals & Entertainment', 180.00, 1),
('ITM-11', 'EXP-1007', N'GoLang Microservices course license', N'Others', 25.00, 1),
('ITM-12', 'EXP-1008', N'Dell DA310 USB-C Adapter', N'Office Supplies', 110.00, 1),
('ITM-13', 'EXP-1009', N'GitHub Copilot subscription (April - June)', N'Software & SaaS', 10.00, 3),
('ITM-14', 'EXP-1010', N'Uber Go ride - Gurugram office', N'Travel', 45.00, 1),
('ITM-15', 'EXP-1011', N'Desktop Glass Whiteboard', N'Office Supplies', 60.00, 1),
('ITM-16', 'EXP-1012', N'Broadband fiber Internet bills', N'Others', 50.00, 1),
('ITM-17', 'EXP-1013', N'IntelliJ IDEA Ultimate Individual license', N'Software & SaaS', 249.00, 1),
('ITM-18', 'EXP-1014', N'Logitech MX Master 3S Mouse', N'Office Supplies', 99.00, 1),
('ITM-19', 'EXP-1015', N'Designing Data-Intensive Applications by Kleppmann', N'Others', 45.00, 1),
('ITM-20', 'EXP-1016', N'Airport cab ride transfer', N'Travel', 75.00, 1),
('ITM-21', 'EXP-1017', N'Beverages and Snacks', N'Meals & Entertainment', 40.00, 1);

-- Insert ActivityLogs (relative to current UTC execution time)
INSERT INTO dbo.ActivityLogs (Id, Action, Timestamp, StatusType)
VALUES
('ACT-1', N'Draft claim initiated: "Book - Designing Data-Intensive Applications"', CONVERT(VARCHAR(19), DATEADD(MINUTE, -5, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-2', N'Draft claim initiated: "Wireless Ergonomic Mouse"', CONVERT(VARCHAR(19), DATEADD(HOUR, -1, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-3', N'Submitted expense request "Flight tickets to Bengaluru Summit" for $685.50', CONVERT(VARCHAR(19), DATEADD(DAY, -2, GETUTCDATE()), 126) + 'Z', 'warning'),
('ACT-4', N'Expense claim "Team dinner & Milestone Celebration" of $350.00 approved by Manager', CONVERT(VARCHAR(19), DATEADD(DAY, -5, GETUTCDATE()), 126) + 'Z', 'success'),
('ACT-5', N'Expense claim "Ergonomic Mechanical Keyboard" of $150.00 was rejected', CONVERT(VARCHAR(19), DATEADD(DAY, -12, GETUTCDATE()), 126) + 'Z', 'danger'),
('ACT-6', N'Expense claim "AWS Server Hosting (July 2026)" of $1250.00 approved automatically', CONVERT(VARCHAR(19), DATEADD(DAY, -7, GETUTCDATE()), 126) + 'Z', 'success');
GO
-- =========================================================================
-- 1. Create the Database
-- =========================================================================
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ExpenseTrackerDb')
BEGIN
    CREATE DATABASE ExpenseTrackerDb;
END
GO

USE ExpenseTrackerDb;
GO

-- =========================================================================
-- 2. Drop existing tables if they exist (in dependency order)
-- =========================================================================
IF OBJECT_ID('dbo.ExpenseItems', 'U') IS NOT NULL DROP TABLE dbo.ExpenseItems;
IF OBJECT_ID('dbo.Expenses', 'U') IS NOT NULL DROP TABLE dbo.Expenses;
IF OBJECT_ID('dbo.UserProfiles', 'U') IS NOT NULL DROP TABLE dbo.UserProfiles;
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NOT NULL DROP TABLE dbo.ActivityLogs;
GO

-- =========================================================================
-- 3. Create Tables
-- =========================================================================

-- Create UserProfiles table
CREATE TABLE dbo.UserProfiles (
    EmployeeId VARCHAR(50) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    BudgetLimit DECIMAL(18, 2) NOT NULL,
    SpentAmount DECIMAL(18, 2) NOT NULL,
    AvatarUrl NVARCHAR(500) NOT NULL
);

-- Create Expenses table
CREATE TABLE dbo.Expenses (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    EmployeeId VARCHAR(50) NULL FOREIGN KEY REFERENCES dbo.UserProfiles(EmployeeId) ON DELETE SET NULL,
    Title NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Date VARCHAR(10) NOT NULL, -- YYYY-MM-DD format matching backend.Models.Expense
    Description NVARCHAR(MAX) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    Notes NVARCHAR(MAX) NULL
);

-- Create ExpenseItems table
CREATE TABLE dbo.ExpenseItems (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    ExpenseId VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES dbo.Expenses(Id) ON DELETE CASCADE,
    Name NVARCHAR(250) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Cost DECIMAL(18, 2) NOT NULL,
    Quantity INT NOT NULL
);

-- Create ActivityLogs table
CREATE TABLE dbo.ActivityLogs (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    Action NVARCHAR(MAX) NOT NULL,
    Timestamp VARCHAR(30) NOT NULL, -- ISO UTC Date String format
    StatusType NVARCHAR(50) NOT NULL DEFAULT 'info'
);
GO

-- =========================================================================
-- 4. Insert Dummy / Seed Data
-- =========================================================================

-- Insert UserProfile
INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl)
VALUES (
    'FP-2024-897', 
    N'Himeshwar', 
    N'himeshwar.s@firstpay.com', 
    N'Senior Software Engineer', 
    N'Engineering', 
    5000.00, 
    1600.00, 
    N'https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop'
);

-- Insert Expenses (linked to employee 'FP-2024-897')
INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes)
VALUES 
('EXP-1001', 'FP-2024-897', N'AWS Server Hosting (July 2026)', N'Software & SaaS', '2026-07-20', N'Monthly cloud hosting charges for production microservices and database instances.', 1250.00, N'Approved', N'Auto-approved under pre-authorized engineering server budget.'),
('EXP-1002', 'FP-2024-897', N'Team dinner & Milestone Celebration', N'Meals & Entertainment', '2026-07-22', N'Catering dinner for the team after successful deployment of the authentication gateway.', 350.00, N'Approved', N'Receipt attached. Approved by Engineering Director.'),
('EXP-1003', 'FP-2024-897', N'Flight tickets to Bengaluru Summit', N'Travel', '2026-07-25', N'Round-trip flight booking to attend the FirstPay Annual Engineering Summit.', 685.50, N'Pending', NULL),
('EXP-1004', 'FP-2024-897', N'Ergonomic Mechanical Keyboard', N'Office Supplies', '2026-07-15', N'Purchase of keychron keyboard for workspace ergonomic enhancement.', 150.00, N'Rejected', N'Rejected: Office furniture and keyboards must be routed through IT standard hardware procurement policy.'),
('EXP-1005', 'FP-2024-897', N'Internet Reimbursement - June', N'Others', '2026-06-30', N'Work from home high speed broadband connection reimbursement.', 50.00, N'Paid', N'Approved and paid in June payroll cycle.'),
('EXP-1006', 'FP-2024-897', N'Client Lunch meeting', N'Meals & Entertainment', '2026-06-18', N'Business meal with prospects from FinTech Corp discussing payment gateway integration.', 180.00, N'Approved', N'Receipt verified. Business justification acceptable.'),
('EXP-1007', 'FP-2024-897', N'Udemy - GoLang Microservices Course', N'Others', '2026-05-12', N'Online video course for backend architecture scaling upskilling.', 25.00, N'Approved', N'Reimbursed under self-learning allowance budget.'),
('EXP-1008', 'FP-2024-897', N'Dell USB-C Monitor Docking Hub', N'Office Supplies', '2026-05-24', N'Multiport adapter for workstation dual monitor display setup.', 110.00, N'Approved', NULL),
('EXP-1009', 'FP-2024-897', N'GitHub Copilot Individual - Q2', N'Software & SaaS', '2026-04-01', N'AI programming assistant quarterly license subscription.', 30.00, N'Paid', N'Paid. Direct corporate credit card reconciliation.'),
('EXP-1010', 'FP-2024-897', N'Local Uber Rides to Client Site', N'Travel', '2026-04-15', N'Commute fares for design reviews with API clients in Gurugram.', 45.00, N'Approved', NULL),
('EXP-1011', 'FP-2024-897', N'Vitreous Whiteboard for Office desk', N'Office Supplies', '2026-03-10', N'Desktop glass dry-erase panel for software design sketches.', 60.00, N'Rejected', N'Rejected: Desk whiteboards must be requested through standard physical facilities desk allocation.'),
('EXP-1012', 'FP-2024-897', N'Monthly Broadband Internet - Feb', N'Others', '2026-02-28', N'WFH monthly broadband connectivity subscription fee.', 50.00, N'Paid', NULL),
('EXP-1013', 'FP-2024-897', N'IntelliJ IDEA Professional annual subscription', N'Software & SaaS', '2026-02-14', N'Annual developer tool license fee.', 249.00, N'Approved', N'Approved under software department pre-cleared tool budget.'),
('EXP-1014', 'FP-2024-897', N'Draft: Wireless Ergonomic Mouse', N'Office Supplies', '2026-07-26', N'Logitech MX Master mouse for daily development workstation comfort.', 99.00, N'Draft', NULL),
('EXP-1015', 'FP-2024-897', N'Draft: Book - Designing Data-Intensive Applications', N'Others', '2026-07-27', N'Hardcopy reference book for software engineering technical design.', 45.00, N'Draft', NULL),
('EXP-1016', 'FP-2024-897', N'Local Cab Fares to Airport', N'Travel', '2026-06-10', N'Travel cab for business travel to Delhi Airport.', 75.00, N'Approved', NULL),
('EXP-1017', 'FP-2024-897', N'Client Tea & Snacks Catering', N'Meals & Entertainment', '2026-03-25', N'Catered refreshments for external clients during design discussions.', 40.00, N'Approved', NULL);

-- Insert ExpenseItems (linked to corresponding Expense IDs)
INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity)
VALUES
('ITM-1', 'EXP-1001', N'AWS EC2 - m5.xlarge instances', N'Software & SaaS', 600.00, 1),
('ITM-2', 'EXP-1001', N'AWS Aurora DB hosting', N'Software & SaaS', 450.00, 1),
('ITM-3', 'EXP-1001', N'AWS S3 storage fees', N'Software & SaaS', 200.00, 1),
('ITM-4', 'EXP-1002', N'Catering & Beverages (15 pax)', N'Meals & Entertainment', 350.00, 1),
('ITM-5', 'EXP-1003', N'Air India Flight (DEL-BLR-DEL)', N'Travel', 550.00, 1),
('ITM-6', 'EXP-1003', N'Airport Cab Transfer', N'Travel', 135.50, 1),
('ITM-7', 'EXP-1004', N'Keychron K2 Keyboard', N'Office Supplies', 99.00, 1),
('ITM-8', 'EXP-1004', N'Ergonomic Mouse pad', N'Office Supplies', 51.00, 1),
('ITM-9', 'EXP-1005', N'Airtel Fiber Broadband monthly plan', N'Others', 50.00, 1),
('ITM-10', 'EXP-1006', N'Business lunch at Taj Diner', N'Meals & Entertainment', 180.00, 1),
('ITM-11', 'EXP-1007', N'GoLang Microservices course license', N'Others', 25.00, 1),
('ITM-12', 'EXP-1008', N'Dell DA310 USB-C Adapter', N'Office Supplies', 110.00, 1),
('ITM-13', 'EXP-1009', N'GitHub Copilot subscription (April - June)', N'Software & SaaS', 10.00, 3),
('ITM-14', 'EXP-1010', N'Uber Go ride - Gurugram office', N'Travel', 45.00, 1),
('ITM-15', 'EXP-1011', N'Desktop Glass Whiteboard', N'Office Supplies', 60.00, 1),
('ITM-16', 'EXP-1012', N'Broadband fiber Internet bills', N'Others', 50.00, 1),
('ITM-17', 'EXP-1013', N'IntelliJ IDEA Ultimate Individual license', N'Software & SaaS', 249.00, 1),
('ITM-18', 'EXP-1014', N'Logitech MX Master 3S Mouse', N'Office Supplies', 99.00, 1),
('ITM-19', 'EXP-1015', N'Designing Data-Intensive Applications by Kleppmann', N'Others', 45.00, 1),
('ITM-20', 'EXP-1016', N'Airport cab ride transfer', N'Travel', 75.00, 1),
('ITM-21', 'EXP-1017', N'Beverages and Snacks', N'Meals & Entertainment', 40.00, 1);

-- Insert ActivityLogs (relative to current UTC execution time)
INSERT INTO dbo.ActivityLogs (Id, Action, Timestamp, StatusType)
VALUES
('ACT-1', N'Draft claim initiated: "Book - Designing Data-Intensive Applications"', CONVERT(VARCHAR(19), DATEADD(MINUTE, -5, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-2', N'Draft claim initiated: "Wireless Ergonomic Mouse"', CONVERT(VARCHAR(19), DATEADD(HOUR, -1, GETUTCDATE()), 126) + 'Z', 'info'),
('ACT-3', N'Submitted expense request "Flight tickets to Bengaluru Summit" for $685.50', CONVERT(VARCHAR(19), DATEADD(DAY, -2, GETUTCDATE()), 126) + 'Z', 'warning'),
('ACT-4', N'Expense claim "Team dinner & Milestone Celebration" of $350.00 approved by Manager', CONVERT(VARCHAR(19), DATEADD(DAY, -5, GETUTCDATE()), 126) + 'Z', 'success'),
('ACT-5', N'Expense claim "Ergonomic Mechanical Keyboard" of $150.00 was rejected', CONVERT(VARCHAR(19), DATEADD(DAY, -12, GETUTCDATE()), 126) + 'Z', 'danger'),
('ACT-6', N'Expense claim "AWS Server Hosting (July 2026)" of $1250.00 approved automatically', CONVERT(VARCHAR(19), DATEADD(DAY, -7, GETUTCDATE()), 126) + 'Z', 'success');
GO


UPDATE UserProfiles 
SET Name = 'Himeshwar (SQL Server Live)' 
WHERE EmployeeId = 'FP-2024-897';

USE ExpenseTrackerDb;
GO

IF OBJECT_ID('dbo.UserCredentials', 'U') IS NOT NULL DROP TABLE dbo.UserCredentials;
GO

CREATE TABLE dbo.UserCredentials (
    Username NVARCHAR(50) NOT NULL PRIMARY KEY,
    Password NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User'
);

-- Seed user login details with roles
INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role)
VALUES 
('himesh', '123', 'Himeshwar', 'Employee'),
('ghiri', 'pass', 'ghiri', 'Employee');
GO

SELECT * 
FROM dbo.UserCredentials;
GO

-- =========================================================================
-- 5. Additional Schema Extensions for Manager, Accountant, and Admin
-- =========================================================================

USE ExpenseTrackerDb;
GO

-- Alter UserProfiles to add ManagerId
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.UserProfiles') AND name = 'ManagerId')
BEGIN
    ALTER TABLE dbo.UserProfiles
    ADD ManagerId VARCHAR(50) NULL;
END
GO

-- Add Foreign Key constraint for self-referencing ManagerId
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserProfiles_Manager')
BEGIN
    ALTER TABLE dbo.UserProfiles
    ADD CONSTRAINT FK_UserProfiles_Manager FOREIGN KEY (ManagerId) REFERENCES dbo.UserProfiles(EmployeeId);
END
GO

-- Alter Expenses to add PaymentDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Expenses') AND name = 'PaymentDate')
BEGIN
    ALTER TABLE dbo.Expenses
    ADD PaymentDate VARCHAR(30) NULL;
END
GO

-- Create ApprovalHistory table
IF OBJECT_ID('dbo.ApprovalHistory', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApprovalHistory (
        Id VARCHAR(50) NOT NULL PRIMARY KEY,
        ExpenseId VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES dbo.Expenses(Id) ON DELETE CASCADE,
        Action NVARCHAR(50) NOT NULL, -- 'Submitted', 'Approved', 'Rejected', 'Paid'
        PerformedBy NVARCHAR(100) NOT NULL,
        Timestamp VARCHAR(30) NOT NULL, -- ISO Date String
        Notes NVARCHAR(MAX) NULL
    );
END
GO

-- Create FreezeDateSettings table
IF OBJECT_ID('dbo.FreezeDateSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FreezeDateSettings (
        Id INT NOT NULL PRIMARY KEY DEFAULT 1,
        FreezeDay INT NOT NULL DEFAULT 18,
        CONSTRAINT CK_FreezeDay CHECK (FreezeDay >= 1 AND FreezeDay <= 31),
        CONSTRAINT UC_FreezeDateSettings_SingleRow UNIQUE (Id)
    );
END
GO

-- Create SystemSettings table
IF OBJECT_ID('dbo.SystemSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemSettings (
        Id INT NOT NULL PRIMARY KEY DEFAULT 1,
        CompanyName NVARCHAR(250) NOT NULL,
        CompanyAddress NVARCHAR(500) NOT NULL,
        CorporateCurrency NVARCHAR(50) NOT NULL,
        SystemMode NVARCHAR(100) NOT NULL,
        CONSTRAINT UC_SystemSettings_SingleRow UNIQUE (Id)
    );
END
GO

-- =========================================================================
-- 6. Insert New Seed Users and Credentials
-- =========================================================================

-- Insert Managers, Accountants, Admin, and extra Employees into UserCredentials
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'manager')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('manager', '123', 'Ishwari Rajmohan', 'Manager');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'manager2')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('manager2', '123', 'Robert Johnson', 'Manager');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'manager3')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('manager3', '123', 'Linda Martinez', 'Manager');

IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'accountant')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('accountant', '123', 'Accountant Office', 'Accountant');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'accountant2')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('accountant2', '123', 'Finance Auditor', 'Accountant');

IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'admin')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('admin', '123', 'System Admin', 'Admin');

IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'aisha')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('aisha', '123', 'Aisha Rahman', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'john')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('john', '123', 'John Doe', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'sarah')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('sarah', '123', 'Sarah Jenkins', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'michael')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('michael', '123', 'Michael Brown', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'emily')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('emily', '123', 'Emily Davis', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'david')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('david', '123', 'David Wilson', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'jessica')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('jessica', '123', 'Jessica Taylor', 'Employee');
IF NOT EXISTS (SELECT * FROM dbo.UserCredentials WHERE Username = 'james')
    INSERT INTO dbo.UserCredentials (Username, Password, DisplayName, Role) VALUES ('james', '123', 'James Thomas', 'Employee');
GO

-- Insert UserProfiles
-- Managers
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-001')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-001', 'Ishwari Rajmohan', 'ishwari.r@firstpay.com', 'Manager', 'Engineering', 50000.00, 15000.00, 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop', NULL);
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-002')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-002', 'Robert Johnson', 'robert.j@firstpay.com', 'Manager', 'Marketing', 40000.00, 8000.00, 'https://images.unsplash.com/photo-1560250097-0b93528c311a?q=80&w=256&auto=format&fit=crop', NULL);
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-003')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-003', 'Linda Martinez', 'linda.m@firstpay.com', 'Manager', 'Sales', 45000.00, 12000.00, 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?q=80&w=256&auto=format&fit=crop', NULL);

-- Accountants
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-010')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-010', 'Accountant Office', 'finance.audit@firstpay.com', 'Accountant', 'Finance', 0.00, 0.00, 'https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?q=80&w=256&auto=format&fit=crop', NULL);
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-011')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-011', 'Finance Auditor', 'auditor@firstpay.com', 'Accountant', 'Finance', 0.00, 0.00, 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?q=80&w=256&auto=format&fit=crop', NULL);

-- Admin
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-ADMIN-01')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-ADMIN-01', 'System Admin', 'admin.hq@firstpay.com', 'Admin', 'Operations', 0.00, 0.00, 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop', NULL);

-- Employees
IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-111')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-111', 'ghiri', 'ghiri@firstpay.com', 'Employee', 'Engineering', 6000.00, 1200.00, 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?q=80&w=256&auto=format&fit=crop', 'FP-2024-001');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-912')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-912', 'Aisha Rahman', 'aisha.r@firstpay.com', 'Employee', 'Engineering', 8000.00, 2500.00, 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?q=80&w=256&auto=format&fit=crop', 'FP-2024-001');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-521')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-521', 'John Doe', 'john.d@firstpay.com', 'Employee', 'Marketing', 5000.00, 1800.00, 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=256&auto=format&fit=crop', 'FP-2024-002');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-340')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-340', 'Sarah Jenkins', 'sarah.j@firstpay.com', 'Employee', 'Sales', 10000.00, 4000.00, 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?q=80&w=256&auto=format&fit=crop', 'FP-2024-003');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-222')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-222', 'Michael Brown', 'michael.b@firstpay.com', 'Employee', 'Marketing', 5000.00, 500.00, 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?q=80&w=256&auto=format&fit=crop', 'FP-2024-002');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-333')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-333', 'Emily Davis', 'emily.d@firstpay.com', 'Employee', 'Sales', 8000.00, 1500.00, 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?q=80&w=256&auto=format&fit=crop', 'FP-2024-003');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-444')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-444', 'David Wilson', 'david.w@firstpay.com', 'Employee', 'Engineering', 7000.00, 900.00, 'https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?q=80&w=256&auto=format&fit=crop', 'FP-2024-001');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-555')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-555', 'Jessica Taylor', 'jessica.t@firstpay.com', 'Employee', 'Marketing', 6000.00, 1100.00, 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?q=80&w=256&auto=format&fit=crop', 'FP-2024-002');

IF NOT EXISTS (SELECT * FROM dbo.UserProfiles WHERE EmployeeId = 'FP-2024-666')
    INSERT INTO dbo.UserProfiles (EmployeeId, Name, Email, Role, Department, BudgetLimit, SpentAmount, AvatarUrl, ManagerId)
    VALUES ('FP-2024-666', 'James Thomas', 'james.t@firstpay.com', 'Employee', 'Sales', 9000.00, 2200.00, 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?q=80&w=256&auto=format&fit=crop', 'FP-2024-003');

-- Set existing Himeshwar's manager
UPDATE dbo.UserProfiles
SET ManagerId = 'FP-2024-001'
WHERE EmployeeId = 'FP-2024-897';
GO

-- =========================================================================
-- 7. Insert Additional Expenses and Items to satisfy count criteria
-- =========================================================================

-- We currently have EXP-1001 to EXP-1017. Add 3 more to reach at least 20 forms.
IF NOT EXISTS (SELECT * FROM dbo.Expenses WHERE Id = 'EXP-1018')
    INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes, PaymentDate)
    VALUES ('EXP-1018', 'FP-2024-912', N'Marketing Standees Paris', N'Office Supplies', '2026-07-28', N'Standees printing for regional launch event.', 220.00, N'Paid', N'Supplier invoice cleared.', '2026-07-30T10:00:00Z');

IF NOT EXISTS (SELECT * FROM dbo.Expenses WHERE Id = 'EXP-1019')
    INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes, PaymentDate)
    VALUES ('EXP-1019', 'FP-2024-340', N'Client Engagement Dinner London', N'Meals & Entertainment', '2026-07-29', N'Dinner with prospective client representatives.', 320.00, N'Approved', N'Cleared within standard guidelines.', NULL);

IF NOT EXISTS (SELECT * FROM dbo.Expenses WHERE Id = 'EXP-1020')
    INSERT INTO dbo.Expenses (Id, EmployeeId, Title, Category, Date, Description, TotalAmount, Status, Notes, PaymentDate)
    VALUES ('EXP-1020', 'FP-2024-521', N'Software IDE Subscription', N'Software & SaaS', '2026-07-30', N'License fees for design tool suites.', 150.00, N'Pending', NULL, NULL);
GO

-- Add extra items. Currently we have ITM-1 to ITM-21. We need 29 more items (ITM-22 to ITM-50) to make it 50 items.
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-22')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-22', 'EXP-1018', N'Vinyl Roll Standees', N'Office Supplies', 110.00, 2);

-- Seed bulk item lists for existing expenses to reach 50 items:
-- Let's add multiple items to EXP-1002, EXP-1003, EXP-1004, etc.
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-23')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-23', 'EXP-1002', N'Desserts & Pastries', N'Meals & Entertainment', 50.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-24')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-24', 'EXP-1002', N'Beverages and Juices', N'Meals & Entertainment', 100.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-25')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-25', 'EXP-1003', N'Hotel Stay (2 nights)', N'Travel', 150.00, 2);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-26')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-26', 'EXP-1004', N'Custom Wrist Rest', N'Office Supplies', 45.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-27')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-27', 'EXP-1005', N'Router lease fee', N'Others', 10.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-28')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-28', 'EXP-1006', N'Taxi ride Taj restaurant', N'Meals & Entertainment', 30.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-29')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-29', 'EXP-1007', N'Companion booklet ebook', N'Others', 15.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-30')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-30', 'EXP-1008', N'HDMI Premium Cable 3m', N'Office Supplies', 25.00, 2);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-31')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-31', 'EXP-1009', N'Copilot additional server node', N'Software & SaaS', 15.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-32')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-32', 'EXP-1010', N'Uber ride return route', N'Travel', 45.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-33')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-33', 'EXP-1011', N'Whiteboard dry erasers pack', N'Office Supplies', 10.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-34')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-34', 'EXP-1013', N'IntelliJ database plugin key', N'Software & SaaS', 50.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-35')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-35', 'EXP-1014', N'MX Mouse travel case', N'Office Supplies', 20.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-36')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-36', 'EXP-1015', N'Reference documentation pdf copy', N'Others', 10.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-37')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-37', 'EXP-1016', N'Toll gate tax', N'Travel', 15.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-38')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-38', 'EXP-1017', N'Mineral water bottles box', N'Meals & Entertainment', 15.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-39')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-39', 'EXP-1019', N'Restaurant Booking Charges', N'Meals & Entertainment', 50.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-40')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-40', 'EXP-1019', N'Desserts & appetizers platter', N'Meals & Entertainment', 90.00, 3);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-41')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-41', 'EXP-1020', N'JetBrains IDE key', N'Software & SaaS', 150.00, 1);

-- Let's add standard items to make the item list exceed 50 cleanly
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-42')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-42', 'EXP-1001', N'Premium AWS Bandwidth Out', N'Software & SaaS', 50.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-43')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-43', 'EXP-1001', N'Premium AWS CloudWatch metric logs', N'Software & SaaS', 40.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-44')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-44', 'EXP-1002', N'Disposable plates & napkins', N'Meals & Entertainment', 25.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-45')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-45', 'EXP-1003', N'Flight luggage checkin fee', N'Travel', 50.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-46')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-46', 'EXP-1004', N'Ergonomic palm pillow', N'Office Supplies', 25.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-47')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-47', 'EXP-1005', N'Broadband fiber installation charge', N'Others', 20.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-48')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-48', 'EXP-1006', N'Service tips at lunch', N'Meals & Entertainment', 20.00, 1);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-49')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-49', 'EXP-1008', N'Workstation USB extension extension', N'Office Supplies', 15.00, 2);
IF NOT EXISTS (SELECT * FROM dbo.ExpenseItems WHERE Id = 'ITM-50')
    INSERT INTO dbo.ExpenseItems (Id, ExpenseId, Name, Category, Cost, Quantity) VALUES ('ITM-50', 'EXP-1010', N'Uber ride convenience charge', N'Travel', 5.00, 1);
GO

-- =========================================================================
-- 8. Seed ApprovalHistory / PaymentHistory Records
-- =========================================================================

-- EXP-1001: Approved
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1001')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-101', 'EXP-1001', 'Submitted', 'Himeshwar', '2026-07-20T09:00:00Z', 'AWS server billing.'),
    ('APH-102', 'EXP-1001', 'Approved', 'Ishwari Rajmohan', '2026-07-20T14:30:00Z', 'Budget cleared.');
END

-- EXP-1002: Approved
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1002')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-201', 'EXP-1002', 'Submitted', 'Himeshwar', '2026-07-22T10:00:00Z', 'Dinner logs.'),
    ('APH-202', 'EXP-1002', 'Approved', 'Ishwari Rajmohan', '2026-07-22T15:30:00Z', 'Approved by engineering lead.');
END

-- EXP-1003: Pending
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1003')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-301', 'EXP-1003', 'Submitted', 'Himeshwar', '2026-07-25T11:00:00Z', 'Summit tickets flight.');
END

-- EXP-1004: Rejected
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1004')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-401', 'EXP-1004', 'Submitted', 'Himeshwar', '2026-07-15T09:00:00Z', 'Ergonomic hardware.'),
    ('APH-402', 'EXP-1004', 'Rejected', 'Ishwari Rajmohan', '2026-07-15T12:00:00Z', 'Purchase standard equipment via IT procurement.');
END

-- EXP-1005: Paid
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1005')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-501', 'EXP-1005', 'Submitted', 'Himeshwar', '2026-06-30T09:00:00Z', 'Internet connection.'),
    ('APH-502', 'EXP-1005', 'Approved', 'Ishwari Rajmohan', '2026-06-30T14:00:00Z', 'WFH reimbursement cleared.'),
    ('APH-503', 'EXP-1005', 'Paid', 'Accountant Office', '2026-07-05T10:00:00Z', 'Reimbursed in June payroll cycle.');
END

-- EXP-1018: Paid
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1018')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-1801', 'EXP-1018', 'Submitted', 'Aisha Rahman', '2026-07-28T09:00:00Z', 'Marketing print materials.'),
    ('APH-1802', 'EXP-1018', 'Approved', 'Ishwari Rajmohan', '2026-07-28T14:00:00Z', 'Approved under roadshow marketing budget.'),
    ('APH-1803', 'EXP-1018', 'Paid', 'Accountant Office', '2026-07-30T10:00:00Z', 'Supplier invoice paid via bank wire.');
END

-- EXP-1019: Approved
IF NOT EXISTS (SELECT * FROM dbo.ApprovalHistory WHERE ExpenseId = 'EXP-1019')
BEGIN
    INSERT INTO dbo.ApprovalHistory (Id, ExpenseId, Action, PerformedBy, Timestamp, Notes) VALUES
    ('APH-1901', 'EXP-1019', 'Submitted', 'Sarah Jenkins', '2026-07-29T10:00:00Z', 'Travel dining client.'),
    ('APH-1902', 'EXP-1019', 'Approved', 'Robert Johnson', '2026-07-29T16:00:00Z', 'Approved sales hospitality.');
END
GO

-- =========================================================================
-- 9. Seed FreezeDateSettings and SystemSettings tables
-- =========================================================================
IF NOT EXISTS (SELECT * FROM dbo.FreezeDateSettings WHERE Id = 1)
    INSERT INTO dbo.FreezeDateSettings (Id, FreezeDay) VALUES (1, 18);

IF NOT EXISTS (SELECT * FROM dbo.SystemSettings WHERE Id = 1)
    INSERT INTO dbo.SystemSettings (Id, CompanyName, CompanyAddress, CorporateCurrency, SystemMode)
    VALUES (1, 'FirstPay Corporate Services', 'Level 21, Fintech Plaza, Istanbul, Turkey', 'USD ($)', 'Production Mode (SQL Server Live)');
GO











