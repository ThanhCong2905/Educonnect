const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const {execFileSync}=require('node:child_process'),signIn=require('./sign-in.cjs');
const ids=JSON.parse(fs.readFileSync('.uc3-build/fixture.json'));assert.ok(ids.database.startsWith('EduConnect_Verification_'));
const base='http://127.0.0.1:5299',out='.codex-local/proposal-slots';fs.mkdirSync(out,{recursive:true});
const sql=q=>execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',ids.database,'-E','-C','-I','-b','-h','-1','-W','-f','65001','-Q','SET NOCOUNT ON; '+q],{encoding:'utf8'}).trim();
const fixture=sql("SELECT TOP 1 CONCAT(r.GiaSuId,'|',r.Id,'|',r.LienHeId,'|',r.NhuCauId) FROM YeuCauKetNoiLopHoc r WHERE TenLopDuKien=N'Ôn tập theo lịch đã trao đổi' ORDER BY GuiLuc DESC").toLowerCase().split('|');
assert.equal(fixture.length,4,'Run ProposalSchedulePrefillTests first');const[tutor,request,contact,need]=fixture;
assert.ok(fixture.every(id=>/^[a-f0-9-]{36}$/i.test(id)));
sql(`UPDATE AspNetUsers SET PasswordHash=(SELECT PasswordHash FROM AspNetUsers WHERE Id='${ids.tutor}'),EmailConfirmed=1 WHERE Id='${tutor}'`);
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),page=await browser.newPage({viewport:{width:1440,height:900}}),checks=[],errors=[];
 page.on('pageerror',e=>errors.push(e.message));const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/ui-results.json',JSON.stringify({checks,errors},null,2));};
 try{
  await signIn(page,base,tutor);await page.goto(base+`/KetNoi/KetNoi/DeXuat?contact=${contact}&id=${request}`);
  assert.equal(await page.locator('[name="LichRanh[0].GioBatDau"]').inputValue(),'17:30');pass('Mở đề xuất đã lưu giữ đúng lịch gia sư chỉnh');
  await page.locator('[data-proposal-need]').selectOption('');await page.locator('[data-proposal-need]').selectOption(need);
  assert.equal(await page.locator('[data-slot-list]>.slot-row').count(),2);
  for(const input of await page.locator('[data-slot-list] [name$=".GioBatDau"]').all())assert.equal(await input.inputValue(),'19:15');
  assert.equal(await page.locator('[name=ThoiLuongPhut]').inputValue(),'60');pass('Chọn nhu cầu điền đủ hai lịch và thời lượng đúng từ lịch nguồn');
  await page.locator('[name="LichRanh[0].GioBatDau"]').fill('18:15');assert.equal(await page.locator('[name="LichRanh[0].GioBatDau"]').inputValue(),'18:15');pass('Lịch tự điền vẫn sửa bình thường');
  for(const width of[1440,390]){await page.setViewportSize({width,height:900});if(width===390)await page.locator('.mobile-menu').click();
   const name=page.locator('.account-menu .person-name');await name.evaluate(el=>{el.textContent='Nguyễn Thị Yến';el.title=el.textContent;});
   assert.ok(await name.evaluate(el=>el.getBoundingClientRect().height<=parseFloat(getComputedStyle(el).lineHeight)+1&&el.scrollWidth<=el.clientWidth+1));
   await name.evaluate(el=>el.textContent='Nguyễn Thị Tên Rất Dài Để Kiểm Tra Hiển Thị');assert.ok(await name.evaluate(el=>el.getBoundingClientRect().height<=parseFloat(getComputedStyle(el).lineHeight)+1));
   assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));
   await page.screenshot({path:out+'/sidebar-'+width+'.png'});
  }
  pass('Tên ngắn một hàng, tên dài rút gọn và sidebar không tràn desktop/mobile');
  const summary=page.locator('.nav-group summary').first();assert.ok(await summary.count());assert.equal(await summary.evaluate(el=>getComputedStyle(el,'::after').content),'""');pass('Dấu mở menu dùng nét vẽ thống nhất, bỏ ký tự cũ');
  assert.deepEqual(errors,[]);pass('Không có lỗi JavaScript trong các thao tác đã kiểm tra');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
