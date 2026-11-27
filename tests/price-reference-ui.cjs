const {chromium}=require('playwright'),fs=require('node:fs'),assert=require('node:assert/strict');
const base=process.env.EDU_TEST_URL||'http://127.0.0.1:5299',out='.codex-local/price-reference';fs.mkdirSync(out,{recursive:true});
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 const page=await browser.newPage({viewport:{width:1440,height:1000}}),errors=[],checks=[];page.on('pageerror',e=>errors.push(e.message));
 const pass=name=>{checks.push(name);console.log('PASS',name);fs.writeFileSync(out+'/results.json',JSON.stringify({base,checks,errors},null,2));};
 try{
  await page.goto(base+'/TimKiem/TimKiem/GiaSu');const profiles=await page.locator('.candidate[data-candidate-id]').evaluateAll(ns=>ns.map(n=>n.dataset.candidateId));
  let found=false;
  for(const id of profiles){await page.goto(base+'/NhuCau/HoSoGiaSu/ChiTiet?maHoSo='+id);if(await page.locator('[data-market-average]:not([data-market-average=""])').count()){found=true;break;}}
  assert.ok(found,'Expected at least one public reference group with three profiles');
  const cards=page.locator('[data-related-tutor]');assert.ok(await cards.count()>0);
  for(const card of await cards.all()){const subject=await card.locator('.alternative-subject>strong').innerText();assert.ok((await card.locator('.alternative-matches').innerText()).includes('Có chung môn '+subject));}
  assert.equal(await page.getByText('Có lớp nhận dạy trùng',{exact:true}).count(),0);pass('Nhãn gợi ý ghi đúng tên môn đang chung, bỏ cụm nhận dạy trùng');
  const data=await page.locator('[data-market-average]:not([data-market-average=""])').evaluateAll(nodes=>nodes.map(n=>({average:+n.dataset.marketAverage,count:+n.dataset.marketCount,price:+n.dataset.listedPrice,difference:+n.dataset.marketDifference,text:n.innerText})));
  assert.ok(data.length>0);for(const row of data){assert.ok(row.count>=3);assert.equal(row.price-row.average,row.difference);assert.match(row.text,/Trung bình EduConnect/);}
  assert.ok(await page.locator('#profile-fees [data-market-average]:not([data-market-average=""])').count()>0);pass('Mức trung bình, số hồ sơ và chênh lệch khớp nhau ở hồ sơ và gợi ý');
  for(const node of await page.locator('.price-benchmark[data-market-average=""],.price-benchmark:not([data-market-average])').all())assert.match(await node.innerText(),/Chưa đủ dữ liệu tham chiếu/);
  await page.screenshot({path:out+'/desktop.png',fullPage:true});
  await page.setViewportSize({width:390,height:844});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);await page.screenshot({path:out+'/mobile.png',fullPage:true});pass('Đối chiếu phí không tràn khung trên điện thoại; thiếu mẫu có thông báo rõ');
  assert.deepEqual(errors,[]);pass('Không có lỗi JavaScript khi xem giá tham chiếu');
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
