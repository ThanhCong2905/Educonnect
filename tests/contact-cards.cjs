const {chromium}=require('playwright'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'}),page=await browser.newPage({viewport:{width:1440,height:1000}});
 const base='http://127.0.0.1:5281',out=path.join(__dirname,'../docs/qa'),checks=[],errors=[];page.on('pageerror',error=>errors.push(error.message));
 try{
  await page.goto(base);await page.locator('.preview summary').click();await page.locator('.preview select').selectOption('10000000-0000-0000-0000-000000000002');
  await Promise.all([page.waitForEvent('framenavigated',frame=>frame===page.mainFrame()),page.getByRole('button',{name:'Chuyển vai trò'}).click()]);
  await page.goto(base+'/KetNoi/KetNoi/LienHe');assert.ok(await page.locator('.contact-grid .contact-card').count());
  assert.equal(await page.locator('.contact-card details').count(),0);
  await page.screenshot({path:path.join(out,'contacts-grid-1440.png'),fullPage:true,animations:'disabled'});
  const open=page.locator('[data-contact-id]').first(),id=await open.getAttribute('data-contact-id');await open.click();
  const modal=page.locator('#connection-request-modal');await modal.waitFor();
  assert.equal(await modal.locator('[name="LienHeId"]').inputValue(),id);assert.equal(await modal.locator('iframe').count(),0);
  await modal.getByRole('button',{name:'Gửi yêu cầu',exact:true}).click();await modal.locator('.ui-field-error').waitFor();
  assert.equal(await modal.locator('[name="LienHeId"]').inputValue(),id);
  await page.screenshot({path:path.join(out,'connection-request-modal-1440.png'),animations:'disabled'});
  checks.push('Contact cards separate summary/actions and open a correctly bound request modal with inline validation');
  await modal.getByRole('button',{name:'Đóng',exact:true}).click();await modal.waitFor({state:'hidden'});
  await page.setViewportSize({width:390,height:844});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
  await page.screenshot({path:path.join(out,'contacts-grid-390.png'),animations:'disabled'});checks.push('Contact grid fits mobile without horizontal overflow');
  await page.setViewportSize({width:1440,height:1000});await page.goto(base+'/TimKiem/TimKiem/Index?TuKhoa=MultiNeeds');
  const name=await page.locator('.candidate h3').first().innerText();
  assert.equal(await page.locator('.candidate').filter({hasText:name}).count(),3);
  await page.locator('.candidate .actions a').first().click();assert.equal(await page.locator('.related-needs .cards .candidate').count(),2);
  const columns=await page.locator('.related-needs .cards').evaluate(el=>getComputedStyle(el).gridTemplateColumns.split(' ').length);assert.equal(columns,3);
  await page.locator('.related-needs').scrollIntoViewIfNeeded();await page.screenshot({path:path.join(out,'related-needs-grid-1440.png'),animations:'disabled'});
  checks.push('All three tutor needs appear in search, with the other two shown as complete cards in a related-needs grid');
  await page.setViewportSize({width:390,height:844});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
  checks.push('Related needs become a single-column grid on mobile');assert.deepEqual(errors,[]);
  fs.writeFileSync(path.join(out,'contact-cards-results.json'),JSON.stringify({passed:checks.length,checks,pageErrors:errors},null,2));console.log(checks);
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
