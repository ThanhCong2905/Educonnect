(()=>{
 const ui=window.EduConnect;
 function renumber(form){form.querySelectorAll('[data-subject-row]').forEach((row,i)=>row.querySelectorAll('[name]').forEach(input=>input.name=input.name.replace(/MonDay\[[^\]]+\]/,`MonDay[${i}]`)));}
 function custom(select){const field=select.closest('[data-subject-row]').querySelector('[data-custom-subject]');field.hidden=select.value!=='0';field.querySelector('input').disabled=field.hidden;field.querySelector('input').required=!field.hidden;}
 function add(form){const fragment=form.querySelector('template').content.cloneNode(true);form.querySelector('[data-subject-rows]').append(fragment);renumber(form);form.querySelectorAll('[data-subject-select]').forEach(custom);ui.markRequired(form);}
 function init(root){root.querySelectorAll('[data-tutor-profile]').forEach(form=>{if(form.dataset.profileReady)return;form.dataset.profileReady='true';if(!form.querySelector('[data-subject-row]')&&!form.querySelector('fieldset').disabled)add(form);form.querySelectorAll('[data-subject-select]').forEach(custom);});}
 const original=ui.init;ui.init=root=>{original(root);init(root||document);};
 document.addEventListener('click',event=>{const addButton=event.target.closest('[data-add-subject]');if(addButton){add(addButton.closest('form'));return;}const remove=event.target.closest('[data-remove-subject]');if(remove){const form=remove.closest('form');remove.closest('[data-subject-row]').remove();renumber(form);}});
 document.addEventListener('change',event=>{if(event.target.matches('[data-subject-select]'))custom(event.target);});
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
