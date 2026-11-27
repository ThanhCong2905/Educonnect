const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const {execFileSync}=require('node:child_process'),signIn=require('./sign-in.cjs');
const lesson=JSON.parse(fs.readFileSync('.uc3-build/fixture.json'));
const profile=JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json'));
const base='http://127.0.0.1:5299',out='.codex-local/ui-layout-review';fs.mkdirSync(out,{recursive:true});
assert.ok(lesson.database.startsWith('EduConnect_Verification_')&&lesson.database===profile.database);
const need=execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',lesson.database,'-E','-C','-h','-1','-Q','SET NOCOUNT ON; SELECT TOP 1 Id FROM NhuCauHoc ORDER BY TaoLuc DESC'],{encoding:'utf8'}).trim();
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),page=await browser.newPage(),errors=[],checks=[];
 page.on('pageerror',e=>errors.push(e.message));const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/results.json',JSON.stringify({checks,errors},null,2));};
 const fits=()=>page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1);
 try{
  await signIn(page,base,lesson.tutor);
  for(const width of[1440,390]){
   await page.setViewportSize({width,height:900});await page.goto(base+'/HocTap/LopHoc/Index');assert.ok(await fits());
   assert.ok(await page.locator('.classroom-card').count()>0);
   await page.locator('[data-class-filter="TamDung"]').click();assert.equal(await page.locator('.classroom-card:visible:not([data-class-state="TamDung"])').count(),0);await page.locator('[data-class-filter=""]').click();assert.equal(await page.locator('.classroom-card:visible').count(),await page.locator('.classroom-card').count());
   await page.screenshot({path:out+'/classes-'+width+'.png',fullPage:true});
   await page.goto(base+'/HocTap/LopHoc/ThanhVien/'+lesson.classroom);assert.ok(await fits());
   const editor=page.locator('.member-permission-editor').first();assert.ok(await editor.count());
   assert.equal(await editor.locator('select').isVisible(),false);await editor.locator('summary').click();assert.ok(await editor.locator('select').isVisible());
   assert.ok(await fits());await page.screenshot({path:out+'/members-'+width+'.png',fullPage:true});
  }
  pass('Lớp học và thành viên cân đối ở máy tính/điện thoại; chỉnh quyền chỉ mở khi chọn');
  await page.goto(base+'/HocTap/LopHoc/Index');const entry=page.locator('.classroom-card a').first(),destination=new URL(await entry.getAttribute('href'),base).pathname;await entry.click();assert.equal(new URL(page.url()).pathname,destination);
  pass('Thẻ lớp vẫn mở đúng lớp và giữ nguyên thao tác');
  await signIn(page,base,profile.admin);
  for(const width of[1440,390]){
   await page.setViewportSize({width,height:900});await page.goto(base+'/QuanTri/QuanTri/NhuCauChiTiet/'+need);assert.ok(await fits());
   const action=page.locator('.admin-need-action');assert.equal(await action.locator('textarea').isVisible(),false);await action.locator('summary').click();assert.ok(await action.locator('textarea').isVisible());assert.ok(await fits());
   await page.screenshot({path:out+'/moderation-'+width+'.png',fullPage:true});
  }
  pass('Hậu kiểm tách nội dung và quyết định, biểu mẫu đóng mặc định, mở được trên mobile');
  assert.deepEqual(errors,[]);pass('Không có lỗi JavaScript trong các trang vừa cải tiến');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
