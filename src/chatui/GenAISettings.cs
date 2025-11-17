namespace ExpenseManagement.chatui
{
    public class GenAISettings
    {
        public bool IncludeChatUI { get; set; }
        public string? OpenAIEndpoint { get; set; }
        public string? OpenAIKey { get; set; }
        public string? OpenAIDeploymentName { get; set; }
        public string? SearchEndpoint { get; set; }
        public string? SearchKey { get; set; }
    }
}
