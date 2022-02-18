Write-Output "Loading environment variables ..."
$ProjectName = 'msl5g331'
$Location = 'eastus'
$ResourceGroupName = @{
  Dev  = 'rg-msl-dev-eu';
  Prod = 'rg-msl-prod-eu'
}

$TemplateResourceGroupName = $ProjectName + '-repo'
$TemplateStorageAccountName = $ProjectName + 'tmplstore'
$TemplateContainerName = 'templates'

Write-Output "Finished loading environment variables"