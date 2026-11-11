window.EduConnect=window.EduConnect||{};
window.EduConnect.motionBehavior=()=>matchMedia('(prefers-reduced-motion: reduce)').matches?'auto':'smooth';
window.EduConnect.formUrl=(form,submitter)=>new URL(submitter?.getAttribute('formaction')||form.getAttribute('action')||location.href,document.baseURI);
window.EduConnect.fileSize=bytes=>bytes<1000?bytes+' B':bytes<1000000?(bytes/1000).toFixed(1)+' KB':(bytes/1000000).toFixed(2)+' MB';
window.EduConnect.initFilePickers=(root=document)=>{
 root.querySelectorAll('input[type="file"]').forEach(picker=>{
  if(picker.dataset.filePickerReady)return;
  picker.dataset.filePickerReady='true';
  let list=picker.closest('[data-certificate-picker]')?.querySelector('[data-certificate-selection]');
  if(picker.hasAttribute('data-chat-files'))list=picker.form.querySelector('[data-chat-file-selection]');
  if(!list){list=document.createElement('ul');list.setAttribute('aria-label','Tệp đã chọn');(picker.closest('label')||picker).after(list);}
  list.classList.add('upload-selection');
  function render(){
   list.replaceChildren();
   [...picker.files].forEach((file,index)=>{
    const row=document.createElement('li');row.className='upload-selected-file';
    const info=document.createElement('span');info.textContent=file.name+' · '+window.EduConnect.fileSize(file.size);
    const remove=document.createElement('button');remove.type='button';remove.className='btn quiet upload-remove';remove.textContent='×';remove.setAttribute('aria-label','Bỏ tệp '+file.name);
    remove.addEventListener('click',()=>{
     const transfer=new DataTransfer();
     [...picker.files].filter((_,i)=>i!==index).forEach(remaining=>transfer.items.add(remaining));
     picker.files=transfer.files;
     picker.dispatchEvent(new Event('input',{bubbles:true}));picker.dispatchEvent(new Event('change',{bubbles:true}));
     (list.children[index]?.querySelector('button')||list.lastElementChild?.querySelector('button')||picker).focus();
    });
    row.append(info,remove);list.append(row);
   });
  }
  picker.addEventListener('change',render);render();
 });
};
window.EduConnect.clearRowErrors=(form,list,prefix)=>{
 list.querySelectorAll('.ui-field-error').forEach(error=>error.remove());
 list.querySelectorAll('[aria-invalid]').forEach(input=>{input.removeAttribute('aria-invalid');input.classList.remove('is-invalid');input.setAttribute('aria-describedby',(input.getAttribute('aria-describedby')||'').split(' ').filter(id=>!id.startsWith('ui-error-')).join(' '));});
 form.querySelectorAll('.ui-error-summary [data-error-field]').forEach(item=>{if(item.dataset.errorField.startsWith(prefix+'['))item.remove();});
 const summary=form.querySelector('.ui-error-summary');if(summary&&!summary.querySelector('li'))summary.remove();
};
window.EduConnect.fieldLabel=input=>input.closest('label')?.querySelector('.field-caption')?.textContent.replace('*','').trim()||input.closest('label')?.childNodes[0]?.textContent.trim()||'Trường này';
window.EduConnect.markRequired=(root=document)=>{
 root.querySelectorAll('form[method="post"],form[method="POST"]').forEach(form=>form.noValidate=true);
 const placeMark=(label,mark)=>{
  const caption=document.createElement('span');caption.className='field-caption';
  [...label.childNodes].filter(node=>node.nodeType===Node.TEXT_NODE&&node.textContent.trim()).forEach(node=>caption.append(node));
  const title=caption.textContent.trim();
  if(!title)return;
  caption.textContent=title+'\u00a0';
  caption.append(mark);label.prepend(caption);
 };
 root.querySelectorAll('label > .required-mark, legend > .required-mark').forEach(mark=>placeMark(mark.parentElement,mark));
 root.querySelectorAll('input:not([type="hidden"]),select,textarea').forEach(input=>{
  if(!input.required&&(input.type==='file'||!input.hasAttribute('data-val-required'))){input.removeAttribute('aria-required');input.closest('label')?.querySelector('.required-mark')?.remove();return;}
  input.setAttribute('aria-required','true');
  const label=input.closest('label');if(!label||label.querySelector('.required-mark'))return;
  const caption=label.querySelector(':scope > .field-caption');
  const mark=document.createElement('span');mark.className='required-mark';mark.textContent='*';mark.setAttribute('aria-hidden','true');
  if(caption){caption.textContent=caption.textContent.trim()+'\u00a0';caption.append(mark);}else placeMark(label,mark);
 });
 const markLegend=(legend,label)=>{if(!legend||legend.querySelector('.required-mark'))return;const mark=document.createElement('span');mark.className='required-mark';mark.textContent='*';mark.setAttribute('aria-label',label);placeMark(legend,mark);};
 root.querySelectorAll('[data-slot-list]').forEach(list=>markLegend(list.closest('fieldset')?.querySelector('legend'),'Bắt buộc khi đăng'));
 root.querySelectorAll('.subject-picker>legend').forEach(legend=>markLegend(legend,'Bắt buộc'));
};
window.EduConnect.validateForm=(form,submitter)=>{
 const errors={};const draft=submitter?.formNoValidate;
 for(const input of form.elements){
  if(!input.name||input.disabled||input.type==='hidden'||input.type==='submit'||input.type==='button')continue;
  const label=window.EduConnect.fieldLabel(input);const value=input.value.trim();
  if(input.type==='file'){
   const files=[...input.files];
   if(files.length>5)errors[input.name]=['Chọn tối đa 5 tệp mỗi lần gửi.'];
   else if(files.some(file=>file.size===0||file.size>10000000))errors[input.name]=['Mỗi tệp phải có dữ liệu và không vượt quá 10 MB.'];
   else if(files.some(file=>!(/\.(pdf|jpe?g|png|docx)$/i).test(file.name)))errors[input.name]=['Chỉ nhận PDF, JPG, PNG, DOCX.'];
   continue;
  }
  if(!draft&&(input.required||input.hasAttribute('data-val-required'))&&!value){errors[input.name]=[label+' không được để trống.'];continue;}
  if(value&&input.validity.rangeUnderflow)errors[input.name]=[label+' phải từ '+input.min+' trở lên.'];
  else if(value&&input.validity.rangeOverflow)errors[input.name]=[label+' không được vượt quá '+input.max+'.'];
  else if(value&&(input.validity.badInput||input.validity.typeMismatch||input.validity.patternMismatch))errors[input.name]=[label+' chưa đúng định dạng.'];
 }
 const slots=[...form.querySelectorAll('[data-slot-list]>.slot-row')];
 if(form.hasAttribute('data-chat-send')){
  const files=[...(form.querySelector('[data-chat-files]')?.files||[])],text=form.querySelector('[name="NoiDung"]').value.trim();
  if(!text&&!files.length)errors['NoiDung']=['Nhập nội dung hoặc chọn ít nhất một tệp.'];
 }
 if(form.querySelector('.subject-picker')&&!form.querySelector('[name="MonHocIds"]:checked')&&!form.querySelector('[name="ChonMonKhac"]:checked'))errors['MonHocIds']=['Chọn ít nhất một môn học.'];
 if(form.querySelector('[data-slot-list]')){
  if(!slots.length&&!draft)errors['']=['Thêm ít nhất một khung giờ rảnh.'];
  if(slots.length>35)errors['']=['Chỉ thêm tối đa 35 khung giờ.'];
  const values=slots.map(row=>({day:row.querySelector('select'),start:row.querySelector('[name$=".GioBatDau"]'),end:row.querySelector('[name$=".GioKetThuc"]')}));
  for(const row of values)if(row.start.value&&row.end.value&&row.start.value>=row.end.value)errors[row.end.name]=['Giờ kết thúc phải sau giờ bắt đầu.'];
  for(let i=0;i<values.length;i++)for(let j=i+1;j<values.length;j++){const a=values[i],b=values[j];if(a.day.value===b.day.value&&a.start.value&&a.end.value&&b.start.value&&b.end.value&&a.start.value<b.end.value&&b.start.value<a.end.value){errors[b.start.name]=['Khung giờ này trùng với dòng '+(i+1)+'. Chỉnh lại giờ hoặc chọn ngày khác.'];}}
 }
 if(form.hasAttribute('data-calendar-form')||(form.hasAttribute('data-schedule-edit')&&!draft)){
  const rows=[...form.querySelectorAll(form.hasAttribute('data-calendar-form')?'.schedule-row':'[data-schedule-times]')];
  if(!rows.length)errors['']=['Chọn ít nhất một ngày trên lịch trước khi lưu.'];
  const now=Date.now();
  const values=rows.map(row=>({title:row.querySelector('[name$=".TieuDe"]'),start:row.querySelector('[name$=".BatDauLuc"]'),end:row.querySelector('[name$=".KetThucLuc"]')}));
  for(const row of values){const start=Date.parse(row.start.value+'+07:00'),end=Date.parse(row.end.value+'+07:00');if(!row.start.value)errors[row.start.name]=['Chọn ngày và giờ bắt đầu.'];else if(start<=now)errors[row.start.name]=['Giờ bắt đầu đã qua. Chọn thời gian sau hiện tại (giờ Việt Nam).'];if(row.start.value&&row.end.value&&start>=end)errors[row.end.name]=['Giờ kết thúc phải sau giờ bắt đầu.'];}
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
