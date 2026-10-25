param(
    [Parameter(Mandatory=$true)][string]$Script,
    [string]$Server='lpc:CONGVJP',
    [string]$Database='KLTN_4NghiepVu_V2'
)
$ErrorActionPreference='Stop'
$scriptPath=(Resolve-Path -LiteralPath $Script).Path
$bytes=[IO.File]::ReadAllBytes($scriptPath)
if($bytes.Length -ge 2 -and (($bytes[0] -eq 255 -and $bytes[1] -eq 254) -or ($bytes[0] -eq 254 -and $bytes[1] -eq 255))){throw "SQL file must be UTF-8: $scriptPath"}
$decoder=New-Object Text.UTF8Encoding($false,$true)
try{$null=$decoder.GetString($bytes)}catch{throw "SQL file contains invalid UTF-8 bytes: $scriptPath"}
& sqlcmd -S $Server -d $Database -E -C -I -b -f 65001 -i $scriptPath
if($LASTEXITCODE -ne 0){throw "SQL execution failed: $scriptPath"}
