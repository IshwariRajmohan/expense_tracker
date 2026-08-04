using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var summary = await _adminService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("expenses")]
    public async Task<IActionResult> GetExpenses()
    {
        var expenses = await _adminService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("expense/{id}")]
    public async Task<IActionResult> GetExpenseById(string id)
    {
        var expense = await _adminService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound(new { message = $"Expense claim with ID {id} not found." });
        }
        return Ok(expense);
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
        var reports = await _adminService.GetReportsAsync();
        return Ok(reports);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetWorkflowHistory()
    {
        var history = await _adminService.GetGlobalWorkflowHistoryAsync();
        return Ok(history);
    }

    [HttpGet("freeze-date")]
    public async Task<IActionResult> GetFreezeDate()
    {
        var freezeDate = await _adminService.GetFreezeDateAsync();
        return Ok(freezeDate);
    }

    [HttpPut("freeze-date")]
    public async Task<IActionResult> UpdateFreezeDate([FromBody] UpdateFreezeDateRequest request)
    {
        if (request == null || request.Day < 1 || request.Day > 31)
        {
            return BadRequest(new { message = "Invalid freeze date. Day must be between 1 and 31." });
        }

        var success = await _adminService.UpdateFreezeDateAsync(request.Day);
        if (!success)
        {
            return BadRequest(new { message = "Failed to update freeze date." });
        }

        var updatedFreezeDate = await _adminService.GetFreezeDateAsync();
        return Ok(new { success = true, message = $"Freeze date updated successfully to the {request.Day}th.", data = updatedFreezeDate });
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _adminService.GetSettingsAsync();
        return Ok(settings);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] AdminUserDto userDto)
    {
        if (userDto == null)
        {
            return BadRequest(new { message = "Invalid user payload." });
        }

        var success = await _adminService.AddUserAsync(userDto);
        if (!success)
        {
            return BadRequest(new { message = "Failed to create user. Ensure Employee ID is unique and required fields are populated." });
        }

        return Ok(new { success = true, message = $"User {userDto.Name} created successfully." });
    }
}
