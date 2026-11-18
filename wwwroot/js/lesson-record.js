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
 init();new MutationObserver(init).observe(document.body,{childList:true,subtree:true});
})();
