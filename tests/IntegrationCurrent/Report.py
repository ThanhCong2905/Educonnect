import argparse,collections,csv,copy,hashlib,json,pathlib,re,xml.etree.ElementTree as E
from datetime import datetime,timedelta,timezone
a=argparse.ArgumentParser();a.add_argument('--root',required=True);a.add_argument('--results-dir',default='TestResults/Integration');a.add_argument('--trx-name',default=None);args=a.parse_args()
root=pathlib.Path(args.root);src=pathlib.Path(__file__).parent;out=root/args.results_dir;out.mkdir(parents=True,exist_ok=True)
plan=json.loads((src/'Manifest.json').read_text(encoding='utf-8-sig'));gaps=json.loads((src/'coverage.json').read_text(encoding='utf-8-sig'))
trx_name=args.trx_name or (out.name+'.trx');trx_path=out/trx_name
if not trx_path.is_file():raise FileNotFoundError(f'TRX không tồn tại: {trx_path}')
trx=E.parse(trx_path);ns={'t':'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'};tag=lambda n:'{'+ns['t']+'}'+n
raw={re.search(r'IT-\d{3}',r.get('testName','')).group():r for r in trx.findall('.//t:UnitTestResult',ns)}
assert len(raw)==len(plan['cases']),(len(raw),'TRX không khớp số test trong Manifest.')
groups={g['id']:g for g in plan['groups']};rows=[]
notes={
 'IT-010':('Sai lệch kỳ vọng TC/SQL cần thống nhất','TC yêu cầu chờ 0/1/5/15 phút, procedure hiện tại quy định 30/60/120/300 giây. Không tự kết luận SQL sai trước khi đối chiếu thiết kế/SRS.','Database/00_EduConnect_Full.sql:1189'),
 'IT-020':('Kỳ vọng TC chưa đúng tầng từ chối','Authorize(Roles=HocSinh,PhuHuynh) ở Controller chặn Gia sư bằng403 trước action; Excel lại yêu cầu422. Không có bằng chứng vượt quyền.','Areas/NhuCau/Controllers/NhuCauController.cs:11'),
 'IT-063':('Sai lệch mã lỗi/quyền cần đối chiếu','Người ngoài không được đọc yêu cầu nên Service trả404 trước nhánh từ chối403. Kiểm tiếp không tạo lớp, không suy diễn đây là vượt quyền.','Areas/KetNoi/Services/KetNoiService.cs:199'),
 'IT-073':('Kỳ vọng TC cần đối chiếu','GuiDeNghiAsync chỉ tìm email tài khoản Học sinh; nhập Gia sư trả404, trong khi Excel yêu cầu400.','Areas/HocTap/Services/LienKetTaiKhoanService.cs:25'),
 'IT-075':('Sai lệch mã lỗi/quyền cần đối chiếu','PhanHoiLoiMoiAsync tra lời mời thuộc người hiện tại; người ngoài nhận404, Excel yêu cầu403.','Areas/HocTap/Services/LopHocService.cs:121'),
 'IT-080':('Ứng viên lỗi thiếu luồng nghiệp vụ theo SRS','DuyetAsync luôn kiểm quyền ghi với giaSuOnly=true; Bên học gỡ thành viên hoặc thành viên tự rời bị403 dù FR-38/QT-08 cho phép. Cần xem cả SRS và thiết kế trước sửa code.','Areas/HocTap/Services/LopHocService.cs:145'),
 'IT-025':('Cần xác minh nhánh validation cho phép lưu','Một DL Excel yêu cầu422 nhưng HTTP trả200. Xem fieldUnderTest và http trong Evidence để xác định trường; chưa mặc định mọi DL validation đều sai.','Areas/NhuCau/Services/NhuCauService.cs:66'),
 'IT-053':('Sai lệch dữ liệu tại khớp nối binding học phí','Ba DL thiếu lịch/mục tiêu/sai thời lượng đã bị422. DL04 gửi HocPhi="150000.5" lại200; đã đọc SQL xác nhận HocPhi=1500005.00. Giá trị đã bị hiểu thành số khác trước khi lưu. Cần kiểm tra culture/decimal model binding; Service có kiểm VND nguyên ở dòng180, vì vậy không thể chỉ sửa assertion hoặc đổ lỗi SQL làm tròn.','Areas/KetNoi/Services/KetNoiService.cs:180'),
 'IT-056':('Sai lệch mã lỗi quyền cần thống nhất','Người ngoài nhận404, Excel yêu cầu403; không chứng minh người ngoài tạo được yêu cầu. Đối chiếu quy tắc che sự tồn tại của liên hệ trước khi kết luận lỗi nghiệp vụ.','Areas/KetNoi/Services/KetNoiService.cs:113'),
 'IT-069':('Sai lệch mã từ chối khi lớp đã kết thúc','Excel yêu cầu409 cho thao tác ghi; quan sát có403. Phân biệt mã hợp đồng HTTP với việc thực tế vẫn chặn ghi, xem response và bảng sau thao tác.','Areas/HocTap/Services/TinNhanService.cs'),
 'IT-088':('Kỳ vọng TC chưa khớp ánh xạ lỗi lịch','Batch trùng bị409, Excel yêu cầu422. LichHocController.ThemLoi giữ Rules.ErrorStatus vào Response.StatusCode; UiResponseFilter giữ status>=400. Không coi chênh mã này là bằng chứng batch đã commit một phần.','Areas/HocTap/Controllers/LichHocController.cs:12'),
 'IT-108':('Sai lệch mã từ chối của form nộp bài','Một DL bị422 thay vì403 như Excel; cần xem ModelState và nhánh fallback ChiTiet trong BaiTapController.Nop/UiResponseFilter. Không tự kết luận có nộp bài trái quyền; query dữ liệu và giữ oracle theo Excel.','Areas/HocTap/Controllers/BaiTapController.cs:104')}
evidence=out/'Evidence';proofs=list(evidence.glob('*.json'))
runstart=datetime.fromisoformat(trx.getroot().find(tag('Times')).get('start').replace('Z','+00:00')).timestamp()
proofs=[p for p in proofs if p.stat().st_mtime>=runstart-2]
for c in plan['cases']:
 r=raw[c['id']];outcome=r.get('outcome');msg=r.findtext('t:Output/t:ErrorInfo/t:Message',default='',namespaces=ns);stack=r.findtext('t:Output/t:ErrorInfo/t:StackTrace',default='',namespaces=ns)
 links=[]
 start=r.get('startTime','');end=r.get('endTime','')
 for p in proofs:
  if p.name.startswith(c['id']+'-'):
   try:
    z=json.loads(p.read_text(encoding='utf8'))
    # Only this final project run writes this Evidence directory.
    links.append(str(p.relative_to(out)))
   except Exception:pass
 if outcome=='NotExecuted':status='Chưa thực thi';rawstatus='Skip';reason='Chưa triển khai script, không phải đã kiểm hoặc lỗi code.'
 elif outcome=='Failed':
  status='Fail';rawstatus='Fail'
  if c['id'] in json.loads((src/'implementation.json').read_text(encoding='utf-8-sig'))['pending']:
   reason='Runner đã gọi test method nhưng dispatcher chưa có script cho scenario này; assertion nghiệp vụ chưa được thực hiện. Đây là Fail của độ đầy đủ bộ test, chưa phải kết luận lỗi ứng dụng.'
  else: reason=notes.get(c['id'],('Cần phân tích','Xem assertion/exception và đối chiếu dữ liệu chuẩn bị; không gán lỗi ứng dụng chỉ theo màu đỏ.',''))[1]
 elif c['id'] in gaps:status='Chưa đủ phạm vi TC';rawstatus='Pass';reason=gaps[c['id']]
 else:status='Pass';rawstatus='Pass';reason='Các assertion đã triển khai đạt; xem script và bằng chứng. Không suy rộng sang các TC hoặc nhánh chưa thực thi.'
 depstate=', '.join(x+':'+raw[x].get('outcome','?') for x in c['dependencyIds'])
 rows.append(dict(id=c['id'],group=c['group'],title=c['title'],stage=c['stage'],order=c['executionOrder'],status=status,scriptResult=rawstatus,scopeGap=gaps.get(c['id'],''),expected=c['expected'],actual=msg or ('Assertion script đạt.' if rawstatus=='Pass' else 'xUnit Skip, chưa gọi khớp nối.'),reason=reason,classification=notes.get(c['id'],('Cần phân tích','',''))[0] if status=='Fail' else '',applicationLocation=notes.get(c['id'],('','',''))[2],stack=stack,evidence=links,duration=r.get('duration'),startTime=start,endTime=end,dependencies=depstate))
assert len({r['id'] for r in rows})==len(plan['cases'])
counts=dict(collections.Counter(r['status'] for r in rows));rawcounts=dict(collections.Counter(r['scriptResult'] for r in rows))
time=datetime.now(timezone(timedelta(hours=7))).strftime('%d/%m/%Y %H:%M:%S +07:00')
hashes={}
for base in ['Areas','Controllers','Data','Infrastructure','Models','ViewModels']:
 for f in (root/base).rglob('*.cs'):hashes[str(f.relative_to(root))]=hashlib.sha256(f.read_bytes()).hexdigest()
for name in ['Program.cs','EduConnect.Mvc.csproj','appsettings.json','Database/00_EduConnect_Full.sql']:
 f=root/name;hashes[name]=hashlib.sha256(f.read_bytes()).hexdigest()
(out/'Source-SHA256.json').write_text(json.dumps(hashes,indent=2,ensure_ascii=False),encoding='utf8')
beforefile=out/'SourceBefore.json'
normalize=lambda s:str(pathlib.PureWindowsPath(s)).replace('\\','/').lower()
before={normalize(k):v for k,v in json.loads(beforefile.read_text(encoding='utf-8-sig')).items()} if beforefile.exists() else None
changed=[k for k,v in hashes.items() if before is not None and before.get(normalize(k))!=v]
summary=dict(time=time,total=len(plan['cases']),executed=len(raw),implemented=len(json.loads((src/'implementation.json').read_text(encoding='utf-8-sig'))['implemented']),rawScriptResults=rawcounts,testCaseConclusions=counts,database='EduConnect_Verification_Integration_20261125_v2',applicationSourceModified=bool(changed) if before is not None else None,changedApplicationFiles=changed,limitations=f"Lượt này chạy {len(plan['cases'])} TC; kết quả kết luận {counts}, kết quả xUnit {rawcounts}. Pass chỉ áp dụng cho assertion hiện có; không đo coverage code. TestServer báo không hỗ trợ IHttpMaxRequestBodySizeFeature/IHttpRequestBodySizeFeature; lượt này chưa chứng minh giới hạn kích thước request của máy chủ Kestrel thật.",rows=rows)
(out/'KetQua.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf8')
def clean(s):return str(s).replace('|','\\|').replace('\n','<br>').replace('\r','')
def report(rs,title):
 co=dict(collections.Counter(r['status'] for r in rs));rc=dict(collections.Counter(r['scriptResult'] for r in rs));text=[f'# {title}','',f'Thời điểm: {time}.',f'Database riêng: `{summary["database"]}`. Ứng dụng EduConnect1; SQL Server thật; không sửa code ứng dụng.','',f'Kết luận TC: {co}. Kết quả script xUnit: {rc}.',
 '','**Kết luận Pass chỉ áp dụng cho các assertion đã triển khai; TC thiếu assertion được đánh dấu riêng. Ca xUnit Skip được ghi là chưa thực thi.** Thứ tự Bottom-up trong Manifest.json; đây là lượt chẩn đoán, không chứng minh điều kiện phát hành khi các ca tiên quyết Fail/chưa chạy.','','| Mã TC | Tình huống | Kết luận TC | Script | Phạm vi/kết luận |','|---|---|---|---|---|']
 for r in rs:text.append('| '+' | '.join(map(clean,[r['id'],r['title'],r['status'],r['scriptResult'],r['scopeGap'] or r['reason']]))+' |')
 for r in rs:
  if r['status']!='Fail':continue
  text+=['',f'## {r["id"]} — {r["title"]}','','Phân loại: '+r['classification']+'.','', '**Kỳ vọng:** '+r['expected'],'','**Quan sát/assertion thực tế:**','```text',r['actual'],'```','', '**Ý nghĩa:** '+r['reason']]
  if r['applicationLocation']:text+=['','Vị trí cần xem trong ứng dụng: `'+r['applicationLocation']+'`. Đây là vị trí luồng liên quan, không tự coi mọi dòng này là nguyên nhân đã chứng minh.']
  text+=['','Script/stack thực thi:','```text',r['stack'],'```','', 'Bằng chứng: '+(', '.join('`'+p+'`' for p in r['evidence']) or 'TRX; kiểm tra lỗi trước khi fixture hoàn tất.')]
 text+=['','## Giới hạn của lượt thực thi','',summary['limitations']]
 return '\n'.join(text)+'\n'
(out/'KetQua.md').write_text(report(rows,f'Kết quả Integration — bộ {len(plan["cases"])} TC'),encoding='utf8')
with (out/'KetQua.csv').open('w',encoding='utf-8-sig',newline='') as f:
 writer=csv.DictWriter(f,fieldnames=['id','title','status','scriptResult','scopeGap','reason','actual','applicationLocation']);writer.writeheader();writer.writerows({k:r[k] for k in writer.fieldnames} for r in rows)
for gid,g in groups.items():
 folder=out/g['sheet'];folder.mkdir(exist_ok=True);rs=[r for r in rows if r['group']==gid]
 (folder/'KetQua.md').write_text(report(rs,g['title']),encoding='utf8');(folder/'KetQua.json').write_text(json.dumps(rs,ensure_ascii=False,indent=2),encoding='utf8')
 # Group-specific TRX retains the actual result, test definition and execution entries.
 subset=copy.deepcopy(trx.getroot());wanted={r['id'] for r in rs}
 for parentname,childname,attr in [('Results','UnitTestResult','testName'),('TestDefinitions','UnitTest','name')]:
  parent=subset.find(tag(parentname))
  if parent is not None:
   for x in list(parent):
    m=re.search(r'IT-\d{3}',x.get(attr,''))
    if not m or m.group() not in wanted:parent.remove(x)
 results=subset.find(tag('Results'));testids={x.get('testId') for x in results}
 entries=subset.find(tag('TestEntries'))
 if entries is not None:
  for x in list(entries):
   if x.get('testId') not in testids:entries.remove(x)
 counters=subset.find('.//'+tag('Counters'))
 if counters is not None:
  outcomes=collections.Counter(x.get('outcome') for x in results);counters.set('total',str(len(rs)));counters.set('executed',str(outcomes['Passed']+outcomes['Failed']));counters.set('passed',str(outcomes['Passed']));counters.set('failed',str(outcomes['Failed']));counters.set('notExecuted',str(outcomes['NotExecuted']))
 resultsummary=subset.find(tag('ResultSummary'))
 if resultsummary is not None:resultsummary.set('outcome','Failed' if any(r['scriptResult']=='Fail' for r in rs) else 'Completed')
 E.ElementTree(subset).write(folder/'Integration.trx',encoding='utf-8',xml_declaration=True)
print(json.dumps({k:v for k,v in summary.items() if k!='rows'},ensure_ascii=False))
