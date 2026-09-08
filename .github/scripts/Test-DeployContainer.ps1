Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$scriptPath = Resolve-Path (Join-Path $PSScriptRoot 'Deploy-Container.ps1')

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) {
        throw "Assertion failed: $Message"
    }
}

Write-Host "Validating Deploy-Container.ps1 AST and syntax..."
$tokens = $null
$parseErrors = $null
$ast = [System.Management.Automation.Language.Parser]::ParseFile($scriptPath, [ref]$tokens, [ref]$parseErrors)

Assert-True ($parseErrors.Count -eq 0) "Deploy-Container.ps1 should have 0 syntax/parse errors."

Write-Host "Validating parameter definitions..."
$cmd = Get-Command $scriptPath
$expectedParams = @(
    'ContainerName',
    'ImageName',
    'ImageTag',
    'HostPort',
    'ContainerPort',
    'EnvironmentName',
    'ApiBaseUrl',
    'ContainerEnvFilePath',
    'LegacyContainerName',
    'HealthcheckRetries',
    'HealthcheckDelaySeconds',
    'HealthcheckUrl'
)

foreach ($expected in $expectedParams) {
    Assert-True ($cmd.Parameters.ContainsKey($expected)) "Deploy-Container.ps1 must declare parameter '$expected'."
}

Write-Host "Validating mandatory parameters..."
$mandatoryParams = @('ContainerName', 'ImageName', 'ImageTag', 'HostPort', 'ContainerPort', 'EnvironmentName')
foreach ($mand in $mandatoryParams) {
    $paramInfo = $cmd.Parameters[$mand]
    $paramAttr = $paramInfo.Attributes | Where-Object { $_ -is [System.Management.Automation.ParameterAttribute] }
    $isMandatory = $paramAttr.Mandatory -contains $true
    Assert-True $isMandatory "Parameter '$mand' must be marked Mandatory."
}

Write-Host "Validating script logic and safety contracts..."
$scriptContent = Get-Content $scriptPath -Raw

Assert-True ($scriptContent -match 'function Remove-ContainerGracefully') `
    "Deploy-Container.ps1 must define Remove-ContainerGracefully."

Assert-True ($scriptContent -match 'if \(-not \[string\]::IsNullOrWhiteSpace\(\$LegacyContainerName\)\)\s*\{\s*Remove-ContainerGracefully -Name \$LegacyContainerName') `
    "Deploy-Container.ps1 must skip legacy container removal when no legacy container name is provided."

Assert-True ($scriptContent -match 'stop.*--time.*10') `
    "Deploy-Container.ps1 must gracefully stop containers with a timeout."

Assert-True ($scriptContent -match 'rm.*-f') `
    "Deploy-Container.ps1 must remove containers."

Assert-True ($scriptContent -match '--restart.*unless-stopped') `
    "Deploy-Container.ps1 must specify restart policy 'unless-stopped'."

Assert-True ($scriptContent -match 'Environment=\$EnvironmentName') `
    "Deploy-Container.ps1 must set Environment environment variable."

Assert-True ($scriptContent -match 'MoopelApiOptions__BaseUrl=') `
    "Deploy-Container.ps1 must set MoopelApiOptions__BaseUrl environment variable."

Assert-True ($scriptContent -match 'app-config\.json') `
    "Deploy-Container.ps1 must default health check to /app-config.json."

Assert-True ($scriptContent -match 'HealthcheckRetries') `
    "Deploy-Container.ps1 must support health check retry count."

Assert-True ($scriptContent -match 'HealthcheckDelaySeconds') `
    "Deploy-Container.ps1 must support health check delay interval."

Assert-True ($scriptContent -match 'docker logs.*tail') `
    "Deploy-Container.ps1 must collect docker logs when health check fails."

Write-Host "All Deploy-Container.ps1 tests passed successfully."
