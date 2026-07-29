using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var summary = await _managerService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var pending = await _managerService.GetPendingExpensesAsync();
        return Ok(pending);
    }

    [HttpGet("expense/{id}")]
    public async Task<IActionResult> GetExpenseById(string id)
    {
        var expense = await _managerService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound(new { message = $"Expense claim with ID {id} not found." });
        }
        return Ok(expense);
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> Approve(string id, [FromBody] ApproveRequestDto request)
    {
        var notes = request?.Notes ?? string.Empty;
        var success = await _managerService.ApproveExpenseAsync(id, notes);
        if (!success)
        {
            return BadRequest(new { message = $"Could not approve expense. Claim might not be in Pending state or not exist." });
        }
        return Ok(new { success = true, message = "Expense requisition approved successfully." });
    }

    [HttpPost("reject/{id}")]
    public async Task<IActionResult> Reject(string id, [FromBody] RejectRequestDto request)
    {
        var reason = request?.Reason ?? string.Empty;
        if (string.IsNullOrWhiteSpace(reason))
        {
            return BadRequest(new { message = "Rejection reason is mandatory." });
        }

        var success = await _managerService.RejectExpenseAsync(id, reason);
        if (!success)
        {
            return BadRequest(new { message = $"Could not reject expense. Claim might not be in Pending state or not exist." });
        }
        return Ok(new { success = true, message = "Expense requisition rejected successfully." });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetGlobalHistory()
    {
        var history = await _managerService.GetGlobalHistoryAsync();
        return Ok(history);
    }

    [HttpGet("history/{id}")]
    public async Task<IActionResult> GetExpenseHistory(string id)
    {
        var history = await _managerService.GetExpenseHistoryAsync(id);
        return Ok(history);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _managerService.GetProfileAsync();
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] backend.Models.UserProfile profile)
    {
        if (profile == null)
        {
            return BadRequest(new { message = "Invalid profile payload." });
        }

        var success = await _managerService.UpdateProfileAsync(profile);
        if (!success)
        {
            return BadRequest(new { message = "Profile failed to update." });
        }

        return Ok(new { success = true, message = "Manager profile updated successfully." });
    }
}

public class ApproveRequestDto
{
    public string Notes { get; set; } = string.Empty;
}

public class RejectRequestDto
{
    public string Reason { get; set; } = string.Empty;
}
