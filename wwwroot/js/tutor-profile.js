(()=>{
 const ui=window.EduConnect;
 function renumber(form){form.querySelectorAll('[data-subject-row]').forEach((row,i)=>row.querySelectorAll('[name]').forEach(input=>input.name=input.name.replace(/MonDay\[[^\]]+\]/,`MonDay[${i}]`)));}
 function custom(select){const field=select.closest('[data-subject-row]').querySelector('[data-custom-subject]');field.hidden=select.value!=='0';field.querySelector('input').disabled=field.hidden;field.querySelector('input').required=!field.hidden;}
 function add(form){const fragment=form.querySelector('template').content.cloneNode(true);form.querySelector('[data-subject-rows]').append(fragment);renumber(form);form.querySelectorAll('[data-subject-select]').forEach(custom);evidenceVisibility(form);ui.markRequired(form);}
 function evidenceVisibility(form){
  for(const section of form.querySelectorAll('[data-evidence-for]')){
   const choice=form.elements.namedItem(section.dataset.evidenceFor)?.value;
   section.hidden=!choice||(section.dataset.evidenceFor==='KinhNghiem'&&choice==='Chưa có');
  }
  for(const group of form.querySelectorAll('.profile-evidence-upload')){
   const input=group.querySelector('input[type="file"]');
   const inactive=!!group.closest('[hidden]');
   input.disabled=inactive;
   input.required=!inactive&&!group.querySelector('input[type="hidden"]').value;
  }
 }
 function updateEvidence(form){
  const fields=[...form.querySelectorAll('.profile-evidence-upload')].map(group=>({
   id:group.querySelector('input[type="hidden"]').value,
   originalId:group.dataset.currentEvidence,
   upload:group.querySelector('input[type="file"]')
  }));
  form.querySelectorAll('[data-replaced-evidence]').forEach(input=>input.remove());
  const ids=new Set(fields.map(field=>field.originalId).filter(Boolean));
  for(const id of ids){
   if(fields.some(field=>field.id===id&&(field.upload.disabled||field.upload.files.length===0)))continue;
   const input=document.createElement('input');
   input.type='hidden';input.name='XoaTepIds';input.value=id;input.dataset.replacedEvidence='';
   form.append(input);
  }
 }
 function init(root){
  root.querySelectorAll('[data-tutor-profile]').forEach(form=>{
   if(form.dataset.profileReady)return;
   form.dataset.profileReady='true';
   if(!form.querySelector('[data-subject-row]')&&!form.querySelector('fieldset').disabled)add(form);
   form.querySelectorAll('[data-subject-select]').forEach(custom);
   evidenceVisibility(form);
  });
 }
 const original=ui.init;ui.init=root=>{original(root);init(root||document);};
 document.addEventListener('click',event=>{
  const edit=event.target.closest('[data-edit-profile]');
  if(edit){const form=edit.closest('form');if(form.dataset.profileLocked==='true')return;form.querySelector('.profile-edit-fields').disabled=false;edit.hidden=true;evidenceVisibility(form);form.querySelector('[name="HoTen"]').focus();return;}
  const addButton=event.target.closest('[data-add-subject]');if(addButton){add(addButton.closest('form'));return;}const remove=event.target.closest('[data-remove-subject]');if(remove){const form=remove.closest('form');remove.closest('[data-subject-row]').remove();renumber(form);}
 });
 document.addEventListener('change',event=>{
  if(event.target.matches('.profile-evidence-upload input[type="file"]')){
   const group=event.target.closest('.profile-evidence-upload');
   group.querySelector('[data-evidence-filename]').textContent=event.target.files.length?'1 tệp đã chọn':'Chưa chọn tệp mới';
   const saved=group.querySelector('[data-saved-evidence]');if(saved)saved.hidden=event.target.files.length>0;
   event.target.form.querySelector('[data-clear-all-evidence]').hidden=![...event.target.form.querySelectorAll('.profile-evidence-upload input[type="file"]')].some(input=>input.files.length>0);
   updateEvidence(event.target.form);
  }
  if(event.target.matches('[data-subject-select]'))custom(event.target);
  if(event.target.form?.hasAttribute('data-tutor-profile'))evidenceVisibility(event.target.form);
  if(event.target.matches('[data-request-need]')){
   const preview=event.target.form?.querySelector('[data-request-preview]');
   if(preview){
    preview.hidden=!event.target.value;
    preview.querySelector('[data-request-goal]').textContent=event.target.selectedOptions[0]?.dataset.goal||'';
   }
  }
  if(event.target.matches('[data-proposal-need]')){
   const option=event.target.selectedOptions[0],form=event.target.form;
   if(!option?.value||!form)return;
   for(const [name,key] of [['TenNguoiHoc','learner'],['MonHocId','subject'],['MucTieu','goal'],['SoBuoiTuan','sessions'],['HinhThucHoc','mode'],['DiaDiem','address']]){
    const input=form.elements.namedItem(name);
    if(input&&option.dataset[key]!=null&&(key!=='learner'||option.dataset[key])){
     input.value=key==='mode'&&option.dataset.mode==='CaHai'?'':option.dataset[key];
     input.dispatchEvent(new Event('change',{bubbles:true}));
    }
   }
   const slots=JSON.parse(option.dataset.slots||'[]'),list=form.querySelector('[data-slot-list]'),add=form.querySelector('[data-add-slot]');
   if(slots.length&&list&&add){
    list.replaceChildren();add.disabled=false;
    for(const slot of slots){
     add.click();const row=list.lastElementChild;
     for(const [property,value] of [['ThuTrongTuan',String(slot.ThuTrongTuan)],['GioBatDau',slot.GioBatDau.slice(0,5)],['GioKetThuc',slot.GioKetThuc.slice(0,5)]]){
      const input=row.querySelector('[name$=".'+property+'"]');input.value=value;input.dispatchEvent(new Event('change',{bubbles:true}));
     }
    }
    const lengths=slots.map(slot=>{const minutes=time=>Number(time.slice(0,2))*60+Number(time.slice(3,5));return minutes(slot.GioKetThuc)-minutes(slot.GioBatDau);});
    if(lengths[0]>0&&lengths[0]<=480&&lengths.every(value=>value===lengths[0]))form.elements.ThoiLuongPhut.value=String(lengths[0]);
    list.dispatchEvent(new Event('edu:slots-restored'));window.EduConnect.initTimePickers?.(form);
   }
  }
 });
 document.addEventListener('click',event=>{
  const savedButton=event.target.closest('[data-remove-saved-evidence]');
  if(savedButton){
   const group=savedButton.closest('.profile-evidence-upload');
   const input=group.querySelector('input[type="hidden"]');
   input.value=input.value?'':group.dataset.currentEvidence;
   group.querySelector('[data-saved-evidence]').classList.toggle('is-pending-removal',!input.value);
   savedButton.textContent=input.value?'×':'↶';
   savedButton.setAttribute('aria-label',input.value?'Bỏ tệp đã lưu':'Hoàn tác bỏ tệp');savedButton.title=input.value?'Bỏ tệp đã lưu khi lưu hồ sơ':'Hoàn tác bỏ tệp';
   updateEvidence(savedButton.form);
   input.dispatchEvent(new Event('change',{bubbles:true}));
   return;
  }
  const button=event.target.closest('[data-clear-all-evidence]');
  if(!button)return;
  button.form.querySelectorAll('.profile-evidence-upload input[type="file"]').forEach(input=>{input.value='';input.dispatchEvent(new Event('input',{bubbles:true}));input.dispatchEvent(new Event('change',{bubbles:true}));});
 });
 const allowed=new WeakSet();let pending;
 document.addEventListener('submit',event=>{
  if(!event.submitter?.hasAttribute('data-confirm-hide'))return;
  if(allowed.delete(event.target))return;
  event.preventDefault();event.stopImmediatePropagation();pending={form:event.target,button:event.submitter};
  let el=document.getElementById('hide-profile-confirm');
  if(!el){el=document.createElement('div');el.className='modal fade';el.id='hide-profile-confirm';el.tabIndex=-1;el.setAttribute('aria-labelledby','hide-profile-title');el.innerHTML='<div class="modal-dialog modal-dialog-centered"><div class="modal-content"><div class="modal-header"><h2 id="hide-profile-title" class="modal-title fs-5">Ẩn hồ sơ gia sư?</h2></div><div class="modal-body">Hồ sơ sẽ tạm dừng xuất hiện trong tìm kiếm và không nhận liên hệ mới. Hội thoại và lớp đang có vẫn được giữ.</div><div class="modal-footer"><button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">Để sau</button><button type="button" class="btn btn-primary" data-confirm-profile>Ẩn hồ sơ</button></div></div></div>';document.body.append(el);el.querySelector('[data-confirm-profile]').addEventListener('click',()=>{const task=pending;bootstrap.Modal.getInstance(el).hide();if(task){allowed.add(task.form);task.form.requestSubmit(task.button);}});}
  bootstrap.Modal.getOrCreateInstance(el).show();
 },true);
 if(document.readyState!=='loading')init(document);else document.addEventListener('DOMContentLoaded',()=>init(document));
})();
