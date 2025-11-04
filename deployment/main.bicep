@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

param tags object = {
  project: 'dotnet-aspire-cosmosdb'
  owner: 'farooq-teqniqly'
}

module cosmosdb 'cosmosdb.bicep' = {
  name: 'cosmosdb-deployment'
  params: {
    rootName: rootName
    tags: tags
  }
}


output database_endpoint string = cosmosdb.outputs.endpoint
