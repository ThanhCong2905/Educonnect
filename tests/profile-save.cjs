const {chromium}=require('playwright'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 const context=await browser.newContext(),page=await context.newPage(),base='http://127.0.0.1:5281',checks=[],errors=[];
 page.on('pageerror',error=>errors.push(error.message));
 try{
  await page.goto(base);await page.locator('.preview summary').click();await page.locator('.preview select').selectOption('10000000-0000-0000-0000-000000000001');
  await Promise.all([page.waitForEvent('framenavigated',frame=>frame===page.mainFrame()),page.getByRole('button',{name:'Chuyển vai trò'}).click()]);
  await page.goto(base+'/NhuCau/HoSoGiaSu/Sua');assert.equal(await page.locator('form').count(),1);
  assert.equal(await page.locator('form').getAttribute('enctype'),'multipart/form-data');
  const text='Kinh nghiệm UI '+Date.now();await page.locator('[name="KinhNghiem"]').fill(text);
  await page.locator('[name="Files"]').setInputFiles({name:'invalid.pdf',mimeType:'application/pdf',buffer:Buffer.from('not a pdf')});
  await page.getByRole('button',{name:'Lưu hồ sơ',exact:true}).click();await page.locator('.ui-error-summary').waitFor();
  assert.equal(await page.locator('[name="KinhNghiem"]').inputValue(),text);
  checks.push('Profile and certificate share one multipart form; a rejected file preserves typed profile data');
  const filename='Ui-degree-'+Date.now()+'.pdf';
  await page.locator('[name="Files"]').setInputFiles({name:filename,mimeType:'application/pdf',buffer:Buffer.from('%PDF-1.4\nprofile test\n%%EOF')});
  await Promise.all([page.waitForEvent('framenavigated',frame=>frame===page.mainFrame()),page.getByRole('button',{name:'Lưu hồ sơ',exact:true}).click()]);
  assert.equal(await page.locator('[name="KinhNghiem"]').inputValue(),text);
  const link=page.getByRole('link',{name:filename,exact:true});assert.equal(await link.count(),1);
  const fileUrl=await link.getAttribute('href');assert.equal((await context.request.get(base+fileUrl)).status(),200);
  checks.push('One Save profile action persists both updated text and downloadable private certificate');
  const row=link.locator('xpath=ancestor::div[contains(@class,"credential-row")]');await row.locator('[name="XoaTepIds"]').check();
  await Promise.all([page.waitForEvent('framenavigated',frame=>frame===page.mainFrame()),page.getByRole('button',{name:'Lưu hồ sơ',exact:true}).click()]);
  assert.equal(await page.getByRole('link',{name:filename,exact:true}).count(),0);
  assert.equal((await context.request.get(base+fileUrl)).status(),404);
  checks.push('Selected certificate removal is applied by the same Save profile action');
  assert.deepEqual(errors,[]);fs.writeFileSync(path.join(__dirname,'../docs/qa/profile-save-results.json'),JSON.stringify({passed:checks.length,checks,pageErrors:errors},null,2));console.log(checks);
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
