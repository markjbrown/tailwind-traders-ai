#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$domain,
    [parameter(Mandatory=$true)][string]$projectId,
    [parameter(Mandatory=$false)][string]$location="us-east1",
    [parameter(Mandatory=$false)][bool]$stepDeployTf=$true,
    [parameter(Mandatory=$false)][bool]$stepBuildPush=$true,
    [parameter(Mandatory=$false)][bool]$stepDeployImages=$true,
    [parameter(Mandatory=$false)][bool]$stepDeployPics=$true,
    [parameter(Mandatory=$false)][bool]$awsConfigure=$false
)
$gValuesFile="gcp-configFile.yaml"

Push-Location $($MyInvocation.InvocationName | Split-Path)

if (-Not $(aws configure list-profiles).Contains("tailwind-traders") -Or $awsConfigure) {
    Write-Host "Configure AWS Credentials" -ForegroundColor Yellow
    aws configure --profile tailwind-traders
}

$env:AWS_PROFILE="tailwind-traders"
$awsRegion=$(aws configure get region)

gcloud auth login
gcloud config set project $projectId
gcloud config set artifacts/location $location

Push-Location powershell

if ($stepDeployTf) {
    # Deploy Terraform
    & ./Deploy-Terraform-Gcp.ps1 -profile tailwind-traders -region $awsRegion -domain $domain
}

# Connecting kubectl to GKE
Write-Host "Retrieving GKE Name" -ForegroundColor Yellow
$gkeName = $(gcloud container clusters list --format json | ConvertFrom-Json)[0].name
Write-Host "The name of your GKE: $gkeName" -ForegroundColor Yellow

Write-Host "Retrieving credentials" -ForegroundColor Yellow
gcloud container clusters get-credentials $gkeName --region=$location

# Generate Config
$gValuesLocation=$(./Join-Path-Recursively.ps1 -pathParts ..,helm,__values,$gValuesFile)
$bucketName = $(gcloud storage buckets list --format json | ConvertFrom-Json)[0].id
& ./Generate-Config-Gke.ps1 -bucketName $bucketName -outputFile $gValuesLocation

if ($stepBuildPush) {
    # Build an Push
    & ./Build-Push-Gcp.ps1 -projectId $projectId
}

if ($stepDeployImages) {
    # Deploy images in AKS
    $gValuesLocation=$(./Join-Path-Recursively.ps1 -pathParts __values,$gValuesFile)
    $chartsToDeploy = "*"
    & ./Deploy-Images-Gke.ps1 -tlsEnv prod -tlsHost "gke.$domain" -projectId $projectId -gkeName $gkeName -gkeRegion $location -charts $chartsToDeploy -valuesFile $gValuesLocation
}

if ($stepDeployPics) {
    # Deploy pictures in AKS
    $bucketName = $(gcloud storage buckets list --format json | ConvertFrom-Json)[0].id
    & ./Deploy-Pictures-Gcp.ps1 -bucketName $bucketName
}

Pop-Location
Pop-Location