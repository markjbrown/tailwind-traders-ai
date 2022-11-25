#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$profile,
    [parameter(Mandatory=$true)][string]$region,
    [parameter(Mandatory=$true)][string]$domain
)

$sourceFolder=$(Join-Path -Path ../../.. -ChildPath terraform/aws)

Push-Location $($MyInvocation.InvocationName | Split-Path)

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying AWS Terraform script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

Write-Host "Begining the AWS Terraform deployment..." -ForegroundColor Yellow
Push-Location $sourceFolder

$env:AWS_PROFILE=$profile
$env:AWS_REGION=$region
$env:TF_VAR_domain_name=$domain

terraform init
terraform apply -auto-approve

Pop-Location
