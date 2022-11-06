#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$bucketName
)

Push-Location $($MyInvocation.InvocationName | Split-Path)
Push-Location ..

Write-Host "Copying to Google bucket $bucketName" -ForegroundColor Green
Write-Host "Copying images..." -ForegroundColor Green

gsutil cp -r $(Join-Path tailwindtraders-images coupon-list) gs://$bucketName/coupon-list 
gsutil cp -r $(Join-Path tailwindtraders-images product-detail) gs://$bucketName/product-detail 
gsutil cp -r $(Join-Path tailwindtraders-images product-list) gs://$bucketName/product-list 
gsutil cp -r $(Join-Path tailwindtraders-images profiles-list) gs://$bucketName/profiles-list

Pop-Location
Pop-Location