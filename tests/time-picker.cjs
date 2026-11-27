const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const signIn=require('./sign-in.cjs'),ids=JSON.parse(fs.readFileSync('.uc3-build/fixture.json'));
assert.ok(ids.database.startsWith('EduConnect_Verification_'));
const base='http://127.0.0.1:5299',out='.codex-local/time-picker';fs.mkdirSync(out,{recursive:true});
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),page=await browser.newPage({viewport:{width:1440,height:900}}),checks=[],errors=[];
 page.on('pageerror',e=>errors.push(e.message));const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/results.json',JSON.stringify({checks,errors},null,2));};
 async function choose(input,hour,minute){await input.locator('..').locator('[data-time-open]').click();const picker=page.locator('.time-picker');await picker.locator('select').nth(0).selectOption(hour);await picker.locator('select').nth(1).selectOption(minute);await picker.getByRole('button',{name:'Chọn thời gian',exact:true}).click();}
 try{
  await signIn(page,base,ids.learner);await page.goto(base+'/NhuCau/NhuCau/Tao');
  const start=page.locator('[name="LichRanh[0].GioBatDau"]'),end=page.locator('[name="LichRanh[0].GioKetThuc"]');
  await choose(start,'13','05');assert.equal(await start.inputValue(),'13:05');
  await end.fill('14:35');assert.match(await page.locator('.slot-row .time-duration').first().innerText(),/1 giờ 30 phút/);
  assert.equal(await start.locator('..').locator('[data-time-open] span').innerText(),'13:05');pass('Chọn giờ 24h và tính thời lượng đúng, giữ nguyên trường gửi lên server');
  await start.locator('..').locator('[data-time-open]').click();await page.keyboard.press('Escape');assert.equal(await page.locator('.time-picker').count(),0);assert.ok(await start.locator('..').locator('[data-time-open]').evaluate(el=>el===document.activeElement));pass('Escape đóng bộ chọn và trả focus đúng ô');
  await page.setViewportSize({width:390,height:844});await start.locator('..').locator('[data-time-open]').click();
  assert.ok(await page.locator('.time-picker').evaluate(el=>{const r=el.getBoundingClientRect();return r.left>=0&&r.right<=innerWidth&&r.top>=0&&r.bottom<=innerHeight;}));
  await page.screenshot({path:out+'/mobile.png'});await page.keyboard.press('Escape');assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));pass('Bộ chọn và các khung giờ không tràn màn hình điện thoại');
  await signIn(page,base,ids.tutor);await page.goto(base+'/HocTap/LichHoc/Sua/'+ids.schedule);
  const date=page.locator('[name="Lich.BatDauLuc"]');await date.locator('..').locator('[data-time-open]').click();
  const picker=page.locator('.time-picker');await picker.locator('input[type=date]').fill('2029-01-15');await picker.locator('select').nth(0).selectOption('13');await picker.locator('select').nth(1).selectOption('05');await picker.getByRole('button',{name:'Chọn thời gian',exact:true}).click();
  assert.equal(await date.inputValue(),'2029-01-15T13:05');assert.equal(await date.locator('..').locator('[data-time-open] span').innerText(),'15/01/2029 · 13:05');
  assert.equal(await date.evaluate(el=>new FormData(el.form).get(el.name)),'2029-01-15T13:05');pass('Ngày hiển thị dd/MM/yyyy, dữ liệu datetime gửi nguyên ISO và không đổi múi giờ');
  await page.goto(base+'/HocTap/LopHoc/Index');await page.evaluate(id=>{const a=document.createElement('a');a.href='/HocTap/LichHoc/Sua/'+id;a.dataset.modal='';a.textContent='Sửa lịch thử';document.querySelector('main').prepend(a);},ids.schedule);await page.getByRole('link',{name:'Sửa lịch thử'}).click();
  const modal=page.locator('#workspace-modal');await modal.locator('[name="Lich.BatDauLuc"]').waitFor();await modal.locator('[name="Lich.BatDauLuc"]').locator('..').locator('[data-time-open]').click();assert.ok(await modal.locator('.time-picker select').first().evaluate(el=>el===document.activeElement||el.closest('.time-picker').contains(document.activeElement)));await page.keyboard.press('Escape');assert.ok(await modal.evaluate(el=>el.classList.contains('show')));pass('Bộ chọn hoạt động trong modal; Escape không đóng nhầm biểu mẫu');
  await page.goto(base+'/TimKiem/TimKiem/NhuCau');await page.setViewportSize({width:1440,height:900});await page.locator('.filter-more > summary').click();await page.locator('[name=GioBatDau]').locator('..').locator('[data-time-open]').click();await page.locator('.time-picker select').first().selectOption('19');assert.ok(await page.locator('.filter-more').evaluate(el=>el.open));await page.keyboard.press('Escape');pass('Chọn giờ trong bộ lọc không đóng nhầm phần Lọc thêm');
  const fallback=await browser.newPage({javaScriptEnabled:false});await signIn(fallback,base,ids.learner);await fallback.goto(base+'/NhuCau/NhuCau/Tao');assert.ok(await fallback.locator('[name="LichRanh[0].GioBatDau"]').isVisible());assert.equal(await fallback.locator('[data-time-open]').count(),0);await fallback.close();pass('Không có JavaScript vẫn dùng được ô thời gian gốc');
  assert.deepEqual(errors,[]);pass('Không phát sinh lỗi JavaScript khi thêm và sửa thời gian');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
