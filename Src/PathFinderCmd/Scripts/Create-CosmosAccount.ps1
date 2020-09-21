#
# Create Cosmos Account
# Note: Must run in Powershell 5.1, not powershell core
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
    Write-Host "Resource group $ResourceGroupName does not exist, creating it";
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location;
}

# Query to see if account already exist
$currentAccount = Get-AzCosmosDBAccount -ResourceGroupName $ResourceGroupName -Name $Name -ErrorAction SilentlyContinue;
if ($currentAccount -and $currentAccount.Id)
{
    Write-Host "The account $Name already exists in the $Location region:";

    $currentAccount;
    return;
}

Write-Host "The $Name namespace does not exist.";
Write-Host "Creating the $Name account in the $Location region...";

New-AzCosmosDBAccount -ResourceGroupName $ResourceGroupName -Name $Name -Location $Location;
Write-Host "The $NamespaceName account in Resource Group $ResourceGroupName in the $Location region has been successfully created.";

Write-Host "Completed";