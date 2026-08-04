using System.Collections.Generic;

namespace backend.Models;

public class Expense
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty; // ISO Format yyyy-MM-dd
    public string Description { get; set; } = string.Empty;
    public List<ExpenseItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // "Draft", "Pending", "Approved", "Rejected", "Paid"
    public string? Notes { get; set; }
    public string? PaymentDate { get; set; }
}
