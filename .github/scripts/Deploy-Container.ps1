[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ContainerName,

    [Parameter(Mandatory = $true)]
    [string]$ImageName,

    [Parameter(Mandatory = $true)]
    [string]$ImageTag,

    [Parameter(Mandatory = $true)]
    [string]$HostPort,

    [Parameter(Mandatory = $true)]
    [string]$ContainerPort,

    [Parameter(Mandatory = $true)]
    [string]$EnvironmentName,

    [Parameter(Mandatory = $false)]
    [string]$ApiBaseUrl = '',

    [Parameter(Mandatory = $false)]
    [string]$ContainerEnvFilePath = '',

    [Parameter(Mandatory = $false)]
    [string]$LegacyContainerName = '',

    [Parameter(Mandatory = $false)]
    [int]$HealthcheckRetries = 12,

    [Parameter(Mandatory = $false)]
    [int]$HealthcheckDelaySeconds = 10,

    [Parameter(Mandatory = $false)]
    [string]$HealthcheckUrl = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-DockerCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,

        [Parameter(Mandatory = $false)]
        [switch]$IgnoreExitCode
    )

    & docker @Arguments

    if (-not $IgnoreExitCode -and $LASTEXITCODE -ne 0) {
        throw "Docker command failed: docker $($Arguments -join ' ')"
    }
}

function Remove-ContainerGracefully {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if ([string]::IsNullOrWhiteSpace($Name)) {
        return
    }

    $existingContainer = @(docker container ls -a --filter "name=^${Name}$" --format '{{.Names}}')
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to query Docker container state for '$Name'."
    }

    if ($existingContainer | Where-Object { $_ -eq $Name }) {
        Write-Host "Gracefully stopping container '$Name'..."
        Invoke-DockerCommand -Arguments @('stop', '--time', '10', $Name) -IgnoreExitCode
        Write-Host "Removing container '$Name'..."
        Invoke-DockerCommand -Arguments @('rm', '-f', $Name) -IgnoreExitCode
    }
}

$imageReference = if ([string]::IsNullOrWhiteSpace($ImageName) -or $ImageTag.Contains(':') -or $ImageTag.StartsWith('ghcr.io/')) {
    $ImageTag
} else {
    "$ImageName`:$ImageTag"
}

$targetImage = $imageReference
Invoke-DockerCommand -Arguments @('image', 'inspect', $targetImage) -IgnoreExitCode
if ($LASTEXITCODE -ne 0 -and -not [string]::IsNullOrWhiteSpace($ImageName)) {
    $fallbackReference = "moopelfrontend:$ImageTag"
    Invoke-DockerCommand -Arguments @('image', 'inspect', $fallbackReference) -IgnoreExitCode
    if ($LASTEXITCODE -eq 0) {
        $targetImage = $fallbackReference
    }
}

if ($LASTEXITCODE -ne 0) {
    throw "Expected deployment image '$imageReference' is not loaded on the runner."
}

Remove-ContainerGracefully -Name $LegacyContainerName
Remove-ContainerGracefully -Name $ContainerName

$deployedAt = (Get-Date).ToUniversalTime().ToString('o')

$dockerRunArguments = @(
    'run',
    '-d',
    '--name', $ContainerName,
    '--restart', 'unless-stopped',
    '-p', "$HostPort`:$ContainerPort",
    '-e', "Environment=$EnvironmentName",
    '-e', "ASPNETCORE_ENVIRONMENT=$EnvironmentName",
    '-e', "VERSION=$ImageTag",
    '-e', "COMMIT_NAME=$ImageTag",
    '-e', "DEPLOYED_AT=$deployedAt"
)

if (-not [string]::IsNullOrWhiteSpace($ApiBaseUrl)) {
    $dockerRunArguments += @('-e', "MoopelApiOptions__BaseUrl=$ApiBaseUrl")
}

if (-not [string]::IsNullOrWhiteSpace($ContainerEnvFilePath)) {
    if (Test-Path $ContainerEnvFilePath) {
        $dockerRunArguments += @('--env-file', $ContainerEnvFilePath)
    } else {
        Write-Warning "ContainerEnvFilePath specified but file does not exist: '$ContainerEnvFilePath'"
    }
}

$dockerRunArguments += $targetImage

Invoke-DockerCommand -Arguments $dockerRunArguments

$resolvedHealthUrl = if (-not [string]::IsNullOrWhiteSpace($HealthcheckUrl)) {
    $HealthcheckUrl
} else {
    "http://127.0.0.1:$HostPort/app-config.json"
}

$healthy = $false
Write-Host "Running health check against $resolvedHealthUrl (retries: $HealthcheckRetries, delay: ${HealthcheckDelaySeconds}s)..."

for ($attempt = 1; $attempt -le $HealthcheckRetries; $attempt++) {
    try {
        $response = Invoke-WebRequest -Uri $resolvedHealthUrl -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200 -and -not [string]::IsNullOrWhiteSpace($response.Content)) {
            Write-Host "Health check passed on attempt $attempt."
            $healthy = $true
            break
        }
    }
    catch {
        Write-Host "Health check attempt $attempt waiting for container to become healthy: $_"
    }

    if ($attempt -lt $HealthcheckRetries) {
        Start-Sleep -Seconds $HealthcheckDelaySeconds
    }
}

if (-not $healthy) {
    Write-Warning "Health check failed. Collecting container logs for '$ContainerName':"
    & docker logs --tail 100 $ContainerName
    throw "Deployment health check failed for '$ContainerName' at $resolvedHealthUrl."
}

Write-Host "Successfully deployed '$ContainerName' running $targetImage on host port $HostPort."

