[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $ContainerName,

    [Parameter(Mandatory)]
    [string] $ImageTag,

    [Parameter(Mandatory)]
    [ValidateRange(1, 65535)]
    [int] $HostPort,

    [Parameter(Mandatory)]
    [string] $EnvironmentName,

    [Parameter(Mandatory)]
    [string] $ApiBaseUrl,

    [string] $LegacyContainerName
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$rollbackContainer = "$ContainerName-rollback"

function Invoke-Docker {
    param([Parameter(ValueFromRemainingArguments)][string[]] $Arguments)

    & docker @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "docker $($Arguments[0]) failed with exit code $LASTEXITCODE."
    }
}

function Test-ContainerExists {
    param([string] $Name)

    & docker container inspect $Name *> $null
    return $LASTEXITCODE -eq 0
}

if ($LegacyContainerName -and
    -not (Test-ContainerExists $ContainerName) -and
    (Test-ContainerExists $LegacyContainerName)) {
    Invoke-Docker rename $LegacyContainerName $ContainerName
}

if (Test-ContainerExists $rollbackContainer) {
    Invoke-Docker rm --force $rollbackContainer
}

$hasCurrentContainer = Test-ContainerExists $ContainerName
if ($hasCurrentContainer) {
    Invoke-Docker stop $ContainerName
    Invoke-Docker rename $ContainerName $rollbackContainer
}

try {
    Invoke-Docker run --detach `
        --name $ContainerName `
        --publish "${HostPort}:8080" `
        --env "Environment=$EnvironmentName" `
        --env "MoopelApiOptions__BaseUrl=$ApiBaseUrl" `
        $ImageTag

    $configurationUri = "http://localhost:$HostPort/app-config.json"
    $healthy = $false

    foreach ($attempt in 1..12) {
        try {
            $configuration = Invoke-RestMethod -Uri $configurationUri -TimeoutSec 5
            if ($configuration.environment -eq $EnvironmentName) {
                $healthy = $true
                break
            }

            Start-Sleep -Seconds 5
        }
        catch {
            Start-Sleep -Seconds 5
        }
    }

    if (-not $healthy) {
        throw "$ContainerName did not pass its runtime configuration smoke test."
    }

    $state = & docker inspect $ContainerName --format '{{.State.Status}}'
    if ($LASTEXITCODE -ne 0 -or $state -ne 'running') {
        throw "$ContainerName is not running after deployment."
    }

    if ($hasCurrentContainer) {
        Invoke-Docker rm --force $rollbackContainer
    }
}
catch {
    if (Test-ContainerExists $ContainerName) {
        & docker logs $ContainerName
        Invoke-Docker rm --force $ContainerName
    }

    if ($hasCurrentContainer -and (Test-ContainerExists $rollbackContainer)) {
        Invoke-Docker rename $rollbackContainer $ContainerName
        Invoke-Docker start $ContainerName
    }

    throw
}
