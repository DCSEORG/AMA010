# Azure Services Architecture

## Default Deployment (includeChatUI = false)

```
┌─────────────────────────────────────────────────────────────┐
│                     Azure Resource Group                     │
│                   (ExpenseManagementRG)                      │
│                                                               │
│   ┌───────────────────────────────────────────────┐         │
│   │         Azure App Service Plan                │         │
│   │         (Free Tier, Linux)                    │         │
│   │                                                │         │
│   │   ┌─────────────────────────────────────┐    │         │
│   │   │     App Service (Web App)           │    │         │
│   │   │                                      │    │         │
│   │   │  • ASP.NET Core 8.0                 │    │         │
│   │   │  • Razor Pages UI                   │    │         │
│   │   │  • REST API (Swagger)               │    │         │
│   │   │  • Managed Identity (Enabled)       │    │         │
│   │   │  • HTTPS Only                       │    │         │
│   │   │                                      │    │         │
│   │   │  Endpoints:                          │    │         │
│   │   │  • /Index - Add Expense             │    │         │
│   │   │  • /Expenses - View List            │    │         │
│   │   │  • /Approve - Manager Approval      │    │         │
│   │   │  • /swagger - API Docs              │    │         │
│   │   │  • /api/expenses/* - REST API       │    │         │
│   │   └─────────────────────────────────────┘    │         │
│   └───────────────────────────────────────────────┘         │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                          │
                          │ HTTPS
                          ▼
                   Internet Users
```

## Enhanced Deployment (includeChatUI = true)

```
┌──────────────────────────────────────────────────────────────────────┐
│                        Azure Resource Group                           │
│                      (ExpenseManagementRG)                           │
│                                                                        │
│   ┌────────────────────────────────────────────────┐                │
│   │         Azure App Service Plan                 │                │
│   │         (Free Tier, Linux)                     │                │
│   │                                                 │                │
│   │   ┌──────────────────────────────────────┐    │                │
│   │   │     App Service (Web App)            │    │                │
│   │   │                                       │    │                │
│   │   │  • ASP.NET Core 8.0                  │────┼────┐          │
│   │   │  • Razor Pages UI                    │    │    │          │
│   │   │  • REST API                          │    │    │          │
│   │   │  • Chat UI (GenAI)                   │    │    │          │
│   │   │  • Managed Identity                  │    │    │          │
│   │   │                                       │    │    │          │
│   │   │  Additional Endpoints:                │    │    │          │
│   │   │  • /Chat - AI Chat Interface         │    │    │          │
│   │   └──────────────────────────────────────┘    │    │          │
│   └────────────────────────────────────────────────┘    │          │
│                                                           │          │
│   ┌──────────────────────────────────────────┐          │          │
│   │     Azure OpenAI Service                 │◄─────────┘          │
│   │                                           │                     │
│   │  • Model: GPT-3.5-Turbo (0613)          │                     │
│   │  • Deployment: gpt-35-turbo             │                     │
│   │  • Location: UK South                    │                     │
│   │  • SKU: S0 (Standard)                   │                     │
│   │  • Managed Identity Auth                 │                     │
│   │                                           │                     │
│   │  Capabilities:                            │                     │
│   │  • Natural language queries              │                     │
│   │  • Function calling to APIs              │                     │
│   │  • Expense data interaction              │                     │
│   └──────────────────────────────────────────┘                     │
│                          ▲                                           │
│                          │ (RAG Pattern)                            │
│                          │                                           │
│   ┌──────────────────────────────────────────┐                     │
│   │   Azure Cognitive Search                 │                     │
│   │                                           │                     │
│   │  • SKU: Basic                            │                     │
│   │  • Location: UK South                    │                     │
│   │  • Vector search enabled                 │                     │
│   │  • Managed Identity Auth                 │                     │
│   │                                           │                     │
│   │  Purpose:                                 │                     │
│   │  • Document retrieval                    │                     │
│   │  • Context augmentation                  │                     │
│   │  • Knowledge base indexing               │                     │
│   └──────────────────────────────────────────┘                     │
│                                                                        │
└──────────────────────────────────────────────────────────────────────┘
                          │
                          │ HTTPS
                          ▼
                   Internet Users


## Future Enhancement: Database Integration

When Azure SQL Database is added (currently using dummy data):

┌──────────────────────────────────────────────────────────────────────┐
│                        Azure Resource Group                           │
│                      (ExpenseManagementRG)                           │
│                                                                        │
│   ┌────────────────────────┐         ┌─────────────────────────┐   │
│   │   App Service          │────────▶│   Azure SQL Database    │   │
│   │                        │         │                         │   │
│   │  • Managed Identity    │ Auth    │  • Managed Identity     │   │
│   │    (System Assigned)   │ via MI  │    Authentication       │   │
│   │                        │         │  • Tables:              │   │
│   │                        │         │    - Expenses           │   │
│   │                        │         │    - Users              │   │
│   │                        │         │    - Categories         │   │
│   │                        │         │    - ExpenseStatus      │   │
│   └────────────────────────┘         └─────────────────────────┘   │
│                                                                        │
└──────────────────────────────────────────────────────────────────────┘

Connection Flow:
1. App Service authenticates to SQL Database using its Managed Identity
2. No connection strings or passwords needed in application code
3. Azure AD authentication for secure, credential-free access


## Data Flow

### User Submitting Expense:
1. User fills form at /Index
2. Browser sends POST to App Service
3. App Service validates data
4. API creates expense (currently in-memory, future: SQL DB)
5. Response returned to user

### Manager Approving Expense:
1. Manager views pending at /Approve
2. Clicks Approve/Reject button
3. POST to App Service API endpoint
4. Status updated (currently in-memory, future: SQL DB)
5. Page refreshes with updated list

### GenAI Chat Query (when enabled):
1. User types natural language query at /Chat
2. App Service sends prompt to Azure OpenAI
3. OpenAI may call Search API for RAG context
4. OpenAI generates function calls to Expense APIs
5. App Service executes API calls
6. Results formatted and returned to user


## Cost Considerations

### Default Deployment (~£0/month for development):
- App Service: Free tier (F1)
- Total: Essentially free for POC/development

### With GenAI (~£30-50/month estimated):
- App Service: Free tier
- Azure OpenAI: Pay-per-use (depends on usage)
- Cognitive Search: Basic tier (~£60/month, but can use free tier)
- Total: Varies with usage

### Production Ready (Future):
- Add Azure SQL Database (~£4/month for Basic tier)
- Upgrade App Service to B1 (~£10/month)
- Keep OpenAI pay-as-you-go
- Total: ~£50-100/month depending on usage


## Security Features

1. **HTTPS Only** - All traffic encrypted
2. **Managed Identities** - No credentials in code
3. **TLS 1.2+** - Modern encryption standards
4. **FTPS Disabled** - Secure file transfer only
5. **Azure AD Integration** - Future: User authentication
6. **Network Security** - Can add VNet integration, Private Endpoints


## Scalability

Current: Single instance suitable for POC/demos
Future Options:
- Auto-scaling based on load
- Multiple instances with load balancing
- Azure Front Door for global distribution
- Redis cache for performance
- Application Insights for monitoring
