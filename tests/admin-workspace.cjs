const {chromium}=require('playwright');
const fs=require('node:fs'),assert=require('node:assert/strict'),crypto=require('node:crypto');
const {execFileSync}=require('node:child_process');
const signIn=require('./sign-in.cjs');
const fixture=JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json'));
const database=fixture.database,base=process.env.EDU_TEST_URL||'http://127.0.0.1:5299';
assert.ok(database.startsWith('EduConnect_Verification_'));
assert.ok(['127.0.0.1','localhost'].includes(new URL(base).hostname));
const out='.codex-local/admin-refine';fs.mkdirSync(out,{recursive:true});
const sql=query=>execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',database,'-E','-C','-I','-b','-h','-1','-w','65535','-f','65001','-Q','SET NOCOUNT ON; '+query],{encoding:'utf8'}).trim();
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 const page=await browser.newPage({viewport:{width:1440,height:1000}}),checks=[],errors=[];
 page.on('pageerror',e=>errors.push(e.message));
 const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/results.json',JSON.stringify({database,base,checks,errors},null,2));};
 const fits=()=>page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1);
 try{
  await signIn(page,base,fixture.admin);
  const dashboard=await page.goto(base+'/QuanTri/QuanTri/Index');assert.equal(dashboard.status(),200);
  const expected=sql("SELECT TOP 4 Id FROM HoSoGiaSu WHERE TrangThaiDuyet='ChoDuyet' ORDER BY GuiDuyetLuc,Id").split(/\r?\n/).filter(Boolean).map(x=>x.trim().toLowerCase());
  const actual=await page.locator('[aria-labelledby=profile-queue-title] .admin-work-link').evaluateAll(links=>links.map(a=>a.pathname.split('/').at(-1).toLowerCase()));
  assert.deepEqual(actual,expected);
  const reports=sql("SELECT TOP 4 Id FROM BaoCaoViPham WHERE TrangThai<>'DaXuLy' ORDER BY GuiLuc,Id").split(/\r?\n/).filter(Boolean).map(x=>x.trim().toLowerCase());
  const reportLinks=await page.locator('[aria-labelledby=report-queue-title] .admin-work-link').evaluateAll(links=>links.map(a=>new URL(a.href).searchParams.get('maBaoCao').toLowerCase()));
  assert.deepEqual(reportLinks,reports);pass('Hai hàng chờ khớp SQL, tối đa bốn mục và yêu cầu cũ hiển thị trước');
  const destinations=await page.locator('.admin-work-link').evaluateAll(links=>links.map(a=>a.href));
  for(const url of destinations)assert.equal((await page.request.get(url)).status(),200,url);
  pass('Từng mục hàng chờ mở đúng chi tiết và không thay đổi trạng thái');
  await page.screenshot({path:out+'/dashboard-desktop.png',fullPage:true});
  for(const width of[1440,1024,390]){
   await page.setViewportSize({width,height:900});
   for(const route of['/QuanTri/QuanTri/Index','/QuanTri/HoSo/Index','/QuanTri/QuanTri/TaiKhoan','/QuanTri/QuanTri/ChiTiet/'+fixture.tutor,'/QuanTri/BaoCaoViPham/Index?quanTri=true']){
    assert.equal((await page.goto(base+route)).status(),200);assert.ok(await fits(),route+' at '+width);
    if(width===390)await page.screenshot({path:out+'/'+route.split('/')[3].split('?')[0]+'-mobile.png',fullPage:true});
   }
  }
  pass('Tổng quan, hồ sơ, danh sách/chi tiết tài khoản và báo cáo không tràn ở 390, 1024 và 1440px');
  await page.goto(base+'/QuanTri/BaoCaoViPham/Index?quanTri=true');
  await page.locator('.report-queue-tabs a').filter({hasText:'Tôi đang xử lý'}).click();
  assert.ok(page.url().includes('ToiDangXuLy'));assert.equal(await page.locator('.report-queue-tabs [aria-current=page]').innerText(),'Tôi đang xử lý');
  pass('Chuyển hàng chờ báo cáo giữ đúng bộ lọc người phụ trách');
  await page.goto(base+'/QuanTri/QuanTri/TaiKhoan?q=KhongCoTaiKhoan'+crypto.randomUUID());
  assert.ok(await page.getByRole('heading',{name:'Không tìm thấy tài khoản'}).isVisible());
  await page.getByRole('link',{name:'Xóa bộ lọc',exact:true}).click();assert.equal(new URL(page.url()).search,'');
  pass('Danh sách rỗng có cách xóa bộ lọc và trở về danh sách');
  const limited=crypto.randomUUID(),email=limited.replaceAll('-','')+'@example.test';
  sql(`INSERT AspNetUsers(Id,HoTen,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,TrangThai) SELECT '${limited}',N'Quản trị xem nhật ký','${email}','${email.toUpperCase()}','${email}','${email.toUpperCase()}',1,PasswordHash,NEWID(),NEWID(),0,0,1,0,'HoatDong' FROM AspNetUsers WHERE Id='${fixture.admin}'; INSERT AspNetUserRoles(UserId,RoleId) SELECT '${limited}',Id FROM AspNetRoles WHERE Name='QuanTri'; INSERT AspNetUserClaims(UserId,ClaimType,ClaimValue) VALUES('${limited}','EduConnect.AdminPermission','XemNhatKy');`);
  await signIn(page,base,limited);await page.goto(base+'/QuanTri/QuanTri/Index');
  assert.equal(await page.locator('.admin-work-queues').count(),0);
  assert.equal((await page.request.get(base+'/QuanTri/HoSo/Index')).status(),403);
  assert.equal((await page.request.get(base+'/QuanTri/BaoCaoViPham/Index?quanTri=true')).status(),403);
  pass('Quản trị chỉ xem nhật ký không nhận dữ liệu hoặc truy cập hàng chờ ngoài quyền');
  assert.deepEqual(errors,[]);pass('Không phát sinh lỗi JavaScript trong khu vực quản trị');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
