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
Assert-Match $development "(?s)workflow_run\.event == 'push'.*workflow_run\.head_branch != 'main'" `
    'Development must deploy successful non-main push builds.'
Assert-Match $test "workflow_run\.event == 'pull_request'" `
    'Test must deploy successful pull request builds.'
Assert-Match $test 'workflow_run\.head_repository\.full_name == github\.repository' `
    'Test must reject artifacts built from fork pull requests.'
Assert-Match $production '(?ms)environment:\s+Staging.*needs:\s+deploy-staging.*environment:\s+Production' `
    'Production must promote through Staging.'
Assert-Match $production "(?s)HOST_PORT: '8083'.*HOST_PORT: '8080'" `
    'Staging and Production must use isolated host ports.'
Assert-Match $production '-LegacyContainerName moopelfrontend' `
    'Production must migrate the legacy container before replacing it.'
Assert-Match $development "HOST_PORT: '8082'" `
    'Development must use its dedicated host port.'
Assert-Match $test "HOST_PORT: '8081'" `
    'Test must use its dedicated host port.'
Assert-NotMatch $manual '(?ms)options:.*-\s+Production' `
    'Manual deployment must not bypass Staging to deploy Production.'
Assert-Match $manual 'Validate build provenance' `
    'Manual deployments must validate artifact provenance.'
Assert-Match $manual 'Authorization = "Bearer \$env:GH_TOKEN"' `
    'Manual provenance checks must authenticate to the Actions API.'

Write-Host 'Workflow policy checks passed.'
