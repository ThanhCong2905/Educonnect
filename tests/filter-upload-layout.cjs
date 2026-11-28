const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const signIn=require('./sign-in.cjs'),lesson=JSON.parse(fs.readFileSync('.uc3-build/fixture.json')),profile=JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json')),chat=JSON.parse(fs.readFileSync('tests/ChatAttachments/bin/Debug/net10.0/results.json'));
assert.ok(lesson.database.startsWith('EduConnect_Verification_')&&lesson.database===profile.database&&lesson.database===chat.database);
const base='http://127.0.0.1:5299',out='.codex-local/filter-upload-layout';fs.mkdirSync(out,{recursive:true});
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),checks=[],errors=[];
 const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/results.json',JSON.stringify({checks,errors},null,2));};
 try{
  for(const[role,id,route]of[['Guest',null,'GiaSu'],['HocSinh',lesson.learner,'GiaSu'],['PhuHuynh',chat.reader,'GiaSu'],['GiaSu',lesson.tutor,'NhuCau']]){
   const page=await browser.newPage({viewport:{width:1440,height:1000}});page.on('pageerror',e=>errors.push(e.message));if(id)await signIn(page,base,id);
   await page.goto(base+'/TimKiem/TimKiem/'+route);await page.locator('.filter-more>summary').click();
   const columns=await page.locator('.filter-advanced').evaluate(el=>getComputedStyle(el).gridTemplateColumns.split(' ').map(Number.parseFloat));assert.equal(columns.length,id?3:2);assert.ok(Math.max(...columns)-Math.min(...columns)<2);
   const groups=await page.locator('.filter-advanced>.filter-group').evaluateAll(ns=>ns.map(n=>getComputedStyle(n).display));assert.ok(groups.every(x=>x==='contents'));
   await page.screenshot({path:out+'/filters-'+role+'.png'});
   await page.setViewportSize({width:390,height:844});assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));await page.close();
  }
  pass('Khách, học sinh, phụ huynh và gia sư có lưới bộ lọc đều, không thừa cột hoặc tràn mobile');
  const page=await browser.newPage({viewport:{width:1440,height:1000}});page.on('pageerror',e=>errors.push(e.message));await signIn(page,base,profile.tutor);await page.goto(base+'/NhuCau/HoSoGiaSu/Sua');
  if(await page.locator('[data-edit-profile]:visible').count())await page.locator('[data-edit-profile]').click();
  const group=page.locator('.profile-evidence-upload').first(),file=group.locator('input[type=file]'),currentId=await group.locator('input[type=hidden]').inputValue();
  const image={name:'minh-chung.png',mimeType:'image/png',buffer:Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aZ1sAAAAASUVORK5CYII=','base64')};
  await file.setInputFiles(image);assert.equal(await group.locator('.upload-remove:visible').count(),1);assert.equal(await page.locator('[data-clear-evidence]').count(),0);assert.equal(await page.locator('[data-clear-all-evidence]:visible').count(),1);assert.equal(await group.locator('[data-saved-evidence]').isVisible(),false);pass('Tệp mới chỉ có một dấu x, một nút Bỏ chọn tất cả và không lặp nút xóa');
  await group.locator('.upload-selection .upload-remove').click();assert.equal(await file.evaluate(el=>el.files.length),0);assert.equal(await group.locator('input[type=hidden]').inputValue(),currentId);assert.ok(await group.locator('[data-saved-evidence]').isVisible());pass('Bỏ tệp mới hiện lại tệp đã lưu, không xóa hoặc đổi mã tệp cũ');
  await page.locator('[name=KinhNghiem]').selectOption({label:'Dưới 1 năm'});await file.setInputFiles(image);await page.locator('.profile-evidence-upload').nth(1).locator('input[type=file]').setInputFiles({...image,name:'kinh-nghiem.png'});
  assert.equal(await page.locator('[data-clear-all-evidence]:visible').count(),1);await page.locator('[data-clear-all-evidence]').click();assert.ok(await page.locator('.profile-evidence-upload input[type=file]').evaluateAll(ns=>ns.every(n=>n.files.length===0)));assert.equal(await group.locator('input[type=hidden]').inputValue(),currentId);pass('Một nút bỏ tất cả tệp mới ở các mục, giữ nguyên minh chứng đang lưu');
  await group.locator('[data-remove-saved-evidence]').click();assert.equal(await group.locator('input[type=hidden]').inputValue(),'');await group.locator('[data-remove-saved-evidence]').click();assert.equal(await group.locator('input[type=hidden]').inputValue(),currentId);pass('Dấu x của tệp đã lưu vẫn hỗ trợ hoàn tác trước khi lưu hồ sơ');
  await page.setViewportSize({width:390,height:844});assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));await page.screenshot({path:out+'/evidence-mobile.png',fullPage:true});
  await page.evaluate(()=>{const form=document.createElement('form');form.id='file-test';form.innerHTML='<input type="file" multiple>';document.querySelector('main').append(form);window.EduConnect.initFilePickers(form);});
  await page.locator('#file-test input').setInputFiles([image,{...image,name:'thu-hai.png'}]);assert.equal(await page.locator('#file-test .upload-remove').count(),2);assert.equal(await page.locator('#file-test [data-clear-all-files]:visible').count(),1);await page.locator('#file-test [data-clear-all-files]').click();assert.equal(await page.locator('#file-test input').evaluate(el=>el.files.length),0);pass('Bộ chọn tệp dùng chung có một x mỗi tệp và một nút bỏ tất cả');
  assert.deepEqual(errors,[]);pass('Không có lỗi JavaScript trong bộ lọc và thao tác tệp');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
