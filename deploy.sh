#!/bin/bash

# Expense Management System - Main Deployment Script
# This script deploys all Azure infrastructure and application code

set -e

echo "=========================================="
echo "Expense Management System Deployment"
echo "=========================================="
echo ""

# Configuration
RESOURCE_GROUP="ExpenseManagementRG"
LOCATION="uksouth"
INCLUDE_CHAT_UI=${INCLUDE_CHAT_UI:-false}

# Check if user is logged in to Azure
echo "Checking Azure login status..."
az account show > /dev/null 2>&1 || {
    echo "ERROR: Not logged in to Azure. Please run 'az login' first."
    exit 1
}

echo "✓ Azure login verified"
echo ""

# Display deployment configuration
echo "Deployment Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  Include Chat UI: $INCLUDE_CHAT_UI"
echo ""
echo "NOTE: To deploy WITH GenAI Chat UI, run: INCLUDE_CHAT_UI=true ./deploy.sh"
echo ""

# Deploy infrastructure
echo "Step 1: Deploying Azure infrastructure..."
echo "=========================================="

DEPLOYMENT_OUTPUT=$(az deployment sub create \
    --location "$LOCATION" \
    --template-file infrastructure/main.bicep \
    --parameters resourceGroupName="$RESOURCE_GROUP" \
                location="$LOCATION" \
                includeChatUI="$INCLUDE_CHAT_UI" \
    --query 'properties.outputs' \
    --output json)

APP_SERVICE_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.appServiceName.value')
APP_SERVICE_URL=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.appServiceUrl.value')
OPENAI_ENDPOINT=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.openAIEndpoint.value // empty')
OPENAI_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.openAIName.value // empty')
DEPLOYMENT_NAME=$(echo "$DEPLOYMENT_OUTPUT" | jq -r '.deploymentName.value')

echo "✓ Infrastructure deployed successfully"
echo ""
echo "App Service Name: $APP_SERVICE_NAME"
echo "App Service URL: $APP_SERVICE_URL"

if [ "$INCLUDE_CHAT_UI" = "true" ]; then
    echo "OpenAI Endpoint: $OPENAI_ENDPOINT"
    echo "OpenAI Name: $OPENAI_NAME"
    
    # Configure GenAI settings in App Service
    echo ""
    echo "Configuring GenAI settings..."
    
    # Get OpenAI API Key
    OPENAI_KEY=$(az cognitiveservices account keys list \
        --resource-group "$RESOURCE_GROUP" \
        --name "$OPENAI_NAME" \
        --query 'key1' \
        --output tsv)
    
    # Configure App Service settings
    az webapp config appsettings set \
        --resource-group "$RESOURCE_GROUP" \
        --name "$APP_SERVICE_NAME" \
        --settings \
            GenAI__IncludeChatUI="true" \
            GenAI__OpenAIEndpoint="$OPENAI_ENDPOINT" \
            GenAI__OpenAIKey="$OPENAI_KEY" \
            GenAI__DeploymentName="$DEPLOYMENT_NAME" \
        --output none
    
    echo "✓ GenAI settings configured"
fi

echo ""

# Deploy application code
echo "Step 2: Deploying application code..."
echo "=========================================="

if [ -f "app.zip" ]; then
    echo "Deploying app.zip to Azure App Service..."
    az webapp deploy \
        --resource-group "$RESOURCE_GROUP" \
        --name "$APP_SERVICE_NAME" \
        --src-path ./app.zip \
        --type zip
    
    echo "✓ Application deployed successfully"
else
    echo "WARNING: app.zip not found. Skipping application deployment."
    echo "Please build the application first using: cd src && dotnet publish -c Release && cd .."
fi

echo ""
echo "=========================================="
echo "Deployment Complete!"
echo "=========================================="
echo ""
echo "IMPORTANT: Access the application at:"
echo "  $APP_SERVICE_URL/Index"
echo ""
echo "Note: Navigate to /Index (not just the root URL)"
echo ""

if [ "$INCLUDE_CHAT_UI" = "true" ]; then
    echo "GenAI Chat UI is enabled at:"
    echo "  $APP_SERVICE_URL/Chat"
    echo ""
fi

echo "To view logs: az webapp log tail --resource-group $RESOURCE_GROUP --name $APP_SERVICE_NAME"
echo ""
