[CmdletBinding()]
param (
  [ValidateSet(
    "dev",
    "prod"
  )]
  [string]
  $Environment = "dev"
)

. ./env.ps1

$ErrorActionPreference = "Stop"
#Requires -Modules Az.Resources

# Authorize
$key = (Get-AzStorageAccountKey -ResourceGroupName $TemplateResourceGroupName -Name $TemplateStorageAccountName).Value[0]
$context = New-AzStorageContext -StorageAccountName $TemplateStorageAccountName -StorageAccountKey $key

$mainTemplateUri = $context.BlobEndPoint + "$TemplateContainerName/azuredeploy.json"
$sasToken = New-AzStorageContainerSASToken `
    -Context $context `
    -Container $TemplateContainerName `
    -Permission r `
    -ExpiryTime (Get-Date).AddHours(2.0)
$newSas = $sasToken.substring(1)

Write-Output "Starting deployment...";
$Output = New-AzResourceGroupDeployment `
  -Name DeployLinkedTemplate `
  -ResourceGroupName $ResourceGroupName[$Environment] `
  -TemplateUri $mainTemplateUri `
  -QueryString $newSas `
  -projectName $ProjectName `
  -verbose

# Are Azure Response strings the same across regions/cultures?
if ($Output.ProvisioningState -eq "Succeeded") {
  Write-Output "Successfully finished deployment";
} else {
  Write-Output "Finished deployment";
}

Write-Output $Output
