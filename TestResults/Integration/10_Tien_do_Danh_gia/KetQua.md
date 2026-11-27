# Tiến độ, đánh giá và lịch sử

Thời điểm: 27/11/2026 13:01:18 +07:00.
Database riêng: `EduConnect_Verification_Integration_20261125_v2`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.

Kết luận TC: {'Pass': 11, 'Fail': 2}. Kết quả script xUnit: {'Pass': 11, 'Fail': 2}.

**Script Pass chưa đủ phạm vi TC không được tính là TC Pass hoàn chỉnh. Lượt này không có ca Skip; 8 mã thiếu dispatcher được gọi và Fail trước khi chạy scenario nghiệp vụ.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.

| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |
|---|---|---|---|---|
| IT-111 | Tổng hợp tiến độ từ nguồn thật với phép tính đối chiếu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-112 | Hai bên đánh giá độc lập sau khi kết thúc có buổi thực tế | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-113 | Khoảng ngày và trạng thái rỗng không tạo điểm 0 giả | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-114 | Bỏ kết quả lần cũ và bài nháp khi tổng hợp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-115 | Gửi lại không sửa điểm/nhận xét kể cả bị ẩn | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-116 | Tiến độ và liên kết nguồn tại tổng quan lớp | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-117 | Người ngoài không đọc tiến độ/điểm | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-118 | Đọc phiếu và lịch sử phản hồi cũ theo quyền lớp | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |
| IT-119 | Gửi đánh giá xác nhận quyền riêng tư | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-120 | Thiếu điều kiện đánh giá không lưu dữ liệu | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-121 | Biên sao và nhận xét đi qua binding rồi nghiệp vụ | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-122 | Đối tác phản hồi giữ lịch sử và phát notice | Pass | Pass | Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi. |
| IT-123 | Ẩn đánh giá thay đổi tổng hợp và quyền xem đánh giá Bên học | Fail | Fail | Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ. |

## IT-118 — Đọc phiếu và lịch sử phản hồi cũ theo quyền lớp

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: H1 200 với dữ liệu cũ; X 403; không giả định có POST TaoDot/Gui/DongDot trong Controller hiện tại.
2. Dữ liệu và side effect: Không thay/xóa phiếu, mức hài lòng null vẫn null; không biến phản hồi cũ thành quyết định tự kết thúc lớp.

**Quan sát/assertion thực tế:**
```text
Assert.Contains() Failure: Sub-string not found
String:    "<!doctype html><html lang="vi"><head><met"···
Not found: "Nội dung cũ"
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProgressCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProgressCases.cs:line 58
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 352
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_118() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 120
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-118-IT15ee7e655c3648e39ea94c335fd10890.json`

## IT-123 — Ẩn đánh giá thay đổi tổng hợp và quyền xem đánh giá Bên học

Phân loại: Cần phân tích.

**Kỳ vọng:** 1. Phản hồi / hợp đồng: Rating G1=4.5,count=2; đánh giá Bên học chỉ Gia sư/chính người nhận/quản trị có quyền được xem, khách không được lộ.
2. Dữ liệu và side effect: Bản ẩn vẫn lưu cho lịch sử; không bị tính vào rating; truy vấn cùng contract giữa tìm kiếm và chi tiết.

**Quan sát/assertion thực tế:**
```text
rating detail missing from HTTP OK: <!doctype html>
<html lang="vi">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>&#x110;&#xE1;nh gi&#xE1; ng&#x1B0;&#x1EDD;i d&#xF9;ng · EduConnect</title>
    <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css">
    <link rel="stylesheet" href="/css/fonts.css">
    <link rel="stylesheet" href="/css/app.css?v=mAsrX0eE1UWydUxrBnHNegBYUOqjjuSeSgsM09r68HU">
    <link rel="stylesheet" href="/css/profile.css?v=HeU4bNwSZV6X2nI3LqKT0iEgDQSlKL0zBorZHX5S1Ds">
    <link rel="stylesheet" href="/css/notifications.css?v=r2hN5jKsMzzXO-E2YY5ZVVmAm6mogxXxnDo3AUkBeLk">
    <link rel="stylesheet" href="/css/guest.css?v=fuEqLuZjDxhyazhuNDveb_vFB7jyDUttkhZtzMY4_JQ">
<link rel="stylesheet" href="/css/help.css?v=csAUm09dG8Y_ElJmnFxptcFbP09DziJWRkRgY3hoKMo">
</head>
<body>
    <button type="button" class="btn mobile-menu" aria-label="Mở menu" aria-expanded="false" aria-controls="navigation">☰</button>
    <aside class="sidebar" id="navigation">
        <a class="brand" href="/"><img src="/images/auth/logo.png" alt="EduConnect"></a>
        <p class="eyebrow">KHÁM PHÁ EDUCONNECT</p>
        <nav ar
```

**Ý nghĩa:** Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.

Script/stack thực thi:
```text
   at Cases.ProgressCases(Fixture f, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\ProgressCases.cs:line 63
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 352
   at IntegrationTests.Run(String id, String key) in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 395
   at IntegrationTests.IT_123() in C:\Khóa luận TN\EduConnect1\tests\IntegrationCurrent\IntegrationTests.cs:line 125
--- End of stack trace from previous location ---
```

Bằng chứng: `Evidence\IT-123-ITbb8b8398757e4f28a58301111cb492ba.json`

## Giới hạn của lượt thực thi

Đã chạy toàn bộ danh mục 191 mã: 118 Pass, 73 Fail, 0 Skip. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. 8 ca trước đây pending hiện chạy và Fail do dispatcher chưa có implementation. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.
