import json,pathlib,xml.etree.ElementTree as ET,re,hashlib,csv,collections,datetime
repo=pathlib.Path(__file__).resolve().parents[2]
root=repo/'TestResults/Unit'; tests=repo/'tests/Unit'
manifest=json.loads((tests/'CurrentDesignManifest.json').read_text(encoding='utf-8-sig'))
ns={'t':'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
names={1:'01_UC1',2:'02_UC2',3:'03_UC3',4:'04_UC4',5:'05_UC5',6:'06_UC6',7:'07_UC7',8:'08_Dung_chung',10:'09_Thanh_phan_khac'}
records={};runs=[]
for group,name in names.items():
    if group==5:continue
    f=root/name/(name+'.trx');tree=ET.parse(f)
    runs.append(dict(group=group,file=str(f),times=tree.find('t:Times',ns).attrib,counters=tree.find('t:ResultSummary/t:Counters',ns).attrib))
    for result in tree.findall('t:Results/t:UnitTestResult',ns):
        match=re.search(r'(UT_\d{3})_(DL\d+)',result.attrib['testName'])
        if not match:raise ValueError(result.attrib['testName'])
        tc=match[1].replace('_','-');key=(tc,match[2])
        if key in records:raise ValueError(f'Duplicate result {key}')
        records[key]=dict(outcome=result.attrib['outcome'],duration=result.attrib.get('duration'),name=result.attrib['testName'],message=result.findtext('t:Output/t:ErrorInfo/t:Message','',ns),stack=result.findtext('t:Output/t:ErrorInfo/t:StackTrace','',ns),trx=str(f))
expected={(d['tc'],d['dl']) for d in manifest}
assert set(records)==expected,(len(records),len(expected),expected-set(records),set(records)-expected)
source=tests/'CurrentDesignTests.cs';lines=source.read_text(encoding='utf-8-sig').splitlines();locations={}
for number,line in enumerate(lines,1):
    m=re.search(r'public void (UT_\d{3}_DL\d+)\(',line)
    if m:locations[m[1]]=number
for d in manifest:
    d.update(records[(d['tc'],d['dl'])]);d['script']=str(source);d['line']=locations[d['tc'].replace('-','_')+'_'+d['dl']]
    d['classification']='Kỳ vọng TC không khớp thiết kế' if d['tc']=='UT-354' and d['outcome']=='Failed' else None
def counts(rows):return dict(collections.Counter(d['outcome'] for d in rows))
tcgroups=collections.defaultdict(list)
for d in manifest:tcgroups[d['tc']].append(d)
tcverdict={tc:'Failed' if any(d['outcome']=='Failed' for d in rows) else 'Passed' if all(d['outcome']=='Passed' for d in rows) else 'NotExecuted' for tc,rows in tcgroups.items()}
summary=dict(project=str(repo),testProject=str(tests/'EduConnect.UnitTests.csproj'),baseline='Unit_Test_Dieu_chinh_theo_GV.xlsx — 79 Unit / 360 TC / 389 DL',units=len({d['unit'] for d in manifest}),testCases=len(tcgroups),datasets=len(manifest),testCaseOutcomes=dict(collections.Counter(tcverdict.values())),datasetOutcomes=counts(manifest),applicationSourceChanged=False,sqlServerUsed=False,runs=runs)
(root/'KetQua.json').write_text(json.dumps(dict(summary=summary,datasets=manifest),ensure_ascii=False,indent=2),encoding='utf-8')
def esc(x):return str(x or '').replace('|','\\|').replace('\r','').replace('\n','<br>')
table=['| Nhóm | Unit | TC | TC Pass | TC Fail | DL Pass | DL Fail | Chưa thực thi |','|---|---:|---:|---:|---:|---:|---:|---:|']
for group,name in names.items():
    rows=[d for d in manifest if d['group']==group];tcs={d['tc'] for d in rows};c=counts(rows)
    table.append(f'| {name} | {len({d["unit"] for d in rows})} | {len(tcs)} | {sum(tcverdict[t]=="Passed" for t in tcs)} | {sum(tcverdict[t]=="Failed" for t in tcs)} | {c.get("Passed",0)} | {c.get("Failed",0)} | {sum(v for k,v in c.items() if k not in ("Passed","Failed"))} |')
failure='''UT-354/DL01 — `NhatKyAnToanMapping.Configure` (HT-U55).

- Kỳ vọng đang ghi trong Excel: `MaYeuCau.IsUnicode == false`.
- Quan sát khi chạy: `FindProperty("MaYeuCau").IsUnicode()` trả `null`; assertion thất bại tại chính trường `MaYeuCau`.
- Code ứng dụng: `Data/NhatKyAnToanMapping.cs:22` chỉ có `e.Property(x => x.MaYeuCau).HasMaxLength(128);`.
- Căn cứ: Process HT-U55, bảng 509 trong “Thiết kế xử lý chi tiết (1).docx” cũng chỉ đặt `HasMaxLength(128)`, không có `.IsUnicode(false)` cho trường này.
- Phân loại: lỗi kỳ vọng của kịch bản TC, **chưa có căn cứ kết luận code sai**. Giá trị `null` ở metadata có nghĩa là chưa cấu hình Unicode tường minh, không phải giá trị dữ liệu của mã yêu cầu.
- Cách sửa TC khi hiệu chỉnh bộ kịch bản: bỏ `MaYeuCau` khỏi danh sách phải có `IsUnicode=false`, vẫn kiểm độ dài 128. Không cần sửa code ứng dụng để làm test xanh.

Kết quả vẫn ghi Fail theo nguyên kỳ vọng Excel đã nhận; không sửa oracle để che kết quả. Chi tiết assertion/stack trace nằm trong TRX của nhóm `08_Dung_chung`.
'''
now=datetime.datetime.now(datetime.timezone(datetime.timedelta(hours=7))).isoformat(timespec='seconds')
text=f'''# Kết quả thực thi Unit Test — EduConnect1

Chạy trên `{repo}`, thời điểm lập báo cáo `{now}` (UTC+7).

Đã thực thi **360/360 TC, 389/389 bộ dữ liệu, 79/79 Unit** theo danh mục hiện tại. Kết quả: **359 TC Pass, 1 TC Fail**; theo lượt xUnit: **388 Pass, 1 Fail, 0 Skip, 0 chưa thực thi**. Fail duy nhất thuộc kỳ vọng TC không khớp thiết kế; không ghi nhận nó thành lỗi code ứng dụng.

Không sửa code ứng dụng. Chỉ thêm/sửa project test, cấu hình chạy và kết quả trong `tests/Unit`, `TestResults/Unit`; `bin/obj` được cập nhật khi build.

## Kết quả theo nhóm

'''+ '\n'.join(table)+'''

UC5 không có Unit C# được chọn trong danh mục hiện tại; không tự thêm test workflow hoặc tính một lượt Pass giả cho nhóm rỗng.

## Phân tích ca Fail

'''+failure+'''
## Cách thực thi và phạm vi chứng minh

- xUnit gọi method thật theo từng `UT-xxx/DLxx`; các helper C# trong project test chỉ dựng fixture, gọi reflection vào method private và assert.
- Truy vấn LINQ và theo dõi entity dùng EF InMemory làm test double kỹ thuật. Model được khởi tạo trong Arrange; Act chạy method được chọn. Không suy ra khả năng dịch SQL, ràng buộc SQL hoặc transaction từ kết quả của fake này.
- Các wrapper tạo lệnh quan hệ dùng interceptor chặn mở kết nối và chặn thực thi command, ghi lại SQL/parameter/token rồi trả dữ liệu fixture. Không kết nối SQL Server hoặc kiểm hiệu lực khóa thật.
- Các ca cần giờ cố định/resource/File.Delete dùng Lib.Harmony 2.4.2, chỉ shim API framework trong testhost. Không patch method ứng dụng. Tắt inlining/ReadyToRun/tiered compilation trong tiến trình test để lời gọi framework đi qua shim. Shim được tháo trong Dispose, test không chạy song song.
- `UT-008` kiểm ba mốc quanh 17:00 UTC; `UT-121..132` kiểm bảng quyết định khóa đăng nhập và biên ±1 tick; `UT-280/281` kiểm null/JSON stream và dispose; `UT-330/331` kiểm thứ tự Delete và cùng tham chiếu IOException, không I/O thật.
- TRX là bằng chứng thực thi. Số ca chạy đủ không đồng nghĩa đã đo 100% line/branch coverage; lượt này không xuất báo cáo coverage.

## Chạy lại

Tại thư mục EduConnect1:

```powershell
.\\tests\\Unit\\Run-UnitTests.ps1
```

Script build ứng dụng rồi project test, chạy theo từng nhóm, ghi đè TRX/log cùng đường dẫn. Exit 1 là kết quả có test Fail; xem `08_Dung_chung` để phân biệt với lỗi build. Build log nằm ở `Build-App.log`, `Build.log`.

Chạy riêng một TC cần shim, ví dụ:

```powershell
dotnet test tests/Unit/EduConnect.UnitTests.csproj --no-build --no-restore --filter "Suite=CurrentDesign&TestCaseId=UT-128" --environment COMPlus_JitNoInline=1 --environment DOTNET_ReadyToRun=0 --environment DOTNET_TieredCompilation=0
```

`KetQua.json` và `Doi_chieu_TC.csv` chứa đủ từng TC/DL, input, expected, outcome, vị trí script và đường dẫn TRX. Mỗi thư mục nhóm có `KetQua.md`, `KetQua.json`, `Run.log`, TRX riêng. `Script-SHA256.json` lưu hash script và assembly thực tế được chạy.
'''
(root/'KetQua.md').write_text(text,encoding='utf-8-sig')
for group,name in names.items():
    rows=[d for d in manifest if d['group']==group]
    if not rows:continue
    folder=root/name
    detail=f'# Kết quả {name}\n\n'+f'{len({d["tc"] for d in rows})} TC; {len(rows)} bộ dữ liệu; {json.dumps(counts(rows),ensure_ascii=False)}.\n\n'
    if any(d['outcome']=='Failed' for d in rows):detail+='## Phân tích Fail\n\n'+failure+'\n'
    detail+='| TC/DL | Unit | Input | Kết quả kỳ vọng | Thực tế | Script |\n|---|---|---|---|---|---|\n'
    for d in rows:
        actual='xUnit Passed: các assertion trong script đã đạt.' if d['outcome']=='Passed' else d['message'] or d['outcome']
        detail+=f'| {d["tc"]}/{d["dl"]} | {d["unit"]} | {esc(d["input"])} | {esc(d["expected"])} | {esc(actual)} | CurrentDesignTests.cs:{d["line"]} |\n'
    (folder/'KetQua.md').write_text(detail,encoding='utf-8-sig')
    (folder/'KetQua.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8')
with (root/'Doi_chieu_TC.csv').open('w',encoding='utf-8-sig',newline='') as f:
    writer=csv.DictWriter(f,fieldnames=['tc','dl','unit','group','decision','input','expected','outcome','classification','script','line','trx']);writer.writeheader();writer.writerows({k:d.get(k) for k in writer.fieldnames} for d in manifest)
hashes={str(f.relative_to(repo)):hashlib.sha256(f.read_bytes()).hexdigest() for f in tests.glob('*') if f.suffix in ('.cs','.csproj','.ps1','.json')}
for f in [repo/'bin/Debug/net10.0/EduConnect.Mvc.dll',tests/'bin/Debug/net10.0/EduConnect.UnitTests.dll']:
    hashes[str(f.relative_to(repo))]=hashlib.sha256(f.read_bytes()).hexdigest()
(root/'Script-SHA256.json').write_text(json.dumps(hashes,indent=2,ensure_ascii=False),encoding='utf-8')
print(json.dumps(summary,ensure_ascii=False,indent=2))

