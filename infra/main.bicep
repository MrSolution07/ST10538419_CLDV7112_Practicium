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
param namePrefix string = 'fcd'

@description('SQL administrator login. Not a secret.')
param sqlAdminLogin string = 'fcdadmin'

@secure()
param sqlAdminPassword string

@description('Create a Storage account for blobs, queues, tables, and files. Leave false until the brief asks.')
param deployStorage bool = false

var suffix = uniqueString(resourceGroup().id)
var planName = '${namePrefix}-plan'
var webName = toLower(take('${namePrefix}${suffix}', 24))
var sqlServerName = toLower(take('${namePrefix}${suffix}', 20))
var databaseName = 'FinalCloudDev'
var storageName = toLower(take('fcd${suffix}', 24))

resource plan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: planName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
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

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = if (deployStorage) {
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

var storageConnection = deployStorage ? 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}' : ''
var sqlConnection = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};User ID=${sqlAdminLogin};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

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
      ]
    }
  }
}

output webAppName string = web.name
output webAppHost string = web.properties.defaultHostName
output sqlServerName string = sqlServer.name
output sqlServerHost string = sqlServer.properties.fullyQualifiedDomainName
