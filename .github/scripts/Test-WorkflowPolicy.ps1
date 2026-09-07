Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')

function Get-Workflow {
    param([string] $Name)

    return Get-Content (Join-Path $repositoryRoot ".github\workflows\$Name") -Raw
}

function Assert-Match {
    param(
        [string] $Content,
        [string] $Pattern,
        [string] $Message
    )

    if ($Content -notmatch $Pattern) {
        throw $Message
    }
}

function Assert-NotMatch {
    param(
        [string] $Content,
        [string] $Pattern,
        [string] $Message
    )

    if ($Content -match $Pattern) {
        throw $Message
    }
}

$build = Get-Workflow 'build.yml'
$deploy = Get-Workflow '_deploy.yml'
$development = Get-Workflow 'Deploy-Development.yml'
$test = Get-Workflow 'Deploy-Test.yml'
$production = Get-Workflow 'Deploy-Production.yml'
$manual = Get-Workflow 'deploy.yml'

Assert-Match $build '(?ms)push:\s+branches:\s+- ''\*\*''' `
    'BuildAndTest must run for pushes to every branch.'
Assert-Match $build '(?ms)pull_request:\s+types:\s+- opened\s+- synchronize\s+- reopened\s+branches:\s+- main' `
    'BuildAndTest must run when PRs to main are opened, synchronized, or reopened.'
Assert-NotMatch $build 'continue-on-error:\s*true' `
    'BuildAndTest must not allow tests to fail.'
Assert-Match $build 'moopelfrontend-docker-\$\{\{\s*github\.sha\s*\}\}' `
    'BuildAndTest must upload Docker artifact tagged with commit SHA.'

Assert-Match $deploy 'workflow_call:' `
    '_deploy.yml must be a reusable workflow called via workflow_call.'
Assert-Match $deploy "DEFAULT_DEVELOPMENT_HOST_PORT:\s*'8084'" `
    '_deploy.yml must configure development host port 8084 to avoid collision with backend.'
Assert-Match $deploy "DEFAULT_STAGING_HOST_PORT:\s*'8085'" `
    '_deploy.yml must configure staging host port 8085 to avoid collision with backend.'
Assert-Match $deploy "DEFAULT_TEST_HOST_PORT:\s*'8085'" `
    '_deploy.yml must configure test host port 8085 to avoid collision with backend.'
Assert-Match $deploy "DEFAULT_PRODUCTION_HOST_PORT:\s*'8081'" `
    '_deploy.yml must configure production host port 8081 to avoid collision with backend.'
Assert-Match $deploy 'HOST_PORT:\s*\$\{\{\s*vars\.HOST_PORT\s*\}\}' `
    '_deploy.yml must support overriding host port via vars.HOST_PORT.'
Assert-Match $deploy 'Deploy-Container\.ps1' `
    '_deploy.yml must execute Deploy-Container.ps1.'
Assert-Match $deploy 'run_smoke_tests' `
    '_deploy.yml must support run_smoke_tests parameter.'

Assert-Match $development "(?s)workflow_run\.event == 'push'.*workflow_run\.head_branch != 'main'" `
    'Development must deploy successful non-main push builds.'
Assert-Match $development 'uses:\s*\./\.github/workflows/_deploy\.yml' `
    'Deploy-Development must delegate deployment to _deploy.yml.'
Assert-Match $development 'environment_name:\s*Development' `
    'Deploy-Development must target Development environment.'

Assert-Match $test "workflow_run\.event == 'pull_request'" `
    'Test must deploy successful pull request builds.'
Assert-Match $test 'workflow_run\.head_repository\.full_name == github\.repository' `
    'Test must reject artifacts built from fork pull requests.'
Assert-Match $test 'uses:\s*\./\.github/workflows/_deploy\.yml' `
    'Deploy-Test must delegate deployment to _deploy.yml.'
Assert-Match $test 'environment_name:\s*Test' `
    'Deploy-Test must target Test environment.'

Assert-Match $production '(?ms)deploy-staging:.*uses:\s*\./\.github/workflows/_deploy\.yml.*deploy-production:.*needs:\s*deploy-staging.*uses:\s*\./\.github/workflows/_deploy\.yml' `
    'Production must promote through Staging and delegate both to _deploy.yml.'
Assert-Match $production 'legacy_container_name:\s*moopelfrontend' `
    'Production must pass legacy container name for migration.'

Assert-NotMatch $manual '(?ms)options:.*-\s+Production' `
    'Manual deployment must not bypass Staging to deploy Production.'
Assert-Match $manual 'Validate build provenance' `
    'Manual deployments must validate artifact provenance.'
Assert-Match $manual 'Authorization = "Bearer \$env:GH_TOKEN"' `
    'Manual provenance checks must authenticate to the Actions API.'
Assert-Match $manual 'uses:\s*\./\.github/workflows/_deploy\.yml' `
    'Manual deploy must delegate to _deploy.yml.'

& (Join-Path $PSScriptRoot 'Test-DeployContainer.ps1')
Write-Host 'Workflow policy checks passed.'
