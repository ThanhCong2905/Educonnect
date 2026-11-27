param([string]$Filter='',[switch]$NoBuild,[string]$RunName='Integration')
$ErrorActionPreference='Stop'
$taskAppRoot=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if($RunName -notmatch '^[A-Za-z0-9_-]+$'){throw 'RunName chỉ được chứa chữ, số, gạch ngang và gạch dưới.'}
$taskOutput=Join-Path (Join-Path $taskAppRoot 'TestResults') $RunName
New-Item -ItemType Directory -Path $taskOutput -Force | Out-Null
$env:EDU_TEST_ROOT=$taskAppRoot
$env:ASPNETCORE_TEST_CONTENTROOT_EDUCONNECT_MVC=$taskAppRoot
$env:EDU_TEST_CONNECTION='Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_Integration_20261125_v2;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Pooling=false'
$env:IT_EVIDENCE=Join-Path $taskOutput 'Evidence'
if(-not $NoBuild){
 dotnet build (Join-Path $taskAppRoot 'EduConnect.Mvc.csproj') --no-restore -v minimal
 if($LASTEXITCODE -ne 0){throw 'Build ứng dụng thất bại; không chạy DLL cũ.'}
 dotnet build (Join-Path $PSScriptRoot 'IntegrationCurrent.csproj') -p:AppRoot=$taskAppRoot -p:NuGetAudit=false --ignore-failed-sources -v minimal
 if($LASTEXITCODE -ne 0){throw 'Build test thất bại; không chạy DLL cũ.'}
}
$taskHashes=@{}
foreach($taskDir in @('Areas','Controllers','Data','Infrastructure','Models','ViewModels')) {
 Get-ChildItem -LiteralPath (Join-Path $taskAppRoot $taskDir) -Recurse -Filter '*.cs' -File | ForEach-Object { $taskHashes[$_.FullName.Substring($taskAppRoot.Length+1)]=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
}
foreach($taskFile in @('Program.cs','EduConnect.Mvc.csproj','appsettings.json','Database\00_EduConnect_Full.sql')) { $taskHashes[$taskFile]=(Get-FileHash -LiteralPath (Join-Path $taskAppRoot $taskFile) -Algorithm SHA256).Hash.ToLowerInvariant() }
if(-not $Filter){$taskHashes | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskOutput 'SourceBefore.json') -Encoding utf8}
$taskLogBase=if($Filter){'Single-'+(Get-Date -Format 'yyyyMMdd-HHmmssfff')}else{$RunName}
$taskArgs=@('test',(Join-Path $PSScriptRoot 'IntegrationCurrent.csproj'),'--no-build','--no-restore','--logger',("trx;LogFileName="+$taskLogBase+'.trx'),'--results-directory',$taskOutput,'--verbosity','normal')
if($Filter){$taskArgs+=@('--filter',$Filter)}
$taskSavedErrorPreference=$ErrorActionPreference
try {
 $ErrorActionPreference='Continue'
 & dotnet @taskArgs *> (Join-Path $taskOutput ($taskLogBase+'.log'))
 $taskTestExit=$LASTEXITCODE
} finally { $ErrorActionPreference=$taskSavedErrorPreference }
Get-Content (Join-Path $taskOutput ($taskLogBase+'.log')) -Tail 12
if(-not $Filter){
 $taskPython='C:\Program Files\WindowsApps\OpenAI.CodexPrimaryRuntime.v26-1123-915-0_26.1123.915.0_x64__3k8sg7r9htsxt\dependencies\python\python.exe'
 & $taskPython -X utf8 (Join-Path $PSScriptRoot 'Report.py') --root $taskAppRoot --results-dir (Join-Path 'TestResults' $RunName) --trx-name ($taskLogBase+'.trx')
}
exit $taskTestExit
