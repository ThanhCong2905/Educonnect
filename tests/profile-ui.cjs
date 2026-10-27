const {chromium}=require('playwright'),assert=require('node:assert/strict'),fs=require('node:fs');
(async()=>{
const ids=JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json','utf8').replace(/^\uFEFF/,''));
assert.ok(ids.database.startsWith('EduConnect_Verification_'));
const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
const base='http://127.0.0.1:5281',errors=[];
async function submit(page,selector){await Promise.all([page.waitForEvent('framenavigated',f=>f===page.mainFrame()),selector.click()]);await page.waitForLoadState();}
async function account(id){const c=await browser.newContext({viewport:{width:1440,height:1000}}),p=await c.newPage();p.on('pageerror',e=>errors.push(e.message));await p.goto(base);await p.locator('.preview summary').click();await p.locator('.preview select').selectOption(id);await Promise.all([p.waitForEvent('framenavigated',f=>f===p.mainFrame()),p.locator('.preview button').click()]);return p;}
try{
const tutor=await account(ids.tutor);tutor.on('response',async r=>{if(r.request().method()==='POST'&&r.status()>=400)console.log('POST ERROR',r.status(),r.url(),(await r.text()).slice(0,1200));});await tutor.goto(base+'/NhuCau/HoSoGiaSu/Sua');await tutor.waitForSelector('[data-tutor-profile]');
if(await tutor.locator('button[value="HuyDuyet"]').count())await submit(tutor,tutor.locator('button[value="HuyDuyet"]'));
assert.equal(await tutor.locator('nav a').filter({hasText:'Nhu cầu nhận lớp'}).count(),0);
await tutor.locator('[data-add-subject]').click();assert.equal(await tutor.locator('[data-subject-row]').count(),2);
await tutor.locator('[data-subject-row]').last().locator('[data-subject-select]').selectOption('0');assert.ok(await tutor.locator('[data-subject-row]').last().locator('[data-custom-subject]').isVisible());
await tutor.locator('[data-subject-row]').last().locator('[data-remove-subject]').click();
await tutor.locator('[name="MoTa"]').fill('Hướng dẫn học tập theo năng lực của từng học sinh.');await submit(tutor,tutor.locator('button').filter({hasText:'Lưu hồ sơ'}));
await submit(tutor,tutor.locator('button[value="GuiDuyet"]'));assert.notEqual(await tutor.locator('fieldset').getAttribute('disabled'),null);
const admin=await account(ids.admin);await admin.goto(base+'/QuanTri/HoSo/ChiTiet/'+ids.hid);assert.ok(await admin.locator('button[value="true"]').isVisible());await submit(admin,admin.locator('button[value="true"]'));
await tutor.reload();assert.ok(await tutor.locator('button[value="Hien"]').isVisible());await submit(tutor,tutor.locator('button[value="Hien"]'));assert.notEqual(await tutor.locator('fieldset').getAttribute('disabled'),null);
const learner=await account(ids.learner);await learner.goto(base+'/TimKiem/TimKiem/ChiTiet/'+ids.hid);assert.equal((await learner.locator('body').innerText()).includes('PRIVATE_CERT'),false);
await tutor.locator('button[value="An"]').click();await tutor.locator('#hide-profile-confirm').waitFor({state:'visible'});await submit(tutor,tutor.locator('[data-confirm-profile]'));assert.equal(await tutor.locator('fieldset').isDisabled(),false);
assert.equal((await learner.request.get(base+'/TimKiem/TimKiem/ChiTiet/'+ids.hid)).status(),404);
assert.equal((await learner.request.get(base+'/QuanTri/HoSo/Index')).status(),403);
await tutor.setViewportSize({width:390,height:844});assert.equal(await tutor.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
await tutor.screenshot({path:process.env.TEMP+'/edu-profile-mobile.png',fullPage:true});
assert.deepEqual(errors,[]);console.log('PASS browser: profile rows/custom subject/save/submit/admin approve/publish/hide confirmation/private detail/roles/mobile/no JS errors');
}finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});

