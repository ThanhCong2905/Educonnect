(() => {
  'use strict';
  const modalElement = document.getElementById('workspace-modal');
  const content = document.getElementById('workspace-content');
  const modal = modalElement ? bootstrap.Modal.getOrCreateInstance(modalElement) : null;
  let trigger, request, saving = false;
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
        item.dataset.errorField=name;const link=document.createElement('button');link.type='button';link.textContent=window.EduConnect.fieldLabel(input);link.addEventListener('click',()=>{input.focus();input.scrollIntoView({block:'center',behavior:'smooth'});});item.append(link);list.append(item);
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
      template.content.querySelectorAll('form').forEach(form=>{if(!form.getAttribute('action'))form.action=target.href;});
      content.replaceChildren(template.content);content.dataset.url=target.href;
      window.EduConnect?.init(content);
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
      event.preventDefault();const url=new URL(form.action,location.href);url.search=new URLSearchParams(values).toString();open(url,'So sánh lựa chọn',event.submitter);return;
    }
    if(event.defaultPrevented||form.method.toLowerCase()!=='post')return;
    const target=new URL(form.action||content?.dataset.url||location.href,location.href);if(target.origin!==location.origin)return;
    event.preventDefault();if(form.dataset.submitting)return;
    const clientErrors=window.EduConnect.validateForm?.(form,event.submitter)||{};
    if(Object.keys(clientErrors).length){errors(form,{errors:clientErrors});return;}
    const button=event.submitter;const data=new FormData(form);if(button?.name)data.append(button.name,button.value);
    const inModal=!!form.closest('#workspace-modal');const label=button?.textContent;
    form.dataset.submitting='true';form.setAttribute('aria-busy','true');if(button){button.disabled=true;button.textContent='Đang lưu…';}if(inModal)saving=true;
    try{
      const response=await fetch(target,{method:'POST',body:data,headers:{'X-Requested-With':'XMLHttpRequest',...(inModal?{'X-Edu-Modal':'true'}:{})}});
      const result=await response.json().catch(()=>({ok:false,message:response.status===400?'Phiên biểu mẫu không còn hợp lệ. Hãy tải lại trang trước khi thử lại.':'Chưa thể xử lý yêu cầu. Thông tin bạn nhập vẫn được giữ lại.'}));
      if(!response.ok||!result.ok){errors(form,result);return;}
      if(await window.EduConnect.afterAjaxSubmit?.(form,result,data))return;
      if(result.message)sessionStorage.setItem('educonnect:toast',result.message);
      if(inModal){saving=false;modal.hide();if(!result.message&&result.redirectUrl){const redirect=new URL(result.redirectUrl,location.href);if(redirect.origin===location.origin)location.assign(redirect);}else location.reload();}
      else if(result.redirectUrl){const redirect=new URL(result.redirectUrl,location.href);if(redirect.origin===location.origin)location.assign(redirect);}
      else {toast(result.message);sessionStorage.removeItem('educonnect:toast');}
    }catch(error){errors(form,{message:'Kết nối bị gián đoạn. Kiểm tra mạng rồi thử lại; dữ liệu vẫn được giữ nguyên.'});}
    finally{delete form.dataset.submitting;form.removeAttribute('aria-busy');if(button){button.disabled=false;button.textContent=label;}if(inModal)saving=false;}
  });
  modalElement?.addEventListener('hide.bs.modal',event=>{if(saving)event.preventDefault();else request?.abort();});
  modalElement?.addEventListener('hidden.bs.modal',()=>{content.replaceChildren();trigger?.focus();});
  window.EduConnect.showFormErrors=errors;
  window.EduConnect.toast=toast;
  window.EduConnect.closeModal=()=>{saving=false;modal?.hide();};
})();
