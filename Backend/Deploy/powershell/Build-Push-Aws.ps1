#! /usr/bin/pwsh

Param(
  [parameter(Mandatory=$false)][bool]$dockerBuild=$true,
  [parameter(Mandatory=$false)][bool]$dockerPush=$true,
  [parameter(Mandatory=$false)][string]$dockerTag="latest"
)

Push-Location $($MyInvocation.InvocationName | Split-Path)
$sourceFolder=$(./Join-Path-Recursively.ps1 -pathParts ..,..,Source)
Write-Host "---------------------------------------------------" -ForegroundColor Yellow

Write-Host "---------------------------------------------------" -ForegroundColor Yellow
Write-Host "Getting info from ECR..." -ForegroundColor Yellow
Write-Host "---------------------------------------------------" -ForegroundColor Yellow
$awsAccountId=$(aws sts get-caller-identity --query "Account" --output text)
$awsRegion=$(aws configure get region)
$ecrLoginServer="$awsAccountId.dkr.ecr.$awsRegion.amazonaws.com"
$ecrUser="AWS"
$ecrPassword=$(aws ecr get-login-password)
$dockerComposeFile= "docker-compose.yml"


if ($dockerBuild) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Using docker compose to build & tag images." -ForegroundColor Yellow
    Write-Host "Images will be named as $ecrLoginServer/imageName:$dockerTag" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location $sourceFolder
    $env:TAG=$dockerTag
    $env:REGISTRY=$ecrLoginServer
    docker-compose -f $dockerComposeFile build
    Pop-Location
}

if ($dockerPush) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Pushing images to $ecrLoginServer" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location $sourceFolder
    docker login -p $ecrPassword -u $ecrUser $ecrLoginServer
    $env:TAG=$dockerTag
    $env:REGISTRY=$ecrLoginServer 
    docker-compose -f $dockerComposeFile push
    Pop-Location
}

Pop-Location