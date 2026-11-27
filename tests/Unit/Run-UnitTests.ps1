$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project=Join-Path $PSScriptRoot 'EduConnect.UnitTests.csproj'
$resultRoot=Join-Path $repoRoot 'TestResults/Unit'
New-Item -ItemType Directory -Path $resultRoot -Force | Out-Null
Push-Location $repoRoot
try {
    dotnet build (Join-Path $repoRoot 'EduConnect.Mvc.csproj') -p:NuGetAudit=false --verbosity minimal *> (Join-Path $resultRoot 'Build-App.log')
    if($LASTEXITCODE -ne 0){throw 'Build ứng dụng thất bại; xem Build-App.log.'}
    dotnet build $project -p:NuGetAudit=false --verbosity minimal *> (Join-Path $resultRoot 'Build.log')
    if($LASTEXITCODE -ne 0){throw 'Build project test thất bại; xem Build.log.'}
    $groups=@(@('01_UC1','01'),@('02_UC2','02'),@('03_UC3','03'),@('04_UC4','04'),@('06_UC6','06'),@('07_UC7','07'),@('08_Dung_chung','08'),@('09_Thanh_phan_khac','10'))
    $failed=$false
    foreach($g in $groups){
        $dir=Join-Path $resultRoot $g[0]
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        dotnet test $project --no-build --no-restore --filter "Suite=CurrentDesign&Group=$($g[1])" --environment COMPlus_JitNoInline=1 --environment DOTNET_ReadyToRun=0 --environment DOTNET_TieredCompilation=0 --logger "trx;LogFileName=$($g[0]).trx" --results-directory $dir --verbosity normal *> (Join-Path $dir 'Run.log')
        $testExit=$LASTEXITCODE
        if($testExit -ne 0){$failed=$true}
        Write-Output "$($g[0]): exit=$testExit"
        Get-Content -LiteralPath (Join-Path $dir 'Run.log') -Tail 8
    }
} finally { Pop-Location }
if($failed){exit 1}
exit 0
