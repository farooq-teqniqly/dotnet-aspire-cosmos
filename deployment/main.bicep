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

@description('Name of the Service Bus topic')
param topicName string = 'wines-topic'

@description('Array of Service Bus subscription names')
param subscriptions array = [
  'wines-subscription'
]

@description('Array of container configurations')
param containers array = [
  {
    name: 'wineries'
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

module servicebus 'servicebus.bicep' = {
  name: 'servicebus-deployment'
  params: {
    rootName: rootName
    tags: tags
    topicName: topicName
    subscriptions: subscriptions
  }
}


output database_endpoint string = cosmosaccount.outputs.endpoint
output servicebus_namespace_name string = servicebus.outputs.namespace_name
output servicebus_topic_name string = servicebus.outputs.topic_name
output servicebus_subscription_names array = servicebus.outputs.subscription_names
