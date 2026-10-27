param([string]$Server='lpc:CONGVJP',[switch]$Existing)
$ErrorActionPreference='Stop'
# Every SQL input is UTF-8. Never run the original CREATE script on an existing DB.
$scripts=if($Existing){@('08_ProfileWorkflow.sql')}else{@('00_Original_DB02.sql')}
foreach($script in $scripts){
    Write-Host "Applying $script"
    $database=if($script -eq '00_Original_DB02.sql'){'master'}else{'KLTN_4NghiepVu_V2'}
    & (Join-Path $PSScriptRoot '..\tools\Invoke-SqlUtf8.ps1') -Server $Server -Database $database -Script (Join-Path $PSScriptRoot $script)
}
Write-Host 'Database setup completed.'
