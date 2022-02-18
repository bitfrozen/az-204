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

#region Template storage
Write-Output "$([Environment]::NewLine)Working with template files ..."

# Create or check for existing repository resource group
$TemplateResourceGroup = Get-AzResourceGroup -Name $TemplateResourceGroupName -ErrorAction SilentlyContinue
if (!$TemplateResourceGroup) {
  Write-Output "Resource group '$TemplateResourceGroupName' does not exist.";
  Write-Output "Creating resource group '$TemplateResourceGroupName' in location '$Location'";
  New-AzResourceGroup -Name $TemplateResourceGroupName -Location $Location
} else {
  Write-Output "Using existing resource group '$TemplateResourceGroupName'";
}

# Create or check for existing template storage account
$TemplateStorageAccount = Get-AzStorageAccount -Name $TemplateStorageAccountName -ResourceGroupName $TemplateResourceGroupName -ErrorAction SilentlyContinue
if (!$TemplateStorageAccount) {
  Write-Output "Storage account '$TemplateStorageAccountName' does not exist.";
  Write-Output "Creating storage account '$TemplateStorageAccountName'";
  $TemplateStorageAccount = New-AzStorageAccount `
    -ResourceGroupName $TemplateResourceGroupName `
    -Name $TemplateStorageAccountName `
    -Location $Location `
    -SkuName "Standard_LRS"
} else {
  Write-Output "Using existing template storage account '$TemplateStorageAccountName'";
}
$context = $TemplateStorageAccount.Context

# Create or check for existing template container
$TemplateContainer = Get-AzStorageContainer -Context $context -Name $TemplateContainerName -ErrorAction SilentlyContinue
if (!$TemplateContainer) {
  Write-Output "Templates container '$TemplateContainerName' does not exist.";
  Write-Output "Creating container '$TemplateContainerName' in storage account '$TemplateStorageAccountName'";
  New-AzStorageContainer -Name $TemplateContainerName -Context $context -Permission Container
} else {
  Write-Output "Using existing container '$TemplateContainerName' in storage account '$TemplateStorageAccountName'";
}

# Template files. This should be enumarated and checked
$TemplateFiles = @{
  'main'    = 'azuredeploy.json';
  'storage' = 'linkedStorageAccount.json'
}

# Upload the templates
Set-AzStorageBlobContent `
  -Container $TemplateContainerName `
  -File $TemplateFiles['main'] `
  -Blob $TemplateFiles['main'] `
  -Context $context `
  -Force


Set-AzStorageBlobContent `
  -Container $TemplateContainerName `
  -File $TemplateFiles['storage'] `
  -Blob $TemplateFiles['storage'] `
  -Context $context `
  -Force

#endregion

Write-Output "$([Environment]::NewLine)Working with resource groups ..."

# Create or check for existing repository resource group
$ResourceGroup = Get-AzResourceGroup -Name $ResourceGroupName[$Environment] -ErrorAction SilentlyContinue
if (!$ResourceGroup) {
  Write-Output "Resource group '$($ResourceGroupName[$Environment])' does not exist.";
  Write-Output "Creating resource group '$($ResourceGroupName[$Environment])' in location '$Location'";
  New-AzResourceGroup -Name $ResourceGroupName[$Environment] -Location $Location
} else {
  Write-Output "Using existing resource group '$($ResourceGroupName[$Environment])'";
}

Write-Output "$([Environment]::NewLine)Finished"
