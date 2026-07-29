using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Services;

public interface IManagerService
{
    Task<ManagerDashboardDto> GetDashboardSummaryAsync();
    Task<IEnumerable<ManagerExpenseDto>> GetPendingExpensesAsync();
    Task<ManagerExpenseDto?> GetExpenseByIdAsync(string id);
    Task<bool> ApproveExpenseAsync(string id, string notes);
    Task<bool> RejectExpenseAsync(string id, string reason);
    Task<IEnumerable<ApprovalHistoryDto>> GetExpenseHistoryAsync(string id);
    Task<IEnumerable<ApprovalHistoryDto>> GetGlobalHistoryAsync();
    Task<backend.Models.UserProfile> GetProfileAsync();
    Task<bool> UpdateProfileAsync(backend.Models.UserProfile profile);
}
