(() => {
 const user=document.querySelector('meta[name="edu-user"]')?.content;
 if(!user)return;
 const prefix=`edu:need:${user}:`;
 const read=key=>{try{return JSON.parse(sessionStorage.getItem(key));}catch{return null;}};
 const pending=read(prefix+'pending');
 if(pending&&document.querySelector('[data-notification-text]')?.textContent.trim()){
  sessionStorage.removeItem(pending);sessionStorage.removeItem(prefix+'pending');
 }
 function init(){
  document.querySelectorAll('form.need-form:not([data-recovery-ready])').forEach(form=>{
   form.dataset.recoveryReady='true';
   const key=prefix+(form.querySelector('[name=MaNhuCau]')?.value||'new');
   const fields=()=>[...form.elements].filter(e=>e.name&&!['hidden','file','submit','button'].includes(e.type));
   const save=()=>{try{sessionStorage.setItem(key,JSON.stringify({time:Date.now(),values:fields().map(e=>({name:e.name,value:e.value,checked:e.checked}))}));}catch{}};
   const saved=read(key);
   if(saved&&Date.now()-saved.time<86400000){
    const bar=document.createElement('div');bar.className='recovery-notice';
    const text=document.createElement('span');text.textContent='Có nội dung chưa gửi trong tab này.';
    const restore=document.createElement('button');restore.type='button';restore.className='btn btn-outline-primary btn-sm';restore.textContent='Khôi phục';
    const discard=document.createElement('button');discard.type='button';discard.className='btn btn-outline-secondary btn-sm';discard.textContent='Bỏ bản lưu';
    bar.append(text,restore,discard);form.prepend(bar);
    discard.onclick=()=>{sessionStorage.removeItem(key);bar.remove();};
    restore.onclick=()=>{
     const slots=saved.values.filter(x=>/LichRanh\[\d+\]\.Thu/.test(x.name));
     const list=form.querySelector('[data-slot-list]'),template=list?.querySelector('.slot-row')?.cloneNode(true);
     if(list&&template){list.replaceChildren();slots.forEach((_,i)=>{const row=template.cloneNode(true);row.querySelectorAll('[name]').forEach(e=>e.name=e.name.replace(/\[\d+\]/,`[${i}]`));list.append(row);});}
     for(const item of saved.values){const e=form.elements.namedItem(item.name);if(!e||!e.tagName)continue;if(e.type==='checkbox'||e.type==='radio')e.checked=item.checked;else e.value=item.value;e.dispatchEvent(new Event('change',{bubbles:true}));}
     bar.remove();save();
    };
   }
   form.addEventListener('input',save);form.addEventListener('change',save);
   form.addEventListener('submit',()=>{save();try{sessionStorage.setItem(prefix+'pending',JSON.stringify(key));}catch{}});
   window.addEventListener('pagehide',()=>{if(form.isConnected)save();});
  });
 }
 init();new MutationObserver(init).observe(document.body,{childList:true,subtree:true});
})();
