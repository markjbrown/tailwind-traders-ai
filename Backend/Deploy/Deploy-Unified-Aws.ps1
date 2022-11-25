#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$domain,
    [parameter(Mandatory=$false)][bool]$stepDeployTf=$true,
    [parameter(Mandatory=$false)][bool]$stepBuildPush=$true,
    [parameter(Mandatory=$false)][bool]$stepDeployImages=$true,
    [parameter(Mandatory=$false)][bool]$stepDeployPics=$true,
    [parameter(Mandatory=$false)][bool]$awsConfigure=$false
)
$gValuesFile="aws-configFile.yaml"

Push-Location $($MyInvocation.InvocationName | Split-Path)

if (-Not $(aws configure list-profiles).Contains("tailwind-traders") -Or $awsConfigure) {
    Write-Host "Configure AWS Credentials" -ForegroundColor Yellow
    aws configure --profile tailwind-traders
}

$env:AWS_PROFILE="tailwind-traders"
$awsRegion=$(aws configure get region)

Push-Location powershell

if ($stepDeployTf) {
    # Deploy Terraform
    & ./Deploy-Terraform-Aws.ps1 -profile tailwind-traders -region $awsRegion -domain $domain
}

# Connecting kubectl to EKS
Write-Host "Retrieving EKS Name" -ForegroundColor Yellow
$eksName = $(aws eks list-clusters --output json | ConvertFrom-Json).clusters[0]
Write-Host "The name of your EKS: $eksName" -ForegroundColor Yellow

Write-Host "Retrieving credentials" -ForegroundColor Yellow
aws eks update-kubeconfig --name $eksName --region $awsRegion

# Generate Config
$gValuesLocation=$(./Join-Path-Recursively.ps1 -pathParts ..,helm,__values,$gValuesFile)
$bucketName = $(aws s3api list-buckets --output json | ConvertFrom-Json).Buckets[0].Name
& ./Generate-Config-Eks.ps1 -bucketName $bucketName -outputFile $gValuesLocation

if ($stepBuildPush) {
    # Build an Push
    & ./Build-Push-Aws.ps1
}

if ($stepDeployImages) {
    # Deploy images in AKS
    $gValuesLocation=$(./Join-Path-Recursively.ps1 -pathParts __values,$gValuesFile)
    $chartsToDeploy = "*"
    & ./Deploy-Images-Eks.ps1 -tlsEnv prod -tlsHost "eks.$domain" -eksName $eksName -charts $chartsToDeploy -valuesFile $gValuesLocation
}

if ($stepDeployPics) {
    # Deploy pictures in AKS
    $bucketName = $(aws s3api list-buckets --output json | ConvertFrom-Json).Buckets[0].Name
    & ./Deploy-Pictures-Aws.ps1 -bucketName $bucketName
}

Pop-Location
Pop-Location