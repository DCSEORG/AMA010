# Quick Reference Guide

## One-Line Commands

### Deploy without AI (Free)
```bash
./deploy.sh
```

### Deploy with AI Chat UI
```bash
INCLUDE_CHAT_UI=true ./deploy.sh
```

### View Application Logs
```bash
az webapp log tail --resource-group ExpenseManagementRG --name <your-app-name>
```

### Redeploy Application Code Only
```bash
az webapp deploy --resource-group ExpenseManagementRG --name <your-app-name> --src-path ./app.zip
```

### Delete All Resources
```bash
az group delete --name ExpenseManagementRG --yes
```

## Application URLs

After deployment, access your application at:

### Main Pages
- **Add Expense:** `https://<your-app>.azurewebsites.net/Index`
- **View Expenses:** `https://<your-app>.azurewebsites.net/Expenses`
- **Approve Expenses:** `https://<your-app>.azurewebsites.net/Approve`
- **AI Chat (if enabled):** `https://<your-app>.azurewebsites.net/Chat`

### Developer Resources
- **API Documentation:** `https://<your-app>.azurewebsites.net/swagger`
- **Health Check:** `https://<your-app>.azurewebsites.net/`

## API Endpoints

### Expenses
```bash
# Get all expenses
GET /api/expenses

# Get pending expenses
GET /api/expenses/pending

# Get expense by ID
GET /api/expenses/{id}

# Create new expense
POST /api/expenses
{
  "amountMinor": 2500,
  "categoryId": 1,
  "expenseDate": "2025-11-17",
  "description": "Client meeting lunch"
}

# Update expense
PUT /api/expenses/{id}

# Submit expense for approval
POST /api/expenses/{id}/submit

# Approve expense
POST /api/expenses/{id}/approve

# Reject expense
POST /api/expenses/{id}/reject

# Get categories
GET /api/expenses/categories

# Get statuses
GET /api/expenses/statuses
```

## Environment Variables

### During Deployment
```bash
# Enable AI Chat UI
export INCLUDE_CHAT_UI=true

# Or inline
INCLUDE_CHAT_UI=true ./deploy.sh
```

### In Azure App Service (automatically set by deploy.sh)
```
GenAI__IncludeChatUI=true
GenAI__OpenAIEndpoint=https://<your-openai>.openai.azure.com/
GenAI__OpenAIKey=<your-key>
GenAI__DeploymentName=gpt-35-turbo
```

## Configuration Files

### Infrastructure (Bicep)
- `infrastructure/main.bicep` - Main deployment
- `infrastructure/appservice.bicep` - App Service config
- `infrastructure/genai.bicep` - GenAI resources

### Application
- `src/appsettings.json` - General settings
- `src/appsettings.Development.json` - Development settings

## Rebuild Application Package

```bash
cd src
dotnet publish -c Release -o publish
cd publish
zip -r ../../app.zip .
cd ../..
```

## Local Development

### Run Locally
```bash
cd src
dotnet run
```

Access at: `https://localhost:5001/Index`

### Build
```bash
cd src
dotnet build
```

### Test
```bash
cd src
dotnet test
```

## Troubleshooting

### App Not Starting
```bash
# Check logs
az webapp log tail --resource-group ExpenseManagementRG --name <your-app-name>

# Restart app
az webapp restart --resource-group ExpenseManagementRG --name <your-app-name>
```

### Chat UI Says "Not Enabled"
- Verify you deployed with `INCLUDE_CHAT_UI=true`
- Check App Service configuration has GenAI settings
- Allow 2-3 minutes for OpenAI deployment to complete

### Deployment Fails
- Ensure you're logged in: `az login`
- Check subscription: `az account show`
- Verify you have permissions to create resources

## Cost Management

### View Current Costs
```bash
az consumption usage list --resource-group ExpenseManagementRG
```

### Stop App Service (preserves resources)
```bash
az webapp stop --resource-group ExpenseManagementRG --name <your-app-name>
```

### Start App Service
```bash
az webapp start --resource-group ExpenseManagementRG --name <your-app-name>
```

## Azure CLI Shortcuts

### List Resources
```bash
az resource list --resource-group ExpenseManagementRG --output table
```

### Get App Service URL
```bash
az webapp show --resource-group ExpenseManagementRG --name <your-app-name> --query defaultHostName -o tsv
```

### Update App Settings
```bash
az webapp config appsettings set \
  --resource-group ExpenseManagementRG \
  --name <your-app-name> \
  --settings KEY=VALUE
```

## Monitoring

### Stream Logs
```bash
az webapp log tail --resource-group ExpenseManagementRG --name <your-app-name>
```

### Download Logs
```bash
az webapp log download --resource-group ExpenseManagementRG --name <your-app-name>
```

## Useful Azure Portal Links

After deployment:
1. Go to https://portal.azure.com
2. Search for "ExpenseManagementRG"
3. View all deployed resources
4. Access App Service for configuration and monitoring
