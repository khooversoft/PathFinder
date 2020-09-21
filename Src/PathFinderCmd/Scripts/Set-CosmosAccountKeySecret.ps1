#
# Create Cosmos Account
# Note: Must run in Powershell 5.1, not powershell core
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "path-finder-testing-rg",

    [string] $Name = "path-finder-test",

    [string] $KeyName = "PrimaryMasterKey",

    [string] $SecretId = "PathFinderCmd"
)

$ErrorActionPreference = "Stop";

# Switch to subscription if required
$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

# Query Cosmos for key value
$keys = Get-AzCosmosDBAccountKey -ResourceGroupName $ResourceGroupName -Name $Name -ErrorAction SilentlyContinue
if ( !$keys )
{
    Write-Host "The account $Name does not  exist";
    return;
}

$secretValue = $keys[$KeyName];
if ( !$secretValue )
{
    Write-Host "The account key $KeyName does not  exist";
    return;
}

Write-Host "Setting key value to secret file $SecretId";

$jsonHash = @{
    "Store" = @{
        "AccountKey" = $secretValue
    }
}

$json = $jsonHash | ConvertTo-Json;
$secretFile = [System.IO.Path]::Combine($env:APPDATA, "Microsoft\UserSecrets", $SecretId, "secrets.json");
$json | Out-File -FilePath $secretFile;

Write-Host "Write secret file $secretFile";
Write-Host "Completed";
