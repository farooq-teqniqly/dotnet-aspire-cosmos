targetScope = 'resourceGroup'

@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('Tags to apply to resources')
param tags object = {}


resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: take('${rootName}-cdb', 40)
  location: resourceGroup().location
  tags: tags
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: resourceGroup().location
        failoverPriority: 0
      }
    ]
    enableFreeTier: true
    enablePartitionMerge: false
    enableMultipleWriteLocations: false
    enableAutomaticFailover: false
    publicNetworkAccess: 'Enabled'
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 1440
        backupRetentionIntervalInHours: 48
        backupStorageRedundancy: 'Local'
      }
    }
    disableLocalAuth: false
  }
}

output endpoint string = cosmosdb.properties.documentEndpoint
