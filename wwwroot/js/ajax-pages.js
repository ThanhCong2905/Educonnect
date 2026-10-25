(() => {
 'use strict';
 const ui=window.EduConnect,headers={'X-Requested-With':'XMLHttpRequest','X-Edu-Partial':'true'};
 const requests=new Map(),drafts=new Map(),selected=new Set();
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
 function rememberDraft(root){const input=root.querySelector('.compose textarea');if(input&&root.dataset.chatId)drafts.set(root.dataset.chatId,input.value);}
 function replace(root,fresh,url,historyMode,scroll){
  rememberDraft(root);rememberComparison(root);
  if(historyMode)history[historyMode+'State']({eduRegion:root.id},'',url);
  root.replaceWith(fresh);restoreComparison(fresh);ui.init(fresh);
  const input=fresh.querySelector('.compose textarea');if(input&&drafts.has(fresh.dataset.chatId))input.value=drafts.get(fresh.dataset.chatId);
  if(scroll)fresh.querySelector('.search-results-toolbar,.chat-header')?.scrollIntoView({block:'start',behavior:matchMedia('(prefers-reduced-motion: reduce)').matches?'auto':'smooth'});
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
  event.preventDefault();const url=new URL(form.action||location.href,location.href);url.search=new URLSearchParams(new FormData(form)).toString();
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
 async function chatRefresh(root,url=location.href,{sent=false}={}){
  if(root.querySelector('[data-submitting]')&&!sent)return;
  const active=new URL(url,location.href);if(root.dataset.chatId)active.searchParams.set('id',root.dataset.chatId);url=active.href;
  const fresh=await load(root,url,!sent);if(!fresh)return false;
  if(root.dataset.chatId!==fresh.dataset.chatId){if(!root.dataset.chatId)replace(root,fresh,url,null,false);return true;}
  swap(root.querySelector('.conversation-list'),fresh.querySelector('.conversation-list'));
  const stream=root.querySelector('.chat-stream'),incoming=fresh.querySelector('.chat-stream');
  if(stream&&incoming&&(sent||Number(root.dataset.chatPage||1)===1)&&stream.innerHTML!==incoming.innerHTML){
   const atBottom=stream.scrollHeight-stream.scrollTop-stream.clientHeight<64,top=stream.scrollTop;
   const anchor=[...stream.querySelectorAll('[data-message-id]')].find(node=>node.offsetTop>=stream.scrollTop);
   const anchorOffset=anchor?anchor.offsetTop-stream.scrollTop:0,anchorId=anchor?.dataset.messageId;
   stream.replaceChildren(...incoming.childNodes);
   if(atBottom||sent)stream.scrollTop=stream.scrollHeight;
   else{
    const kept=[...stream.querySelectorAll('[data-message-id]')].find(node=>node.dataset.messageId===anchorId);
    stream.scrollTop=kept?kept.offsetTop-anchorOffset:top;
    if(!root.querySelector('[data-new-messages]')){const button=document.createElement('button');button.type='button';button.className='btn btn-outline-primary new-messages';button.dataset.newMessages='true';button.textContent='Có tin nhắn mới · Xem';button.addEventListener('click',()=>{stream.scrollTop=stream.scrollHeight;button.remove();});stream.before(button);}
   }
  }
  const current=root.querySelector('.compose'),next=fresh.querySelector('.compose');
  if(current&&!next){current.replaceWith(fresh.querySelector('.chat-panel>p')||document.createTextNode('Bạn không còn quyền gửi tin trong lớp này.'));}
  else if(!current&&next){root.querySelector('.chat-panel>p')?.remove();root.querySelector('.chat-panel')?.append(next);ui.init(next.parentElement);}
  if(sent&&current&&next){
   root.dataset.chatPage=fresh.dataset.chatPage;
   swap(root.querySelector('.pagination'),fresh.querySelector('.pagination'));
   swap(root.querySelector('.inline'),fresh.querySelector('.inline'));
   history.replaceState({...history.state,eduRegion:root.id},'',url);
   const input=current.querySelector('textarea');if(input?.value===sent){input.value='';drafts.delete(root.dataset.chatId);}
   const token=current.querySelector('[name="MaThaoTac"]');if(token)token.value=next.querySelector('[name="MaThaoTac"]').value;
   root.querySelector('.ui-error-summary')?.remove();root.querySelector('[data-new-messages]')?.remove();input?.focus();
  }
  status(root,'');return true;
 }
 ui.afterAjaxSubmit=async(form,result,data)=>{
  if(form.hasAttribute('data-chat-send')){
   const root=form.closest('[data-ajax-page]'),text=data.get('NoiDung');
   const ok=await chatRefresh(root,result.redirectUrl||location.href,{sent:text});
   if(!ok)ui.showFormErrors(form,{message:'Tin đã được gửi nhưng chưa tải được hội thoại. Thử tải lại để xem.'});
   return true;
  }
  const root=pageRegion();
  if(form.closest('#workspace-modal')&&result.message&&root&&['search','home'].includes(root.dataset.ajaxPage)){
   const ok=await navigate(root,location.href,{historyMode:null});ui.closeModal();ui.toast(result.message);if(!ok)status(root,'Đã lưu. Hãy tải lại danh sách để xem thay đổi.');return true;
  }
  return false;
 };
 let polling=false,round=0;
 async function poll(){
  if(polling||document.hidden)return;polling=true;round++;
  try{
   const chat=document.querySelector('#chat-content');
   if(chat&&!document.querySelector('#workspace-modal.show'))await chatRefresh(chat);
   if(round%5===0||round===1){
    const badge=document.getElementById('notification-badge');
    if(badge){try{const response=await fetch('/Home/ChuaDoc',{headers,cache:'no-store'});if(response.ok){const data=await response.json();badge.textContent=data.count;badge.hidden=!data.count;badge.setAttribute('aria-label',data.count+' thông báo chưa đọc');}}catch{}}
    const notices=document.getElementById('notification-list');
    if(notices&&!notices.querySelector('[data-submitting]')){const fresh=await load(notices,'/Home/ThongBao',true);if(fresh){const parent=notices.closest('.modal-body'),top=parent?.scrollTop;swap(notices,fresh);ui.markRequired(notices);if(parent)parent.scrollTop=top;}}
   }
   const home=document.getElementById('home-updates');
   if(home&&round%10===0&&!document.querySelector('#workspace-modal.show')){const fresh=await load(home,location.href,true);if(fresh){home.replaceWith(fresh);ui.init(fresh);}}
  }finally{polling=false;}
 }
 setInterval(poll,3000);
 document.addEventListener('visibilitychange',()=>{if(!document.hidden)poll();});
 ui.refreshLive=()=>{round=0;return poll();};
 setTimeout(poll,500);
})();
