# Expense Management System - Deployment Guide

A modern cloud-native expense management application built on ASP.NET Core and deployed to Azure.

## Quick Start

### Prerequisites
- Azure CLI installed and configured
- Active Azure subscription
- Git (for cloning the repository)

### One-Line Deployment

1. **Login to Azure:**
   ```bash
   az login
   ```

2. **Set your subscription (if you have multiple):**
   ```bash
   az account set --subscription "Your Subscription Name"
   ```

3. **Deploy everything:**
   ```bash
   ./deploy.sh
   ```

**That's it!** The script will:
- Create all Azure resources (App Service, etc.)
- Deploy the application code
- Display the application URL

### Accessing the Application

After deployment completes, access your application at:
```
https://your-app-name.azurewebsites.net/Index
```

**IMPORTANT:** Navigate to `/Index` (not just the root URL)

## Features

The application provides three main pages:

1. **Add Expense** (`/Index`) - Submit new expenses
2. **View Expenses** (`/Expenses`) - List and filter all expenses
3. **Approve Expenses** (`/Approve`) - Manager approval workflow

### API Documentation

Swagger API documentation is available at:
```
https://your-app-name.azurewebsites.net/swagger
```

## Optional: GenAI Chat UI

By default, the application deploys **WITHOUT** the GenAI Chat UI to keep costs low.

### To Deploy WITH GenAI Chat UI:

```bash
INCLUDE_CHAT_UI=true ./deploy.sh
```

This will additionally deploy:
- Azure OpenAI Service (GPT-3.5-Turbo model)
- Azure Cognitive Search (for RAG pattern)
- Chat UI at `/Chat`

**Note:** GenAI resources will incur additional Azure costs.

## Architecture

### Default Deployment (Chat UI = false)
- **App Service** (Free tier, UK South)
- **ASP.NET Core 8.0** application with Razor Pages
- **Swagger/OpenAPI** documentation

### With GenAI (Chat UI = true)
- All of the above, plus:
- **Azure OpenAI Service** (GPT-3.5-Turbo, UK South)
- **Azure Cognitive Search** (Basic tier)
- **Natural language** expense queries via chat interface

## Data

Currently uses **dummy data** for demonstration purposes. The application returns mock expense data without requiring a database connection.

### Database Schema

The full database schema is available in `/Database-Schema/database_schema.sql` for future integration with Azure SQL Database.

## API Endpoints

All API endpoints are documented in Swagger. Key endpoints include:

- `GET /api/expenses` - Get all expenses
- `GET /api/expenses/pending` - Get pending approvals
- `POST /api/expenses` - Create new expense
- `POST /api/expenses/{id}/approve` - Approve an expense
- `POST /api/expenses/{id}/reject` - Reject an expense
- `GET /api/expenses/categories` - Get all categories

## Development

### Local Development

1. Navigate to the `src` folder:
   ```bash
   cd src
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Access at `https://localhost:5001/Index`

### Build and Publish

```bash
cd src
dotnet publish -c Release -o publish
cd publish
zip -r ../../app.zip .
```

## Infrastructure as Code

All infrastructure is defined in Bicep files in the `/infrastructure` folder:

- `main.bicep` - Main orchestrator
- `appservice.bicep` - App Service configuration
- `genai.bicep` - Optional GenAI resources

## Troubleshooting

### View Application Logs
```bash
az webapp log tail --resource-group ExpenseManagementRG --name <your-app-name>
```

### Redeploy Application Code
```bash
az webapp deploy \
  --resource-group ExpenseManagementRG \
  --name <your-app-name> \
  --src-path ./app.zip
```

### Delete All Resources
```bash
az group delete --name ExpenseManagementRG --yes
```

## Technology Stack

- **Backend:** ASP.NET Core 8.0 (Razor Pages + Web API)
- **Frontend:** Bootstrap 5, jQuery
- **API Documentation:** Swagger/OpenAPI
- **Cloud Platform:** Azure (UK South region)
- **IaC:** Bicep
- **Optional AI:** Azure OpenAI (GPT-3.5-Turbo)

## Security

- HTTPS enforced
- Managed Identity enabled (for future database integration)
- TLS 1.2 minimum
- FTPS disabled

## License

See LICENSE file for details.

## Support

For issues or questions, please refer to the repository documentation or create an issue.
