[CmdletBinding()]
param(
    [string]$Case,
    [string]$Connection = 'Server=LAPTOP-3L2V1AFT\SQLEXPRESS;Database=EduConnect_Verification_MT1;Integrated Security=True;Encrypt=False;TrustServerCertificate=True'
)
$ErrorActionPreference='Stop'
$taskRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$taskResults=Join-Path $taskRoot 'TestResults/MT1'
$taskProject=Join-Path $PSScriptRoot 'MT1.csproj'
if ($Case) {
    if ($Case -notmatch '^MT1-IT-(\d{3})$' -or [int]$Matches[1] -lt 14 -or [int]$Matches[1] -gt 86) {
        throw 'Chọn mã từ MT1-IT-014 đến MT1-IT-086.'
    }
    $taskFilter="Suite=MT1Integration&FullyQualifiedName~.MT1_IT_$($Case.Substring($Case.Length-3))"
} else { $taskFilter='Suite=MT1Integration' }
$taskDetails=Join-Path $taskResults 'integration-details.jsonl'
$taskTrx=Join-Path $taskResults 'MT1-integration.trx'
# Ba file cố định chứa kết quả của lần chạy hiện tại; không tạo thư mục theo ngày/lượt.
$taskOldConnection=$env:EDU_TEST_CONNECTION
$taskOldEvidence=$env:MT1_EVIDENCE
$taskOldRunId=$env:MT1_RUN_ID
$taskOldTelemetry=$env:DOTNET_CLI_TELEMETRY_OPTOUT
$taskRunId=[guid]::NewGuid().ToString()
$taskCsv=Join-Path $taskResults 'BangKetQua-Integration-MT1.csv'
$taskMutex=[System.Threading.Mutex]::new($false,'Local\EduConnect_MT1_Integration_Runner')
try { $taskHasLock=$taskMutex.WaitOne(0) } catch [System.Threading.AbandonedMutexException] { $taskHasLock=$true }
if(!$taskHasLock){$taskMutex.Dispose();throw 'Đang có một lượt chạy Integration khác; đợi lượt đó kết thúc.'}
try {
    New-Item -ItemType Directory -Force -Path $taskResults | Out-Null
    foreach($taskFile in @($taskDetails,$taskTrx,$taskCsv)){if(Test-Path -LiteralPath $taskFile){Remove-Item -LiteralPath $taskFile}}
    $env:EDU_TEST_CONNECTION=$Connection
    $env:MT1_EVIDENCE=$taskResults
    $env:MT1_RUN_ID=$taskRunId
    $env:DOTNET_CLI_TELEMETRY_OPTOUT='1'
    $taskLabel=if($Case){$Case}else{'MT1-IT-014..086'}
    Write-Host "RunId: $taskRunId | $taskLabel"
    & dotnet test $taskProject --filter $taskFilter --logger 'trx;LogFileName=MT1-integration.trx' --results-directory $taskResults --verbosity minimal
    $taskExit=$LASTEXITCODE
    if(!(Test-Path -LiteralPath $taskTrx)){throw "Chưa có TRX: build/khởi tạo bộ chạy thất bại (exit $taskExit)."}
    [xml]$taskXml=Get-Content -LiteralPath $taskTrx -Raw
    $taskProof=@{}
    if(Test-Path -LiteralPath $taskDetails){
        foreach($taskLine in Get-Content -LiteralPath $taskDetails){
            if($taskLine.Trim()){$taskRecord=$taskLine|ConvertFrom-Json;$taskProof[$taskRecord.caseId]=$taskRecord}
        }
    }
    $taskRows=@(foreach($taskResult in $taskXml.TestRun.Results.UnitTestResult){
        $taskMatch=[regex]::Match($taskResult.testName,'MT1-IT-\d{3}')
        if(!$taskMatch.Success){throw "Không tìm thấy mã TC trong TRX: $($taskResult.testName)"}
        $taskId=$taskMatch.Value
        $taskRecord=$taskProof[$taskId]
        $taskStatus=switch($taskResult.outcome){'Passed'{'Pass'}'Failed'{'Fail'}'NotExecuted'{'Chờ đặc tả'}default{$taskResult.outcome}}
        $taskActual=if($taskRecord){$taskRecord.actual}else{[string]$taskResult.Output.ErrorInfo.Message}
        if(!$taskActual -and $taskStatus -eq 'Chờ đặc tả'){
            $taskActual=switch($taskId){
                'MT1-IT-019'{'Chưa đặc tả quy tắc giữ/xóa địa chỉ trực tuyến.'}
                'MT1-IT-049'{'Chưa chốt quy tắc tìm từ khóa/dấu/hoa thường.'}
                'MT1-IT-059'{'Chưa thống nhất bộ lọc khoảng học phí.'}
            }
        }
        # CSV để đọc nhanh; log JSONL và TRX giữ đầy đủ assertion/HTTP/SQL snapshot.
        $taskActual=(@($taskActual -split '\r?\n'|Select-Object -First 7) -join ' ').Trim()
        if($taskActual.Length -gt 1400){$taskActual=$taskActual.Substring(0,1400)+'… (xem JSONL/TRX)'}
        [pscustomobject]@{RunId=$taskRunId;MaTC=$taskId;TenTC=$taskResult.testName;TrangThai=$taskStatus;Actual=$taskActual;ThoiGian=$taskResult.duration;MinhChung='MT1-integration.trx; integration-details.jsonl'}
    })
    $taskRows=$taskRows|Sort-Object MaTC
    if(($taskRows.MaTC|Select-Object -Unique).Count -ne @($taskRows).Count){throw 'TRX có mã TC trùng, không xuất bảng gộp.'}
    if(!$Case -and @($taskRows).Count -ne 73){throw "Thiếu ca: mong 73, thực tế $(@($taskRows).Count)."}
    if($Case -and (@($taskRows).Count -ne 1 -or $taskRows.MaTC -ne $Case)){throw 'Bộ lọc chạy không đúng ca đã chọn.'}
    [System.IO.File]::WriteAllLines($taskCsv,[string[]]@($taskRows|ConvertTo-Csv -NoTypeInformation),(New-Object System.Text.UTF8Encoding($true)))
    Write-Host "`nKết quả theo TC (lần chạy này):"
    $taskRows|Select-Object MaTC,TrangThai,ThoiGian|Format-Table -AutoSize
    $taskRows|Group-Object TrangThai|Select-Object Name,Count|Format-Table -AutoSize
    Write-Host "Kết quả được lưu tại $taskResults"
    exit $taskExit
} finally {
    $env:EDU_TEST_CONNECTION=$taskOldConnection
    $env:MT1_EVIDENCE=$taskOldEvidence
    $env:MT1_RUN_ID=$taskOldRunId
    $env:DOTNET_CLI_TELEMETRY_OPTOUT=$taskOldTelemetry
    if($taskHasLock){$taskMutex.ReleaseMutex()}
    $taskMutex.Dispose()
}
