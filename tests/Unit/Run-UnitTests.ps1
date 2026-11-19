$ErrorActionPreference = 'Stop'
$unitProject = Join-Path $PSScriptRoot 'EduConnect.UnitTests.csproj'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$resultRoot = Join-Path $repoRoot 'TestResults/Unit'
New-Item -ItemType Directory -Path $resultRoot -Force | Out-Null
dotnet build $unitProject -p:NuGetAudit=false --verbosity minimal *> (Join-Path $resultRoot 'Build.log')
if ($LASTEXITCODE -ne 0) { throw "Unit project build failed. See $resultRoot/Build.log" }
$groups = @(
    @('01_Quy_tac_chung', 'FullyQualifiedName~RulesTests'),
    @('02_Du_lieu_dau_vao', 'FullyQualifiedName~ProfileInputTests|FullyQualifiedName~FieldValidationTests'),
    @('03_Controller_Middleware', 'FullyQualifiedName~ControllerGuardTests'),
    @('04_Lich_hoc', 'FullyQualifiedName~ClassCalendarTests'),
    @('05_Dia_gioi_Danh_muc', 'FullyQualifiedName~AdministrativeAreaTests'),
    @('06_Khach_Xac_thuc', 'FullyQualifiedName~GuestAccessTests'),
    @('07_Bien_dau_vao_Mat_khau', 'FullyQualifiedName~InputBoundaryTests')
)
$previousEvidence = $env:UNIT_EVIDENCE
$failed = $false
try {
    foreach ($group in $groups) {
        $resultDir = Join-Path $resultRoot $group[0]
        New-Item -ItemType Directory -Path $resultDir -Force | Out-Null
        $env:UNIT_EVIDENCE = $resultDir
        dotnet test $unitProject --no-build --no-restore --filter $group[1] --logger "trx;LogFileName=$($group[0]).trx" --results-directory $resultDir --verbosity normal *> (Join-Path $resultDir 'Run.log')
        $testExit = $LASTEXITCODE
        if ($testExit -ne 0) { $failed = $true }
        Write-Output "$($group[0]): dotnet test exit=$testExit"
        Get-Content -LiteralPath (Join-Path $resultDir 'Run.log') -Tail 10
    }
} finally { $env:UNIT_EVIDENCE = $previousEvidence }
if ($failed) { exit 1 }
exit 0
