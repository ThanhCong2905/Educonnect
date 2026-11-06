# Kiểm thử UC1

Bộ này kiểm tra UC1.1–UC1.4. Các ca `RulesTests` không cần SQL. Các ca còn lại dùng SQL Server thật và chỉ chấp nhận tên database bắt đầu bằng `EduConnect_Verification_`.

## Chạy quy tắc độc lập

```powershell
dotnet test tests/MT1/MT1.csproj --filter FullyQualifiedName~RulesTests
```

## Chạy toàn bộ 138 ca xUnit

Tạo database kiểm thử mới từ bộ cài thống nhất và đổi tên database trong bản script dùng để kiểm thử. Không chạy script tạo fixture trên database đang sử dụng.

```powershell
$env:EDU_TEST_CONNECTION='Server=lpc:CONGVJP;Database=EduConnect_Verification_MT1_20261105;Integrated Security=True;Encrypt=False'
$env:MT1_EVIDENCE='D:\Educonnect\docs\mt1-run'
dotnet test tests/MT1/MT1.csproj --logger 'trx;LogFileName=MT1-final.trx' --results-directory docs/mt1-run/results
```

`IntegrationTests` gồm 89 ca dùng Controller, Service và SQL thật. Trong đó 21 ca giữ mã UT từ bảng cũ để truy vết, nhưng không được coi là unit test có mock. Mỗi ca tạo người dùng và dấu nhận biết riêng. Hai trigger gây lỗi để kiểm rollback chỉ tồn tại trong DB test và được xóa trong `finally`.

## Kiểm tra HTTP và trình duyệt

`browser.cjs` cần Node 20 trở lên, Playwright và Edge. Trước khi chạy, thực hiện bộ xUnit và `tests/DesignContracts` trên cùng DB mới; sao chép `results.json` của DesignContracts thành `docs/mt1-run/contracts-results.json`. Tạo `parent.json` chứa `{"parent":"GUID phụ huynh trong DB test"}`.

Chạy ứng dụng ở cổng 5298 với `ConnectionStrings__EduConnect` trỏ DB test, sau đó:

```powershell
node tests/MT1/browser.cjs
```

Mỗi lần chạy toàn bộ cần fixture mới để lời mời liên hệ của lần trước không làm thay đổi điều kiện đầu vào. `MT1_CASE_FILTER` cho phép chạy lại các mã riêng; những ca phụ thuộc dữ liệu của ca khác cần được chọn cùng nhau. Bộ kiểm thử ghi assertion, phản hồi HTTP và ảnh; ảnh riêng không thay cho assertion.

Các ca xác thực thật có thể thất bại vì bản ứng dụng hiện dùng người mẫu. Không đổi Expected thành cho phép truy cập để làm những ca này đạt.
