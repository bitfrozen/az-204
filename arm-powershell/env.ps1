Write-Output "Loading environment variables ..."
$ProjectName = 'mslearn'
$Location = 'eastus'
$ResourceGroupName = @{
  Dev  = 'rg-msl-dev-eu';
  Prod = 'rg-msl-prod-eu'
}

$TemplateResourceGroupName = $ProjectName + '-repo'
$TemplateStorageAccountName = $ProjectName + 'templstore5g331'
$TemplateContainerName = 'templates'

Write-Output "Finished loading environment variables"