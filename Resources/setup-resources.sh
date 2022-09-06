#!/bin/bash

set -e

urls=$(curl -H "Accept: application/vnd.github.v3+json" \
    https://api.github.com/repos/justinyoo/azure-event-grid-cloudevents-sample/releases/latest | \
    jq '[.assets[] | .browser_download_url]')

resource_group="rg-$AZ_RESOURCE_NAME"

# Deploy Function app
fncapp_name="fncapp-$AZ_RESOURCE_NAME"
fncapp_zip=$(echo $urls | jq --argjson value 0 '.[$value]' -r)
fncapp=$(az functionapp deploy -g $resource_group -n $fncapp_name --src-url $fncapp_zip --type zip)

# Set Key Valut access policy - secrets
user_object_id=$(az ad user show --id "$AZ_UPN" --query id -o tsv)
kv_name="kv-$AZ_RESOURCE_NAME"
kv=$(az keyvault set-policy -g $resource_group -n $kv_name --object-id $user_object_id --secret-permissions all)
