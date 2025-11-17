using Azure;
using Azure.AI.OpenAI;
using ExpenseManagement.Services;
using System.Text.Json;

namespace ExpenseManagement.chatui
{
    public interface IChatService
    {
        Task<string> SendMessageAsync(string userMessage);
        bool IsEnabled { get; }
    }

    public class ChatService : IChatService
    {
        private readonly GenAISettings _settings;
        private readonly IExpenseService _expenseService;
        private readonly OpenAIClient? _client;
        private readonly ILogger<ChatService> _logger;

        public bool IsEnabled => _settings.IncludeChatUI && !string.IsNullOrEmpty(_settings.OpenAIEndpoint);

        public ChatService(GenAISettings settings, IExpenseService expenseService, ILogger<ChatService> logger)
        {
            _settings = settings;
            _expenseService = expenseService;
            _logger = logger;

            if (IsEnabled && !string.IsNullOrEmpty(_settings.OpenAIKey))
            {
                _client = new OpenAIClient(
                    new Uri(_settings.OpenAIEndpoint!),
                    new AzureKeyCredential(_settings.OpenAIKey));
            }
        }

        public async Task<string> SendMessageAsync(string userMessage)
        {
            if (!IsEnabled || _client == null)
            {
                return "Chat UI is not enabled. Set INCLUDE_CHAT_UI=true during deployment.";
            }

            try
            {
                // Build system message with function definitions
                var systemMessage = @"You are an expense management assistant. You help users manage their expenses through natural language.

Available functions you can call:
1. get_all_expenses() - Get all expenses
2. get_pending_expenses() - Get expenses waiting for approval
3. create_expense(amount, date, category, description) - Create a new expense
4. approve_expense(expenseId) - Approve an expense
5. reject_expense(expenseId) - Reject an expense

When users ask about expenses, use these functions to help them. Always be helpful and explain what you're doing.";

                var chatOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = _settings.OpenAIDeploymentName ?? "gpt-35-turbo",
                    Messages =
                    {
                        new ChatRequestSystemMessage(systemMessage),
                        new ChatRequestUserMessage(userMessage)
                    },
                    Temperature = 0.7f,
                    MaxTokens = 800
                };

                // Check if the user is asking about expenses
                if (userMessage.ToLower().Contains("expense") ||
                    userMessage.ToLower().Contains("pending") ||
                    userMessage.ToLower().Contains("approve") ||
                    userMessage.ToLower().Contains("list") ||
                    userMessage.ToLower().Contains("show"))
                {
                    // Get context from expenses
                    var expenses = await _expenseService.GetAllExpensesAsync();
                    var pendingExpenses = await _expenseService.GetPendingExpensesAsync();

                    var context = $@"
Current expense data:
- Total expenses: {expenses.Count}
- Pending approval: {pendingExpenses.Count}

Recent expenses:
{string.Join("\n", expenses.Take(5).Select(e => $"- ID:{e.ExpenseId} {e.FormattedAmount} for {e.CategoryName} on {e.ExpenseDate:dd/MM/yyyy} - Status: {e.StatusName}"))}
";

                    chatOptions.Messages.Add(new ChatRequestSystemMessage($"Context: {context}"));
                }

                Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(chatOptions);
                ChatCompletions completions = response.Value;

                if (completions.Choices.Count > 0)
                {
                    return completions.Choices[0].Message.Content;
                }

                return "I apologize, but I couldn't generate a response. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error communicating with Azure OpenAI");
                return $"An error occurred while processing your request: {ex.Message}";
            }
        }
    }
}
