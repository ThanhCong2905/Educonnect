(() => {
 'use strict';
 // Class chat lives in an iframe; share its parent's connection, not a second socket.
 if(window.parent!==window){try{if(window.parent.EduRealtime){window.EduRealtime=window.parent.EduRealtime;return;}}catch{}}
 if(window.EduRealtime||!window.signalR)return;
 const expected=document.querySelector('meta[name="edu-user"]')?.content;
 const listeners=new Set(),rooms=new Map(),seen=new Set();
 const connection=new signalR.HubConnectionBuilder().withUrl('/cap-nhat')
  .withAutomaticReconnect([0,2000,10000,30000]).configureLogging(signalR.LogLevel.Error).build();
 let ready=false,stopped=false,retry=null,joining=Promise.resolve();
 const announce=()=>{for(const fn of listeners){try{fn({type:'state',connected:ready,stopped});}catch{}}};
 async function restore(){
  const user=await connection.invoke('KiemTraPhien');
  if(user!==expected){stopped=true;ready=false;await connection.stop();announce();return;}
  for(const id of rooms.keys())await connection.invoke('ThamGiaHoiThoai',id).catch(()=>{});
  ready=true;announce();
  for(const fn of listeners){try{fn({type:'resync'});}catch{}}
 }
 async function start(){
  if(stopped||connection.state!==signalR.HubConnectionState.Disconnected)return;
  try{await connection.start();await restore();}
  catch{ready=false;announce();await connection.stop().catch(()=>{});if(!stopped){clearTimeout(retry);retry=setTimeout(start,5000);}}
 }
 connection.on('CapNhat',data=>{
  if(!ready||!data?.maSuKien||seen.has(data.maSuKien))return;
  seen.add(data.maSuKien);if(seen.size>512)seen.delete(seen.values().next().value);
  for(const fn of listeners){try{fn({type:'update',data});}catch{}}
 });
 connection.onreconnecting(()=>{ready=false;announce();});
 connection.onreconnected(()=>restore().catch(()=>connection.stop()));
 connection.onclose(()=>{ready=false;announce();if(!stopped){clearTimeout(retry);retry=setTimeout(start,5000);}});
 window.EduRealtime={
  get connected(){return ready;},
  subscribe(fn){listeners.add(fn);fn({type:'state',connected:ready,stopped});return()=>listeners.delete(fn);},
  watch(id){
   if(!id)return()=>{};
   rooms.set(id,(rooms.get(id)||0)+1);
   joining=joining.then(async()=>{if(ready&&rooms.has(id)){await connection.invoke('ThamGiaHoiThoai',id);for(const fn of listeners){try{fn({type:'resync'});}catch{}}}}).catch(()=>{});
   return()=>{const count=(rooms.get(id)||1)-1;if(count)rooms.set(id,count);else{rooms.delete(id);joining=joining.then(async()=>{if(ready)await connection.invoke('RoiHoiThoai',id);}).catch(()=>{});}};
  }
 };
 setInterval(()=>{if(ready)connection.invoke('KiemTraPhien').then(user=>{if(user!==expected){stopped=true;connection.stop();}}).catch(()=>connection.stop());},20000);
 window.addEventListener('pagehide',()=>{stopped=true;clearTimeout(retry);connection.stop();});
 window.addEventListener('pageshow',event=>{if(event.persisted){stopped=false;start();}});
 start();
})();
