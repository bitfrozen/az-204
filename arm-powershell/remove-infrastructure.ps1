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

Write-Output "Removing infrastructure ..."

Remove-AzResourceGroup -Name $TemplateResourceGroupName -Force
Remove-AzResourceGroup -Name $ResourceGroupName[$Environment] -Force

Write-Output "Finished"
