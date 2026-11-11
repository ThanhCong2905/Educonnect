const { chromium } = require('playwright');
const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const crypto = require('node:crypto');
const database = process.env.EDU_FULL_TEST_DATABASE;
assert.ok(database?.startsWith('EduConnect_Verification_'));
const base = process.env.EDU_FULL_TEST_URL || 'http://127.0.0.1:5298';
const suffix = crypto.randomUUID().replaceAll('-', '');
const results = [];
const output = '.codex-local/fullver0-results';
fs.mkdirSync(output, { recursive: true });
function sql(q) { return execFileSync('sqlcmd', ['-S','lpc:CONGVJP','-d',database,'-E','-C','-I','-b','-h','-1','-w','65535','-f','65001','-Q','SET NOCOUNT ON; '+q], { encoding:'utf8' }).trim(); }
function rows(q) { return JSON.parse(sql(q+' FOR JSON PATH')); }
async function check(name, action) {
    try { await action(); results.push({ name, status:'Pass' }); }
    catch (e) { results.push({ name, status:'Fail', error:e.stack }); }
    fs.writeFileSync(output+'/results.json',JSON.stringify({database,base,results},null,2));
    console.log(results.at(-1).status,name);
}
async function csrf(c,path) {
    const r=await c.get(base+path); assert.equal(r.status(),200,path);
    const m=(await r.text()).match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/);
    assert.ok(m,path);return m[1];
}
async function post(c,path,data,source='/TaiKhoan/DangNhap') {
    return c.post(base+path,{form:{...data,__RequestVerificationToken:await csrf(c,source)},maxRedirects:0});
}
const version=(table,id)=>Buffer.from(sql(`SELECT RowVersion FROM ${table} WHERE Id='${id}'`).replace(/^0x/,''),'hex').toString('base64');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 const users={};
 try {
  for(const [key,role]of [['student','HocSinh'],['parent','PhuHuynh'],['other','HocSinh'],['tutor','GiaSu']]) {
   const context=await browser.newContext();const c=context.request;
   const email=`fv-${key}-${suffix}@example.test`,password='Học cùng EduConnect 2026!';
   const register=await post(c,'/TaiKhoan/DangKy',{HoTen:`Kiểm thử ${key}`,Email:email,VaiTro:role,MatKhau:password,XacNhanMatKhau:password},'/TaiKhoan/DangKy');
   assert.equal(register.status(),302,await register.text());
   const login=await post(c,'/TaiKhoan/DangNhap',{Email:email,MatKhau:password});assert.equal(login.status(),302,await login.text());
   users[key]={context,c,email,id:sql(`SELECT Id FROM AspNetUsers WHERE Email='${email}'`)};
  }
  const {student,parent,other,tutor}=users;
  await check('Trang liên kết hiển thị cho tài khoản hợp lệ',async()=>assert.equal((await parent.c.get(base+'/HocTap/LienKetTaiKhoan/Index')).status(),200));
  await check('Học sinh không tự gửi đề nghị như phụ huynh',async()=>assert.equal((await post(student.c,'/HocTap/LienKetTaiKhoan/GuiDeNghi',{email:other.email},'/HocTap/LienKetTaiKhoan/Index')).status(),403));
  let link;
  await check('Phụ huynh gửi đề nghị, chưa tự cấp liên kết',async()=>{
   const r=await post(parent.c,'/HocTap/LienKetTaiKhoan/GuiDeNghi',{email:student.email},'/HocTap/LienKetTaiKhoan/Index');assert.equal(r.status(),302,await r.text());
   link=rows(`SELECT Id,TrangThai FROM LienKetPhuHuynhHocSinh WHERE PhuHuynhId='${parent.id}' AND HocSinhId='${student.id}'`)[0];assert.equal(link.TrangThai,'ChoXacNhan');
  });
  await check('Người khác không phản hồi đề nghị',async()=>assert.equal((await post(other.c,'/HocTap/LienKetTaiKhoan/PhanHoi',{id:link.Id,dongY:'true',rowVersion:version('LienKetPhuHuynhHocSinh',link.Id)},'/HocTap/LienKetTaiKhoan/Index')).status(),404));
  await check('Học sinh xác nhận liên kết',async()=>{
   assert.equal((await post(student.c,'/HocTap/LienKetTaiKhoan/PhanHoi',{id:link.Id,dongY:'true',rowVersion:version('LienKetPhuHuynhHocSinh',link.Id)},'/HocTap/LienKetTaiKhoan/Index')).status(),302);
   assert.equal(sql(`SELECT TrangThai FROM LienKetPhuHuynhHocSinh WHERE Id='${link.Id}'`),'DaLienKet');
  });
  await check('Không tạo liên kết trùng',async()=>assert.equal((await post(parent.c,'/HocTap/LienKetTaiKhoan/GuiDeNghi',{email:student.email},'/HocTap/LienKetTaiKhoan/Index')).status(),409));
  const subject=Number(sql('SELECT MIN(Id) FROM MonHoc WHERE DangSuDung=1'));
  const needData=i=>({'MonHocIds[0]':subject,MonHocId:subject,KhoiLop:'Lớp 10',MucTieu:'Mục tiêu kiểm thử '+i,SoBuoiTuan:1,HinhThucHoc:'TrucTuyen',HocPhiDuKien:i===0?20000:1000000,DonViHocPhi:'VND/Gio',BanNhap:'false','LichRanh[0].ThuTrongTuan':1,'LichRanh[0].GioBatDau':'19:00','LichRanh[0].GioKetThuc':'20:00'});
  const firstPost={...needData(0),MaThaoTac:crypto.randomUUID()};
  await check('Gửi đồng thời cùng biểu mẫu chỉ ghi một nhu cầu và một lượt đăng',async()=>{
   const replies=await Promise.all([1,2].map(()=>post(student.c,'/NhuCau/NhuCau/TaoNhuCauHoc',firstPost,'/NhuCau/NhuCau/Tao')));
   for(const reply of replies) assert.equal(reply.status(),302,await reply.text());
   assert.equal(Number(sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE ChuSoHuuId='${student.id}'`)),1);
   assert.equal(Number(sql(`SELECT COUNT(*) FROM LuotDangNhuCau WHERE TaiKhoanId='${student.id}'`)),1);
  });
  await check('Nhu cầu trùng hoàn toàn bị chặn dù chưa hết hạn mức',async()=>{
   const r=await post(student.c,'/NhuCau/NhuCau/TaoNhuCauHoc',{...firstPost,MaThaoTac:crypto.randomUUID(),MucTieu:'  '+firstPost.MucTieu+'  '},'/NhuCau/NhuCau/Tao');
   assert.ok((await r.text()).includes('validation-summary-errors'));
   assert.equal(Number(sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE ChuSoHuuId='${student.id}'`)),1);
  });
  await check('Giá 20k và 1 triệu hợp lệ được đăng; tối đa 3 đang mở',async()=>{
   for(let i=1;i<3;i++){const r=await post(student.c,'/NhuCau/NhuCau/TaoNhuCauHoc',needData(i),'/NhuCau/NhuCau/Tao');assert.equal(r.status(),302,await r.text());}
   assert.equal(Number(sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE ChuSoHuuId='${student.id}' AND TrangThai='DangMo'`)),3);
  });
  await check('Nhu cầu thứ tư bị chặn',async()=>{const r=await post(student.c,'/NhuCau/NhuCau/TaoNhuCauHoc',needData(4),'/NhuCau/NhuCau/Tao');assert.ok([200,400,409].includes(r.status()));assert.ok((await r.text()).includes('validation-summary-errors'));assert.equal(Number(sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE ChuSoHuuId='${student.id}'`)),3);});
  await check('Xác nhận học phí ngay trong biểu mẫu ở màn hình hẹp',async()=>{
   const page=await other.context.newPage();
   await page.setViewportSize({width:390,height:844});
   await page.goto(base+'/NhuCau/NhuCau/Index');
   await page.getByRole('link',{name:'+ Tạo nhu cầu',exact:true}).click();
   const modal=page.locator('#workspace-modal');
   await modal.locator('input[name="MonHocIds"]').first().check();
   await modal.locator('[name="MucTieu"]').fill('Kiểm thử biểu mẫu học phí');
   await modal.locator('[name="HocPhiDuKien"]').fill('20000');
   await modal.locator('[name="DonViHocPhi"]').selectOption('VND/Gio');
   await modal.getByRole('button',{name:'Đăng nhu cầu',exact:true}).click();
   const confirmation=modal.locator('[data-action-confirmation]');
   assert.match(await confirmation.innerText(),/20[.,]000/);
   await confirmation.getByRole('button',{name:'Kiểm tra lại'}).click();
   assert.equal(await modal.locator('[name="HocPhiDuKien"]').inputValue(),'20000');
   await modal.getByRole('button',{name:'Đăng nhu cầu',exact:true}).click();
   await confirmation.getByRole('button',{name:'Xác nhận',exact:true}).click();
   await page.getByText('Kiểm thử biểu mẫu học phí',{exact:true}).first().waitFor();
   assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=window.innerWidth));
   await page.close();
  });

  const contact=crypto.randomUUID(),request=crypto.randomUUID(),lop=crypto.randomUUID();
  sql(`DECLARE @Now datetimeoffset(0)=SYSDATETIMEOFFSET();
   INSERT HoSoGiaSu(GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet,DangHienThi,DayTrucTuyen,DayTrucTiep) VALUES('${tutor.id}',N'Đại học',N'Toán',N'3 năm',N'Kiểm thử',N'Hà Nội','DaDuyet',1,1,0);
   INSERT HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) SELECT Id,${subject},N'Lớp 10',150000,'VND/Gio' FROM HoSoGiaSu WHERE GiaSuId='${tutor.id}';
   INSERT YeuCauLienHe(Id,GiaSuId,BenHocId,NguoiGuiId,LoiNhan,TrangThai,GuiLuc,NguoiPhanHoiId,PhanHoiLuc) VALUES('${contact}','${tutor.id}','${student.id}','${student.id}',N'Trao đổi','ChapNhan',@Now,'${tutor.id}',@Now);
   INSERT YeuCauKetNoiLopHoc(Id,GiaSuId,BenHocId,NguoiGuiId,LoiNhan,TrangThai,GuiLuc,HetHanLuc,TenLopDuKien,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,BatDauHocLuc,HinhThucHoc,HocPhi,DonViHocPhi,LienHeId,NguoiDeXuatId,DeXuatLuc) VALUES('${request}','${tutor.id}','${student.id}','${tutor.id}',N'Đề xuất','DaChapNhan',@Now,DATEADD(hour,72,@Now),N'Lớp kiểm thử',${subject},N'Ôn tập',1,60,DATEADD(day,1,@Now),'TrucTuyen',150000,'VND/Gio','${contact}','${tutor.id}',@Now);
   INSERT LopHoc(Id,YeuCauKetNoiId,GiaSuId,BenHocId,TenLop,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,HocPhi,TrangThai,DonViHocPhi,TenNguoiHoc,HocSinhLienKetId) VALUES('${lop}','${request}','${tutor.id}','${student.id}',N'Lớp kiểm thử',${subject},N'Ôn tập',1,60,'TrucTuyen',150000,'DangHoatDong','VND/Gio',N'Học sinh kiểm thử','${student.id}');`);
  const ongoing=crypto.randomUUID(),future=crypto.randomUUID();
  sql(`INSERT LichHoc(Id,LopHocId,TieuDe,BatDauLuc,KetThucLuc,TrangThai) VALUES('${ongoing}','${lop}',N'Buổi đang học',DATEADD(minute,-10,SYSDATETIMEOFFSET()),DATEADD(minute,50,SYSDATETIMEOFFSET()),'DaLenLich'),('${future}','${lop}',N'Buổi sắp tới',DATEADD(day,1,SYSDATETIMEOFFSET()),DATEADD(minute,60,DATEADD(day,1,SYSDATETIMEOFFSET())),'DaLenLich');`);
  const lesson=id=>sql(`SELECT Id FROM BuoiHoc WHERE LichHocId='${id}'`);
  await check('Lên lịch tạo buổi dự kiến, không gán có mặt',async()=>{assert.equal(Number(sql(`SELECT COUNT(*) FROM BuoiHoc b JOIN LichHoc l ON l.Id=b.LichHocId WHERE l.LopHocId='${lop}' AND b.TrangThai='DuKien' AND b.GhiNhanLuc IS NULL AND b.TrangThaiThamDuGiaSu IS NULL`)),2);});
  await check('Chi tiết buổi và form giao bài hoạt động',async()=>{assert.equal((await tutor.c.get(base+'/HocTap/LichHoc/Buoi/'+ongoing)).status(),200);assert.equal((await tutor.c.get(base+`/HocTap/BaiTap/Giao/${lop}?buoi=${lesson(ongoing)}`)).status(),200);});
  await check('Học sinh không bắt đầu buổi thay gia sư',async()=>assert.equal((await post(student.c,'/HocTap/LichHoc/BatDauBuoi',{id:ongoing,rowVersion:version('BuoiHoc',lesson(ongoing))},'/HocTap/LichHoc/Buoi/'+ongoing)).status(),403));
  await check('Gia sư bắt đầu buổi trong khung giờ',async()=>{const r=await post(tutor.c,'/HocTap/LichHoc/BatDauBuoi',{id:ongoing,rowVersion:version('BuoiHoc',lesson(ongoing))},'/HocTap/LichHoc/Buoi/'+ongoing);assert.equal(r.status(),302,await r.text());assert.equal(sql(`SELECT TrangThai FROM BuoiHoc WHERE LichHocId='${ongoing}'`),'DangDienRa');});
  const taskData={LopHocId:lop,BuoiHocId:lesson(ongoing),TieuDe:'Bài tập buổi đang học',NoiDung:'Nội dung kiểm thử',HanNopLuc:'2026-12-20T21:00',LuuNhap:'false'};
  await check('Giao bài gắn buổi đang diễn ra',async()=>{const r=await post(tutor.c,'/HocTap/BaiTap/Giao',taskData,`/HocTap/BaiTap/Giao/${lop}?buoi=${lesson(ongoing)}`);assert.equal(r.status(),302,await r.text());assert.equal(Number(sql(`SELECT COUNT(*) FROM BaiTapGiao WHERE BuoiHocId='${lesson(ongoing)}' AND TrangThai='DaGiao'`)),1);});
  await check('Không tạo bài rời buổi',async()=>{const r=await post(tutor.c,'/HocTap/BaiTap/Giao',{...taskData,BuoiHocId:''},`/HocTap/BaiTap/Giao/${lop}`);assert.equal(r.status(),400);});
  await check('Nghỉ chưa có lịch bù vẫn lưu được',async()=>{const r=await post(tutor.c,'/HocTap/LichHoc/ThongBaoNghi',{id:future,reason:'Gia sư bận, chưa chốt ngày bù',canHocBu:'true',rowVersion:version('BuoiHoc',lesson(future))},'/HocTap/LichHoc/Nghi/'+future);assert.equal(r.status(),302,await r.text());assert.equal(sql(`SELECT TrangThai FROM BuoiHoc WHERE LichHocId='${future}'`),'DaNghi');assert.equal(Number(sql(`SELECT COUNT(*) FROM LichHoc WHERE LichGocId='${future}'`)),0);});
  await check('Mời tài khoản ngoài liên kết bị chặn',async()=>assert.equal((await post(student.c,'/HocTap/LopHoc/DeNghi',{id:lop,email:other.email},'/HocTap/LopHoc/ThanhVien/'+lop)).status(),403));
  await check('Mời phụ huynh đã liên kết không tự cấp quyền lớp',async()=>{const r=await post(student.c,'/HocTap/LopHoc/DeNghi',{id:lop,email:parent.email},'/HocTap/LopHoc/ThanhVien/'+lop);assert.equal(r.status(),302,await r.text());assert.equal((await parent.c.get(base+'/HocTap/LopHoc/ChiTiet/'+lop,{headers:{'X-Requested-With':'XMLHttpRequest'}})).status(),403);});
  const member=sql(`SELECT Id FROM ThanhVienLopHoc WHERE LopHocId='${lop}' AND TaiKhoanId='${parent.id}'`);
  await check('Duyệt trước khi người nhận đồng ý bị chặn',async()=>assert.equal((await post(tutor.c,'/HocTap/LopHoc/Duyet',{id:lop,member,action:'DaDuyet',permission:'Xem',rowVersion:version('ThanhVienLopHoc',member)},'/HocTap/LopHoc/ThanhVien/'+lop)).status(),409));
  await check('Người nhận đồng ý, gia sư duyệt mới truy cập lớp',async()=>{
   assert.equal((await post(parent.c,'/HocTap/LopHoc/PhanHoiLoiMoi',{id:member,agree:'true',rowVersion:version('ThanhVienLopHoc',member)},'/HocTap/LopHoc/LoiMoi/'+member)).status(),302);
   assert.equal((await post(tutor.c,'/HocTap/LopHoc/Duyet',{id:lop,member,action:'DaDuyet',permission:'Xem',rowVersion:version('ThanhVienLopHoc',member)},'/HocTap/LopHoc/ThanhVien/'+lop)).status(),302);
   assert.equal((await parent.c.get(base+'/HocTap/LopHoc/ChiTiet/'+lop)).status(),200);
  });
  const assignment=sql(`SELECT TOP(1) Id FROM BaiTapGiao WHERE BuoiHocId='${lesson(ongoing)}'`);
  await check('Nộp bài rồi chấm trong buổi; không nhận lần nộp sau chấm',async()=>{
   const payload={BaiTapGiaoId:assignment,MaThaoTac:crypto.randomUUID(),NoiDung:'Bài làm của học sinh'};
   assert.equal((await post(student.c,'/HocTap/BaiTap/Nop',payload,'/HocTap/BaiTap/ChiTiet/'+assignment)).status(),302);
   const submitted=sql(`SELECT Id FROM BaiTapNop WHERE BaiTapGiaoId='${assignment}'`);
   assert.equal((await post(tutor.c,'/HocTap/BaiTap/Cham',{BaiTapNopId:submitted,BaiTapGiaoId:assignment,Diem:'8.5',NhanXet:'Làm đúng, cần trình bày rõ hơn'},'/HocTap/BaiTap/ChiTiet/'+assignment)).status(),302);
   const r=await post(student.c,'/HocTap/BaiTap/Nop',{...payload,MaThaoTac:crypto.randomUUID()},'/HocTap/BaiTap/ChiTiet/'+assignment);
   assert.ok([400,409].includes(r.status()),await r.text());
   assert.equal(Number(sql(`SELECT COUNT(*) FROM BaiTapNop WHERE BaiTapGiaoId='${assignment}'`)),1);
  });
  await check('Tiến độ lấy điểm hiện hành và liên kết về bài đã chấm',async()=>{
   const r=await student.c.get(base+'/DanhGia/DanhGia/DanhSach?maLop='+lop);
   assert.equal(r.status(),200);
   const html=await r.text();
   assert.match(html,/8[.,]5/);
   assert.ok(html.toLowerCase().includes('/hoctap/baitap/chitiet/'+assignment.toLowerCase()));
   assert.match(html,/K&#x1EBF;t qu&#x1EA3; ch&#x1EA5;m g&#x1EA7;n &#x111;&#xE2;y|Kết quả chấm gần đây/);
  });
  const later=crypto.randomUUID();
  sql(`INSERT LichHoc(Id,LopHocId,TieuDe,BatDauLuc,KetThucLuc,TrangThai) VALUES('${later}','${lop}',N'Buổi chuẩn bị',DATEADD(day,4,SYSDATETIMEOFFSET()),DATEADD(minute,60,DATEADD(day,4,SYSDATETIMEOFFSET())),'DaLenLich');`);
  let draft;
  await check('Bài trước buổi học lưu nháp, người học không xem được',async()=>{
   const r=await post(tutor.c,'/HocTap/BaiTap/Giao',{...taskData,BuoiHocId:lesson(later),TieuDe:'Bài chuẩn bị',LuuNhap:'true'},'/HocTap/BaiTap/Giao/'+lop);
   assert.equal(r.status(),302,await r.text());
   draft=sql(`SELECT Id FROM BaiTapGiao WHERE BuoiHocId='${lesson(later)}'`);
   assert.equal((await student.c.get(base+'/HocTap/BaiTap/ChiTiet/'+draft)).status(),404);
   assert.equal((await post(tutor.c,'/HocTap/BaiTap/CongBo',{id:draft,rowVersion:version('BaiTapGiao',draft)},'/HocTap/BaiTap/ChiTiet/'+draft)).status(),409);
  });
  const moveData={'Id':later,'RowVersion':version('LichHoc',later),'TrangThai':'DaDieuChinh','Lich.TieuDe':'Buổi chuẩn bị đổi giờ','Lich.BatDauLuc':'2026-12-10T18:00','Lich.KetThucLuc':'2026-12-10T19:00'};
  await check('Sửa lịch giữ mã buổi, bài tập và lịch sử giờ cũ',async()=>{
   const original=lesson(later);
   const r=await post(tutor.c,'/HocTap/LichHoc/Sua',moveData,'/HocTap/LichHoc/Sua/'+later);
   assert.equal(r.status(),302,await r.text());assert.equal(lesson(later),original);
   assert.equal(sql(`SELECT BuoiHocId FROM BaiTapGiao WHERE Id='${draft}'`),original);
   assert.equal(Number(sql(`SELECT COUNT(*) FROM LichSuDieuChinhLich WHERE LichHocId='${later}'`)),1);
   assert.equal(sql(`SELECT TieuDe FROM BuoiHoc WHERE Id='${original}'`),'Buổi chuẩn bị đổi giờ');
   assert.equal((await post(tutor.c,'/HocTap/LichHoc/Sua',moveData,'/HocTap/LichHoc/Sua/'+later)).status(),409);
  });
  const makeupData={Id:future,RowVersion:version('LichHoc',future),'Lich.TieuDe':'Buổi bù','Lich.BatDauLuc':'2026-12-11T18:00','Lich.KetThucLuc':'2026-12-11T19:00'};
  await check('Xếp lịch bù đúng buổi nghỉ, không tạo hai lịch bù còn hiệu lực',async()=>{
   assert.equal((await post(tutor.c,'/HocTap/LichHoc/TaoBuoiBu',makeupData,'/HocTap/LichHoc/Sua/'+future)).status(),302);
   const r=await post(tutor.c,'/HocTap/LichHoc/TaoBuoiBu',{...makeupData,'Lich.BatDauLuc':'2026-12-12T18:00','Lich.KetThucLuc':'2026-12-12T19:00'},'/HocTap/LichHoc/Sua/'+future);
   assert.equal(r.status(),409,await r.text());
   const view=await tutor.context.newPage();await view.goto(base+'/HocTap/LichHoc/Buoi/'+future);assert.ok((await view.locator('main').innerText()).includes('Đã xếp lịch bù'));await view.close();
  });
  const past=crypto.randomUUID();
  sql(`INSERT LichHoc(Id,LopHocId,TieuDe,BatDauLuc,KetThucLuc,TrangThai) VALUES('${past}','${lop}',N'Buổi đã qua giờ',DATEADD(hour,-3,SYSDATETIMEOFFSET()),DATEADD(hour,-2,SYSDATETIMEOFFSET()),'DaLenLich');`);
  await check('Qua giờ không tự điểm danh; gia sư ghi nhận và lưu phiên bản sửa',async()=>{
   assert.equal(sql(`SELECT TrangThai FROM BuoiHoc WHERE LichHocId='${past}'`),'DuKien');
   const data={LichHocId:past,RowVersion:version('BuoiHoc',lesson(past)),KetQuaBuoiHoc:'DaDienRa',TrangThaiGiaSu:'CoMat',TrangThaiBenHoc:'CoMat',NoiDung:'Ôn tập kiến thức',NhanXet:'Học sinh hiểu bài'};
   const r=await post(tutor.c,'/HocTap/LichHoc/GhiNhan',data,'/HocTap/LichHoc/Buoi/'+past);assert.equal(r.status(),302,await r.text());
   assert.equal(sql(`SELECT TrangThai FROM BuoiHoc WHERE LichHocId='${past}'`),'DaHoc');
   assert.equal((await post(tutor.c,'/HocTap/LichHoc/GhiNhan',{...data,RowVersion:version('BuoiHoc',lesson(past)),NhanXet:'Bổ sung nhận xét'},'/HocTap/LichHoc/Buoi/'+past)).status(),302);
   assert.equal(Number(sql(`SELECT COUNT(*) FROM LichSuGhiNhanBuoiHoc WHERE BuoiHocId='${lesson(past)}'`)),1);
   assert.equal((await post(tutor.c,'/HocTap/LichHoc/GhiNhan',data,'/HocTap/LichHoc/Buoi/'+past)).status(),409);
  });
  await check('Bài cũ chưa gắn buổi vẫn xem được, chỉ gia sư chọn lại buổi',async()=>{
   const legacy=crypto.randomUUID();
   sql(`BEGIN TRAN;DISABLE TRIGGER dbo.tr_BaiTap_Buoi ON dbo.BaiTapGiao;INSERT BaiTapGiao(Id,LopHocId,NguoiGiaoId,TieuDe,NoiDung,HanNopLuc,TrangThai) VALUES('${legacy}','${lop}','${tutor.id}',N'Bài cũ chưa gắn buổi',N'Dữ liệu lịch sử',DATEADD(day,10,SYSDATETIMEOFFSET()),'DaGiao');ENABLE TRIGGER dbo.tr_BaiTap_Buoi ON dbo.BaiTapGiao;COMMIT;`);
   assert.ok((await (await tutor.c.get(base+'/HocTap/BaiTap/ChiTiet/'+legacy)).text()).includes('Chưa gắn buổi'));
   assert.equal((await student.c.get(base+'/HocTap/BaiTap/GanBuoi/'+legacy)).status(),403);
   const r=await post(tutor.c,'/HocTap/BaiTap/GanBuoi',{id:legacy,buoiHocId:lesson(past),rowVersion:version('BaiTapGiao',legacy)},'/HocTap/BaiTap/GanBuoi/'+legacy);assert.equal(r.status(),302,await r.text());
   assert.equal(sql(`SELECT BuoiHocId FROM BaiTapGiao WHERE Id='${legacy}'`),lesson(past));
  });
  await check('Phụ huynh được xem lớp chưa đồng nghĩa được nộp bài thay',async()=>{
   assert.equal((await post(parent.c,'/HocTap/BaiTap/Nop',{BaiTapGiaoId:assignment,MaThaoTac:crypto.randomUUID(),NoiDung:'Nộp thay'},'/HocTap/BaiTap/ChiTiet/'+assignment)).status(),403);
  });

  await check('Quyền chat và nộp thay được cấp riêng, lịch sử cũ giữ riêng',async()=>{
   const room=crypto.randomUUID(),privateRoom=crypto.randomUUID();
   sql(`INSERT CuocHoiThoai(Id,LoaiHoiThoai,GiaSuId,BenHocId,LopHocId) VALUES('${room}','TheoLop','${tutor.id}','${student.id}','${lop}'); INSERT CuocHoiThoai(Id,LoaiHoiThoai,GiaSuId,BenHocId,LienHeId) VALUES('${privateRoom}','TruocKetNoi','${tutor.id}','${student.id}','${contact}');
   DECLARE @approved datetimeoffset(7)=(SELECT DuyetLuc FROM ThanhVienLopHoc WHERE Id='${member}');
   INSERT TinNhan(CuocHoiThoaiId,NguoiGuiId,NoiDung,GuiLuc,MaThaoTac) VALUES('${room}','${tutor.id}',N'Tin trước khi duyệt',DATEADD(millisecond,-1,@approved),NEWID()),('${room}','${tutor.id}',N'Tin sau khi duyệt',DATEADD(millisecond,1,@approved),NEWID());`);
   const chat='/HocTap/TinNhan/DuLieu/'+room;
   const classChat=await (await parent.c.get(base+chat)).json();
   assert.deepEqual(classChat.items.map(x=>x.text),['Tin sau khi duyệt']);
   assert.equal(classChat.canSend,false);
   assert.equal((await parent.c.get(base+'/HocTap/TinNhan/DuLieu/'+privateRoom)).status(),403);
   assert.equal((await (await student.c.get(base+chat)).json()).items.length,2);
   const data={id:lop,member,action:'DaDuyet',permission:'ChinhSua'};
   const setPermission=flag=>post(tutor.c,'/HocTap/LopHoc/Duyet',{...data,choPhepNopBai:String(flag),rowVersion:version('ThanhVienLopHoc',member)},'/HocTap/LopHoc/ThanhVien/'+lop);
   assert.equal((await setPermission(false)).status(),302);
   const currentChat=await (await parent.c.get(base+chat)).json();
   assert.equal(currentChat.canSend,true);
   assert.deepEqual(currentChat.items.map(x=>x.text),['Tin sau khi duyệt']);
   const newAssignment=crypto.randomUUID();
   sql(`INSERT BaiTapGiao(Id,LopHocId,BuoiHocId,NguoiGiaoId,TieuDe,NoiDung,HanNopLuc,TrangThai) VALUES('${newAssignment}','${lop}','${lesson(past)}','${tutor.id}',N'Bài phụ huynh hỗ trợ',N'Nội dung',DATEADD(day,1,SYSDATETIMEOFFSET()),'DaGiao');`);
   const submit=()=>post(parent.c,'/HocTap/BaiTap/Nop',{BaiTapGiaoId:newAssignment,MaThaoTac:crypto.randomUUID(),NoiDung:'Nộp thay'},'/HocTap/BaiTap/ChiTiet/'+newAssignment);
   assert.equal((await submit()).status(),403);
   assert.equal((await setPermission(true)).status(),302);
   assert.equal((await submit()).status(),302);
   assert.equal(sql(`SELECT NguoiNopId FROM BaiTapNop WHERE BaiTapGiaoId='${newAssignment}'`),parent.id);
  });
  await check('Hủy liên kết thu hồi quyền thành viên',async()=>{assert.equal((await post(parent.c,'/HocTap/LienKetTaiKhoan/Huy',{id:link.Id,rowVersion:version('LienKetPhuHuynhHocSinh',link.Id)},'/HocTap/LienKetTaiKhoan/Index')).status(),302);assert.equal((await parent.c.get(base+'/HocTap/LopHoc/ChiTiet/'+lop,{headers:{'X-Requested-With':'XMLHttpRequest'}})).status(),403);});
  await check('Worker nhắc, đóng nhu cầu hết hạn và giữ lớp đang học',async()=>{
   const [remind,close]=rows(`SELECT TOP(2) Id FROM NhuCauHoc WHERE ChuSoHuuId='${student.id}' ORDER BY Id`);
   sql(`UPDATE NhuCauHoc SET XacNhanConNhuCauLuc=DATEADD(day,-31,SYSDATETIMEOFFSET()),NhacXacNhanLuc=NULL WHERE Id='${remind.Id}'; UPDATE NhuCauHoc SET XacNhanConNhuCauLuc=DATEADD(day,-39,SYSDATETIMEOFFSET()),NhacXacNhanLuc=DATEADD(day,-8,SYSDATETIMEOFFSET()) WHERE Id='${close.Id}';`);
   let status;
   for(let i=0;i<20;i++){
    status=sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE (Id='${remind.Id}' AND TrangThai='DangMo' AND NhacXacNhanLuc IS NOT NULL) OR (Id='${close.Id}' AND TrangThai='DaDong' AND LyDoDong='HetHieuLucTimKiem')`);
    if(status==='2')break;
    await new Promise(resolve=>setTimeout(resolve,2000));
   }
   assert.equal(status,'2');
   const confirm=await post(student.c,'/NhuCau/NhuCau/XacNhanConNhuCau',{id:remind.Id,rowVersion:version('NhuCauHoc',remind.Id)},'/NhuCau/NhuCau/Index');
   assert.equal(confirm.status(),302,await confirm.text());
   assert.equal(sql(`SELECT COUNT(*) FROM NhuCauHoc WHERE Id='${remind.Id}' AND NhacXacNhanLuc IS NULL AND TrangThai='DangMo'`),'1');
   assert.equal(sql(`SELECT TrangThai FROM LopHoc WHERE Id='${lop}'`),'DangHoatDong');
  });
  const page=await tutor.context.newPage();await page.goto(base+'/HocTap/LichHoc/Buoi/'+ongoing);await page.screenshot({path:output+'/buoi-desktop.png',fullPage:true});await page.setViewportSize({width:390,height:844});await page.screenshot({path:output+'/buoi-mobile.png',fullPage:true});
 } finally {await browser.close();}
 if(results.some(x=>x.status==='Fail'))process.exitCode=1;
})().catch(e=>{console.error(e);process.exitCode=1;});
