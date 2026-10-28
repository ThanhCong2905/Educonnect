(() => {
  'use strict';
  const modalElement = document.getElementById('workspace-modal');
  const content = document.getElementById('workspace-content');
  const modal = modalElement ? bootstrap.Modal.getOrCreateInstance(modalElement) : null;
  let trigger, request, saving = false;
  const baselines=new WeakMap(),discardAllowed=new WeakSet(),completedForms=new WeakSet();
  function fingerprint(form){return JSON.stringify([...new FormData(form)].filter(([name])=>name!=='__RequestVerificationToken').map(([name,value])=>[name,typeof value==='string'?value:value.name?[value.name,value.size,value.lastModified]:null]));}
  function rememberForms(root){root.querySelectorAll('form[method="post"],form[method="POST"]').forEach(form=>baselines.set(form,fingerprint(form)));}
  function confirmDiscard(element,event){
    if(discardAllowed.delete(element))return;
    const changed=[...element.querySelectorAll('form')].find(form=>baselines.has(form)&&fingerprint(form)!==baselines.get(form));
    if(!changed)return;
    event.preventDefault();
    const existing=element.querySelector('[data-discard-warning]');if(existing){existing.querySelector('button')?.focus();return;}
    const previous=document.activeElement,warning=document.createElement('div');warning.className='alert alert-warning discard-warning';warning.dataset.discardWarning='';warning.setAttribute('role','alert');
    const text=document.createElement('p');text.textContent='Bạn có thay đổi chưa lưu. Đóng biểu mẫu sẽ bỏ những thay đổi này.';
    const actions=document.createElement('div');actions.className='actions';
    const keep=document.createElement('button');keep.type='button';keep.className='btn btn-primary';keep.textContent='Tiếp tục nhập';keep.addEventListener('click',()=>{warning.remove();if(previous?.isConnected)previous.focus();});
    const discard=document.createElement('button');discard.type='button';discard.className='btn btn-outline-danger';discard.textContent='Bỏ thay đổi và đóng';discard.addEventListener('click',()=>{discardAllowed.add(element);bootstrap.Modal.getInstance(element)?.hide();});
    actions.append(keep,discard);warning.append(text,actions);element.querySelector('.modal-body').prepend(warning);keep.focus();warning.scrollIntoView({block:'nearest',behavior:window.EduConnect.motionBehavior()});
  }
  function pendingLabel(button){
    if(button?.dataset.loadingText)return button.dataset.loadingText;
    const label=button?.textContent.trim()||'';
    if(/Chuyển vai trò/.test(label))return 'Đang chuyển…';
    if(/Gửi|Nộp/.test(label))return /Nộp/.test(label)?'Đang nộp…':'Đang gửi…';
    if(/Duyệt/.test(label))return 'Đang duyệt…';
    if(/Lưu/.test(label))return 'Đang lưu…';
    return 'Đang xử lý…';
  }
  function toast(message) {
    if (!message) return;
    const container = document.createElement('div'); container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    const element = document.createElement('div'); element.className = 'toast app-toast'; element.setAttribute('role','status'); element.setAttribute('aria-live','polite');
    const body = document.createElement('div'); body.className = 'toast-body';
    const mark = document.createElement('span'); mark.className = 'toast-mark'; mark.textContent = '✓'; mark.setAttribute('aria-hidden','true');
    const text = document.createElement('span'); text.textContent = message;
    const close = document.createElement('button'); close.type='button';close.className='btn-close';close.dataset.bsDismiss='toast';close.setAttribute('aria-label','Đóng thông báo');
    body.append(mark,text,close);element.append(body);container.append(element);document.body.append(container);
    element.addEventListener('hidden.bs.toast',()=>container.remove());
    bootstrap.Toast.getOrCreateInstance(element,{delay:4500}).show();
  }
  document.querySelectorAll('.app-toast').forEach(element=>bootstrap.Toast.getOrCreateInstance(element).show());
  const pending=sessionStorage.getItem('educonnect:toast');
  if(pending){sessionStorage.removeItem('educonnect:toast');toast(pending);}
  function errors(form, problem) {
    form.querySelectorAll('.ui-field-error,.ui-error-summary').forEach(node=>node.remove());
    form.querySelectorAll('[aria-invalid="true"]').forEach(input=>{input.removeAttribute('aria-invalid');input.classList.remove('is-invalid');input.setAttribute('aria-describedby',(input.getAttribute('aria-describedby')||'').split(' ').filter(id=>!id.startsWith('ui-error-')).join(' '));});
    const summary=document.createElement('div');summary.className='ui-error-summary';summary.setAttribute('role','alert');summary.tabIndex=-1;
    const title=document.createElement('strong');title.textContent='Kiểm tra các mục được đánh dấu bên dưới.';summary.append(title);
    const list=document.createElement('ul');let index=0;const globalMessages=new Set();
    for(const [name,raw] of Object.entries(problem.errors||{'':[problem.message||'Chưa thể lưu. Vui lòng thử lại.']})){
      const messages=[...new Set(raw)];
      const input=[...form.elements].find(input=>input.name===name);
      const item=document.createElement('li');
      if(input){
        const error=document.createElement('span');error.id='ui-error-'+Date.now()+'-'+index++;error.className='ui-field-error invalid-feedback d-block';error.textContent=messages.join(' ');input.classList.add('is-invalid');input.setAttribute('aria-invalid','true');input.setAttribute('aria-describedby',((input.getAttribute('aria-describedby')||'')+' '+error.id).trim());input.insertAdjacentElement('afterend',error);
        item.dataset.errorField=name;const link=document.createElement('button');link.type='button';link.textContent=window.EduConnect.fieldLabel(input);link.addEventListener('click',()=>{input.focus({preventScroll:true});input.scrollIntoView({block:'center',behavior:window.EduConnect.motionBehavior()});});item.append(link);list.append(item);
      }else for(const message of messages){if(globalMessages.has(message))continue;globalMessages.add(message);const entry=document.createElement('li');entry.textContent=message;list.append(entry);}
    }
    if(list.childElementCount)summary.append(list);
    form.prepend(summary);summary.focus();
  }
  async function open(url,title,source) {
    if(!modal)return;
    const target=new URL(url,location.href);if(target.origin!==location.origin)return;
    trigger=source;request?.abort();request=new AbortController();
    document.getElementById('workspace-modal-title').textContent=title;
    content.replaceChildren();const loading=document.createElement('p');loading.className='modal-loading';loading.textContent='Đang tải…';content.append(loading);content.setAttribute('aria-busy','true');modal.show();
    try{
      const response=await fetch(target,{headers:{'X-Requested-With':'XMLHttpRequest','X-Edu-Modal':'true'},signal:request.signal});
      if(!response.ok){const problem=await response.json().catch(()=>({message:'Không thể mở nội dung. Hãy thử lại.'}));throw new Error(problem.message||'Không thể mở nội dung.');}
      const html=await response.text();const template=document.createElement('template');template.innerHTML=html;
      template.content.querySelectorAll('script').forEach(script=>script.remove());
      const heading=template.content.querySelector('h1');if(heading){document.getElementById('workspace-modal-title').textContent=heading.textContent;heading.remove();}
      template.content.querySelectorAll('form').forEach(form=>{if(!form.getAttribute('action'))form.setAttribute('action',target.href);});
      content.replaceChildren(template.content);content.dataset.url=target.href;
      window.EduConnect?.init(content);
      rememberForms(content);
      content.querySelector('input:not([type="hidden"]),select,textarea,button,a')?.focus();
    }catch(error){if(error.name!=='AbortError'){content.textContent=error.message;}}
    finally{content.removeAttribute('aria-busy');}
  }
  document.addEventListener('click',event=>{
    const link=event.target.closest('a[data-modal]');
    if(!link||event.ctrlKey||event.metaKey||event.shiftKey||event.altKey||!modal)return;
    event.preventDefault();open(link.href,link.textContent.trim(),link);
  });
  document.addEventListener('submit',async event=>{
    const form=event.target;
    if(form.id==='compare-form'){
      if(event.defaultPrevented)return;
      const values=new FormData(form);if(!values.getAll('ids').length)return;
      event.preventDefault();const url=window.EduConnect.formUrl(form);url.search=new URLSearchParams(values).toString();open(url,'So sánh lựa chọn',event.submitter);return;
    }
    if(event.defaultPrevented||form.method.toLowerCase()!=='post')return;
    const target=window.EduConnect.formUrl(form);if(target.origin!==location.origin)return;
    event.preventDefault();if(form.dataset.submitting)return;
    const clientErrors=window.EduConnect.validateForm?.(form,event.submitter)||{};
    if(Object.keys(clientErrors).length){errors(form,{errors:clientErrors});return;}
    const button=event.submitter;const data=new FormData(form);if(button?.name)data.append(button.name,button.value);
    const inModal=!!form.closest('#workspace-modal');const label=button?.innerHTML;const oldMinWidth=button?.style.minWidth;
    form.dataset.submitting='true';form.setAttribute('aria-busy','true');if(button){button.style.minWidth=button.getBoundingClientRect().width+'px';button.disabled=true;button.textContent=pendingLabel(button);}if(inModal)saving=true;
    try{
      const response=await fetch(target,{method:'POST',body:data,headers:{'X-Requested-With':'XMLHttpRequest',...(inModal?{'X-Edu-Modal':'true'}:{})}});
      const result=await response.json().catch(()=>({ok:false,message:response.status===400?'Phiên biểu mẫu không còn hợp lệ. Hãy tải lại trang trước khi thử lại.':'Chưa thể xử lý yêu cầu. Thông tin bạn nhập vẫn được giữ lại.'}));
      if(!response.ok||!result.ok){errors(form,result);return;}
      baselines.set(form,fingerprint(form));
      completedForms.add(form);
      if(await window.EduConnect.afterAjaxSubmit?.(form,result,data))return;
      if(result.message)sessionStorage.setItem('educonnect:toast',result.message);
      if(inModal){saving=false;modal.hide();if(!result.message&&result.redirectUrl){const redirect=new URL(result.redirectUrl,location.href);if(redirect.origin===location.origin)location.assign(redirect);}else location.reload();}
      else if(result.redirectUrl){const redirect=new URL(result.redirectUrl,location.href);if(redirect.origin===location.origin)location.assign(redirect);}
      else {toast(result.message);sessionStorage.removeItem('educonnect:toast');}
    }catch(error){errors(form,{message:'Kết nối bị gián đoạn. Kiểm tra mạng rồi thử lại; dữ liệu vẫn được giữ nguyên.'});}
    finally{delete form.dataset.submitting;completedForms.delete(form);form.removeAttribute('aria-busy');if(button){button.disabled=false;button.innerHTML=label;button.style.minWidth=oldMinWidth;}if(inModal)saving=false;}
  });
  document.addEventListener('show.bs.modal',event=>{if(event.target.id==='connection-request-modal')rememberForms(event.target);});
  document.addEventListener('hide.bs.modal',event=>{if(!['workspace-modal','connection-request-modal'].includes(event.target.id))return;if(saving||[...event.target.querySelectorAll('form[data-submitting]')].some(form=>!completedForms.has(form))){event.preventDefault();return;}confirmDiscard(event.target,event);if(!event.defaultPrevented&&event.target===modalElement)request?.abort();});
  document.addEventListener('hidden.bs.modal',event=>event.target.querySelector('[data-discard-warning]')?.remove());
  modalElement?.addEventListener('hidden.bs.modal',()=>{content.replaceChildren();trigger?.focus();});
  window.EduConnect.showFormErrors=errors;
  window.EduConnect.toast=toast;
  window.EduConnect.closeModal=()=>{saving=false;modal?.hide();};
})();
