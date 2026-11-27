const {chromium}=require('playwright');
const fs=require('node:fs'),assert=require('node:assert/strict');
const base=process.env.EDU_TEST_URL||'http://127.0.0.1:5299';
const output='.codex-local/horizontal-search';fs.mkdirSync(output,{recursive:true});
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 const context=await browser.newContext({viewport:{width:1440,height:1000}}),page=await context.newPage(),checks=[],errors=[];
 page.on('pageerror',error=>errors.push(error.message));
 const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(output+'/results.json',JSON.stringify({base,checks,errors},null,2));};
 const fits=async()=>assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
 try{
  await page.goto(base+'/TimKiem/TimKiem/GiaSu');
  const positions=await page.locator('.filter-primary>fieldset select').evaluateAll(nodes=>nodes.map(node=>node.getBoundingClientRect().y));
  assert.equal(positions.length,5);assert.ok(Math.max(...positions)-Math.min(...positions)<2);assert.equal(await page.locator('.filter-advanced').isVisible(),false);
  const searchWidth=(await page.locator('.search-keyword').boundingBox()).width,filterWidth=(await page.locator('#search-filter-drawer').boundingBox()).width;assert.ok(Math.abs(searchWidth-filterWidth)<2);
  await fits();await page.screenshot({path:output+'/filters-desktop.png',fullPage:true});pass('Năm tiêu chí chính cùng hàng, Lọc thêm đóng và không chiếm chỗ kết quả');
  await page.locator('.filter-more>summary').click();await page.locator('[name=SoSaoToiThieu]').selectOption('4');
  await page.waitForURL(/SoSaoToiThieu=4/);await page.waitForFunction(()=>!document.getElementById('search-content').hasAttribute('aria-busy'));
  await page.locator('[name=SoSaoToiThieu]').press('Escape');assert.equal(await page.locator('.filter-advanced').isVisible(),false);
  await page.locator('.filter-more>summary').click();await page.getByRole('heading',{name:'Tìm gia sư',exact:true}).click();assert.equal(await page.locator('.filter-advanced').isVisible(),false);
  pass('Lọc thêm tự tải AJAX, đóng bằng Escape hoặc bấm ra ngoài');
  for(const width of [1024,1200]){await page.setViewportSize({width,height:900});await fits();assert.ok((await page.locator('#search-filter-drawer').boundingBox()).height<160);}
  pass('Bộ lọc ngang vẫn gọn ở màn hình máy tính nhỏ');
  await page.setViewportSize({width:390,height:844});await page.goto(base+'/TimKiem/TimKiem/GiaSu');
  assert.ok((await page.locator('.candidate').first().boundingBox()).y<700);
  await page.getByRole('button',{name:'Bộ lọc',exact:true}).click();await page.locator('#search-filter-drawer.show').waitFor();
  await page.locator('.filter-more>summary').click();await page.locator('[name=SoSaoToiThieu]').selectOption('4');
  await page.waitForURL(/SoSaoToiThieu=4/);await page.waitForFunction(()=>!document.getElementById('search-content').hasAttribute('aria-busy'));
  await page.screenshot({path:output+'/filters-mobile.png',animations:'disabled'});await page.locator('.filter-show-results').click();await page.locator('.offcanvas-backdrop').waitFor({state:'detached'});await fits();
  pass('Điện thoại vẫn mở phễu, lọc ngay và trở về danh sách không tràn khung');
  await page.setViewportSize({width:1440,height:1000});
  const source=await page.locator('.candidate[data-candidate-id]').first().getAttribute('data-candidate-id');
  await page.goto(base+'/NhuCau/HoSoGiaSu/ChiTiet?maHoSo='+source);
  await page.getByRole('heading',{name:'Gợi ý gia sư tương tự',exact:true}).waitFor();
  const related=page.locator('[data-related-tutor]');await related.first().waitFor();const ids=await related.evaluateAll(nodes=>nodes.map(node=>node.dataset.relatedTutor));
  assert.ok(ids.length>0&&ids.length<=3);assert.equal(new Set(ids).size,ids.length);assert.ok(!ids.includes(source));
  await page.screenshot({path:output+'/detail-desktop.png',fullPage:true});
  const link=await related.first().getByRole('link',{name:'So sánh',exact:true}).getAttribute('href');const compare=new URL(link,base);
  assert.deepEqual(compare.searchParams.getAll('ids'),[source,ids[0]]);await page.goto(compare.href);assert.equal(await page.locator('thead [data-comparison-id]').count(),2);
  pass('Hồ sơ có tối đa ba gợi ý khác nhau, liên kết so sánh đúng hai hồ sơ');
  await page.goto(base+'/NhuCau/HoSoGiaSu/ChiTiet?maHoSo='+source);await page.setViewportSize({width:390,height:844});await fits();await page.screenshot({path:output+'/detail-mobile.png',fullPage:true});
  pass('Hồ sơ và gia sư liên quan xếp một cột trên điện thoại');
  const noScript=await browser.newContext({javaScriptEnabled:false,viewport:{width:1440,height:1000}}),plain=await noScript.newPage();
  await plain.goto(base+'/TimKiem/TimKiem/GiaSu');await plain.locator('[name=MonHocId]').selectOption('1');await plain.getByRole('button',{name:'Áp dụng bộ lọc',exact:true}).click();
  assert.equal(new URL(plain.url()).searchParams.get('MonHocId'),'1');await noScript.close();pass('Không có JavaScript vẫn dùng được biểu mẫu lọc');
  assert.deepEqual(errors,[]);pass('Không có lỗi JavaScript trong giao diện mới');
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
