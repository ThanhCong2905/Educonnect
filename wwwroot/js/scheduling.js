window.EduConnect=window.EduConnect||{};
window.EduConnect.initScheduling=(root=document)=>{
 root.querySelectorAll('[data-schedule-edit]').forEach(form=>{
  if(form.dataset.timePreviewReady)return;
  form.dataset.timePreviewReady='true';
  const start=form.querySelector('[name="Lich.BatDauLuc"]'),end=form.querySelector('[name="Lich.KetThucLuc"]');
  const preview=form.querySelector('[data-schedule-time-preview]');
  const format=new Intl.DateTimeFormat('vi-VN',{timeZone:'Asia/Ho_Chi_Minh',day:'2-digit',month:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit',hourCycle:'h23'});
  function update(){
   const from=new Date(start.value+'+07:00'),to=new Date(end.value+'+07:00');
   preview.textContent=Number.isNaN(from.getTime())||Number.isNaN(to.getTime())?'Ngày giờ tính theo giờ Việt Nam.':format.format(from)+' – '+format.format(to)+' (giờ Việt Nam)';
  }
  start.addEventListener('input',update);end.addEventListener('input',update);update();
 });
 const reindex=(list,prefix)=>[...list.children].forEach((row,index)=>row.querySelectorAll('[name]').forEach(input=>input.name=input.name.replace(new RegExp(prefix+'\\[\\d+\\]'),prefix+'['+index+']')));
 root.querySelectorAll('[data-slot-list]').forEach(list=>{
  if(list.dataset.ready)return;list.dataset.ready='true';
  const form=list.closest('form'),button=form.querySelector('[data-add-slot]');
  const feedback=document.createElement('p');feedback.className='muted';feedback.setAttribute('role','status');list.after(feedback);
  function sync(){window.EduConnect.clearRowErrors(form,list,'LichRanh');reindex(list,'LichRanh');button.disabled=list.children.length>=35;feedback.textContent=list.children.length?list.children.length+' khung giờ · Các khung cùng ngày cần tránh trùng nhau.':'Chưa có khung giờ. Chọn “Thêm khung giờ” để bắt đầu.';list.dispatchEvent(new Event('edu:slots-changed',{bubbles:true}));}
  list.addEventListener('click',event=>{const remove=event.target.closest('[data-remove-row]');if(!remove)return;const next=remove.closest('.slot-row').nextElementSibling;remove.closest('.slot-row').remove();sync();(next?.querySelector('select')||button).focus();});
  list.addEventListener('edu:slots-restored',sync);
  button.addEventListener('click',()=>{
   if(list.children.length>=35)return;
   const previous=list.lastElementChild;const day=previous?Number(previous.querySelector('select').value):0;
   let start=previous?.querySelector('[name$=".GioBatDau"]').value||'19:00',end=previous?.querySelector('[name$=".GioKetThuc"]').value||'20:30';
   const used=new Set([...list.querySelectorAll('select')].map(input=>Number(input.value)));let next=(day+1)%7;
   for(let i=1;i<=7;i++)if(!used.has((day+i)%7)){next=(day+i)%7;break;}
   if(used.size===7){
    const minutes=value=>{const [h,m]=value.split(':').map(Number);return h*60+m;},format=value=>String(Math.floor(value/60)).padStart(2,'0')+':'+String(value%60).padStart(2,'0');
    const duration=Math.max(30,minutes(end)-minutes(start));
    const ends=[...list.children].filter(row=>Number(row.querySelector('select').value)===next).map(row=>row.querySelector('[name$=".GioKetThuc"]').value).filter(Boolean);
    const after=Math.max(...ends.map(minutes));
    if(Number.isFinite(after)&&after+duration<1440){start=format(after);end=format(after+duration);}else{start='';end='';}
   }
   const row=document.createElement('div');row.className='slot-row';
   row.innerHTML='<label>Ngày trong tuần<select class="form-select" name="LichRanh[0].ThuTrongTuan"></select></label><label>Giờ bắt đầu<input class="form-control" type="time" name="LichRanh[0].GioBatDau" required></label><label>Giờ kết thúc<input class="form-control" type="time" name="LichRanh[0].GioKetThuc" required></label><button class="btn btn-outline-primary" type="button" data-remove-row aria-label="Xóa khung giờ">Xóa</button>';
   const select=row.querySelector('select');for(const d of [1,2,3,4,5,6,0])select.add(new Option(d===0?'Chủ nhật':'Thứ '+(d+1),d));select.value=next;
   row.querySelector('[name$=".GioBatDau"]').value=start;row.querySelector('[name$=".GioKetThuc"]').value=end;list.append(row);sync();window.EduConnect.markRequired(row);select.focus();
  });
  sync();
 });
 root.querySelectorAll('[data-calendar-form]').forEach(form=>{
  if(form.dataset.calendarReady)return;form.dataset.calendarReady='true';
  const $=selector=>form.querySelector(selector),list=$('[data-schedule-list]'),month=$('#batch-month'),calendar=$('#calendar'),feedback=$('#calendar-feedback');
  const today=new Intl.DateTimeFormat('en-CA',{timeZone:'Asia/Ho_Chi_Minh',year:'numeric',month:'2-digit',day:'2-digit'}).format(new Date());
  const dates=()=>[...list.querySelectorAll('[name$=".BatDauLuc"]')].map(input=>input.value.slice(0,10));
  function timesValid(){if(!$('#batch-title').value.trim()){feedback.textContent='Nhập tên buổi học trước khi chọn ngày.';$('#batch-title').focus();return false;}if(!$('#batch-start').value||!$('#batch-end').value||$('#batch-start').value>=$('#batch-end').value){feedback.textContent='Chọn giờ kết thúc sau giờ bắt đầu.';$('#batch-end').focus();return false;}return true;}
  function add(day){
   const start=day+'T'+$('#batch-start').value,end=day+'T'+$('#batch-end').value;
   if(list.children.length>=100||day<today||Date.parse(start+'+07:00')<=Date.now())return false;
   if([...list.children].some(row=>row.querySelector('[name$=".BatDauLuc"]').value<end&&start<row.querySelector('[name$=".KetThucLuc"]').value))return false;
   const row=document.createElement('div');row.className='schedule-row';
   for(const [property,label,type,value] of [['TieuDe','Tên buổi','text',$('#batch-title').value],['BatDauLuc','Bắt đầu','datetime-local',day+'T'+$('#batch-start').value],['KetThucLuc','Kết thúc','datetime-local',day+'T'+$('#batch-end').value]]){
    const field=document.createElement('label');field.textContent=label;const input=document.createElement('input');input.className='form-control';input.type=type;input.name='Buoi[0].'+property;input.required=true;input.value=value;if(type==='text')input.maxLength=256;field.append(input);row.append(field);
   }
   const remove=document.createElement('button');remove.className='btn btn-outline-primary';remove.type='button';remove.dataset.removeRow='';remove.textContent='Bỏ';remove.setAttribute('aria-label','Xóa buổi học ngày '+day);row.append(remove);list.append(row);window.EduConnect.markRequired(row);return true;
  }
  function draw(){
   const [year,number]=month.value.split('-').map(Number);if(!year||!number)return;
   calendar.replaceChildren();for(const day of ['T2','T3','T4','T5','T6','T7','CN']){const label=document.createElement('small');label.textContent=day;calendar.append(label);}
   for(let i=0;i<(new Date(year,number-1,1).getDay()+6)%7;i++)calendar.append(document.createElement('span'));
   const selected=new Set(dates());
   for(let day=1;day<=new Date(year,number,0).getDate();day++){
    const key=month.value+'-'+String(day).padStart(2,'0'),button=document.createElement('button');
    const sameTime=[...list.children].filter(row=>row.querySelector('[name$=".BatDauLuc"]').value===key+'T'+$('#batch-start').value&&row.querySelector('[name$=".KetThucLuc"]').value===key+'T'+$('#batch-end').value);
    button.type='button';button.textContent=day;button.disabled=key<today;button.setAttribute('aria-label',(sameTime.length?'Bỏ chọn khung giờ ngày ':'Chọn ngày ')+key);button.setAttribute('aria-pressed',sameTime.length>0);if(key===today)button.classList.add('today');if(selected.has(key))button.classList.add('selected');
    const count=dates().filter(date=>date===key).length;if(count>1){const badge=document.createElement('small');badge.className='calendar-session-count';badge.textContent=count+' buổi';button.append(badge);}
    button.addEventListener('click',()=>{if(!timesValid())return;if(sameTime.length){sameTime.forEach(row=>row.remove());feedback.textContent='Đã bỏ khung '+$('#batch-start').value+'–'+$('#batch-end').value+' ngày '+key;}else if(add(key))feedback.textContent='Đã thêm buổi ngày '+key;else feedback.textContent=list.children.length>=100?'Tối đa 100 buổi cho mỗi lần lưu.':'Khung giờ này đã qua hoặc chồng với buổi đã chọn. Chọn giờ khác.';sync();calendar.querySelector('[aria-label$="'+key+'"]')?.focus();});calendar.append(button);
   }
  }
  function sync(){window.EduConnect.clearRowErrors(form,list,'Buoi');reindex(list,'Buoi');$('[data-schedule-count]').textContent='('+list.children.length+')';$('[data-schedule-empty]').hidden=list.children.length>0;$('[data-clear-schedule]').hidden=!list.children.length;$('#apply-times').hidden=!list.children.length;draw();}
  month.addEventListener('change',draw);
  $('#batch-start').addEventListener('change',draw);$('#batch-end').addEventListener('change',draw);
  form.querySelectorAll('[data-schedule-mode]').forEach(button=>button.addEventListener('click',()=>{
   const repeat=button.dataset.scheduleMode==='repeat';
   $('[data-single-panel]').hidden=repeat;$('[data-repeat-panel]').hidden=!repeat;
   form.querySelectorAll('[data-schedule-mode]').forEach(choice=>choice.setAttribute('aria-pressed',String(choice===button)));
  }));
  list.addEventListener('click',event=>{const button=event.target.closest('[data-remove-row]');if(button){button.closest('.schedule-row').remove();sync();}});
  list.addEventListener('change',sync);
  $('[data-clear-schedule]').addEventListener('click',()=>{list.replaceChildren();feedback.textContent='Đã bỏ tất cả ngày đã chọn.';sync();});
  $('#apply-weekdays').addEventListener('click',()=>{
   const from=$('#range-from').value,to=$('#range-to').value,days=[...form.querySelectorAll('[data-weekday]:checked')].map(input=>Number(input.value));
   if(!from||!to||from>to||!days.length){feedback.textContent='Chọn khoảng ngày hợp lệ và ít nhất một thứ.';return;}
   if(!timesValid())return;
   const start=new Date(from+'T12:00:00'),last=new Date(to+'T12:00:00');if((last-start)/86400000>365){feedback.textContent='Mỗi lần chỉ chọn khoảng ngày tối đa một năm.';return;}
   let count=0;for(let date=start;date<=last;date.setDate(date.getDate()+1)){const day=date.getFullYear()+'-'+String(date.getMonth()+1).padStart(2,'0')+'-'+String(date.getDate()).padStart(2,'0');if(days.includes(date.getDay())&&add(day))count++;}
   feedback.textContent='Đã thêm '+count+' buổi. '+(list.children.length>=100?'Đã đạt giới hạn 100 buổi cho một lần lưu.':'Các khung đã qua hoặc chồng giờ được bỏ qua.');
   if(count)month.value=from.slice(0,7);
   sync();
   if(count)$('[data-schedule-count]').closest('section').scrollIntoView({behavior:window.EduConnect.motionBehavior(),block:'start'});
  });
  $('#apply-times').addEventListener('click',()=>{if(!timesValid())return;if(new Set(dates()).size!==dates().length){feedback.textContent='Có nhiều buổi cùng ngày. Hãy chỉnh giờ riêng cho từng buổi.';return;}list.querySelectorAll('[name$=".BatDauLuc"]').forEach(input=>input.value=input.value.slice(0,10)+'T'+$('#batch-start').value);list.querySelectorAll('[name$=".KetThucLuc"]').forEach(input=>input.value=input.value.slice(0,10)+'T'+$('#batch-end').value);feedback.textContent='Đã áp dụng giờ chung cho '+list.children.length+' buổi.';sync();});
  sync();
 });
};
