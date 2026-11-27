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
   let restoring=false,touched=false;
   const save=event=>{if(restoring)return;if(event)touched=true;if(!touched)return;try{sessionStorage.setItem(key,JSON.stringify({time:Date.now(),values:fields().map(e=>({name:e.name,value:e.value,checked:e.checked}))}));}catch{}};
   const saved=read(key);
   if(saved&&Date.now()-saved.time<86400000){
    const bar=document.createElement('div');bar.className='recovery-notice';
    const text=document.createElement('span');text.textContent='Có nội dung chưa gửi trong tab này.';
    const restore=document.createElement('button');restore.type='button';restore.className='btn btn-outline-primary btn-sm';restore.textContent='Khôi phục';
    const discard=document.createElement('button');discard.type='button';discard.className='btn btn-outline-primary btn-sm';discard.textContent='Bỏ bản lưu';
    bar.append(text,restore,discard);form.prepend(bar);
    discard.onclick=()=>{sessionStorage.removeItem(key);touched=false;bar.remove();};
    restore.onclick=async()=>{
     if(restoring)return;
     restoring=true;restore.disabled=true;discard.disabled=true;
     const submits=[...form.querySelectorAll('button:not([type]),button[type="submit"],input[type="submit"]')].map(button=>({button,disabled:button.disabled}));
     submits.forEach(({button})=>button.disabled=true);
     try{
     const slots=saved.values.filter(x=>/LichRanh\[\d+\]\.Thu/.test(x.name));
     const list=form.querySelector('[data-slot-list]'),add=form.querySelector('[data-add-slot]');
     if(list&&add){list.replaceChildren();add.disabled=false;for(const slot of slots.slice(0,35))add.click();}
     for(const item of saved.values){const e=form.elements.namedItem(item.name);if(!e||!e.tagName||e.matches('[data-province],[data-ward]'))continue;if(e.type==='checkbox'||e.type==='radio')e.checked=item.checked;else e.value=item.value;e.dispatchEvent(new Event('change',{bubbles:true}));}
     list?.dispatchEvent(new Event('edu:slots-restored'));
     for(const province of form.querySelectorAll('[data-province]')){
      province.value=saved.values.find(item=>item.name===province.name)?.value||'';
      const ward=province.closest('[data-area-picker]').querySelector('[data-ward]');
      const selectedWard=saved.values.find(item=>item.name===ward.name)?.value||'';
      if(!await window.EduConnect.updateAreaPicker(province,selectedWard)){
       text.textContent='Chưa tải được địa danh. Nội dung đã lưu vẫn được giữ; bấm Khôi phục để thử lại.';
       return;
      }
     }
     bar.remove();restoring=false;touched=true;save();
     }finally{restoring=false;restore.disabled=false;discard.disabled=false;submits.forEach(({button,disabled})=>button.disabled=disabled);}
    };
   }
   form.addEventListener('input',save);form.addEventListener('change',save);
   form.addEventListener('edu:slots-changed',save);
   form.addEventListener('submit',()=>{touched=true;save();try{sessionStorage.setItem(prefix+'pending',JSON.stringify(key));}catch{}});
   window.addEventListener('pagehide',()=>{if(form.isConnected)save();});
  });
 }
 init();new MutationObserver(init).observe(document.body,{childList:true,subtree:true});
})();
