namespace backend.Models;

public class UserProfile
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public decimal BudgetLimit { get; set; }
    public decimal SpentAmount { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}
