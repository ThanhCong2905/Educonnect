(() => {
 function init(){document.querySelectorAll('[data-lesson-record]').forEach(form=>{
  const rest=form.querySelector('[name=KetQuaBuoiHoc]:checked')?.value==='KhongDienRa';
  if(form.dataset.recordState===String(rest))return;
  form.dataset.recordState=String(rest);
  form.querySelector('[data-record-comment-label]').textContent=rest?'Lý do không diễn ra':'Nhận xét';
  const comment=form.elements.NhanXet;comment.required=rest;comment.removeAttribute('data-val-required');
  form.querySelector('[data-record-rest]').hidden=!rest;
  if(!rest)form.elements.CanHocBu.checked=false;
 });}
 document.addEventListener('change',e=>{if(e.target.name==='KetQuaBuoiHoc')init();});
 document.addEventListener('click',event=>{
  const button=event.target.closest('[data-record-edit],[data-record-cancel]');
  if(!button)return;
  const editor=button.closest('[data-record-editor]'),fields=editor.querySelector('[data-record-fields]');
  if(button.hasAttribute('data-record-cancel')){
   const form=fields.querySelector('form');form.reset();
   form.querySelectorAll('.ui-field-error,.ui-error-summary').forEach(element=>element.remove());
   form.querySelectorAll('.validation,[data-valmsg-for]').forEach(element=>element.replaceChildren());
   form.querySelectorAll('.is-invalid,[aria-invalid]').forEach(element=>{element.classList.remove('is-invalid');element.removeAttribute('aria-invalid');element.removeAttribute('aria-describedby');});
   delete form.dataset.recordState;init();
   fields.querySelector('.record-edit-fields').disabled=true;fields.hidden=true;editor.dataset.editing='false';
   const edit=editor.querySelector('[data-record-edit]');edit.hidden=false;edit.setAttribute('aria-expanded','false');edit.focus({preventScroll:true});return;
  }
  editor.dataset.editing='true';fields.hidden=false;
  fields.querySelector('.record-edit-fields').disabled=false;
  button.setAttribute('aria-expanded','true');button.hidden=true;
  fields.querySelector('input:not([type=hidden]),select,textarea')?.focus({preventScroll:true});
 });
 init();new MutationObserver(init).observe(document.body,{childList:true,subtree:true});
})();
