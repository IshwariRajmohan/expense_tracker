using System.Collections.Generic;
using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardSummaryAsync();
    Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
    Task<IEnumerable<AdminExpenseDto>> GetAllExpensesAsync();
    Task<AdminExpenseDto?> GetExpenseByIdAsync(string id);
    Task<AdminReportsDto> GetReportsAsync();
    Task<IEnumerable<ApprovalHistoryDto>> GetGlobalWorkflowHistoryAsync();
    Task<AdminFreezeDateDto> GetFreezeDateAsync();
    Task<bool> UpdateFreezeDateAsync(int day);
    Task<AdminSettingsDto> GetSettingsAsync();
    Task<bool> AddUserAsync(AdminUserDto user);
}
