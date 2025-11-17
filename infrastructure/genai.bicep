// Azure OpenAI and supporting resources for GenAI Chat UI
// Model: GPT-3.5-Turbo in UK South
// This deployment is OPTIONAL - controlled by includeChatUI parameter

@description('Include Chat UI and GenAI resources')
param includeChatUI bool = false

@description('Location for all resources')
param location string = 'uksouth'

@description('Name of the Azure OpenAI resource')
param openAIName string = 'openai-${uniqueString(resourceGroup().id)}'

@description('Name of the Cognitive Search resource for RAG')
param searchServiceName string = 'search-${uniqueString(resourceGroup().id)}'

// Azure OpenAI Service
resource openAIAccount 'Microsoft.CognitiveServices/accounts@2023-05-01' = if (includeChatUI) {
  name: openAIName
  location: location
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    customSubDomainName: openAIName
    publicNetworkAccess: 'Enabled'
  }
}

// GPT-3.5-Turbo deployment
resource gpt35TurboDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = if (includeChatUI) {
  parent: openAIAccount
  name: 'gpt-35-turbo'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-35-turbo'
      version: '0613'
    }
  }
}

// Azure Cognitive Search for RAG (Retrieval-Augmented Generation)
resource searchService 'Microsoft.Search/searchServices@2023-11-01' = if (includeChatUI) {
  name: searchServiceName
  location: location
  sku: {
    name: 'basic'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    publicNetworkAccess: 'enabled'
  }
}

output openAIEndpoint string = includeChatUI ? openAIAccount.properties.endpoint : ''
output openAIKey string = includeChatUI ? openAIAccount.listKeys().key1 : ''
output openAIName string = includeChatUI ? openAIAccount.name : ''
output searchEndpoint string = includeChatUI ? 'https://${searchService.name}.search.windows.net' : ''
output searchKey string = includeChatUI ? searchService.listAdminKeys().primaryKey : ''
output deploymentName string = 'gpt-35-turbo'
output includeChatUI bool = includeChatUI
