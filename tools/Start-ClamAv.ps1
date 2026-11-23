param(
    [ValidateRange(1, 65535)][int]$Port = 3310,
    [string]$ClamDirectory = (Join-Path $PSScriptRoot 'clamav')
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ClamDirectory).Path
$executable = Join-Path $root 'clamd.exe'
$database = Join-Path $root 'database'
$config = Join-Path $root "clamd-local-$Port.conf"
if (!(Test-Path -LiteralPath $executable -PathType Leaf) -or !(Test-Path -LiteralPath $database -PathType Container)) {
    throw 'Can clamd.exe va thu muc database trong thu muc ClamAV.'
}

function Test-Scanner {
    $client = [Net.Sockets.TcpClient]::new()
    try {
        if (!$client.ConnectAsync('127.0.0.1', $Port).Wait(400)) { return $false }
        $stream = $client.GetStream()
        $stream.ReadTimeout = 400
        $ping = [Text.Encoding]::ASCII.GetBytes("nPING`n")
        $stream.Write($ping, 0, $ping.Length)
        $reader = [IO.StreamReader]::new($stream)
        return $reader.ReadLine() -eq 'PONG'
    } catch [Exception] {
        return $false
    } finally {
        $client.Dispose()
    }
}

$listeners = @(Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue)
if ($listeners.Count) {
    $owned = Get-CimInstance Win32_Process -Filter "ProcessId=$($listeners[0].OwningProcess)"
    if ($listeners.Count -eq 1 -and $listeners[0].LocalAddress -eq '127.0.0.1' -and
        $owned.ExecutablePath -eq $executable -and (Test-Scanner)) { return $owned.ProcessId }
    throw "Cong $Port dang duoc tien trinh khac su dung."
}

@(
    'Foreground yes'
    "DatabaseDirectory `"$database`""
    'TCPAddr 127.0.0.1'
    "TCPSocket $Port"
    'StreamMaxLength 10M'
    'MaxThreads 4'
    'CommandReadTimeout 10'
) | Set-Content -LiteralPath $config -Encoding ASCII
$daemon = Start-Process -FilePath $executable -ArgumentList "--config-file=`"$config`"" -WorkingDirectory $root -WindowStyle Hidden `
    -RedirectStandardOutput (Join-Path $root "clamd-$Port.log") -RedirectStandardError (Join-Path $root "clamd-$Port-error.log") -PassThru
for ($attempt = 0; $attempt -lt 45; $attempt++) {
    $daemon.Refresh()
    if ($daemon.HasExited) { break }
    if (Test-Scanner) { return $daemon.Id }
    Start-Sleep -Milliseconds 650
}
$daemon.Refresh()
if (!$daemon.HasExited) { Stop-Process -Id $daemon.Id }
throw "ClamAV chua san sang. Xem clamd-$Port-error.log trong $root."
