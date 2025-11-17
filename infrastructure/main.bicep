// Main deployment file for Expense Management System
// Orchestrates all Azure resources

targetScope = 'subscription'

@description('Resource Group name')
param resourceGroupName string = 'ExpenseManagementRG'

@description('Location for all resources')
param location string = 'uksouth'

@description('Include Chat UI and GenAI resources (default: false)')
param includeChatUI bool = false

// Create Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

// Deploy App Service
module appService 'appservice.bicep' = {
  scope: rg
  name: 'appServiceDeployment'
  params: {
    location: location
  }
}

// Deploy GenAI resources (conditional)
module genAI 'genai.bicep' = {
  scope: rg
  name: 'genAIDeployment'
  params: {
    location: location
    includeChatUI: includeChatUI
  }
}

output appServiceName string = appService.outputs.appServiceName
output appServiceUrl string = appService.outputs.appServiceUrl
output appServicePrincipalId string = appService.outputs.appServicePrincipalId
output openAIEndpoint string = genAI.outputs.openAIEndpoint
output openAIName string = genAI.outputs.openAIName
output deploymentName string = genAI.outputs.deploymentName
output resourceGroupName string = rg.name
output includeChatUI bool = includeChatUI
