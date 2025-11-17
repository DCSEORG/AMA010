# Getting Started - Expense Management System

## 🚀 Quick Start (3 Steps)

### Step 1: Prerequisites
```bash
# Install Azure CLI (if not already installed)
# Windows: Download from https://aka.ms/installazurecliwindows
# macOS: brew install azure-cli
# Linux: See https://docs.microsoft.com/cli/azure/install-azure-cli

# Login to Azure
az login

# Set your subscription (if you have multiple)
az account list --output table
az account set --subscription "Your Subscription Name"
```

### Step 2: Deploy
```bash
# Clone or download this repository
git clone <your-repo-url>
cd AMA010

# Make deploy script executable
chmod +x deploy.sh

# Deploy (choose one option)

# Option A: Deploy WITHOUT AI Chat (FREE)
./deploy.sh

# Option B: Deploy WITH AI Chat (~£30-50/month)
INCLUDE_CHAT_UI=true ./deploy.sh
```

### Step 3: Access Your App
After deployment completes (2-5 minutes), you'll see:
```
Deployment Complete!
IMPORTANT: Access the application at:
  https://your-app-name.azurewebsites.net/Index
```

🎉 **You're done!** Open that URL in your browser.

---

## 📋 What You Get

### Pages
1. **Add Expense** (`/Index`) - Submit new expenses
2. **View Expenses** (`/Expenses`) - List and filter expenses  
3. **Approve Expenses** (`/Approve`) - Manager workflow
4. **AI Chat** (`/Chat`) - Natural language queries (if enabled)

### API
- **Swagger Docs:** `https://your-app.azurewebsites.net/swagger`
- RESTful endpoints for all operations

---

## 💡 Common Tasks

### View Application Logs
```bash
az webapp log tail \
  --resource-group ExpenseManagementRG \
  --name <your-app-name>
```

### Redeploy Code Changes
```bash
# After modifying code:
cd src
dotnet publish -c Release -o publish
cd publish
zip -r ../../app.zip .
cd ../..

# Deploy to Azure
az webapp deploy \
  --resource-group ExpenseManagementRG \
  --name <your-app-name> \
  --src-path ./app.zip
```

### Delete Everything
```bash
az group delete --name ExpenseManagementRG --yes
```

---

## 🤔 Troubleshooting

### "Page not found"
- Make sure you navigate to `/Index` (not just the root URL)
- Wait 1-2 minutes after deployment for app to warm up

### "Chat UI not enabled"
- Verify you deployed with `INCLUDE_CHAT_UI=true`
- Check App Service settings include GenAI configuration
- Azure OpenAI deployment takes 2-3 minutes

### Deployment fails
- Check you're logged in: `az account show`
- Verify you have Contributor access to subscription
- Try a different resource group name if it exists

### Still having issues?
- Check logs: `az webapp log tail --resource-group ExpenseManagementRG --name <app-name>`
- Restart app: `az webapp restart --resource-group ExpenseManagementRG --name <app-name>`
- Review [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for detailed instructions

---

## 📚 Next Steps

1. **Customize the Application**
   - Modify screens in `src/Pages/`
   - Update business logic in `src/Services/`
   - Adjust UI in layout files

2. **Connect to Database**
   - Use schema in `Database-Schema/database_schema.sql`
   - Update `ExpenseService` to use Entity Framework
   - Configure connection string in App Settings

3. **Add Authentication**
   - Integrate Azure AD
   - Implement role-based access control
   - See [ARCHITECTURE.md](ARCHITECTURE.md) for recommendations

4. **Monitor in Production**
   - Add Application Insights
   - Set up alerts
   - Review [SECURITY_SUMMARY.md](SECURITY_SUMMARY.md)

---

## 📖 Documentation

- **[README.md](README.md)** - Overview and features
- **[DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)** - Detailed deployment guide
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - System architecture
- **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** - Command reference
- **[SECURITY_SUMMARY.md](SECURITY_SUMMARY.md)** - Security information

---

## 💰 Cost Information

**Default (No AI):** ~£0/month
- Free tier App Service

**With AI Chat:** ~£30-50/month  
- Free tier App Service
- Azure OpenAI (pay-per-use)
- Cognitive Search (Basic tier)

**Stop costs at any time:**
```bash
az group delete --name ExpenseManagementRG --yes
```

---

## 🎯 Technology Stack

- **Backend:** ASP.NET Core 8.0
- **Frontend:** Bootstrap 5 + jQuery
- **Cloud:** Azure (UK South)
- **IaC:** Bicep
- **AI:** Azure OpenAI GPT-3.5-Turbo (optional)

---

## ✅ Support

For issues, questions, or contributions:
1. Review documentation in this repository
2. Check Azure Portal for resource status
3. Review deployment logs
4. Create an issue in the repository

---

**Happy Modernizing! 🚀**
