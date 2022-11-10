Param(
    [parameter(Mandatory = $false)][string]$name = "my-tt-web",
    [parameter(Mandatory = $false)][string]$gkeName,
    [parameter(Mandatory = $false)][string]$gkeHost = "gke.tailwindtraders.click",
    [parameter(Mandatory = $false)][string]$gkeRegion = "us-east1",
    [parameter(Mandatory = $false)][string]$projectId = "tailwind-traders-363214",
    [parameter(Mandatory = $false)][string]$tag = "latest",
    [parameter(Mandatory = $false)][string]$valueSFile = "gcp-gvalues.yaml",
    [parameter(Mandatory = $false)][string]$b2cValuesFile = "values.b2c.yaml",
    [parameter(Mandatory = $false)][string]$afHost = "https://gke.tailwindtraders.click/",
    [parameter(Mandatory = $false)][string][ValidateSet('prod', 'staging', 'none', 'custom', IgnoreCase = $false)]$tlsEnv = "custom",
    [parameter(Mandatory = $false)][string]$tlsHost = "gke.tailwindtraders.click",
    [parameter(Mandatory = $false)][string]$tlsSgcretName = "",
    [parameter(Mandatory = $false)][string]$appInsightsName = "",    
    [parameter(Mandatory = $false)][string]$build = $false,
    [parameter(Mandatory = $false)][string]$subscription = "",
    [parameter(Mandatory = $false)][string]$acsConnectionString = "",
    [parameter(Mandatory = $false)][string]$acsResource = "",
    [parameter(Mandatory = $false)][string]$acsEmail = "",
    [parameter(Mandatory = $false)][string]$logicAppUrl = ""
)

function validate {
    $valid = $true


    if ([string]::IsNullOrEmpty($gkeName)) {
        Write-Host "No GKE name. Use -gkeName to specify name" -ForegroundColor Red
        $valid = $false
    }

    if ([string]::IsNullOrEmpty($gkeHost) -and $tlsEnv -ne "custom") {
        Write-Host "GKE host of HttpRouting can't be found. Are you using right GKE ($gkeName) and RG ($resourceGroup)?" -ForegroundColor Red
        $valid = $false
    }     
    if ($tlsEnv -eq "custom" -and [string]::IsNullOrEmpty($tlsHost)) {
        Write-Host "If tlsEnv is custom must use -tlsHost to set the hostname of GKE (inferred name of Http Application Routing won't be used)"
        $valid = $false
    }
    if ($valid -eq $false) {
        exit 1
    }
}

function buildPushImageDocker() {
    Push-Location $($MyInvocation.InvocationName | Split-Path)
    $sourceFolder = $(./Join-Path-Recursively.ps1 -pathParts .., Source)  

    Write-Host "Getting info from GCR us.gcr.io" -ForegroundColor Yellow    
    $gcrLoginServer="us.gcr.io"
    $gcrRegistry="$gcrLoginServer/$projectId"
    $gcrUser="oauth2accesstoken"
    $gcrPassword=$(gcloud auth print-access-token)
    $dockerComposeFile = "docker-compose.yml"

    Write-Host "Using docker compose to build & tag images." -ForegroundColor Yellow
    Write-Host "Images will be named as $gcrRegistry/imageName:$tag" -ForegroundColor Yellow
    Push-Location $sourceFolder
    $env:TAG = $tag
    $env:REGISTRY = $gcrRegistry
    docker-compose -f $dockerComposeFile build   

    Write-Host "Pushing images to $acrLogin" -ForegroundColor Yellow
    docker login -p $gcrPassword -u $gcrUser $gcrLoginServer
    docker-compose -f $dockerComposeFile push
    Pop-Location
}

function createHelmCommand([string]$command, $chart) {
    $tlsSecretNameToUse = ""
    if ($tlsEnv -eq "staging") {
        $tlsSecretNameToUse = "tt-letsencrypt-staging"
    }
    if ($tlsEnv -eq "prod") {
        $tlsSecretNameToUse = "tt-letsencrypt-prod"
    }
    if ($tlsEnv -eq "custom") {
        $tlsSecretNameToUse = $tlsSecretName
    }

    $newcmd = $command

    if (-not [string]::IsNullOrEmpty($tlsSecretNameToUse)) {
        $newcmd = "$newcmd --set ingress.protocol=https --set ingress.tls[0].secretName=$tlsSecretNameToUse --set ingress.tls[0].hosts='{$gkeHost}'"
    }
    else {
        $newcmd = "$newcmd --set ingress.protocol=http"
    }

    $newcmd = "$newcmd $chart"
    return $newcmd;
}


Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Deploying images on cluster $gkeName"  -ForegroundColor Yellow
Write-Host " "  -ForegroundColor Yellow
Write-Host " Additional parameters are:"  -ForegroundColor Yellow
Write-Host " Release Name: $name"  -ForegroundColor Yellow
Write-Host " GKE to use: $gkeName and GCR"  -ForegroundColor Yellow
Write-Host " Images tag: $tag"  -ForegroundColor Red
Write-Host " TLS/SSL environment to enable: $tlsEnv"  -ForegroundColor Red
Write-Host " --------------------------------------------------------" 

$gcrLoginServer="us.gcr.io"
$gcrRegistry="$gcrLoginServer/$projectId"
$gcrUser="oauth2accesstoken"
$gcrPassword=$(gcloud auth print-access-token)

# if ($tlsEnv -ne "custom") {
#     $gkeHost = $(az aks show -n $gkeName -g $resourceGroup | ConvertFrom-Json).addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName

#     Write-Host "acr login server is $acrLogin" -ForegroundColor Yellow
#     Write-Host "gkeHost is $gkeHost" -ForegroundColor Yellow 
# }
# else {
    $gkeHost = $tlsHost
# }

if ($build) {    
    buildPushImageDocker
}

validate

$appinsightsId = ""

Push-Location helm

Write-Host "Deploying web chart" -ForegroundColor Yellow
gcloud container clusters get-credentials $gkeName --region=$gkeRegion

$command = createHelmCommand "helm upgrade --install $name -f $valuesFile -f $b2cValuesFile --set inf.appinsights.id=$appinsightsId --set az.productvisitsurl=$afHost --set ingress.hosts='{$gkeHost}' --set image.repository=$gcrLogin/web --set image.tag=$tag" "web" 
echo $command
Invoke-Expression "$command"

Pop-Location

Write-Host "Tailwind traders web deployed on GKE" -ForegroundColor Yellow