![Header image](https://github.com/DougChisholm/App-Mod-Assist/blob/main/repo-header.png)

# Expense Management System - Modern Cloud Application

A modern cloud-native expense management application built with ASP.NET Core 8.0 and deployed to Azure. This application demonstrates how legacy applications can be modernized into cloud-native solutions with optional AI capabilities.

## Features

### Core Functionality
- ✅ **Add Expense** - Submit new expense entries with amount, date, category, and description
- ✅ **View Expenses** - List all expenses with filtering capabilities
- ✅ **Approve Expenses** - Manager workflow for approving/rejecting submitted expenses
- ✅ **RESTful API** - Complete API with Swagger documentation
- ✅ **Modern UI** - Clean Bootstrap 5 interface with responsive design

### Optional AI Enhancement
- 🤖 **AI Chat Assistant** - Natural language interface for expense management
- 🔍 **RAG Pattern** - Retrieval-Augmented Generation for contextual responses
- 💡 **Function Calling** - AI can execute expense operations via APIs

## Quick Start

### Prerequisites
- Azure CLI installed and configured
- Active Azure subscription

### Deployment

1. **Login to Azure:**
   ```bash
   az login
   az account set --subscription "Your Subscription Name"
   ```

2. **Deploy (Default - No AI):**
   ```bash
   ./deploy.sh
   ```

3. **Deploy with AI Chat UI:**
   ```bash
   INCLUDE_CHAT_UI=true ./deploy.sh
   ```

4. **Access the application:**
   ```
   https://your-app-name.azurewebsites.net/Index
   ```

**IMPORTANT:** Navigate to `/Index` endpoint (not just the root URL)

## Documentation

- **[Deployment Guide](DEPLOYMENT_GUIDE.md)** - Detailed deployment instructions and configuration
- **[Architecture](ARCHITECTURE.md)** - System architecture, diagrams, and technical details

## Technology Stack

- **Backend:** ASP.NET Core 8.0 (Razor Pages + Web API)
- **Frontend:** Bootstrap 5, jQuery
- **API Documentation:** Swagger/OpenAPI
- **Cloud:** Azure (UK South region)
- **IaC:** Bicep
- **AI (Optional):** Azure OpenAI (GPT-3.5-Turbo), Cognitive Search

## Project Structure

```
├── infrastructure/          # Bicep infrastructure as code
│   ├── main.bicep          # Main orchestrator
│   ├── appservice.bicep    # App Service resources
│   └── genai.bicep         # Optional GenAI resources
├── src/                    # ASP.NET Core application
│   ├── Controllers/        # API controllers
│   ├── Models/            # Data models
│   ├── Pages/             # Razor Pages
│   ├── Services/          # Business logic
│   └── chatui/            # GenAI chat functionality
├── Database-Schema/        # SQL Server schema
├── Legacy-Screenshots/     # Original app screenshots
├── deploy.sh              # Deployment script
└── app.zip                # Deployable application package
```

## API Endpoints

Access Swagger documentation at: `https://your-app-name.azurewebsites.net/swagger`

Key endpoints:
- `GET /api/expenses` - Get all expenses
- `GET /api/expenses/pending` - Get pending approvals  
- `POST /api/expenses` - Create new expense
- `POST /api/expenses/{id}/approve` - Approve an expense
- `POST /api/expenses/{id}/reject` - Reject an expense

## Cost Estimation

**Default Deployment (No AI):** ~£0/month
- App Service: Free tier (F1)

**With AI Chat UI:** ~£30-50/month
- App Service: Free tier
- Azure OpenAI: Pay-per-use
- Cognitive Search: Basic tier

See [Architecture](ARCHITECTURE.md) for detailed cost breakdown.

## Data

Currently uses **in-memory dummy data** for demonstration. The application can be easily connected to Azure SQL Database using the provided schema in `/Database-Schema/database_schema.sql`.

## Development

### Local Development
```bash
cd src
dotnet run
```

Access at `https://localhost:5001/Index`

### Build and Package
```bash
cd src
dotnet publish -c Release -o publish
cd publish
zip -r ../../app.zip .
```

## Security Features

- ✅ HTTPS enforced
- ✅ Managed Identity enabled
- ✅ TLS 1.2 minimum
- ✅ FTPS disabled
- ✅ No credentials in code

## License

See LICENSE file for details.

## Contributing

This is a demonstration project showing legacy app modernization. For collaboration guidelines, see the Guiding-Principles document.
