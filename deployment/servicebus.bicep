targetScope = 'resourceGroup'

@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('Tags to apply to resources')
param tags object = {}

@description('Name of the Service Bus topic')
param topicName string

@description('Array of Service Bus subscription names')
param subscriptions array

resource servicebusNamespace 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: '${rootName}-sbns'
  location: resourceGroup().location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    zoneRedundant: false
    disableLocalAuth: false
  }
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2021-11-01' = {
  parent: servicebusNamespace
  name: topicName
  properties: {
    defaultMessageTimeToLive: 'PT1H'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: true
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    enableBatchedOperations: true
    supportOrdering: false
    enablePartitioning: false
    enableExpress: false
  }
}

resource subscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-11-01' = [for subscriptionName in subscriptions: {
  parent: topic
  name: subscriptionName
  properties: {
    lockDuration: 'PT1M'
    requiresSession: false
    defaultMessageTimeToLive: 'PT1H'
    deadLetteringOnMessageExpiration: true
    deadLetteringOnFilterEvaluationExceptions: true
    maxDeliveryCount: 10
    enableBatchedOperations: true
    status: 'Active'
  }
}]
output namespace_name string = servicebusNamespace.name
output topic_name string = topic.name
output subscription_names array = [for subscriptionName in subscriptions: subscriptionName]
