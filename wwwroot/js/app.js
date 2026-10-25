window.EduConnect = window.EduConnect || {};
window.EduConnect.init = (root) => {
  'use strict';
  root = root || document;
  if(root===document)document.addEventListener('show.bs.modal',event=>{
    if(event.target.id!=='connection-request-modal')return;
    const modal=event.target,button=event.relatedTarget,form=modal.querySelector('form');
    form.reset();form.querySelectorAll('.ui-field-error,.ui-error-summary').forEach(node=>node.remove());
    form.querySelectorAll('.is-invalid').forEach(input=>{input.classList.remove('is-invalid');input.removeAttribute('aria-invalid');input.setAttribute('aria-describedby',(input.getAttribute('aria-describedby')||'').split(' ').filter(id=>!id.startsWith('ui-error-')).join(' '));});
    form.querySelector('[name="LienHeId"]').value=button?.dataset.contactId||'';
    modal.querySelector('[data-contact-recipient]').textContent=button?.dataset.contactName||'gia sư';
  });
  const $ = (selector, scope = root) => scope.querySelector(selector);
  const $$ = (selector, scope = root) => [...scope.querySelectorAll(selector)];
  $$('[data-other-subject]').forEach(group => {
    const toggle = $('input[type="checkbox"]', group), field = $('[data-other-subject-field]', group), input = $('input', field);
    const subjectSelect = group.closest('form')?.querySelector('select[name="MonHocId"]');
    function update(){field.hidden = !toggle.checked;input.disabled = !toggle.checked;input.required = toggle.checked;if(subjectSelect)subjectSelect.disabled=toggle.checked;window.EduConnect.markRequired?.(group);}
    toggle.addEventListener('change', update);update();
  });
  $('.mobile-menu')?.addEventListener('click', event => {
    const open = $('.sidebar').classList.toggle('open');
    event.currentTarget.setAttribute('aria-expanded', open);
  });
  $('[data-search-sort]')?.addEventListener('change', event => event.target.form.requestSubmit());
  function reindex(list, prefix) {
    $$(':scope > div', list).forEach((row, i) => {
      $$('[name]', row).forEach(input => {
        input.name = input.name.replace(new RegExp(prefix + '\\[\\d+\\]'), prefix + '[' + i + ']');
      });
    });
  }
  function modes() {
    const mode = $('[data-mode]');
    if (mode) $$('[data-inperson]').forEach(row => row.hidden = mode.value !== 'TrucTiep');
    if(mode){const form=mode.closest('form');const location=form.querySelector('[name="DiaDiem"]')||(form.querySelector('.subject-picker')?form.querySelector('[name="KhuVuc"]'):null);if(location)location.required=mode.value==='TrucTiep';window.EduConnect.markRequired?.(form);}
  }
  modes();
  $('[data-mode]')?.addEventListener('change', modes);
  $$('[name="KetQuaBuoiHoc"]').forEach(select=>{const form=select.closest('form'),input=form.querySelector('[name="NoiDung"]');if(!input)return;const update=()=>{input.required=select.value==='DaDienRa';window.EduConnect.markRequired?.(form);};select.addEventListener('change',update);update();});
  $$('a[data-page]').forEach(link => {
    const url = new URL(link.href);
    url.searchParams.set('Trang', link.dataset.page);
    link.href = url.href;
  });
  function updateCompare(limitReached = false) {
    const form = $('#compare-form');
    if (!form) return;
    const count = $$('input[form="compare-form"]:checked').length;
    form.hidden = count === 0;
    document.body.classList.toggle('has-compare-selection', count > 0);
    const status = $('#compare-status');
    status.replaceChildren(document.createTextNode('Đã chọn '));
    const number = document.createElement('strong');
    number.textContent = count + '/3';
    status.append(number, document.createTextNode(limitReached ? ' đối tượng · Tối đa 3 lựa chọn' : ' đối tượng'));
  }
  $$('input[form="compare-form"]').forEach(input => input.addEventListener('change', () => {
    const tooMany = $$('input[form="compare-form"]:checked').length > 3;
    if (tooMany) input.checked = false;
    updateCompare(tooMany);
  }));
  $('[data-compare-clear]')?.addEventListener('click', () => {
    const selected = $$('input[form="compare-form"]:checked');
    selected.forEach(input => input.checked = false);
    updateCompare();
    selected[0]?.focus();
  });
  if (root === document) window.addEventListener('pageshow', () => updateCompare());
  updateCompare();
  $('#compare-form')?.addEventListener('submit', event => {
    if (!$$('input[form="compare-form"]:checked').length) {
      event.preventDefault();
      $('#compare-status').textContent = 'Chọn ít nhất một đối tượng để so sánh.';
    }
  });
  const stream = $('.chat-stream');
  if (stream && !new URLSearchParams(location.search).has('page')) stream.scrollTop = stream.scrollHeight;
  window.EduConnect.initScheduling?.(root);
  window.EduConnect.markRequired?.(root);
};
window.EduConnect.init();
