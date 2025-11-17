using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseManagement.Models;
using ExpenseManagement.Services;

namespace ExpenseManagement.Pages;

public class ApproveModel : PageModel
{
    private readonly IExpenseService _expenseService;
    
    public List<Expense> PendingExpenses { get; set; } = new();
    
    [TempData]
    public string? SuccessMessage { get; set; }

    public ApproveModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public async Task OnGetAsync()
    {
        PendingExpenses = await _expenseService.GetPendingExpensesAsync();
    }
    
    public async Task<IActionResult> OnPostAsync(int expenseId, string action)
    {
        var reviewerId = 2; // Manager ID (Bob Manager)
        
        if (action == "approve")
        {
            await _expenseService.ApproveExpenseAsync(expenseId, reviewerId);
            SuccessMessage = "Expense approved successfully!";
        }
        else if (action == "reject")
        {
            await _expenseService.RejectExpenseAsync(expenseId, reviewerId);
            SuccessMessage = "Expense rejected successfully!";
        }
        
        return RedirectToPage();
    }
}
