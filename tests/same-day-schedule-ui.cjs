const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const {execFileSync}=require('node:child_process'),signIn=require('./sign-in.cjs'),crypto=require('node:crypto');
const ids=JSON.parse(fs.readFileSync('.uc3-build/fixture.json'));assert.ok(ids.database.startsWith('EduConnect_Verification_'));
const base='http://127.0.0.1:5299',out='.codex-local/same-day-schedule';fs.mkdirSync(out,{recursive:true});
const sql=q=>execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',ids.database,'-E','-C','-I','-b','-h','-1','-W','-Q','SET NOCOUNT ON; '+q],{encoding:'utf8'}).trim();
const day=sql(`SELECT ISNULL(CONVERT(varchar(10),DATEADD(day,2,MAX(BatDauLuc)),23),'2035-01-05') FROM LichHoc WHERE LopHocId='${ids.classroom}' AND BatDauLuc>'2035-01-01'`);
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),page=await browser.newPage({viewport:{width:1440,height:1000}}),checks=[],errors=[];
 page.on('pageerror',e=>errors.push(e.message));const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/ui-results.json',JSON.stringify({checks,errors},null,2));};
 async function times(start,end){await page.locator('#batch-start').fill(start);await page.locator('#batch-end').fill(end);await page.locator('#batch-end').dispatchEvent('change');}
 const rows=()=>page.locator('[data-schedule-list]>.schedule-row');
 try{
  await signIn(page,base,ids.tutor);await page.goto(base+'/HocTap/LichHoc/Them/'+ids.classroom+'?ngay='+day);await page.locator('#batch-title').fill('Kiểm thử nhiều buổi cùng ngày');
  await times('09:00','10:00');await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),1);
  await times('10:00','11:00');await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),2);
  await times('10:30','11:30');await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),2);assert.match(await page.locator('#calendar-feedback').innerText(),/chồng/);
  await times('14:00','15:00');await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),3);
  await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),2);await page.locator('#calendar button[aria-label$="'+day+'"]').click();assert.equal(await rows().count(),3);
  pass('Cùng ngày thêm được buổi nối tiếp và khác giờ; chặn chồng giờ, bỏ đúng một khung');
  assert.equal(await page.locator('[data-repeat-panel]').isVisible(),false);
  const response=page.waitForResponse(r=>r.request().method()==='POST'&&r.url().includes('/LichHoc/Them'));await page.getByRole('button',{name:'Lưu lịch học',exact:true}).click();assert.equal((await response).status(),200);await page.waitForLoadState('networkidle');
  assert.equal(sql(`SELECT COUNT(*) FROM LichHoc WHERE LopHocId='${ids.classroom}' AND TieuDe=N'Kiểm thử nhiều buổi cùng ngày' AND CONVERT(date,BatDauLuc)='${day}'`),'3');
  pass('Chỉ chọn ngày riêng vẫn lưu được ba buổi cùng ngày xuống SQL');
  const repeatDay=new Date(day+'T12:00:00');repeatDay.setDate(repeatDay.getDate()+2);const key=repeatDay.toISOString().slice(0,10);
  await page.goto(base+'/HocTap/LichHoc/Them/'+ids.classroom+'?ngay='+key);await page.locator('#batch-title').fill('Kiểm thử lịch lặp riêng');await times('16:00','17:00');
  await page.locator('[data-schedule-mode=repeat]').click();assert.equal(await page.locator('[data-single-panel]').isVisible(),false);await page.locator('#range-from').fill(key);await page.locator('#range-to').fill(key);await page.locator('[data-weekday][value="'+repeatDay.getDay()+'"]').check();await page.locator('#apply-weekdays').click();assert.equal(await rows().count(),1);
  await page.setViewportSize({width:390,height:844});assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));await page.screenshot({path:out+'/repeat-mobile.png',fullPage:true});
  const repeatSaved=page.waitForResponse(r=>r.request().method()==='POST'&&r.url().includes('/LichHoc/Them'));await page.getByRole('button',{name:'Lưu lịch học',exact:true}).click();assert.equal((await repeatSaved).status(),200);await page.waitForLoadState('networkidle');
  assert.equal(sql(`SELECT COUNT(*) FROM LichHoc WHERE LopHocId='${ids.classroom}' AND TieuDe=N'Kiểm thử lịch lặp riêng' AND CONVERT(date,BatDauLuc)='${key}'`),'1');pass('Chỉ dùng lịch lặp vẫn lưu được, không cần chọn thêm ngày riêng; mobile không tràn');
  await signIn(page,base,ids.learner);
  for(const state of['DangMo','DaDong','BanNhap']){await page.goto(base+'/NhuCau/NhuCau/Index?state='+state);assert.equal(new URL(await page.locator('.need-state-tabs [aria-current=page]').getAttribute('href'),base).searchParams.get('state'),state);const labels=await page.locator('.candidate .card-statuses>.badge:first-child').allTextContents();const expected={DangMo:'Đang mở',DaDong:'Đã đóng',BanNhap:'Bản nháp'}[state];assert.ok(labels.every(label=>label.trim()===expected));assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));}
  pass('Ba tab nhu cầu tách đúng đang mở, đã đóng, bản nháp trên mobile');
  const draftUser=crypto.randomUUID(),email=draftUser.replaceAll('-','')+'@example.test';
  sql(`INSERT AspNetUsers(Id,HoTen,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,TrangThai) SELECT '${draftUser}',N'Kiểm thử tab bản nháp','${email}','${email.toUpperCase()}','${email}','${email.toUpperCase()}',1,PasswordHash,NEWID(),NEWID(),0,0,1,0,'HoatDong' FROM AspNetUsers WHERE Id='${ids.learner}'; INSERT AspNetUserRoles(UserId,RoleId) SELECT '${draftUser}',Id FROM AspNetRoles WHERE Name='HocSinh'; INSERT HocSinh(TaiKhoanId) VALUES('${draftUser}');`);
  await signIn(page,base,draftUser);await page.goto(base+'/NhuCau/NhuCau/Index');await page.getByRole('link',{name:'Tạo nhu cầu đầu tiên',exact:true}).click();const modal=page.locator('#workspace-modal');await modal.locator('[name=MucTieu]').fill('Bản nháp kiểm thử phân nhóm nhu cầu');await modal.getByRole('button',{name:'Lưu bản nháp',exact:true}).click();await page.waitForURL(url=>url.searchParams.get('state')==='BanNhap');assert.equal(await page.locator('.candidate').count(),1);pass('Lưu nháp từ modal chuyển đúng tab Bản nháp, không mất dấu nhu cầu vừa lưu');
  await page.goto(base+'/TimKiem/TimKiem/GiaSu');const empty=page.locator('.rating-unrated').first();await empty.waitFor();assert.equal(await empty.locator('.rating-stars-empty').innerText(),'☆☆☆☆☆');assert.match(await empty.innerText(),/Chưa có đánh giá/);pass('Chưa có đánh giá hiện năm sao rỗng và nhãn rõ ràng');
  assert.deepEqual(errors,[]);pass('Không phát sinh lỗi JavaScript trong các luồng đã sửa');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
