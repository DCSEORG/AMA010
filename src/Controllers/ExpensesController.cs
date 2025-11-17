using Microsoft.AspNetCore.Mvc;
using ExpenseManagement.Models;
using ExpenseManagement.Services;

namespace ExpenseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Get all expenses
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }

        /// <summary>
        /// Get expenses by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Expense>>> GetExpensesByUser(int userId)
        {
            var expenses = await _expenseService.GetExpensesByUserAsync(userId);
            return Ok(expenses);
        }

        /// <summary>
        /// Get pending expenses for approval
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<List<Expense>>> GetPendingExpenses()
        {
            var expenses = await _expenseService.GetPendingExpensesAsync();
            return Ok(expenses);
        }

        /// <summary>
        /// Get a specific expense by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            return Ok(expense);
        }

        /// <summary>
        /// Create a new expense
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> CreateExpense([FromBody] Expense expense)
        {
            var expenseId = await _expenseService.CreateExpenseAsync(expense);
            return CreatedAtAction(nameof(GetExpense), new { id = expenseId }, expenseId);
        }

        /// <summary>
        /// Update an existing expense
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateExpense(int id, [FromBody] Expense expense)
        {
            expense.ExpenseId = id;
            var success = await _expenseService.UpdateExpenseAsync(expense);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Submit an expense for approval
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<ActionResult> SubmitExpense(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            expense.StatusId = 2; // Submitted
            var success = await _expenseService.UpdateExpenseAsync(expense);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Approve an expense
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult> ApproveExpense(int id, [FromBody] int reviewerId = 2)
        {
            var success = await _expenseService.ApproveExpenseAsync(id, reviewerId);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Reject an expense
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<ActionResult> RejectExpense(int id, [FromBody] int reviewerId = 2)
        {
            var success = await _expenseService.RejectExpenseAsync(id, reviewerId);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Get all expense categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<ExpenseCategory>>> GetCategories()
        {
            var categories = await _expenseService.GetCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get all expense statuses
        /// </summary>
        [HttpGet("statuses")]
        public async Task<ActionResult<List<ExpenseStatus>>> GetStatuses()
        {
            var statuses = await _expenseService.GetStatusesAsync();
            return Ok(statuses);
        }
    }
}
