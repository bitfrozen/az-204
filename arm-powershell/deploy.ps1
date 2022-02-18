[CmdletBinding()]
param (
  [string]
  $DeploymentName,

  [string]
  $TemplateFilePath = "template.json",

  [string]
  $ParametersFilePath,

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

# Start the deployment
Write-Output "Starting deployment...";

if (Test-Path $ParametersFilePath) {
  $Output = New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName[$Environment] -TemplateFile $TemplateFilePath -TemplateParameterFile $ParametersFilePath;
} else {
  $Output = New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName[$Environment] -TemplateFile $TemplateFilePath;
}

# Are Azure Response strings the same across regions/cultures?
if ($Output.ProvisioningState -eq "Succeeded") {
  Write-Output "Successfully finished deployment";
} else {
  Write-Output "Finished deployment";
}

Write-Output $Output
