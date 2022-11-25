#! /usr/bin/pwsh

Param(
  [parameter(Mandatory=$false)][bool]$dockerBuild=$true,
  [parameter(Mandatory=$false)][bool]$dockerPush=$true,
  [parameter(Mandatory=$true)][string]$projectId,
  [parameter(Mandatory=$false)][string]$dockerTag="latest"
)

Push-Location $($MyInvocation.InvocationName | Split-Path)
$sourceFolder=$(./Join-Path-Recursively.ps1 -pathParts ..,..,Source)
Write-Host "---------------------------------------------------" -ForegroundColor Yellow

Write-Host "---------------------------------------------------" -ForegroundColor Yellow
Write-Host "Getting info from GCR..." -ForegroundColor Yellow
Write-Host "---------------------------------------------------" -ForegroundColor Yellow
$gcrLoginServer="gcr.io"
$gcrUser="oauth2accesstoken"
$gcrPassword=$(gcloud auth print-access-token)
$dockerComposeFile= "docker-compose.yml"


if ($dockerBuild) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Using docker compose to build & tag images." -ForegroundColor Yellow
    Write-Host "Images will be named as $gcrLoginServer/$projectId/imageName:$dockerTag" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location $sourceFolder
    $env:TAG=$dockerTag
    $env:REGISTRY="$gcrLoginServer/$projectId"
    docker-compose -f $dockerComposeFile build
    Pop-Location
}

if ($dockerPush) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Pushing images to $gcrLoginServer" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location $sourceFolder
    docker login -p $gcrPassword -u $gcrUser $gcrLoginServer
    $env:TAG=$dockerTag
    $env:REGISTRY="$gcrLoginServer/$projectId" 
    docker-compose -f $dockerComposeFile push
    Pop-Location
}

Pop-Location