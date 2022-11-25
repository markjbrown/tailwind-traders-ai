#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$profile,
    [parameter(Mandatory=$true)][string]$region,
    [parameter(Mandatory=$true)][string]$domain
)

$sourceFolder=$(Join-Path -Path ../../.. -ChildPath terraform/google)

Push-Location $($MyInvocation.InvocationName | Split-Path)

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying GCP Terraform script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

Write-Host "Begining the GCP Terraform deployment..." -ForegroundColor Yellow
Push-Location $sourceFolder

$env:GOOGLE_PROJECT=$(gcloud config get-value project)
$env:AWS_PROFILE=$profile
$env:AWS_REGION=$region
$env:TF_VAR_domain_name=$domain

terraform init
terraform apply -auto-approve

Pop-Location
