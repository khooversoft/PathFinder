#
# Delete test resource account
# Note: Must run in PowerShell 5.1, not PowerShell core
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "path-finder-testing-rg",

    [string] $Location = "westus2",

    [string] $Name = "path-finder-test"
)

$ErrorActionPreference = "Stop";

# Switch to subscription if required
$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

# Query to see if resource group exists, if not create it.
$resourceGroup = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue;
if( !$resourceGroup )
{
    Write-Host "Resource group $ResourceGroupName does not exist";
    return;
}

Write-Host "Removing $ResourceGroupName resource group...";
Remove-AzResourceGroup -Name $ResourceGroupName -Force;

Write-Host "Completed";