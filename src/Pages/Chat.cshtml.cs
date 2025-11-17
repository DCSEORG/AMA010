using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExpenseManagement.chatui;

namespace ExpenseManagement.Pages;

public class ChatModel : PageModel
{
    private readonly IChatService _chatService;
    
    [BindProperty]
    public string? UserMessage { get; set; }
    
    public List<ChatMessage> ChatHistory { get; set; } = new();
    
    public bool IsEnabled => _chatService.IsEnabled;

    public ChatModel(IChatService chatService)
    {
        _chatService = chatService;
    }

    public void OnGet()
    {
        // Load chat history from session if available
        var history = HttpContext.Session.GetString("ChatHistory");
        if (!string.IsNullOrEmpty(history))
        {
            ChatHistory = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(history) ?? new();
        }
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(UserMessage))
        {
            return RedirectToPage();
        }

        // Load existing chat history
        var history = HttpContext.Session.GetString("ChatHistory");
        if (!string.IsNullOrEmpty(history))
        {
            ChatHistory = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(history) ?? new();
        }
        
        // Add user message
        ChatHistory.Add(new ChatMessage { Content = UserMessage, IsUser = true });
        
        // Get AI response
        var response = await _chatService.SendMessageAsync(UserMessage);
        ChatHistory.Add(new ChatMessage { Content = response, IsUser = false });
        
        // Save to session
        HttpContext.Session.SetString("ChatHistory", 
            System.Text.Json.JsonSerializer.Serialize(ChatHistory));
        
        return RedirectToPage();
    }
}

public class ChatMessage
{
    public string Content { get; set; } = string.Empty;
    public bool IsUser { get; set; }
}
