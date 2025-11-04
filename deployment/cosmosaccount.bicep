targetScope = 'resourceGroup'

@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('Tags to apply to resources')
param tags object = {}

@description('Name of the SQL database')
param databaseName string

@description('Array of container configurations')
param containers array


resource cosmosaccount 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: take('${rootName}-cosacct', 40)
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

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  parent: cosmosaccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = [for (item, index) in containers: {
  parent: database
  name: item.name
  properties: {
    resource: {
      id: item.name
      partitionKey: {
        paths: [
          item.partitionKeyPath
        ]
        kind: 'Hash'
      }
      indexingPolicy: item.indexingPolicy
    }
  }
}]

output endpoint string = cosmosaccount.properties.documentEndpoint
