using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountantController : ControllerBase
{
    private readonly IAccountantService _accountantService;

    public AccountantController(IAccountantService accountantService)
    {
        _accountantService = accountantService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var summary = await _accountantService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("approved")]
    public async Task<IActionResult> GetApproved()
    {
        var approved = await _accountantService.GetApprovedExpensesAsync();
        return Ok(approved);
    }

    [HttpGet("expense/{id}")]
    public async Task<IActionResult> GetExpenseById(string id)
    {
        var expense = await _accountantService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound(new { message = $"Expense claim with ID {id} not found." });
        }
        return Ok(expense);
    }

    [HttpPost("pay/{id}")]
    public async Task<IActionResult> PayExpense(string id, [FromBody] PayExpenseRequestDto request)
    {
        var notes = request?.Notes;
        var success = await _accountantService.PayExpenseAsync(id, notes);
        if (!success)
        {
            return BadRequest(new { message = $"Expense {id} cannot be marked as Paid. It might not exist, or its status is not 'Approved'." });
        }
        return Ok(new { success = true, message = $"Expense claim {id} has been marked as Paid successfully." });
    }

    [HttpGet("payment-history")]
    public async Task<IActionResult> GetPaymentHistory()
    {
        var history = await _accountantService.GetPaymentHistoryAsync();
        return Ok(history);
    }
}
