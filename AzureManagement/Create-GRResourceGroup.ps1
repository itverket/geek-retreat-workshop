[CmdletBinding()]
Param(
	[Parameter(Position=1)]
	[string]$groupname,
	[string]$resourceGroupName,
	[string]$tweetPublishServiceName,
	[string]$tweetHandlerServiceName,
	[string]$storageAccountName,
	[string]$deploymentStorageName,
	[string]$searchName,
	[string]$templateFile = ".\tweet-publish-subscribe.json"
)

if (!$groupname){
	$groupname = Read-Host "Enter group number or short group name (max 5 characters!)" 	
}
	 
if($groupname.Length -lt 3){
	$groupname = "GRGroup" + $groupname
}
     
$resourceGroupName = $groupname + "Resources"
$tweetPublishServiceName = $groupname + "TweetPublisher"
$tweetHandlerServiceName = $groupname + "TweetHandler"
$searchName = $groupname.ToLower() + "search"
$storageAccountName = $groupname.ToLower() + "storage"
$deploymentStorageName = $groupname.ToLower() + "deployments"
$hostingPlanName = $groupname + "HostingPlan"
$websiteName = $groupname + "Web"
	
New-AzureRMResourceGroup -Name $resourceGroupName -Location "North Europe"
New-AzureRMResourceGroupDeployment -ErrorAction Stop -ResourceGroupName $resourceGroupName -TemplateFile $templateFile `
												-tweetPublishServiceName $tweetPublishServiceName `
												-tweetHandlerServiceName $tweetHandlerServiceName `
												-tweetHandlerStorageAccountName $storageAccountName `
												-searchName $searchName `
												-deploymentStorageName $deploymentStorageName `
												-hostingPlanName $hostingPlanName `
												-websiteName $websiteName
																												   


