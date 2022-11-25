#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$profile,
    [parameter(Mandatory=$true)][string]$region
)

$sourceFolder=$(Join-Path -Path ../../.. -ChildPath terraform/google)

Push-Location $($MyInvocation.InvocationName | Split-Path)

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying GCP Terraform script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

Write-Host "Begining the GCP Terraform deployment..." -ForegroundColor Yellow
Push-Location $sourceFolder

terraform init
terraform apply -auto-approve

Pop-Location
