@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

param tags object = {
  project: 'dotnet-aspire-cosmosdb'
  owner: 'farooq-teqniqly'
}

@description('Name of the SQL database')
param databaseName string = 'envino'

@description('Array of container configurations')
param containers array = [
  {
    name: 'Wines'
    partitionKeyPath: '/id'
    indexingPolicy: {
      indexingMode: 'consistent'
      includedPaths: [
        {
          path: '/*'
        }
      ]
      excludedPaths: [
        {
          path: '/_etag/?'
        }
      ]
    }
  }
  {
    name: 'Wineries'
    partitionKeyPath: '/id'
    indexingPolicy: {
      indexingMode: 'consistent'
      includedPaths: [
        {
          path: '/*'
        }
      ]
      excludedPaths: [
        {
          path: '/_etag/?'
        }
      ]
    }
  }
  {
    name: 'Regions'
    partitionKeyPath: '/rootId'
    indexingPolicy: {
      indexingMode: 'consistent'
      includedPaths: [
        {
          path: '/*'
        }
      ]
      excludedPaths: [
        {
          path: '/_etag/?'
        }
      ]
    }
  }
]

module cosmosaccount 'cosmosaccount.bicep' = {
  name: 'cosmosaccount-deployment'
  params: {
    rootName: rootName
    tags: tags
    databaseName: databaseName
    containers: containers
  }
}


output database_endpoint string = cosmosaccount.outputs.endpoint
