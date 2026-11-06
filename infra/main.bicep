targetScope = 'resourceGroup'

@description('Only regions allowed by policy "Allowed resource deployment regions". southafricanorth is denied on this subscription.')
@allowed([
  'uaenorth'
  'switzerlandnorth'
  'centralindia'
  'brazilsouth'
  'austriaeast'
])
param location string = 'uaenorth'

@description('Short prefix used in resource names.')
param namePrefix string = 'st10538419'

@description('SQL administrator login. Not a secret.')
param sqlAdminLogin string = 'st10538419'

@secure()
param sqlAdminPassword string

@description('Shared secret the MVC app sends to the HTTP functions. Not committed.')
@secure()
@minLength(16)
param functionSharedSecret string

var suffix = uniqueString(resourceGroup().id)
var planName = '${namePrefix}-plan'
var webName = 'st10538419'
var sqlServerName = toLower(take('${namePrefix}${suffix}', 20))
var databaseName = 'ST10538419_CLDV7112_Practicium'
var storageName = toLower(take('st10538419${suffix}', 24))
var functionName = '${namePrefix}-functions'
var logName = toLower(take('log${suffix}', 24))
var insightsName = '${namePrefix}-insights'

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: planName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource azureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
  }
}

var storageKeys = storage.listKeys()
var storageConnection = 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storageKeys.keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
var sqlConnection = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};User ID=${sqlAdminLogin};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
    IngestionMode: 'LogAnalytics'
  }
}

resource web 'Microsoft.Web/sites@2024-04-01' = {
  name: webName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
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
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: insights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'Functions__BaseUrl'
          value: 'https://${functionName}.azurewebsites.net'
        }
        {
          name: 'Functions__SharedSecret'
          value: functionSharedSecret
        }
      ]
    }
  }
}

module functions 'functionapp.bicep' = {
  name: 'functions'
  params: {
    location: location
    functionName: functionName
    serverFarmId: plan.id
    storageConnection: storageConnection
    sqlConnection: sqlConnection
    functionSharedSecret: functionSharedSecret
    insightsConnection: insights.properties.ConnectionString
  }
}

output webAppName string = web.name
output webAppHost string = web.properties.defaultHostName
output sqlServerName string = sqlServer.name
output sqlServerHost string = sqlServer.properties.fullyQualifiedDomainName
output functionAppName string = functions.outputs.functionAppName
output functionAppHost string = functions.outputs.functionAppHost
output storageAccountName string = storage.name
