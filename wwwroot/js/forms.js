window.EduConnect=window.EduConnect||{};
window.EduConnect.clearRowErrors=(form,list,prefix)=>{
 list.querySelectorAll('.ui-field-error').forEach(error=>error.remove());
 list.querySelectorAll('[aria-invalid]').forEach(input=>{input.removeAttribute('aria-invalid');input.classList.remove('is-invalid');input.setAttribute('aria-describedby',(input.getAttribute('aria-describedby')||'').split(' ').filter(id=>!id.startsWith('ui-error-')).join(' '));});
 form.querySelectorAll('.ui-error-summary [data-error-field]').forEach(item=>{if(item.dataset.errorField.startsWith(prefix+'['))item.remove();});
 const summary=form.querySelector('.ui-error-summary');if(summary&&!summary.querySelector('li'))summary.remove();
};
window.EduConnect.fieldLabel=input=>input.closest('label')?.querySelector('.field-caption')?.textContent.replace('*','').trim()||input.closest('label')?.childNodes[0]?.textContent.trim()||'Trường này';
window.EduConnect.markRequired=(root=document)=>{
 root.querySelectorAll('form[method="post"],form[method="POST"]').forEach(form=>form.noValidate=true);
 root.querySelectorAll('input:not([type="hidden"]),select,textarea').forEach(input=>{
  if(!input.required&&!input.hasAttribute('data-val-required')){input.removeAttribute('aria-required');input.closest('label')?.querySelector('.required-mark')?.remove();return;}
  input.setAttribute('aria-required','true');
  const label=input.closest('label');if(!label||label.querySelector('.required-mark'))return;
  const caption=document.createElement('span');caption.className='field-caption';
  [...label.childNodes].filter(node=>node.nodeType===Node.TEXT_NODE&&node.textContent.trim()).forEach(node=>caption.append(node));
  if(!caption.textContent.trim())return;
  const mark=document.createElement('span');mark.className='required-mark';mark.textContent='*';mark.setAttribute('aria-hidden','true');caption.append(mark);label.prepend(caption);
 });
 root.querySelectorAll('[data-slot-list]').forEach(list=>{const legend=list.closest('fieldset')?.querySelector('legend');if(legend&&!legend.querySelector('.required-mark')){const mark=document.createElement('span');mark.className='required-mark';mark.textContent='*';mark.setAttribute('aria-label','Bắt buộc khi đăng');legend.append(mark);}});
 root.querySelectorAll('.subject-picker>legend').forEach(legend=>{if(legend.querySelector('.required-mark'))return;const mark=document.createElement('span');mark.className='required-mark';mark.textContent='*';mark.setAttribute('aria-label','Bắt buộc');legend.append(mark);});
};
window.EduConnect.validateForm=(form,submitter)=>{
 const errors={};const draft=submitter?.formNoValidate;
 for(const input of form.elements){
  if(!input.name||input.disabled||input.type==='hidden'||input.type==='submit'||input.type==='button')continue;
  const label=window.EduConnect.fieldLabel(input);const value=input.value.trim();
  if(!draft&&(input.required||input.hasAttribute('data-val-required'))&&!value){errors[input.name]=[label+' không được để trống.'];continue;}
  if(value&&input.validity.rangeUnderflow)errors[input.name]=[label+' phải từ '+input.min+' trở lên.'];
  else if(value&&input.validity.rangeOverflow)errors[input.name]=[label+' không được vượt quá '+input.max+'.'];
  else if(value&&(input.validity.badInput||input.validity.typeMismatch||input.validity.patternMismatch))errors[input.name]=[label+' chưa đúng định dạng.'];
 }
 const slots=[...form.querySelectorAll('[data-slot-list]>.slot-row')];
 if(form.querySelector('.subject-picker')&&!form.querySelector('[name="MonHocIds"]:checked')&&!form.querySelector('[name="ChonMonKhac"]:checked'))errors['MonHocIds']=['Chọn ít nhất một môn học.'];
 if(form.querySelector('[data-slot-list]')){
  if(!slots.length&&!draft)errors['']=['Thêm ít nhất một khung giờ rảnh.'];
  if(slots.length>35)errors['']=['Chỉ thêm tối đa 35 khung giờ.'];
  const values=slots.map(row=>({day:row.querySelector('select'),start:row.querySelector('[name$=".GioBatDau"]'),end:row.querySelector('[name$=".GioKetThuc"]')}));
  for(const row of values)if(row.start.value&&row.end.value&&row.start.value>=row.end.value)errors[row.end.name]=['Giờ kết thúc phải sau giờ bắt đầu.'];
  for(let i=0;i<values.length;i++)for(let j=i+1;j<values.length;j++){const a=values[i],b=values[j];if(a.day.value===b.day.value&&a.start.value&&a.end.value&&b.start.value&&b.end.value&&a.start.value<b.end.value&&b.start.value<a.end.value){errors[b.start.name]=['Khung giờ này trùng với dòng '+(i+1)+'. Chỉnh lại giờ hoặc chọn ngày khác.'];}}
 }
 if(form.hasAttribute('data-calendar-form')){
  const rows=[...form.querySelectorAll('.schedule-row')];
  if(!rows.length)errors['']=['Chọn ít nhất một ngày trên lịch trước khi lưu.'];
  const now=Date.now();
  const values=rows.map(row=>({title:row.querySelector('[name$=".TieuDe"]'),start:row.querySelector('[name$=".BatDauLuc"]'),end:row.querySelector('[name$=".KetThucLuc"]')}));
  for(const row of values){const start=Date.parse(row.start.value+'+07:00'),end=Date.parse(row.end.value+'+07:00');if(!row.start.value)errors[row.start.name]=['Chọn ngày và giờ bắt đầu.'];else if(start<=now)errors[row.start.name]=['Giờ bắt đầu phải ở tương lai.'];if(row.start.value&&row.end.value&&start>=end)errors[row.end.name]=['Giờ kết thúc phải sau giờ bắt đầu.'];}
  for(let i=0;i<values.length;i++)for(let j=i+1;j<values.length;j++){const a=values[i],b=values[j];if(a.start.value<b.end.value&&b.start.value<a.end.value)errors[b.start.name]=['Buổi này trùng thời gian với buổi '+(i+1)+'.'];}
 }
 return errors;
};
document.addEventListener('input',event=>{
 const input=event.target;if(!input.matches('input,select,textarea'))return;
 const form=input.closest('form');if(!form)return;
 const ids=(input.getAttribute('aria-describedby')||'').split(' ');
 ids.filter(id=>id.startsWith('ui-error-')).forEach(id=>document.getElementById(id)?.remove());
 input.classList.remove('is-invalid');input.removeAttribute('aria-invalid');input.setAttribute('aria-describedby',ids.filter(id=>!id.startsWith('ui-error-')).join(' '));
 form.querySelectorAll('.ui-error-summary [data-error-field]').forEach(item=>{if(item.dataset.errorField===input.name)item.remove();});
 const summary=form.querySelector('.ui-error-summary');if(summary&&!summary.querySelector('li'))summary.remove();
});
