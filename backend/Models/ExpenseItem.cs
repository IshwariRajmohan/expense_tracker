namespace backend.Models;

public class ExpenseItem
{
    public string? Id { get; set; }
    public string ExpenseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
}
