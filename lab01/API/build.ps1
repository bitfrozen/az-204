param (
    [Parameter()]
    [switch]$Publish = $false,

    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ArchiveFile=".\api.zip"
$PublishLocation=".\publish"

$env:DOTNET_CLI_TELEMETRY_OPTOUT=1

if ($Publish) {
    Write-Output "Publishing projects"
    dotnet publish .\API.sln --configuration $Configuration --self-contained false --output $PublishLocation
    if (Test-Path $ArchiveFile) {
      Remove-Item -LiteralPath $ArchiveFile
    }
    Compress-Archive -Path $PublishLocation\* -DestinationPath $ArchiveFile
    Remove-Item -LiteralPath $PublishLocation -Force -Recurse

} else {
  dotnet build .\API.sln --configuration $Configuration --self-contained false
}