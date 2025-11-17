using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseManagement.Models;
using ExpenseManagement.Services;

namespace ExpenseManagement.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IExpenseService _expenseService;

    [BindProperty]
    public decimal Amount { get; set; }
    
    [BindProperty]
    public DateTime ExpenseDate { get; set; }
    
    [BindProperty]
    public int CategoryId { get; set; }
    
    [BindProperty]
    public string? Description { get; set; }
    
    public List<ExpenseCategory> Categories { get; set; } = new();
    
    [TempData]
    public string? SuccessMessage { get; set; }
    
    [TempData]
    public string? ErrorMessage { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IExpenseService expenseService)
    {
        _logger = logger;
        _expenseService = expenseService;
    }

    public async Task OnGetAsync()
    {
        Categories = await _expenseService.GetCategoriesAsync();
        ExpenseDate = DateTime.Today;
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        Categories = await _expenseService.GetCategoriesAsync();
        
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please check the form for errors.";
            return Page();
        }
        
        try
        {
            var expense = new Expense
            {
                AmountMinor = (int)(Amount * 100), // Convert to pence
                ExpenseDate = ExpenseDate,
                CategoryId = CategoryId,
                Description = Description,
                Currency = "GBP"
            };
            
            var expenseId = await _expenseService.CreateExpenseAsync(expense);
            
            SuccessMessage = $"Expense created successfully! (ID: {expenseId})";
            
            // Clear form
            Amount = 0;
            ExpenseDate = DateTime.Today;
            CategoryId = 0;
            Description = null;
            
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            ErrorMessage = "An error occurred while creating the expense.";
            return Page();
        }
    }
}
