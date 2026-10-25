param([string]$Server='lpc:CONGVJP',[switch]$Existing)
$ErrorActionPreference='Stop'
# Every SQL input is UTF-8. Never run the original CREATE script on an existing DB.
$scripts=if($Existing){@('01_MainLearnerPermission.sql','02_DemoSupplement.sql','05_SubjectNames.sql','06_RepairSubjectEncoding.sql','07_RepairDemoEncoding.sql')}else{@('00_Original_DB02.sql','01_MainLearnerPermission.sql','02_DemoSupplement.sql','05_SubjectNames.sql')}
foreach($script in $scripts){
    Write-Host "Applying $script"
    $database=if($script -eq '00_Original_DB02.sql'){'master'}else{'KLTN_4NghiepVu_V2'}
    & (Join-Path $PSScriptRoot '..\tools\Invoke-SqlUtf8.ps1') -Server $Server -Database $database -Script (Join-Path $PSScriptRoot $script)
}
Write-Host 'Database setup completed.'
