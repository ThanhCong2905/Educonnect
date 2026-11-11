const { chromium } = require('playwright');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const crypto = require('node:crypto');
const assert = require('node:assert/strict');

const database = process.env.EDU_FULL_TEST_DATABASE;
assert.ok(database?.startsWith('EduConnect_Verification_'));
const base = process.env.EDU_FULL_TEST_URL || 'http://127.0.0.1:5295';
const output = '.codex-local/full-system/results';
fs.mkdirSync(output, { recursive: true });
const suffix = crypto.randomUUID().replaceAll('-', '');
const password = 'Học cùng EduConnect 2026!';
const results = [];
function sql(query) {
    return execFileSync('sqlcmd', ['-S','lpc:CONGVJP','-d',database,'-E','-C','-I','-b','-h','-1','-w','65535','-f','65001','-Q','SET NOCOUNT ON; '+query], { encoding:'utf8' }).trim();
}
function version(table,id) { return Buffer.from(sql(`SELECT RowVersion FROM ${table} WHERE Id='${id}'`).replace(/^0x/,''),'hex').toString('base64'); }
function plain(html) { return html.replace(/&#x([0-9a-f]+);/gi,(_,n)=>String.fromCodePoint(parseInt(n,16))); }
async function check(name, action) {
    try { await action(); results.push({name,status:'Pass'}); }
    catch(error) { results.push({name,status:'Fail',error:error.stack}); }
    fs.writeFileSync(output+'/results.json', JSON.stringify({database,base,results},null,2));
    console.log(results.at(-1).status, name);
}
async function token(c, path='/TaiKhoan/HoSo') {
    const r=await c.get(base+path); assert.equal(r.status(),200,path+': '+(await r.text()).slice(0,150));
    const found=(await r.text()).match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/);
    assert.ok(found,path); return found[1];
}
async function post(c,path,data,source='/TaiKhoan/HoSo') {
    return c.post(base+path,{form:{...data,__RequestVerificationToken:await token(c,source)},maxRedirects:0});
}
async function login(user) {
    const r=await post(user.c,'/TaiKhoan/DangNhap',{Email:user.email,MatKhau:password},'/TaiKhoan/DangNhap');
    assert.equal(r.status(),302,await r.text());
}
async function main() {
    const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
    const people={};
    try {
        for(const [key,role] of [['tutor','GiaSu'],['student','HocSinh'],['parent','PhuHuynh'],['other','HocSinh'],['admin','HocSinh'],['admin2','HocSinh'],['limited','HocSinh']]) {
            const context=await browser.newContext(); const c=context.request;
            const email=`full-${key}-${suffix}@example.test`;
            const r=await post(c,'/TaiKhoan/DangKy',{HoTen:`Kiểm thử ${key}`,Email:email,VaiTro:role,MatKhau:password,XacNhanMatKhau:password},'/TaiKhoan/DangKy');
            assert.equal(r.status(),302,await r.text());
            const id=sql(`SELECT Id FROM AspNetUsers WHERE Email='${email}'`);
            people[key]={context,c,email,id};
            if(['admin','admin2','limited'].includes(key)) sql(`DELETE HocSinh WHERE TaiKhoanId='${id}'; UPDATE AspNetUserRoles SET RoleId=(SELECT Id FROM AspNetRoles WHERE Name='QuanTri') WHERE UserId='${id}';`);
        }
        execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',database,'-E','-C','-I','-b','-f','65001','-i','Database/14_DanhGiaBaoCaoQuanTri.sql']);
        sql(`DELETE AspNetUserClaims WHERE UserId='${people.limited.id}' AND ClaimType='EduConnect.AdminPermission' AND ClaimValue<>'XemNhatKy'`);
        for(const user of Object.values(people)) await login(user);
        const {tutor,student,parent,other,admin,admin2,limited}=people;
        const subject=Number(sql('SELECT MIN(Id) FROM MonHoc WHERE DangSuDung=1'));
        const contact=crypto.randomUUID(),request=crypto.randomUUID(),lop=crypto.randomUUID();
        sql(`DECLARE @Now datetimeoffset=SYSDATETIMEOFFSET();
        INSERT HoSoGiaSu(GiaSuId,TrinhDo,ChuyenMon,KinhNghiem,MoTa,KhuVuc,TrangThaiDuyet,DangHienThi,DayTrucTuyen,DayTrucTiep) VALUES('${tutor.id}',N'Đại học',N'Toán',N'3 năm',N'Giới thiệu kiểm thử',N'Hà Nội','DaDuyet',1,1,0);
        INSERT HoSoGiaSuMonDay(HoSoGiaSuId,MonHocId,PhamViDay,HocPhiDeXuat,DonViHocPhi) SELECT Id,${subject},N'Lớp 10',150000,'VND/Gio' FROM HoSoGiaSu WHERE GiaSuId='${tutor.id}';
        INSERT YeuCauLienHe(Id,GiaSuId,BenHocId,NguoiGuiId,LoiNhan,TrangThai,GuiLuc,NguoiPhanHoiId,PhanHoiLuc) VALUES('${contact}','${tutor.id}','${student.id}','${student.id}',N'Trao đổi','ChapNhan',@Now,'${tutor.id}',@Now);
        INSERT YeuCauKetNoiLopHoc(Id,GiaSuId,BenHocId,NguoiGuiId,LoiNhan,TrangThai,GuiLuc,HetHanLuc,TenLopDuKien,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,BatDauHocLuc,HinhThucHoc,HocPhi,DonViHocPhi,LienHeId,NguoiDeXuatId,DeXuatLuc) VALUES('${request}','${tutor.id}','${student.id}','${tutor.id}',N'Đề xuất','DaChapNhan',@Now,DATEADD(hour,72,@Now),N'Lớp phản hồi',${subject},N'Ôn tập',1,60,DATEADD(day,1,@Now),'TrucTuyen',150000,'VND/Gio','${contact}','${tutor.id}',@Now);
        INSERT LopHoc(Id,YeuCauKetNoiId,GiaSuId,BenHocId,TenLop,MonHocId,MucTieu,SoBuoiTuan,ThoiLuongPhut,HinhThucHoc,HocPhi,TrangThai,DonViHocPhi,TenNguoiHoc,HocSinhLienKetId) VALUES('${lop}','${request}','${tutor.id}','${student.id}',N'Lớp phản hồi',${subject},N'Ôn tập',1,60,'TrucTuyen',150000,'DangHoatDong','VND/Gio',N'Học sinh kiểm thử','${student.id}');`);
        const progress=`/DanhGia/DanhGia/DanhSach?maLop=${lop}`;
        const now=Date.now();
        const dotInput={MaLop:lop,Ten:'Đánh giá tháng đầu',TuNgay:new Date(now-86400000*30).toISOString(),DenNgay:new Date(now).toISOString(),HanGui:new Date(now+86400000*7).toISOString()};
        await check('Trang tiến độ hoạt động từ lớp học',async()=>assert.equal((await student.c.get(base+progress)).status(),200));
        await check('Người ngoài không xem tiến độ lớp',async()=>assert.equal((await other.c.get(base+progress)).status(),403));
        await check('Hai bên chính mở đợt phản hồi',async()=>assert.equal((await post(tutor.c,'/DanhGia/DanhGia/TaoDot',dotInput)).status(),302));
        const dot=sql(`SELECT Id FROM DotPhanHoi WHERE LopHocId='${lop}'`);
        await check('Không tạo hai đợt trùng cùng khoảng',async()=>{await post(student.c,'/DanhGia/DanhGia/TaoDot',dotInput);assert.equal(sql(`SELECT COUNT(*) FROM DotPhanHoi WHERE LopHocId='${lop}'`),'1');});
        await check('Người ngoài không gửi phản hồi lớp',async()=>{const r=await post(other.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'Nội dung không đúng quyền'});assert.equal(sql(`SELECT COUNT(*) FROM PhieuDanhGia WHERE LopHocId='${lop}'`),'0');assert.notEqual(r.status(),302);});
        await check('Gửi phản hồi Unicode và hài lòng tùy chọn',async()=>{assert.equal((await post(student.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'Em cần hướng dẫn thêm phần hàm số.'})).status(),302);assert.equal(sql(`SELECT NoiDung FROM PhieuDanhGia WHERE DotPhanHoiId='${dot}'`),'Em cần hướng dẫn thêm phần hàm số.');});
        const phieu=sql(`SELECT Id FROM PhieuDanhGia WHERE DotPhanHoiId='${dot}'`);
        const oldVersion=version('PhieuDanhGia',phieu);
        await check('Sửa phiếu có phiên bản và lưu lịch sử',async()=>{assert.equal((await post(student.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'Em đã hiểu hơn sau buổi trao đổi.',MucDoHaiLong:4,RowVersion:oldVersion})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM LichSuPhanHoi WHERE PhieuDanhGiaId='${phieu}'`),'1');});
        await check('Phiên bản cũ không ghi đè phản hồi',async()=>{const r=await post(student.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'Ghi đè sai',RowVersion:oldVersion});assert.notEqual(r.status(),302);assert.equal(sql(`SELECT NoiDung FROM PhieuDanhGia WHERE Id='${phieu}'`),'Em đã hiểu hơn sau buổi trao đổi.');});
        await check('Hai bên lưu quyết định riêng',async()=>{for(const u of [tutor,student])assert.equal((await post(u.c,'/DanhGia/DanhGia/QuyetDinh',{DotPhanHoiId:dot,QuyetDinh:'TiepTuc'})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM QuyetDinhSauPhanHoi WHERE DotPhanHoiId='${dot}'`),'2');});
        await check('Đóng đợt không nhận phiếu mới',async()=>{assert.equal((await post(tutor.c,'/DanhGia/DanhGia/DongDot',{id:dot,maLop:lop,rowVersion:version('DotPhanHoi',dot)})).status(),302);await post(tutor.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'Gửi sau đóng'});assert.equal(sql(`SELECT COUNT(*) FROM PhieuDanhGia WHERE DotPhanHoiId='${dot}'`),'1');});
        await check('Phản hồi dài quá 4000 ký tự không được ghi',async()=>{await post(student.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:dot,NoiDung:'a'.repeat(4001),RowVersion:version('PhieuDanhGia',phieu)});assert.equal(sql(`SELECT COUNT(*) FROM LichSuPhanHoi WHERE PhieuDanhGiaId='${phieu}'`),'1');});
        const linked=crypto.randomUUID();
        sql(`INSERT LienKetPhuHuynhHocSinh(Id,PhuHuynhId,HocSinhId,NguoiGuiId,TrangThai,GuiLuc,XacNhanLuc) VALUES('${linked}','${parent.id}','${student.id}','${parent.id}','DaLienKet',SYSDATETIMEOFFSET(),SYSDATETIMEOFFSET());
        INSERT ThanhVienLopHoc(LopHocId,TaiKhoanId,NguoiDeNghiId,TrangThaiDuyet,MucQuyen,DuyetLuc,DongYThamGiaLuc,LienKetId) VALUES('${lop}','${parent.id}','${student.id}','DaDuyet','Xem',SYSDATETIMEOFFSET(),SYSDATETIMEOFFSET(),'${linked}');`);
        await check('Thành viên xem được tiến độ nhưng không mở đợt',async()=>{assert.equal((await parent.c.get(base+progress)).status(),200);await post(parent.c,'/DanhGia/DanhGia/TaoDot',{...dotInput,Ten:'Không đủ quyền',DenNgay:new Date(now+1).toISOString()});assert.equal(sql(`SELECT COUNT(*) FROM DotPhanHoi WHERE LopHocId='${lop}'`),'1');});
        await check('Đợt thứ 21 phân trang và mở được form của đợt cũ',async()=>{for(let i=1;i<=20;i++)sql(`INSERT DotPhanHoi(Id,LopHocId,Ten,TuNgay,DenNgay,HanGui,NguoiTaoId,TrangThai) VALUES(NEWID(),'${lop}',N'Đợt trang ${i}',DATEADD(day,${i},SYSDATETIMEOFFSET()),DATEADD(day,${i+1},SYSDATETIMEOFFSET()),DATEADD(day,${i+2},SYSDATETIMEOFFSET()),'${tutor.id}','DangMo')`);const second=await student.c.get(base+progress+'&trang=2');assert.equal(second.status(),200);assert.match(plain(await second.text()),/Đánh giá tháng đầu/);assert.equal((await student.c.get(base+`/DanhGia/DanhGia/Gui?maLop=${lop}&dotPhanHoiId=${dot}`)).status(),200);});
        await check('Lớp đang học chưa đánh giá công khai',async()=>assert.equal((await tutor.c.get(base+`/DanhGia/DoiTac/Gui?maLop=${lop}`)).status(),409));
        sql(`INSERT LichHoc(LopHocId,TieuDe,BatDauLuc,KetThucLuc,TrangThai) VALUES('${lop}',N'Buổi đã học',DATEADD(hour,-3,SYSDATETIMEOFFSET()),DATEADD(hour,-2,SYSDATETIMEOFFSET()),'DaLenLich');
        DECLARE @slot uniqueidentifier=(SELECT TOP 1 Id FROM LichHoc WHERE LopHocId='${lop}');
        DECLARE @version binary(8)=(SELECT RowVersion FROM BuoiHoc WHERE LichHocId=@slot);
        EXEC dbo.usp_GhiNhanBuoiHoc @LichHocId=@slot,@GiaSuId='${tutor.id}',@KetQuaBuoiHoc='DaDienRa',@NhanXet=N'Đã hoàn thành buổi học',@TrangThaiGiaSu='CoMat',@TrangThaiBenHoc='CoMat',@NoiDung=N'Ôn tập hàm số',@RowVersionCu=@version;
        UPDATE LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='DongY' WHERE Id='${lop}';`);
        await check('Hai bên đánh giá sau kết thúc có buổi thực tế',async()=>{for(const u of [student,tutor])assert.equal((await post(u.c,'/DanhGia/DoiTac/Gui',{MaLop:lop,SoSao:5,NhanXet:'Đã trao đổi rõ ràng và phối hợp tốt.',XacNhanRiengTu:'true'})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM DanhGiaDoiTac WHERE LopHocId='${lop}'`),'2');});
        const review=sql(`SELECT Id FROM DanhGiaDoiTac WHERE LopHocId='${lop}' AND NguoiGuiId='${student.id}'`);
        await check('Đánh giá bên học chỉ mở đúng quyền',async()=>{assert.equal((await tutor.c.get(base+'/DanhGia/DoiTac/ChiTiet/'+student.id)).status(),200);assert.equal((await parent.c.get(base+'/DanhGia/DoiTac/ChiTiet/'+student.id)).status(),403);});
        await check('Đối tác phản hồi đánh giá và giữ bản cũ',async()=>{assert.equal((await post(tutor.c,'/DanhGia/DoiTac/PhanHoi',{DanhGiaId:review,NoiDung:'Cảm ơn bạn đã phản hồi.',RowVersion:version('DanhGiaDoiTac',review)})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM LichSuDanhGiaDoiTac WHERE DanhGiaId='${review}'`),'1');});
        await check('Thành viên bổ sung không đánh giá công khai thay bên chính',async()=>assert.equal((await parent.c.get(base+`/DanhGia/DoiTac/Gui?maLop=${lop}`)).status(),403));
        await check('Không nhận điểm sao ngoài 1 đến 5',async()=>{await post(student.c,'/DanhGia/DoiTac/Gui',{MaLop:lop,SoSao:6,NhanXet:'Nội dung kiểm thử hợp lệ.',XacNhanRiengTu:'true',RowVersion:version('DanhGiaDoiTac',review)});assert.equal(sql(`SELECT SoSao FROM DanhGiaDoiTac WHERE Id='${review}'`),'5');});
        const reportInput={MaThaoTac:crypto.randomUUID(),LoaiDoiTuong:'DanhGiaDoiTac',DoiTuongId:review,LyDo:'Cần xem xét nhận xét',MoTa:'Đề nghị quản trị kiểm tra lại nội dung nhận xét này.'};
        await check('Gửi báo cáo và gửi lại cùng mã chỉ lưu một lần',async()=>{for(let i=0;i<2;i++)assert.equal((await post(tutor.c,'/QuanTri/BaoCaoViPham/Gui',reportInput)).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM BaoCaoViPham WHERE MaThaoTac='${reportInput.MaThaoTac}'`),'1');});
        const report=sql(`SELECT Id FROM BaoCaoViPham WHERE MaThaoTac='${reportInput.MaThaoTac}'`);
        await check('Người ngoài và quản trị thiếu quyền không xem bằng chứng',async()=>{for(const u of [other,limited])assert.equal((await u.c.get(base+'/QuanTri/BaoCaoViPham/ChiTiet?maBaoCao='+report)).status(),403);});
        await check('Quản trị nhận xử lý báo cáo',async()=>assert.equal((await post(admin.c,'/QuanTri/BaoCaoViPham/NhanXuLy',{id:report,rowVersion:version('BaoCaoViPham',report)})).status(),302));
        await check('Quản trị khác không kết luận thay người nhận xử lý',async()=>assert.equal((await post(admin2.c,'/QuanTri/BaoCaoViPham/XuLy',{Id:report,RowVersion:version('BaoCaoViPham',report),KetQua:'XuLy',BienPhap:'AnNoiDung',KetLuan:'Đã xem xét bằng chứng.'})).status(),200));
        await check('Ẩn đánh giá và loại khỏi điểm tổng hợp',async()=>{assert.equal((await post(admin.c,'/QuanTri/BaoCaoViPham/XuLy',{Id:report,RowVersion:version('BaoCaoViPham',report),KetQua:'XuLy',BienPhap:'AnNoiDung',KetLuan:'Ẩn để bảo vệ thông tin riêng tư.'})).status(),302);const html=await (await other.c.get(base+'/DanhGia/DoiTac/ChiTiet/'+tutor.id)).text();assert.match(plain(html),/Chưa có đánh giá/);assert.equal(sql(`SELECT KetQua FROM BaoCaoViPham WHERE Id='${report}'`),'XuLy');});
        const measure=sql(`SELECT Id FROM BienPhapQuanTri WHERE BaoCaoId='${report}' AND Loai='AnNoiDung'`);
        await check('Khôi phục đánh giá bằng biện pháp có phiên bản',async()=>{assert.equal((await post(admin.c,'/QuanTri/QuanTri/GoBienPhap',{id:measure,reason:'Đã kiểm tra và đủ điều kiện công khai.',rowVersion:version('BienPhapQuanTri',measure)})).status(),302);assert.ok(sql(`SELECT GoLuc FROM BienPhapQuanTri WHERE Id='${measure}'`));});
        await check('Gỡ lại bằng phiên bản cũ bị từ chối',async()=>assert.equal((await post(admin.c,'/QuanTri/QuanTri/GoBienPhap',{id:measure,reason:'Thử lại',rowVersion:version('BienPhapQuanTri',measure)})).status(),409));
        await check('Danh sách quản trị, nhật ký, danh mục và duyệt hồ sơ mở được',async()=>{for(const path of ['/QuanTri/QuanTri/Index','/QuanTri/QuanTri/TaiKhoan','/QuanTri/QuanTri/XemNhatKy','/QuanTri/QuanTri/MonHoc','/QuanTri/HoSo/Index','/QuanTri/BaoCaoViPham/Index?quanTri=true'])assert.equal((await admin.c.get(base+path)).status(),200,path);});
        await check('Quản trị chỉ có quyền nhật ký không quản lý tài khoản',async()=>{assert.equal((await limited.c.get(base+'/QuanTri/QuanTri/XemNhatKy')).status(),200);assert.equal((await limited.c.get(base+'/QuanTri/QuanTri/TaiKhoan')).status(),403);});
        await check('Người bị báo cáo chỉ xem kết luận và được đề nghị xem lại',async()=>{assert.equal((await student.c.get(base+'/QuanTri/BaoCaoViPham/KetQua?id='+report)).status(),200);assert.equal((await post(student.c,'/QuanTri/BaoCaoViPham/BoSung',{id:report,text:'Tôi bổ sung căn cứ để quản trị xem lại.',rowVersion:version('BaoCaoViPham',report),xemLai:'true'})).status(),302);assert.equal(sql(`SELECT TrangThai FROM BaoCaoViPham WHERE Id='${report}'`),'ChoXuLy');assert.equal(sql(`SELECT COUNT(*) FROM LichSuXuLyBaoCao WHERE BaoCaoId='${report}' AND HanhDong='XuLy'`),'1');});
        const evidence=crypto.randomUUID();
        await check('Tải bằng chứng PDF qua form báo cáo',async()=>{
            const r=await other.c.post(base+'/QuanTri/BaoCaoViPham/Gui',{multipart:{MaThaoTac:evidence,LoaiDoiTuong:'TaiKhoan',DoiTuongId:tutor.id,LyDo:'Kiểm tra bằng chứng',MoTa:'Nội dung kèm bằng chứng để quản trị kiểm tra.',__RequestVerificationToken:await token(other.c),Files:{name:'bang-chung.pdf',mimeType:'application/pdf',buffer:Buffer.from('%PDF-1.4\nEvidence test\n%%EOF')}},maxRedirects:0});
            assert.equal(r.status(),302,await r.text());
            assert.equal(sql(`SELECT COUNT(*) FROM TepDinhKem f JOIN BaoCaoViPham b ON b.Id=f.BaoCaoViPhamId WHERE b.MaThaoTac='${evidence}'`),'1');
        });
        const evidenceFile=sql(`SELECT f.Id FROM TepDinhKem f JOIN BaoCaoViPham b ON b.Id=f.BaoCaoViPhamId WHERE b.MaThaoTac='${evidence}'`);
        await check('Bằng chứng chỉ tải bởi người gửi và quản trị được cấp quyền',async()=>{for(const u of [other,admin])assert.equal((await u.c.get(base+'/QuanTri/BaoCaoViPham/Tep/'+evidenceFile)).status(),200);for(const u of [student,limited])assert.equal((await u.c.get(base+'/QuanTri/BaoCaoViPham/Tep/'+evidenceFile)).status(),403);});
        await check('Tệp vượt 10 MB bị từ chối và không lưu báo cáo dở',async()=>{const operation=crypto.randomUUID();const r=await student.c.post(base+'/QuanTri/BaoCaoViPham/Gui',{multipart:{MaThaoTac:operation,LoaiDoiTuong:'TaiKhoan',DoiTuongId:other.id,LyDo:'Kiểm tra dung lượng',MoTa:'Nội dung đủ độ dài để kiểm tra tệp.',__RequestVerificationToken:await token(student.c),Files:{name:'qua-lon.pdf',mimeType:'application/pdf',buffer:Buffer.concat([Buffer.from('%PDF-'),Buffer.alloc(10000000)])}},maxRedirects:0});assert.notEqual(r.status(),302);assert.equal(sql(`SELECT COUNT(*) FROM BaoCaoViPham WHERE MaThaoTac='${operation}'`),'0');});
        await check('Danh mục chống ghi đè bằng RowVersion',async()=>{const name='Môn kiểm thử '+suffix;assert.equal((await post(admin.c,'/QuanTri/QuanTri/LuuMon',{TenMon:name,DangSuDung:'true'})).status(),302);const id=Number(sql(`SELECT Id FROM MonHoc WHERE TenMon=N'${name}'`));const rv=version('MonHoc',id);assert.equal((await post(admin.c,'/QuanTri/QuanTri/LuuMon',{Id:id,TenMon:name,DangSuDung:'false',RowVersion:rv})).status(),302);const retry=await post(admin.c,'/QuanTri/QuanTri/LuuMon',{Id:id,TenMon:name+' sai',DangSuDung:'true',RowVersion:rv});assert.notEqual(retry.status(),302);assert.equal(sql(`SELECT TenMon FROM MonHoc WHERE Id=${id}`),name);});
        sql(`UPDATE LopHoc SET TrangThai='DangHoatDong',KetThucLuc=NULL,CachKetThuc=NULL WHERE Id='${lop}'`);
        const stamp=id=>sql(`SELECT ConcurrencyStamp FROM AspNetUsers WHERE Id='${id}'`);
        await check('Khóa tài khoản tạm dừng lớp và thu hồi đăng nhập',async()=>{assert.equal((await post(admin.c,'/QuanTri/QuanTri/KhoaTaiKhoan',{id:student.id,lyDo:'Kiểm thử khóa tài khoản',concurrencyStamp:stamp(student.id)})).status(),302);assert.equal(sql(`SELECT TrangThai FROM LopHoc WHERE Id='${lop}'`),'TamDung');assert.equal((await student.c.get(base+'/TaiKhoan/HoSo',{maxRedirects:0})).status(),302);});
        await check('Hai nguyên nhân khóa được giữ riêng',async()=>{assert.equal((await post(admin.c,'/QuanTri/QuanTri/KhoaTaiKhoan',{id:tutor.id,lyDo:'Kiểm thử nguyên nhân khóa thứ hai',concurrencyStamp:stamp(tutor.id)})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM BienPhapQuanTri WHERE LoaiDoiTuong='LopHoc' AND DoiTuongId='${lop}' AND GoLuc IS NULL`),'2');});
        await check('Mở một tài khoản chưa tiếp tục lớp khi bên kia còn khóa',async()=>{const id=sql(`SELECT Id FROM BienPhapQuanTri WHERE Loai='KhoaTaiKhoan' AND DoiTuongId='${student.id}' AND GoLuc IS NULL`);assert.equal((await post(admin.c,'/QuanTri/QuanTri/GoBienPhap',{id,reason:'Đủ điều kiện mở khóa',rowVersion:version('BienPhapQuanTri',id)})).status(),302);assert.equal(sql(`SELECT TrangThai FROM LopHoc WHERE Id='${lop}'`),'TamDung');});
        await check('Gỡ đủ nguyên nhân mới tiếp tục lớp',async()=>{const id=sql(`SELECT Id FROM BienPhapQuanTri WHERE Loai='KhoaTaiKhoan' AND DoiTuongId='${tutor.id}' AND GoLuc IS NULL`);assert.equal((await post(admin.c,'/QuanTri/QuanTri/GoBienPhap',{id,reason:'Đủ điều kiện mở khóa',rowVersion:version('BienPhapQuanTri',id)})).status(),302);assert.equal(sql(`SELECT TrangThai FROM LopHoc WHERE Id='${lop}'`),'DangHoatDong');});
        await login(tutor); await login(student);
        await check('Mở khóa cho phép đăng nhập lại',async()=>assert.equal((await student.c.get(base+progress)).status(),200));
        await check('Thông tin cá nhân giữ tên và số điện thoại Unicode hợp lệ',async()=>{const r=await post(other.c,'/TaiKhoan/HoSo',{HoTen:'Nguyễn Minh An',SoDienThoai:'090 123 4567',ConcurrencyStamp:stamp(other.id)});assert.equal(r.status(),302);assert.equal(sql(`SELECT HoTen FROM AspNetUsers WHERE Id='${other.id}'`),'Nguyễn Minh An');});
        await check('Sửa tên gia sư đang công khai cần ẩn hồ sơ trước',async()=>{await post(tutor.c,'/TaiKhoan/HoSo',{HoTen:'Tên thay đổi khi công khai',ConcurrencyStamp:stamp(tutor.id)});assert.equal(sql(`SELECT HoTen FROM AspNetUsers WHERE Id='${tutor.id}'`),'Kiểm thử tutor');});
        await check('Nhật ký lưu thao tác quản trị thành công và thất bại',async()=>{assert.ok(Number(sql(`SELECT COUNT(*) FROM NhatKyQuanTri WHERE NguoiThucHienId='${admin.id}' AND KetQua='ThanhCong'`))>0);assert.ok(Number(sql(`SELECT COUNT(*) FROM NhatKyQuanTri WHERE NguoiThucHienId='${admin.id}' AND KetQua='KhongThanhCong'`))>0);});
        await check('Quản trị không thể tự khóa tài khoản đang thao tác',async()=>{const r=await post(admin.c,'/QuanTri/QuanTri/KhoaTaiKhoan',{id:admin.id,lyDo:'Thử khóa chính mình',concurrencyStamp:stamp(admin.id)});assert.equal(r.status(),400);assert.equal(sql(`SELECT TrangThai FROM AspNetUsers WHERE Id='${admin.id}'`),'HoatDong');});
        await check('Đổi mật khẩu thu hồi phiên cũ và đăng nhập bằng mật khẩu mới',async()=>{const newPassword='Mật khẩu mới đủ dài 2026!';const r=await post(other.c,'/TaiKhoan/DoiMatKhau',{MatKhauHienTai:password,MatKhau:newPassword,XacNhanMatKhau:newPassword});assert.equal(r.status(),302);assert.equal((await other.c.get(base+'/TaiKhoan/HoSo',{maxRedirects:0})).status(),302);const signIn=await post(other.c,'/TaiKhoan/DangNhap',{Email:other.email,MatKhau:newPassword},'/TaiKhoan/DangNhap');assert.equal(signIn.status(),302);});

        const noticesPrefix='notice-'+suffix;
        sql(`DECLARE @i int=0,@event uniqueidentifier; WHILE @i<21 BEGIN SET @event=NEWID(); INSERT SuKienChoXuLy(Id,LoaiSuKien,DuLieuToiThieu,TaoLuc,TrangThai) VALUES(@event,'TaiKhoan',N'{}',SYSDATETIMEOFFSET(),'HoanTat'); INSERT ThongBao(Id,NguoiNhanId,LoaiThongBao,DoiTuongId,NoiDungTomTat,TaoLuc,MaSuKien) VALUES(NEWID(),'${student.id}','TaiKhoan','${student.id}',N'${noticesPrefix}-'+CAST(@i AS nvarchar),DATEADD(second,@i,SYSDATETIMEOFFSET()),@event); SET @i+=1; END;`);
        await check('Thông báo phân trang đúng 20/21 mục và giữ bộ lọc',async()=>{const first=await student.c.get(base+'/Home/ThongBao?q='+noticesPrefix);assert.equal((await first.text()).match(/class="notice-item /g)?.length,20);const second=await student.c.get(base+'/Home/ThongBao?trang=2&q='+noticesPrefix);assert.equal((await second.text()).match(/class="notice-item /g)?.length,1);});
        await check('Đánh dấu tất cả chỉ thay thông báo của chính mình',async()=>{const before=sql(`SELECT COUNT(*) FROM ThongBao WHERE NguoiNhanId='${tutor.id}' AND DaDocLuc IS NULL`);assert.equal((await post(student.c,'/Home/DocTatCa',{})).status(),302);assert.equal(sql(`SELECT COUNT(*) FROM ThongBao WHERE NguoiNhanId='${student.id}' AND DaDocLuc IS NULL`),'0');assert.equal(sql(`SELECT COUNT(*) FROM ThongBao WHERE NguoiNhanId='${tutor.id}' AND DaDocLuc IS NULL`),before);assert.equal((await (await student.c.get(base+'/Home/ThongBao?state=DaDoc&q='+noticesPrefix)).text()).match(/class="notice-item /g)?.length,20);});
        await check('Thông báo mở trong modal, thao tác đọc giữ trang nền',async()=>{const p=await student.context.newPage();await p.goto(base+'/TaiKhoan/HoSo');await p.locator('a.notification').click();await p.locator('#notification-list').waitFor();const at=p.url();const mark=p.locator('[data-mark-all-notices] button');if(await mark.count())await mark.click();assert.equal(p.url(),at);assert.ok(await p.locator('#notification-list').isVisible());await p.keyboard.press('Escape');await p.close();});
        const profile=sql(`SELECT Id FROM HoSoGiaSu WHERE GiaSuId='${tutor.id}'`);
        await check('Quản trị thiếu quyền không mở chi tiết hồ sơ',async()=>assert.equal((await limited.c.get(base+'/QuanTri/HoSo/ChiTiet/'+profile)).status(),403));
        await check('Duyệt hồ sơ có phiên bản, yêu cầu sửa và từ chối lưu lý do',async()=>{for(const [approve,decision,state]of [['false','CanChinhSua','CanChinhSua'],['false','TuChoi','TuChoi'],['true','Duyet','DaDuyet']]){sql(`UPDATE HoSoGiaSu SET DangHienThi=0,TrangThaiDuyet='ChoDuyet' WHERE Id='${profile}'`);assert.equal((await post(admin.c,'/QuanTri/HoSo/Duyet',{id:profile,rowVersion:version('HoSoGiaSu',profile),approve,quyetDinh:decision,reason:'Cần kiểm tra thông tin chuyên môn.'})).status(),302);assert.equal(sql(`SELECT TrangThaiDuyet FROM HoSoGiaSu WHERE Id='${profile}'`),state);}assert.equal(sql(`SELECT DangHienThi FROM HoSoGiaSu WHERE Id='${profile}'`),'0');});
        await check('Đánh giá quá bảy ngày không sửa được',async()=>{sql(`UPDATE LopHoc SET TrangThai='DaKetThuc',KetThucLuc=SYSDATETIMEOFFSET(),CachKetThuc='DongY' WHERE Id='${lop}'; UPDATE DanhGiaDoiTac SET GuiLuc=DATEADD(day,-8,SYSDATETIMEOFFSET()) WHERE Id='${review}'`);const old=sql(`SELECT NhanXet FROM DanhGiaDoiTac WHERE Id='${review}'`);const r=await post(student.c,'/DanhGia/DoiTac/Gui',{MaLop:lop,SoSao:1,NhanXet:'Thay đổi ngoài hạn chỉnh sửa.',XacNhanRiengTu:'true',RowVersion:version('DanhGiaDoiTac',review)});assert.match(plain(await r.text()),/7 ngày/);assert.equal(sql(`SELECT NhanXet FROM DanhGiaDoiTac WHERE Id='${review}'`),old);sql(`UPDATE LopHoc SET TrangThai='DangHoatDong',KetThucLuc=NULL,CachKetThuc=NULL WHERE Id='${lop}'`);});
        const due=crypto.randomUUID();
        sql(`INSERT DotPhanHoi(Id,LopHocId,Ten,TuNgay,DenNgay,HanGui,NguoiTaoId,TrangThai) VALUES('${due}','${lop}',N'Đợt hết hạn',DATEADD(day,-3,SYSDATETIMEOFFSET()),DATEADD(day,-2,SYSDATETIMEOFFSET()),DATEADD(second,-1,SYSDATETIMEOFFSET()),'${tutor.id}','DangMo');`);
        await check('Tới hạn từ chối phiếu ngay, Worker đóng đợt trong lượt kế tiếp',async()=>{await post(student.c,'/DanhGia/DanhGia/Gui',{MaLop:lop,DotPhanHoiId:due,NoiDung:'Nội dung gửi khi đã quá hạn.'});assert.equal(sql(`SELECT COUNT(*) FROM PhieuDanhGia WHERE DotPhanHoiId='${due}'`),'0');for(let i=0;i<35&&sql(`SELECT TrangThai FROM DotPhanHoi WHERE Id='${due}'`)!=='DaDong';i++)await new Promise(r=>setTimeout(r,1000));assert.equal(sql(`SELECT TrangThai FROM DotPhanHoi WHERE Id='${due}'`),'DaDong');});
        const room=crypto.randomUUID(),hidden=crypto.randomUUID(),visible=crypto.randomUUID();
        sql(`INSERT CuocHoiThoai(Id,LoaiHoiThoai,GiaSuId,BenHocId,LopHocId,TaoLuc) VALUES('${room}','TheoLop','${tutor.id}','${student.id}','${lop}',SYSDATETIMEOFFSET());
        INSERT TinNhan(Id,CuocHoiThoaiId,NguoiGuiId,NoiDung,GuiLuc,MaThaoTac) VALUES('${hidden}','${room}','${tutor.id}',N'Nội dung cần ẩn',DATEADD(second,-1,SYSDATETIMEOFFSET()),NEWID()),('${visible}','${room}','${student.id}',N'Nội dung được giữ',SYSDATETIMEOFFSET(),NEWID());
        INSERT BienPhapQuanTri(Id,LoaiDoiTuong,DoiTuongId,Loai,LyDo,NguoiApDungId,ApDungLuc) VALUES(NEWID(),'TinNhan','${hidden}','AnNoiDung',N'Kiểm thử ẩn tin','${admin.id}',SYSDATETIMEOFFSET());`);
        await check('Tin đã ẩn không xuất hiện, mốc tải cũ vẫn hoạt động',async()=>{const r=await student.c.get(base+`/HocTap/TinNhan/DuLieu?id=${room}`);assert.equal(r.status(),200);const data=await r.json();assert.ok(data.items.some(x=>x.id===visible));assert.ok(!data.items.some(x=>x.id===hidden));assert.equal((await student.c.get(base+`/HocTap/TinNhan/DuLieu?id=${room}&after=${hidden}`)).status(),200);});
        await check('Cấp quyền quản trị lưu lý do và thu hồi phiên',async()=>{const r=await post(admin.c,'/QuanTri/QuanTri/PhanQuyen',{TaiKhoanId:limited.id,ConcurrencyStamp:stamp(limited.id),Quyen:'QuanLyDanhMuc',LyDo:'Phân công phụ trách danh mục môn học.'});assert.equal(r.status(),302);assert.equal((await limited.c.get(base+'/QuanTri/QuanTri/XemNhatKy',{maxRedirects:0})).status(),302);await login(limited);assert.equal((await limited.c.get(base+'/QuanTri/QuanTri/MonHoc')).status(),200);assert.equal((await limited.c.get(base+'/QuanTri/QuanTri/XemNhatKy')).status(),403);assert.match(sql(`SELECT TOP 1 ChiTiet FROM NhatKyQuanTri WHERE HanhDong='PhanQuyenQuanTri' AND DoiTuongId='${limited.id}' ORDER BY ThoiDiem DESC`),/Phân công phụ trách/);});
        await check('Không gỡ quyền quản lý tài khoản cuối cùng',async()=>{sql(`DELETE AspNetUserClaims WHERE ClaimType='EduConnect.AdminPermission' AND ClaimValue IN('QuanLyTaiKhoan','QuanLyQuyen') AND UserId<>'${admin.id}'`);const r=await post(admin.c,'/QuanTri/QuanTri/PhanQuyen',{TaiKhoanId:admin.id,ConcurrencyStamp:stamp(admin.id),Quyen:'XemNhatKy',LyDo:'Thử gỡ quyền quan trọng cuối cùng.'});assert.notEqual(r.status(),302);assert.equal(sql(`SELECT COUNT(*) FROM AspNetUserClaims WHERE UserId='${admin.id}' AND ClaimType='EduConnect.AdminPermission' AND ClaimValue='QuanLyQuyen'`),'1');});

        const page=await admin.context.newPage();
        await page.goto(base+'/QuanTri/QuanTri/Index');await page.screenshot({path:output+'/admin-desktop.png',fullPage:true});
        await page.setViewportSize({width:390,height:844});await page.goto(base+'/QuanTri/BaoCaoViPham/ChiTiet?maBaoCao='+report);await page.screenshot({path:output+'/report-mobile.png',fullPage:true});
        await check('Màn hình báo cáo không tràn ngang ở 390px',async()=>assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth)));
        const learnerPage=await student.context.newPage();await learnerPage.goto(base+progress);await learnerPage.screenshot({path:output+'/progress-desktop.png',fullPage:true});
    } finally { await browser.close(); }
    if(results.some(x=>x.status==='Fail'))process.exitCode=1;
}
main().catch(error=>{console.error(error);process.exitCode=1;});
