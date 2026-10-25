param([int]$Port=5280)
$ErrorActionPreference='Stop'
Push-Location $PSScriptRoot
try { dotnet run --project .\EduConnect.Mvc.csproj --no-launch-profile --urls "http://127.0.0.1:$Port" }
finally { Pop-Location }
