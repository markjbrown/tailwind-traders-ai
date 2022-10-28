#! /usr/bin/pwsh

Param(
    [parameter(Mandatory=$true)][string]$bucketName
)

Push-Location $($MyInvocation.InvocationName | Split-Path)
Push-Location ..

Write-Host "Copying to s3 bucket $bucketName" -ForegroundColor Green
Write-Host "Copying images..." -ForegroundColor Green

aws s3 cp $(Join-Path tailwindtraders-images coupon-list) s3://$bucketName/coupon-list/ --recursive 
aws s3 cp $(Join-Path tailwindtraders-images product-detail) s3://$bucketName/product-detail/ --recursive 
aws s3 cp $(Join-Path tailwindtraders-images product-list) s3://$bucketName/product-list/ --recursive 
aws s3 cp $(Join-Path tailwindtraders-images profiles-list) s3://$bucketName/profiles-list/ --recursive 

Pop-Location
Pop-Location