using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var summary = await _employeeService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("expenses")]
    public async Task<IActionResult> GetExpenses()
    {
        var expenses = await _employeeService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("expenses/{id}")]
    public async Task<IActionResult> GetExpenseById(string id)
    {
        var expense = await _employeeService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound(new { message = $"Expense claim with ID {id} not found." });
        }
        return Ok(expense);
    }

    [HttpPost("save-draft")]
    public async Task<IActionResult> SaveDraft([FromBody] Expense expense)
    {
        if (expense == null)
        {
            return BadRequest(new { message = "Invalid expense claim payload." });
        }
        
        var saved = await _employeeService.SaveDraftAsync(expense);
        return Ok(saved);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitExpense([FromBody] Expense expense)
    {
        if (expense == null)
        {
            return BadRequest(new { message = "Invalid expense claim payload." });
        }

        var submitted = await _employeeService.SubmitExpenseAsync(expense);
        return Ok(submitted);
    }

    [HttpPut("expenses/{id}")]
    public async Task<IActionResult> UpdateExpense(string id, [FromBody] Expense expense)
    {
        if (expense == null)
        {
            return BadRequest(new { message = "Invalid expense claim payload." });
        }

        var success = await _employeeService.UpdateExpenseAsync(id, expense);
        if (!success)
        {
            return BadRequest(new { message = "Only Draft or Rejected expenses can be updated, or the record does not exist." });
        }

        return Ok(new { success = true, message = "Expense requisition updated successfully." });
    }

    [HttpDelete("expenses/{id}")]
    public async Task<IActionResult> DeleteExpense(string id)
    {
        var success = await _employeeService.DeleteExpenseAsync(id);
        if (!success)
        {
            return BadRequest(new { message = "Only Draft expenses can be deleted, or the record does not exist." });
        }

        return Ok(new { success = true, message = "Draft expense deleted successfully." });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _employeeService.GetProfileAsync();
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfile profile)
    {
        if (profile == null)
        {
            return BadRequest(new { message = "Invalid profile payload." });
        }

        var success = await _employeeService.UpdateProfileAsync(profile);
        if (!success)
        {
            return BadRequest(new { message = "Profile failed to update." });
        }

        return Ok(new { success = true, message = "Profile settings updated successfully." });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
        {
            return BadRequest(new { message = "Credentials update requires both current and new passwords." });
        }

        var success = await _employeeService.ChangePasswordAsync(request.OldPassword, request.NewPassword);
        return Ok(new { success = true, message = "Password changed successfully." });
    }
}
