param([string]$Server = 'lpc:CONGVJP')
$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot '..\tools\Invoke-SqlUtf8.ps1') -Server $Server -Database master -Script (Join-Path $PSScriptRoot '00_EduConnect_Full.sql')
Write-Host 'Database KLTN_Full_V2 đã được tạo.'
