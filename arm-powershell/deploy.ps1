[CmdletBinding()]
param (
  [string]
  $ResourceGroupName = "rg-msl-dev-eu",

  [string]
  $ResourceGroupLocation = "eastus",

  [string]
  $DeploymentName,

  [string]
  $TemplateFilePath = "template.json",

  [string]
  $ParametersFilePath = "template.parameters.json",

  [ValidateSet(
    "dev",
    "prod"
  )]
  [string]
  $Environment = "dev"
)

$ErrorActionPreference = "Stop"
#Requires -Modules Az.Resources

#Create or check for existing resource group
$ResourceGroup = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (!$ResourceGroup) {
  Write-Output "Resource group '$ResourceGroupName' does not exist.";
  Write-Output "Creating resource group '$ResourceGroupName' in location '$ResourceGroupLocation'";
  New-AzResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation
} else {
  Write-Output "Using existing resource group '$ResourceGroupName'";
}

# Start the deployment
Write-Output "Starting deployment...";

if (Test-Path $ParametersFilePath) {
  $Output = New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile $TemplateFilePath -TemplateParameterFile $ParametersFilePath;
} else {
  $Output = New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile $TemplateFilePath;
}

# Are Azure Response strings the same across regions/cultures?
if ($Output.ProvisioningState -eq "Succeeded") {
  Write-Output "Successfully finished deployment";
} else {
  Write-Output "Finished deployment";
}

Write-Output $Output
