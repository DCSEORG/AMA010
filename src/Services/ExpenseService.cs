using ExpenseManagement.Models;

namespace ExpenseManagement.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpensesAsync();
        Task<List<Expense>> GetExpensesByUserAsync(int userId);
        Task<List<Expense>> GetPendingExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(int expenseId);
        Task<int> CreateExpenseAsync(Expense expense);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> ApproveExpenseAsync(int expenseId, int reviewerId);
        Task<bool> RejectExpenseAsync(int expenseId, int reviewerId);
        Task<List<ExpenseCategory>> GetCategoriesAsync();
        Task<List<ExpenseStatus>> GetStatusesAsync();
        Task<List<User>> GetUsersAsync();
    }

    public class ExpenseService : IExpenseService
    {
        private static int _nextExpenseId = 5;
        private static List<Expense> _expenses = new List<Expense>
        {
            new Expense
            {
                ExpenseId = 1,
                UserId = 1,
                CategoryId = 1,
                StatusId = 2,
                AmountMinor = 2540,
                Currency = "GBP",
                ExpenseDate = new DateTime(2025, 10, 20),
                Description = "Taxi from airport to client site",
                ReceiptFile = "/receipts/alice/taxi_oct20.jpg",
                SubmittedAt = DateTime.UtcNow.AddDays(-28),
                CreatedAt = DateTime.UtcNow.AddDays(-28),
                UserName = "Alice Example",
                CategoryName = "Travel",
                StatusName = "Submitted"
            },
            new Expense
            {
                ExpenseId = 2,
                UserId = 1,
                CategoryId = 2,
                StatusId = 3,
                AmountMinor = 1425,
                Currency = "GBP",
                ExpenseDate = new DateTime(2025, 9, 15),
                Description = "Client lunch meeting",
                ReceiptFile = "/receipts/alice/lunch_sep15.jpg",
                SubmittedAt = DateTime.UtcNow.AddDays(-63),
                ReviewedBy = 2,
                ReviewedAt = DateTime.UtcNow.AddDays(-62),
                CreatedAt = DateTime.UtcNow.AddDays(-63),
                UserName = "Alice Example",
                CategoryName = "Meals",
                StatusName = "Approved"
            },
            new Expense
            {
                ExpenseId = 3,
                UserId = 1,
                CategoryId = 3,
                StatusId = 1,
                AmountMinor = 799,
                Currency = "GBP",
                ExpenseDate = new DateTime(2025, 11, 1),
                Description = "Office stationery",
                CreatedAt = DateTime.UtcNow.AddDays(-16),
                UserName = "Alice Example",
                CategoryName = "Supplies",
                StatusName = "Draft"
            },
            new Expense
            {
                ExpenseId = 4,
                UserId = 1,
                CategoryId = 4,
                StatusId = 3,
                AmountMinor = 12300,
                Currency = "GBP",
                ExpenseDate = new DateTime(2025, 8, 10),
                Description = "Hotel during client visit",
                ReceiptFile = "/receipts/alice/hotel_aug10.jpg",
                SubmittedAt = DateTime.UtcNow.AddDays(-98),
                ReviewedBy = 2,
                ReviewedAt = DateTime.UtcNow.AddDays(-97),
                CreatedAt = DateTime.UtcNow.AddDays(-99),
                UserName = "Alice Example",
                CategoryName = "Accommodation",
                StatusName = "Approved"
            }
        };

        private static List<ExpenseCategory> _categories = new List<ExpenseCategory>
        {
            new ExpenseCategory { CategoryId = 1, CategoryName = "Travel", IsActive = true },
            new ExpenseCategory { CategoryId = 2, CategoryName = "Meals", IsActive = true },
            new ExpenseCategory { CategoryId = 3, CategoryName = "Supplies", IsActive = true },
            new ExpenseCategory { CategoryId = 4, CategoryName = "Accommodation", IsActive = true },
            new ExpenseCategory { CategoryId = 5, CategoryName = "Other", IsActive = true }
        };

        private static List<ExpenseStatus> _statuses = new List<ExpenseStatus>
        {
            new ExpenseStatus { StatusId = 1, StatusName = "Draft" },
            new ExpenseStatus { StatusId = 2, StatusName = "Submitted" },
            new ExpenseStatus { StatusId = 3, StatusName = "Approved" },
            new ExpenseStatus { StatusId = 4, StatusName = "Rejected" }
        };

        private static List<User> _users = new List<User>
        {
            new User { UserId = 1, UserName = "Alice Example", Email = "alice@example.co.uk", RoleId = 1, ManagerId = 2, IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-6), RoleName = "Employee" },
            new User { UserId = 2, UserName = "Bob Manager", Email = "bob.manager@example.co.uk", RoleId = 2, IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-12), RoleName = "Manager" }
        };

        public Task<List<Expense>> GetAllExpensesAsync()
        {
            return Task.FromResult(_expenses.OrderByDescending(e => e.CreatedAt).ToList());
        }

        public Task<List<Expense>> GetExpensesByUserAsync(int userId)
        {
            return Task.FromResult(_expenses.Where(e => e.UserId == userId).OrderByDescending(e => e.CreatedAt).ToList());
        }

        public Task<List<Expense>> GetPendingExpensesAsync()
        {
            return Task.FromResult(_expenses.Where(e => e.StatusId == 2).OrderBy(e => e.SubmittedAt).ToList());
        }

        public Task<Expense?> GetExpenseByIdAsync(int expenseId)
        {
            return Task.FromResult(_expenses.FirstOrDefault(e => e.ExpenseId == expenseId));
        }

        public Task<int> CreateExpenseAsync(Expense expense)
        {
            expense.ExpenseId = _nextExpenseId++;
            expense.CreatedAt = DateTime.UtcNow;
            expense.StatusId = 1; // Draft
            expense.StatusName = "Draft";
            
            // Set user info (in real app this would come from auth)
            expense.UserId = 1;
            expense.UserName = "Alice Example";
            
            // Get category name
            var category = _categories.FirstOrDefault(c => c.CategoryId == expense.CategoryId);
            expense.CategoryName = category?.CategoryName ?? "Unknown";
            
            _expenses.Add(expense);
            return Task.FromResult(expense.ExpenseId);
        }

        public Task<bool> UpdateExpenseAsync(Expense expense)
        {
            var existing = _expenses.FirstOrDefault(e => e.ExpenseId == expense.ExpenseId);
            if (existing == null) return Task.FromResult(false);

            existing.CategoryId = expense.CategoryId;
            existing.AmountMinor = expense.AmountMinor;
            existing.ExpenseDate = expense.ExpenseDate;
            existing.Description = expense.Description;
            existing.ReceiptFile = expense.ReceiptFile;
            
            // Update category name
            var category = _categories.FirstOrDefault(c => c.CategoryId == expense.CategoryId);
            existing.CategoryName = category?.CategoryName ?? "Unknown";

            // If submitting, update status
            if (expense.StatusId == 2 && existing.StatusId == 1)
            {
                existing.StatusId = 2;
                existing.StatusName = "Submitted";
                existing.SubmittedAt = DateTime.UtcNow;
            }

            return Task.FromResult(true);
        }

        public Task<bool> ApproveExpenseAsync(int expenseId, int reviewerId)
        {
            var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
            if (expense == null) return Task.FromResult(false);

            expense.StatusId = 3;
            expense.StatusName = "Approved";
            expense.ReviewedBy = reviewerId;
            expense.ReviewedAt = DateTime.UtcNow;

            return Task.FromResult(true);
        }

        public Task<bool> RejectExpenseAsync(int expenseId, int reviewerId)
        {
            var expense = _expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
            if (expense == null) return Task.FromResult(false);

            expense.StatusId = 4;
            expense.StatusName = "Rejected";
            expense.ReviewedBy = reviewerId;
            expense.ReviewedAt = DateTime.UtcNow;

            return Task.FromResult(true);
        }

        public Task<List<ExpenseCategory>> GetCategoriesAsync()
        {
            return Task.FromResult(_categories.Where(c => c.IsActive).ToList());
        }

        public Task<List<ExpenseStatus>> GetStatusesAsync()
        {
            return Task.FromResult(_statuses.ToList());
        }

        public Task<List<User>> GetUsersAsync()
        {
            return Task.FromResult(_users.Where(u => u.IsActive).ToList());
        }
    }
}
