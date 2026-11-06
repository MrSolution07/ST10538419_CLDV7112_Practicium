targetScope = 'resourceGroup'

param location string
param functionName string
param serverFarmId string

@secure()
param storageConnection string

@secure()
param sqlConnection string

@secure()
param functionSharedSecret string

@secure()
param insightsConnection string

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionName
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: serverFarmId
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|10'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storageConnection
        }
        {
          name: 'Database__Provider'
          value: 'SqlServer'
        }
        {
          name: 'ConnectionStrings__AzureSql'
          value: sqlConnection
        }
        {
          name: 'Storage__BlobConnectionString'
          value: storageConnection
        }
        {
          name: 'Storage__QueueConnectionString'
          value: storageConnection
        }
        {
          name: 'Storage__TableConnectionString'
          value: storageConnection
        }
        {
          name: 'Storage__FileShareConnectionString'
          value: storageConnection
        }
        {
          name: 'Functions__SharedSecret'
          value: functionSharedSecret
        }
        {
          name: 'AZURE_FUNCTIONS_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: insightsConnection
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
        {
          name: 'ENABLE_ORYX_BUILD'
          value: 'false'
        }
      ]
    }
  }
}

output functionAppName string = functionApp.name
output functionAppHost string = functionApp.properties.defaultHostName
