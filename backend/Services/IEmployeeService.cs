using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.DTOs;

namespace backend.Services;

public interface IEmployeeService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(string id);
    Task<Expense> SaveDraftAsync(Expense expense);
    Task<Expense> SubmitExpenseAsync(Expense expense);
    Task<bool> UpdateExpenseAsync(string id, Expense expense);
    Task<bool> DeleteExpenseAsync(string id);
    Task<UserProfile> GetProfileAsync();
    Task<bool> UpdateProfileAsync(UserProfile profile);
    Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
}
