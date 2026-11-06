const {chromium}=require('playwright'),assert=require('node:assert/strict'),fs=require('node:fs/promises'),{execFileSync}=require('node:child_process');
(async()=>{
 const f=JSON.parse(await fs.readFile('tests/ChatAttachments/bin/Debug/net10.0/results.json','utf8'));
 assert.ok(f.database.startsWith('EduConnect_Verification_'));
 const sql=q=>execFileSync('sqlcmd',['-S','lpc:CONGVJP','-d',f.database,'-E','-C','-b','-f','65001'],{input:q+'\nGO\n',encoding:'utf8'});
 sql(`UPDATE dbo.LopHoc SET TrangThai='DangHoatDong' WHERE Id='${f.classId}';`);
 const base=process.env.EDU_TEST_URL||'http://127.0.0.1:5282',checks=[],errors=[];
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 async function user(id,disablePolling=true){
  const ctx=await browser.newContext({viewport:{width:1440,height:1000}});
  if(disablePolling)await ctx.addInitScript(()=>{const interval=window.setInterval;window.setInterval=(fn,ms,...args)=>ms===3000?0:interval(fn,ms,...args);});
  const sockets=[];let blocked=false;
  await ctx.routeWebSocket('**/cap-nhat*',ws=>{if(blocked){ws.close();return;}sockets.push(ws);ws.connectToServer();});
  const p=await ctx.newPage();p.on('pageerror',e=>errors.push(e.message));
  await p.goto(base);await p.locator('.preview summary').click();await p.locator('.preview select').selectOption(id);
  await Promise.all([p.waitForEvent('framenavigated',frame=>frame===p.mainFrame()),p.locator('.preview button').click()]);
  await p.waitForFunction(()=>window.EduRealtime?.connected);
  return {p,ctx,sockets,block(value){blocked=value;if(value)for(const ws of sockets)ws.close({code:1001,reason:'test reconnect'});}};
 }
 async function open(u,path){await u.p.goto(base+path);await u.p.waitForFunction(()=>window.EduRealtime?.connected);await u.p.evaluate(()=>{window.rtEvents=[];window.EduRealtime.subscribe(e=>{if(e.type==='update')window.rtEvents.push(e.data);});});}
 async function send(p,text){const root=p.locator('#chat-content');await p.waitForFunction(()=>!document.querySelector('.compose')?.dataset.submitting);console.log('Sending '+text);await root.locator('.compose textarea').fill(text);await Promise.all([p.waitForResponse(r=>r.url().includes('/TinNhan/Gui')&&r.request().method()==='POST'),root.locator('.compose button[type=submit],.compose button:not([type])').last().click()]);await root.getByText(text,{exact:true}).waitFor();await p.waitForFunction(()=>!document.querySelector('.compose')?.dataset.submitting);console.log('Sent '+text);}
 try{
  const a=await user(f.tutor),b=await user(f.learner);
  await open(a,'/HocTap/TinNhan/Index?id='+f.room);await open(b,'/HocTap/TinNhan/Index?id='+f.room);
  const text='rt-'+Date.now();await b.p.locator('.compose textarea').fill('Draft retained');const start=Date.now();await send(a.p,text);
  await b.p.getByText(text,{exact:true}).waitFor({timeout:10000});assert.equal(await b.p.locator('.compose textarea').inputValue(),'Draft retained');
  assert.ok(await b.p.evaluate(()=>window.rtEvents.some(e=>e.loai==='TinNhan')));checks.push({name:'Private message arrives via SignalR with polling disabled and draft preserved',elapsedMs:Date.now()-start});
  sql(`UPDATE dbo.SuKienChoXuLy SET TrangThai='ChoXuLy',LeaseId=NULL,LeaseHetHanLuc=NULL,ThuLaiLuc=NULL WHERE LoaiSuKien='TinNhan' AND JSON_VALUE(DuLieuToiThieu,'$.target') IN(SELECT CONVERT(nvarchar(36),Id) FROM dbo.TinNhan WHERE NoiDung=N'${text}');`);
  await b.p.waitForTimeout(1300);assert.equal(await b.p.getByText(text,{exact:true}).count(),1);checks.push({name:'Redelivered event does not duplicate message'});
  b.block(true);await b.p.waitForFunction(()=>!window.EduRealtime.connected);
  const missed='missed-'+Date.now();await send(a.p,missed);b.block(false);
  await b.p.waitForFunction(()=>window.EduRealtime.connected,{},{timeout:20000});await b.p.getByText(missed,{exact:true}).waitFor({timeout:10000});checks.push({name:'Reconnect restores group and catches up missed SQL history'});
  const outside=await user(f.outsider);const denied=await outside.p.evaluate(async({base,room})=>{const c=new signalR.HubConnectionBuilder().withUrl(base+'/cap-nhat').build();try{await c.start();try{await c.invoke('ThamGiaHoiThoai',room);return false;}catch{return true;}}finally{await c.stop();}},{base,room:f.room});assert.equal(denied,true);checks.push({name:'Outsider cannot join known private room'});
  const response=await b.ctx.request.post(base+'/cap-nhat/negotiate?negotiateVersion=1',{headers:{Origin:'https://untrusted.example'}});assert.equal(response.status(),403);checks.push({name:'Cross-origin hub negotiation is rejected'});
  const classDenied=await outside.p.evaluate(async room=>{const c=new signalR.HubConnectionBuilder().withUrl('/cap-nhat').build();try{await c.start();try{await c.invoke('ThamGiaHoiThoai',room);return false;}catch{return true;}}finally{await c.stop();}},f.classRoom);assert.equal(classDenied,true);checks.push({name:'Outsider cannot join known class room'});
  const anonymous=await browser.newContext(),anonymousPage=await anonymous.newPage();await anonymousPage.goto(base+'/lib/bootstrap/dist/css/bootstrap.min.css');await anonymousPage.addScriptTag({url:base+'/lib/signalr/signalr.min.js'});
  const anonymousDenied=await anonymousPage.evaluate(async()=>{const c=new signalR.HubConnectionBuilder().withUrl('/cap-nhat').build();try{await c.start();await c.invoke('KiemTraPhien');return false;}catch{return true;}finally{await c.stop();}});assert.equal(anonymousDenied,true);await anonymous.close();checks.push({name:'Hub rejects missing session without falling back to sample student'});
  await b.p.locator('.compose textarea').fill('');await open(b,'/');await b.p.waitForTimeout(1000);
  const before=Number(await b.p.locator('#notification-badge').textContent());
  sql(`BEGIN TRAN; DECLARE @e uniqueidentifier=NEWID();INSERT dbo.SuKienChoXuLy(Id,LoaiSuKien,DuLieuToiThieu,TaoLuc,TrangThai) VALUES(@e,N'KetNoi',N'{"recipient":"${f.learner}","target":"${f.classId}"}',SYSDATETIMEOFFSET(),'ChoXuLy');INSERT dbo.ThongBao(Id,NguoiNhanId,LoaiThongBao,DoiTuongId,NoiDungTomTat,TaoLuc,MaSuKien) VALUES(NEWID(),'${f.learner}','KetNoi','${f.classId}',N'Realtime browser test',SYSDATETIMEOFFSET(),@e);COMMIT;`);
  await b.p.waitForFunction(n=>Number(document.getElementById('notification-badge').textContent)>n,before);checks.push({name:'Committed notification refreshes unread badge without polling'});
  await open(a,'/HocTap/LopHoc/ChiTiet/'+f.classId+'?tab=chat');await open(b,'/HocTap/LopHoc/ChiTiet/'+f.classId+'?tab=chat');
  const af=a.p.frameLocator('iframe'),bf=b.p.frameLocator('iframe');await af.locator('.compose textarea').waitFor();await bf.locator('.compose textarea').waitFor();
  assert.equal(await a.p.locator('iframe').evaluate(frame=>frame.contentWindow.EduRealtime===window.EduRealtime),true);
  const classText='class-rt-'+Date.now();await af.locator('.compose textarea').fill(classText);await af.locator('.compose button[type=submit],.compose button:not([type])').last().click();await bf.getByText(classText,{exact:true}).waitFor({timeout:10000});checks.push({name:'Class iframe shares parent connection and receives message'});
  const fallback=await user(f.learner,false);await fallback.ctx.route('**/lib/signalr/signalr.min.js*',route=>route.abort());await fallback.p.goto(base+'/HocTap/TinNhan/Index?id='+f.room);assert.equal(await fallback.p.evaluate(()=>!!window.EduRealtime),false);
  await open(a,'/HocTap/TinNhan/Index?id='+f.room);const fallbackText='fallback-'+Date.now();await send(a.p,fallbackText);await fallback.p.getByText(fallbackText,{exact:true}).waitFor({timeout:10000});checks.push({name:'HTTP chat and polling remain functional when SignalR client is unavailable'});await fallback.ctx.close();
  const second=await a.ctx.newPage();await second.goto(base);await second.locator('.preview summary').click();await second.locator('.preview select').selectOption(f.learner);await Promise.all([second.waitForEvent('framenavigated',frame=>frame===second.mainFrame()),second.locator('.preview button').click()]);
  await a.p.waitForFunction(()=>!window.EduRealtime.connected);await a.p.waitForFunction(()=>document.querySelector('[data-realtime-status]')?.textContent.includes('Phiên đã thay đổi'),{},{timeout:15000});checks.push({name:'Changing account in another tab stops stale identity connection'});
  assert.deepEqual(errors,[]);await fs.writeFile('docs/qa/realtime-browser-results.json',JSON.stringify({checks,errors,database:f.database},null,2));console.log(JSON.stringify(checks,null,2));
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
