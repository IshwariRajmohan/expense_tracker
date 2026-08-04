using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Services;

public interface IAccountantService
{
    Task<AccountantDashboardDto> GetDashboardSummaryAsync();
    Task<IEnumerable<AccountantExpenseDto>> GetApprovedExpensesAsync();
    Task<AccountantExpenseDto?> GetExpenseByIdAsync(string id);
    Task<bool> PayExpenseAsync(string id, string? notes);
    Task<IEnumerable<AccountantExpenseDto>> GetPaymentHistoryAsync();
}
