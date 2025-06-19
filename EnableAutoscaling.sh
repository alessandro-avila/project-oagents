#!/bin/bash
 cd $(dirname $0)
# Azure CLI script to enable autoscaling on an Azure App Service plan

# Variables - replace these with your actual resource group and App Service plan names
RESOURCE_GROUP="myResourceGroup"
APP_SERVICE_PLAN="myAppServicePlan"

# Set autoscale settings name
AUTOSCALE_SETTING_NAME="autoscaleSetting"

# Create autoscale rule to scale out if CPU percentage > 70% for 5 minutes
SCALE_OUT_RULE='{
  "metricTrigger": {
    "metricName": "CpuPercentage",
    "metricNamespace": "",
    "metricResourceUri": "/subscriptions/{subscriptionId}/resourceGroups/'"$RESOURCE_GROUP"'/providers/Microsoft.Web/serverfarms/'"$APP_SERVICE_PLAN"'",
    "timeGrain": "PT1M",
    "statistic": "Average",
    "timeWindow": "PT5M",
    "timeAggregation": "Average",
    "operator": "GreaterThan",
    "threshold": 70
  },
  "scaleAction": {
    "direction": "Increase",
    "type": "ChangeCount",
    "value": "1",
    "cooldown": "PT5M"
  }
}'

# Create autoscale rule to scale in if CPU percentage < 30% for 10 minutes
SCALE_IN_RULE='{
  "metricTrigger": {
    "metricName": "CpuPercentage",
    "metricNamespace": "",
    "metricResourceUri": "/subscriptions/{subscriptionId}/resourceGroups/'"$RESOURCE_GROUP"'/providers/Microsoft.Web/serverfarms/'"$APP_SERVICE_PLAN"'",
    "timeGrain": "PT1M",
    "statistic": "Average",
    "timeWindow": "PT10M",
    "timeAggregation": "Average",
    "operator": "LessThan",
    "threshold": 30
  },
  "scaleAction": {
    "direction": "Decrease",
    "type": "ChangeCount",
    "value": "1",
    "cooldown": "PT5M"
  }
}'

# Replace {subscriptionId} with actual subscription ID in rules
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
SCALE_OUT_RULE="${SCALE_OUT_RULE//\{subscriptionId\}/$SUBSCRIPTION_ID}"
SCALE_IN_RULE="${SCALE_IN_RULE//\{subscriptionId\}/$SUBSCRIPTION_ID}"

# Create autoscale profile JSON
AUTOSCALE_PROFILE=$(cat <<-JSON
{
  "name": "AutoScaleProfile",
  "capacity": {
    "minimum": "1",
    "maximum": "5",
    "default": "1"
  },
  "rules": [
    $SCALE_OUT_RULE,
    $SCALE_IN_RULE
  ]
}
JSON
)

# Create autoscale setting with the above profile
az monitor autoscale create   --resource-group $RESOURCE_GROUP   --resource $APP_SERVICE_PLAN   --resource-type "Microsoft.Web/serverFarms"   --name $AUTOSCALE_SETTING_NAME   --profiles "$AUTOSCALE_PROFILE"   --enabled true

# Note: User must ensure the RESOURCE_GROUP and APP_SERVICE_PLAN variables are set correctly before running this script.
