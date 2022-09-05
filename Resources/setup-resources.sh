#!/bin/bash

set -e

declare -a fncapp_suffixes
suffix_index=1

urls=$(curl -H "Accept: application/vnd.github.v3+json" \
    https://api.github.com/repos/justinyoo/azure-event-grid-cloudevents-sample/releases/latest | \
    jq '[.assets[] | .browser_download_url]')

subscription_id=$(az account show --query id -o tsv)
resource_group="rg-$AZ_RESOURCE_NAME"
fncapp_name="fncapp-$AZ_RESOURCE_NAME"

# Deploy function apps
api_zip=$(echo $urls | jq --argjson value 0 '.[$value]' -r)
api_app=$(az functionapp deploy -g $resource_group -n $fncapp_name --src-url $api_zip --type zip)
