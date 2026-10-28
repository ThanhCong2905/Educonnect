(() => {
 'use strict';
 const ui=window.EduConnect,headers={'X-Requested-With':'XMLHttpRequest','X-Edu-Partial':'true'};
 const requests=new Map(),drafts=new Map(),fileDrafts=new Map(),selected=new Set();
 const pageRegion=()=>document.querySelector('main>[data-ajax-page],main>section[data-ajax-page]');
 function status(root,message){
  let box=root.querySelector(':scope>.ajax-status');
  if(!message){box?.remove();return;}
  if(!box){box=document.createElement('p');box.className='ajax-status';box.setAttribute('role','status');root.prepend(box);}
  box.textContent=message;
 }
 async function load(root,url,quiet=false){
  const key=root.id,target=new URL(url,location.href);
  if(target.origin!==location.origin)throw new Error('Địa chỉ không hợp lệ.');
  if(requests.has(key)){if(quiet)return null;requests.get(key).abort();}
  const controller=new AbortController();requests.set(key,controller);
  if(!quiet){root.setAttribute('aria-busy','true');root.classList.add('ajax-loading');status(root,'Đang cập nhật…');}
  try{
   const response=await fetch(target,{headers,signal:controller.signal,cache:'no-store'});
   if(!response.ok){const problem=await response.json().catch(()=>({}));throw new Error(problem.message||'Chưa thể cập nhật. Bạn có thể thử lại; nội dung hiện tại vẫn được giữ.');}
   const html=await response.text();
   if(requests.get(key)!==controller)return null;
   const doc=new DOMParser().parseFromString(html,'text/html'),fresh=doc.getElementById(key);
   if(!fresh)throw new Error('Nội dung chưa sẵn sàng. Hãy tải lại trang.');
   fresh.querySelectorAll('script').forEach(script=>script.remove());
   return fresh;
  }catch(error){if(error.name!=='AbortError')status(root,error.message);return null;}
  finally{if(requests.get(key)===controller){requests.delete(key);root.removeAttribute('aria-busy');root.classList.remove('ajax-loading');if(root.querySelector(':scope>.ajax-status')?.textContent==='Đang cập nhật…')status(root,'');}}
 }
 function rememberComparison(root){
  if(root.dataset.ajaxPage!=='search')return;
  root.querySelectorAll('input[form="compare-form"]').forEach(input=>{if(input.checked)selected.add(input.value);else selected.delete(input.value);});
 }
 function restoreComparison(root){
  if(root.dataset.ajaxPage!=='search')return;
  const visible=new Set();root.querySelectorAll('input[form="compare-form"]').forEach(input=>{visible.add(input.value);input.checked=selected.has(input.value);});
  const form=root.querySelector('#compare-form');
  if(form)for(const id of selected){if(visible.has(id))continue;const input=document.createElement('input');input.type='checkbox';input.hidden=true;input.name='ids';input.value=id;input.checked=true;input.setAttribute('form','compare-form');input.dataset.compareHeld='true';form.append(input);}
 }
 function rememberDraft(root){const input=root.querySelector('.compose textarea');if(input&&root.dataset.chatId){drafts.set(root.dataset.chatId,input.value);const files=[...(root.querySelector('[data-chat-files]')?.files||[])];if(files.length)fileDrafts.set(root.dataset.chatId,files);else fileDrafts.delete(root.dataset.chatId);}}
 function replace(root,fresh,url,historyMode,scroll){
  const active=root.contains(document.activeElement)?document.activeElement:null;
  const focusName=active?.getAttribute('name'),focusId=active?.id;
  rememberDraft(root);rememberComparison(root);
  if(historyMode)history[historyMode+'State']({eduRegion:root.id},'',url);
  root.replaceWith(fresh);restoreComparison(fresh);ui.init(fresh);initChat(fresh);
  const input=fresh.querySelector('.compose textarea');if(input&&drafts.has(fresh.dataset.chatId))input.value=drafts.get(fresh.dataset.chatId);
  const picker=fresh.querySelector('[data-chat-files]');if(picker&&fileDrafts.has(fresh.dataset.chatId)){const transfer=new DataTransfer();for(const file of fileDrafts.get(fresh.dataset.chatId))transfer.items.add(file);picker.files=transfer.files;picker.dispatchEvent(new Event('change',{bubbles:true}));}
  const heading=fresh.querySelector('.search-results-toolbar h2,.chat-header h2');
  if(active){
   const focus=focusId?fresh.querySelector('#'+CSS.escape(focusId)):focusName?fresh.querySelector('[name="'+CSS.escape(focusName)+'"]'):heading;
   if(focus){if(focus===heading)focus.tabIndex=-1;focus.focus({preventScroll:true});}
  }
  if(fresh.dataset.ajaxPage==='search'&&heading){let announcement=document.querySelector('[data-ajax-announcement]');if(!announcement){announcement=document.createElement('div');announcement.className='visually-hidden';announcement.dataset.ajaxAnnouncement='';announcement.setAttribute('role','status');announcement.setAttribute('aria-live','polite');document.body.append(announcement);}announcement.textContent='Kết quả tìm kiếm: '+heading.textContent.trim();}
  if(scroll)fresh.querySelector('.search-results-toolbar,.chat-header')?.scrollIntoView({block:'start',behavior:ui.motionBehavior()});
 }
 async function navigate(root,url,{historyMode='push',scroll=false}={}){
  rememberComparison(root);
  const fresh=await load(root,url);if(fresh)replace(root,fresh,url,historyMode,scroll);
  return !!fresh;
 }
 const initial=pageRegion();if(initial)history.replaceState({...history.state,eduRegion:initial.id},'',location.href);
 document.addEventListener('submit',event=>{
  const form=event.target,root=form.closest('[data-ajax-page]');
  if(event.defaultPrevented||form.method.toLowerCase()!=='get'||!root||root.closest('#workspace-modal'))return;
  if(root.dataset.ajaxPage!=='search'&&root.dataset.ajaxPage!=='chat')return;
  if(form.id==='compare-form')return;
  event.preventDefault();const url=ui.formUrl(form);url.search=new URLSearchParams(new FormData(form)).toString();
  if(root.dataset.ajaxPage==='search')url.searchParams.delete('Trang');
  navigate(root,url.href);
 });
 document.addEventListener('click',event=>{
  const refresh=event.target.closest('[data-refresh-home]');
  if(refresh){const root=refresh.closest('[data-ajax-page]');if(root)navigate(root,location.href,{historyMode:null});return;}
  const link=event.target.closest('a');
  if(!link||event.defaultPrevented||event.button!==0||event.ctrlKey||event.metaKey||event.shiftKey||event.altKey||link.hasAttribute('data-modal'))return;
  const root=link.closest('[data-ajax-page]');if(!root||root.closest('#workspace-modal'))return;
  const target=new URL(link.href,location.href);if(target.origin!==location.origin)return;
  const search=root.dataset.ajaxPage==='search'&&(link.closest('.pagination,.active-filters')||(!target.search&&/\/TimKiem\/TimKiem(?:\/Index)?$/i.test(target.pathname)));
  const chat=root.dataset.ajaxPage==='chat'&&link.closest('.conversation-list,.pagination');
  if(!search&&!chat)return;
  event.preventDefault();navigate(root,target.href,{scroll:!!link.closest('.pagination')});
 });
 window.addEventListener('popstate',event=>{const root=pageRegion();if(root&&event.state?.eduRegion===root.id)navigate(root,location.href,{historyMode:null});});
 document.addEventListener('change',event=>{if(event.target.matches('input[form="compare-form"]'))rememberComparison(event.target.closest('[data-ajax-page]'));});
 document.addEventListener('click',event=>{if(event.target.closest('[data-compare-clear]'))selected.clear();});
 function swap(old,fresh){if(old&&fresh&&old.innerHTML!==fresh.innerHTML){old.replaceChildren(...fresh.childNodes);}}
 const chatStates=new WeakMap();
 let watchedRoom=null,unwatch=null;
 function watchChat(root){
  const id=root?.dataset.chatId||null;if(id===watchedRoom)return;
  unwatch?.();watchedRoom=id;unwatch=window.EduRealtime?.watch(id);
 }
 function initChat(root){
  watchChat(root);
  if(!root||!root.dataset.chatId||chatStates.has(root))return;
  const stream=root.querySelector('.chat-stream');if(!stream)return;
  chatStates.set(root,{olderBusy:false,refresh:null});
  requestAnimationFrame(()=>{stream.scrollTop=stream.scrollHeight;});
  stream.addEventListener('scroll',()=>{
   if(stream.scrollTop<60)olderMessages(root);
   if(stream.scrollHeight-stream.scrollTop-stream.clientHeight<60)root.querySelector('[data-new-messages]').hidden=true;
  },{passive:true});
  root.querySelector('[data-chat-older]')?.addEventListener('click',()=>olderMessages(root));
  root.querySelector('[data-new-messages]')?.addEventListener('click',()=>{stream.scrollTo({top:stream.scrollHeight,behavior:ui.motionBehavior()});root.querySelector('[data-new-messages]').hidden=true;});
  root.querySelector('.compose textarea')?.addEventListener('keydown',event=>{
   if(event.key==='Enter'&&!event.shiftKey&&!event.isComposing&&!event.ctrlKey&&!event.altKey){event.preventDefault();const form=event.target.form;if(!form.dataset.submitting&&(event.target.value.trim()||form.querySelector('[data-chat-files]')?.files.length))form.requestSubmit(form.querySelector('button[type="submit"],button:not([type])'));}
  });
  const picker=root.querySelector('[data-chat-files]'),clear=root.querySelector('[data-clear-chat-files]'),selection=root.querySelector('[data-chat-file-selection]');
  const selectedFiles=()=>{const files=[...(picker?.files||[])];if(selection)selection.textContent=files.map(f=>f.name+' ('+ui.fileSize(f.size)+')').join(' · ');if(clear)clear.hidden=!files.length;};
  picker?.addEventListener('change',selectedFiles);clear?.addEventListener('click',()=>{picker.value='';picker.dispatchEvent(new Event('input',{bubbles:true}));picker.dispatchEvent(new Event('change',{bubbles:true}));picker.focus();});selectedFiles();
 }
 function messageNode(item){
  const article=document.createElement('article');article.className='bubble'+(item.mine?' mine':'');article.dataset.messageId=item.id;
  const sender=document.createElement('small'),body=document.createElement('p'),time=document.createElement('time');
  sender.textContent=item.sender;body.className='pre';body.textContent=item.text;time.textContent=item.time;article.append(sender);if(item.text)article.append(body);
  if(item.attachments?.length){const list=document.createElement('div');list.className='message-attachments';for(const f of item.attachments){
   const block=document.createElement('div');block.className='message-attachment';const url='/HocTap/TinNhan/Tai/'+encodeURIComponent(f.id);
   if(['image/png','image/jpeg'].includes(f.mime)){const view=document.createElement('a');view.href=url+'?xem=true';view.target='_blank';view.rel='noopener';view.setAttribute('aria-label','Xem ảnh '+f.name);const img=document.createElement('img');img.className='message-image';img.src=view.href;img.alt=f.name;img.loading='lazy';img.width=260;img.height=180;view.append(img);block.append(view);}
   const link=document.createElement('a');link.href=url;link.textContent=f.name;const size=document.createElement('small');size.textContent=ui.fileSize(f.size)+' · Tải xuống';block.append(link,size);list.append(block);
  }article.append(list);}article.append(time);return article;
 }
 async function chatData(root,direction,cursor){
  const url=new URL(root.dataset.chatFeed,location.href);url.searchParams.set('id',root.dataset.chatId);
  if(cursor)url.searchParams.set(direction,cursor);
  if(root.dataset.messageQuery)url.searchParams.set('message',root.dataset.messageQuery);
  const response=await fetch(url,{headers,cache:'no-store'}),data=await response.json();
  if(!response.ok)throw new Error(data.message||'Chưa tải được tin nhắn. Hãy thử lại.');
  return data;
 }
 async function olderMessages(root){
  const state=chatStates.get(root),button=root.querySelector('[data-chat-older]');
  if(!state||state.olderBusy||!button||button.hidden)return;
  const stream=root.querySelector('.chat-stream'),list=root.querySelector('.chat-messages'),first=list.querySelector('[data-message-id]');if(!first)return;
  state.olderBusy=true;button.disabled=true;const hint=root.querySelector('[data-chat-history-status]');hint.textContent='Đang tải…';
  try{
   const data=await chatData(root,'before',first.dataset.messageId);if(!root.isConnected)return;
   const anchor=first.getBoundingClientRect().top;
   const fragment=document.createDocumentFragment();for(const item of data.items)if(!list.querySelector(`[data-message-id="${item.id}"]`))fragment.append(messageNode(item));
   list.prepend(fragment);button.hidden=!data.hasMore;hint.textContent=data.hasMore?'':'Đã đến đầu cuộc trò chuyện';
   stream.scrollTop+=first.getBoundingClientRect().top-anchor;
  }catch(error){hint.textContent=error.message;}
  finally{state.olderBusy=false;button.disabled=false;}
 }
 async function chatRefresh(root,url=location.href,{sent=false}={}){
  initChat(root);const state=chatStates.get(root);if(!state)return true;
  if(state.refresh){await state.refresh;if(!root.isConnected)return true;}
  state.refresh=(async()=>{
   const stream=root.querySelector('.chat-stream'),list=root.querySelector('.chat-messages');
   try{
    let more=true,rounds=0;
    while(more&&rounds++<10){
     const last=list.querySelector('[data-message-id]:last-child'),data=await chatData(root,'after',last?.dataset.messageId);
     if(!root.isConnected)return false;
     const nearBottom=stream.scrollHeight-stream.scrollTop-stream.clientHeight<64;if(!last&&data.hasMore)root.querySelector('[data-chat-older]').hidden=false;
     let added=0;for(const item of data.items){if(list.querySelector(`[data-message-id="${item.id}"]`))continue;list.querySelector('.chat-empty')?.remove();list.append(messageNode(item));added++;}
     if(added){if(nearBottom||sent){stream.scrollTop=stream.scrollHeight;root.querySelector('[data-new-messages]').hidden=true;}else root.querySelector('[data-new-messages]').hidden=false;}
     const compose=root.querySelector('.compose');if(compose)compose.hidden=!data.canSend;
     const readonly=root.querySelector('.chat-readonly');if(readonly)readonly.hidden=data.canSend;
     more=!!last&&data.hasMore;
    }
    if(sent){
     const form=root.querySelector('.compose'),input=form?.querySelector('textarea');if(input?.value===sent.text){input.value='';drafts.delete(root.dataset.chatId);}
     const picker=form?.querySelector('[data-chat-files]');const keys=files=>files.map(f=>[f.name,f.size,f.lastModified].join('|')).join(';');if(picker&&keys([...picker.files])===keys(sent.files)){picker.value='';fileDrafts.delete(root.dataset.chatId);picker.dispatchEvent(new Event('change',{bubbles:true}));}
     const token=form?.querySelector('[name="MaThaoTac"]');if(token)token.value=crypto.randomUUID();
     form?.querySelectorAll('.ui-error-summary,.ui-field-error').forEach(node=>node.remove());input?.focus();
     form?.querySelectorAll('[aria-invalid="true"]').forEach(input=>{input.classList.remove('is-invalid');input.removeAttribute('aria-invalid');input.setAttribute('aria-describedby',(input.getAttribute('aria-describedby')||'').split(' ').filter(id=>!id.startsWith('ui-error-')).join(' '));});
    }
    status(root,'');return true;
   }catch(error){status(root,error.message);return false;}
  })();
  try{return await state.refresh;}finally{state.refresh=null;}
 }
 ui.afterAjaxSubmit=async(form,result,data)=>{
  if(form.hasAttribute('data-chat-send')){
   const root=form.closest('[data-ajax-page]');
   const ok=await chatRefresh(root,result.redirectUrl||location.href,{sent:{text:data.get('NoiDung')||'',files:data.getAll('Files').filter(f=>f.name)}});
   if(!ok)ui.showFormErrors(form,{message:'Tin đã được gửi nhưng chưa tải được hội thoại. Thử tải lại để xem.'});
   return true;
  }
  const root=pageRegion();
  if(form.closest('#workspace-modal')&&result.message&&root&&['search','home'].includes(root.dataset.ajaxPage)){
   const ok=await navigate(root,location.href,{historyMode:null});ui.closeModal();ui.toast(result.message);if(!ok)status(root,'Đã lưu. Hãy tải lại danh sách để xem thay đổi.');return true;
  }
  return false;
 };
 let polling=false,round=0,repoll=false;
 async function poll(force=false){
  if(document.hidden)return;
  if(polling){if(force)repoll=true;return;}polling=true;round++;
  try{
   const chat=document.querySelector('#chat-content');
   watchChat(chat);
   const connected=window.EduRealtime?.connected;
   if(chat&&(force||!connected||round%10===0)&&!document.querySelector('#workspace-modal.show'))await chatRefresh(chat);
   if(force||round===1||round%(connected?20:5)===0){
    const badge=document.getElementById('notification-badge');
    if(badge){try{const response=await fetch('/Home/ChuaDoc',{headers,cache:'no-store'});if(response.ok){const data=await response.json();badge.textContent=data.count;badge.hidden=!data.count;badge.setAttribute('aria-label',data.count+' thông báo chưa đọc');}}catch{}}
    const notices=document.getElementById('notification-list');
    if(notices&&!notices.querySelector('[data-submitting]')){const fresh=await load(notices,'/Home/ThongBao',true);if(fresh){const parent=notices.closest('.modal-body'),top=parent?.scrollTop;swap(notices,fresh);ui.markRequired(notices);if(parent)parent.scrollTop=top;}}
   }
   const home=document.getElementById('home-updates');
   if(home&&round%10===0&&!document.querySelector('#workspace-modal.show')){const fresh=await load(home,location.href,true);if(fresh){home.replaceWith(fresh);ui.init(fresh);}}
  }finally{polling=false;if(repoll){repoll=false;queueMicrotask(()=>poll(true));}}
 }
 setInterval(poll,3000);
 document.addEventListener('visibilitychange',()=>{if(!document.hidden)poll(true);});
 ui.refreshLive=()=>{round=0;return poll(true);};
 async function refreshConnections(){
  const region=document.getElementById('connection-content');if(!region||document.hidden)return;
  const dirty=[...region.querySelectorAll('input,textarea,select')].some(el=>el.type==='file'?el.files.length:el.type==='checkbox'||el.type==='radio'?el.checked!==el.defaultChecked:el.tagName==='SELECT'?[...el.options].some(o=>o.selected!==o.defaultSelected):el.value!==el.defaultValue);
  if(dirty||region.contains(document.activeElement)||document.querySelector('#workspace-modal.show')){
   if(!region.querySelector('[data-live-changes]')){const button=document.createElement('button');button.type='button';button.className='btn btn-outline-primary mb-3';button.dataset.liveChanges='';button.textContent='Có cập nhật mới · Xem';button.addEventListener('click',async()=>{const fresh=await load(region,location.href,true);if(fresh){region.replaceWith(fresh);ui.init(fresh);}});region.prepend(button);}return;
  }
  const fresh=await load(region,location.href,true);if(fresh){region.replaceWith(fresh);ui.init(fresh);}
 }
 const unsubscribe=window.EduRealtime?.subscribe(event=>{
  if(event.type==='state'){
   let hint=document.querySelector('[data-realtime-status]');
   if(!hint){hint=document.createElement('small');hint.dataset.realtimeStatus='';hint.className='text-muted ms-2';hint.setAttribute('role','status');(document.querySelector('.chat-header')||document.querySelector('.shell>header'))?.append(hint);}
   if(hint){hint.hidden=event.connected;hint.textContent=event.stopped?'Phiên đã thay đổi. Hãy tải lại trang.':'Đang kết nối lại · vẫn có thể thao tác';}
  }else if(event.type==='resync'){poll(true);refreshConnections();}
  else if(event.type==='update'){
   if(event.data.loai!=='TinNhan'||event.data.hoiThoaiId===document.querySelector('#chat-content')?.dataset.chatId)poll(true);
   if(event.data.loai==='KetNoi')refreshConnections();
  }
 });
 window.addEventListener('pagehide',event=>{if(!event.persisted){unwatch?.();unsubscribe?.();}});
 initChat(document.querySelector('#chat-content'));
 setTimeout(poll,500);
})();
