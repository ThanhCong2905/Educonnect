(() => {
  const ui = window.EduConnect = window.EduConnect || {};
  let active, popup, previous;
  const pad = value => String(value).padStart(2, '0');
  const caption = input => input.closest('label')?.querySelector('.field-caption')?.textContent.replace('*', '').trim()
    || input.closest('label')?.childNodes[0]?.textContent.trim() || 'Thời gian';
  const display = input => {
    if (!input.value) return input.type === 'time' ? 'Chọn giờ' : 'Chọn ngày và giờ';
    if (input.type === 'time') return input.value.slice(0, 5);
    const [date, time] = input.value.split('T'), [year, month, day] = date.split('-');
    return `${day}/${month}/${year} · ${time.slice(0, 5)}`;
  };
  function sync(input) {
    const button = input.parentElement.querySelector('[data-time-open]');
    if (!button) return;
    const text = button.querySelector('span'), value = display(input);
    if (text.textContent !== value) text.textContent = value;
    button.classList.toggle('time-empty', !input.value);
    if (button.disabled !== input.disabled) button.disabled = input.disabled;
    button.setAttribute('aria-label', `${caption(input)}: ${display(input)}`);
    button.setAttribute('aria-required', String(input.required));
    if (input.getAttribute('aria-invalid') !== 'true' && button.hasAttribute('aria-invalid')) {button.removeAttribute('aria-invalid');button.removeAttribute('aria-describedby');}
  }
  function durations(root = document) {
    root.querySelectorAll('.slot-row,.schedule-row,[data-schedule-times],.schedule-builder>.edu-form-grid').forEach(row => {
      const inputs = [...row.querySelectorAll('input[type="time"],input[type="datetime-local"]')];
      if (inputs.length !== 2) return;
      const [start, end] = inputs;
      let minutes = 0;
      if (start.value && end.value) {
        if (start.type === 'time') {
          const number = value => Number(value.slice(0, 2)) * 60 + Number(value.slice(3, 5));
          minutes = number(end.value) - number(start.value);
        } else minutes = (Date.parse(end.value + '+07:00') - Date.parse(start.value + '+07:00')) / 60000;
      }
      let hint = row.querySelector(':scope > .time-duration');
      if (!hint) { hint = document.createElement('span'); hint.className = 'time-duration'; hint.setAttribute('role', 'status'); row.append(hint); }
      hint.hidden = !Number.isFinite(minutes) || minutes <= 0;
      const text = minutes >= 60 ? `${Math.floor(minutes / 60)} giờ${minutes % 60 ? ' ' + minutes % 60 + ' phút' : ''}` : `${minutes} phút`;
      if (hint.textContent !== text) hint.textContent = text;
    });
  }
  function close(restore = true) {
    if (!popup) return;
    popup.remove();popup = null;
    previous?.setAttribute('aria-expanded', 'false');
    if (restore && previous?.isConnected) previous.focus({preventScroll: true});
    active = null;
  }
  function position() {
    if (!popup || !previous?.isConnected) return close(false);
    const rect = previous.getBoundingClientRect(), box = popup.getBoundingClientRect();
    popup.style.left = Math.max(12, Math.min(rect.left, innerWidth - box.width - 12)) + 'px';
    popup.style.top = Math.max(12, Math.min(rect.bottom + 8, innerHeight - box.height - 12)) + 'px';
  }
  function open(input, button) {
    if (input.matches(':disabled') || input.readOnly) return;
    if (active === input) return close();
    close(false);active = input;previous = button;button.setAttribute('aria-expanded', 'true');
    popup = document.createElement('section');popup.className = 'time-picker';popup.setAttribute('role', 'dialog');popup.setAttribute('aria-label', caption(input));
    const heading = document.createElement('div');heading.className = 'time-picker-heading';
    const title = document.createElement('strong');title.textContent = caption(input);
    const dismiss = document.createElement('button');dismiss.type = 'button';dismiss.className = 'btn-close';dismiss.setAttribute('aria-label', 'Đóng bộ chọn thời gian');dismiss.onclick = () => close();heading.append(title, dismiss);popup.append(heading);
    let date;
    if (input.type === 'datetime-local') {
      const label = document.createElement('label');label.textContent = 'Ngày';date = document.createElement('input');date.type = 'date';date.className = 'form-control';date.value = input.value.slice(0, 10);label.append(date);popup.append(label);
    }
    const clock = document.createElement('div');clock.className = 'time-picker-clock';
    function select(labelText, maximum, selected) {
      const label = document.createElement('label');label.textContent = labelText;
      const select = document.createElement('select');select.className = 'form-select';select.add(new Option(labelText, ''));
      for (let n = 0; n < maximum; n++) select.add(new Option(pad(n), pad(n)));
      select.value = selected;label.append(select);clock.append(label);return select;
    }
    const time = input.type === 'time' ? input.value : input.value.split('T')[1] || '';
    const hour = select('Giờ', 24, time.slice(0, 2)), minute = select('Phút', 60, time.slice(3, 5));popup.append(clock);
    const note = document.createElement('p');note.className = 'time-picker-zone';note.textContent = 'Giờ Việt Nam · 24 giờ';popup.append(note);
    const actions = document.createElement('div');actions.className = 'time-picker-actions';
    if (!input.required) {
      const clear = document.createElement('button');clear.type = 'button';clear.className = 'btn btn-outline-primary';clear.textContent = 'Xóa';clear.onclick = () => apply('');actions.append(clear);
    }
    const confirm = document.createElement('button');confirm.type = 'button';confirm.className = 'btn btn-primary';confirm.textContent = 'Chọn thời gian';actions.append(confirm);popup.append(actions);
    function update() { confirm.disabled = hour.value === '' || minute.value === '' || (date && !date.value); }
    function apply(value) { input.value = value;input.dispatchEvent(new Event('input', {bubbles: true}));input.dispatchEvent(new Event('change', {bubbles: true}));close(); }
    confirm.onclick = () => apply((date ? date.value + 'T' : '') + hour.value + ':' + minute.value);
    popup.addEventListener('change', update);update();(input.closest('.modal,.offcanvas,.offcanvas-lg') || document.body).append(popup);position();(date || hour).focus();
  }
  ui.initTimePickers = (root = document) => {
    root.querySelectorAll('input[type="time"],input[type="datetime-local"]').forEach(input => {
      if (input.dataset.timeReady) { sync(input);return; }
      input.dataset.timeReady = 'true';
      const wrapper = document.createElement('span');wrapper.className = 'time-control';input.before(wrapper);wrapper.append(input);
      input.classList.add('time-source');input.tabIndex = -1;input.setAttribute('aria-hidden', 'true');
      const button = document.createElement('button');button.type = 'button';button.className = 'form-control time-display';button.dataset.timeOpen = '';button.setAttribute('aria-haspopup', 'dialog');button.setAttribute('aria-expanded', 'false');
      const text = document.createElement('span'), icon = document.createElement('img');icon.src = '/images/ui/clock-history.svg';icon.alt = '';button.append(text, icon);wrapper.append(button);button.onclick = () => open(input, button);
      input.addEventListener('input', () => {sync(input);durations(input.closest('form') || document);});input.addEventListener('change', () => sync(input));sync(input);
    });
    durations(root);
  };
  document.addEventListener('click', event => { if (popup && !popup.contains(event.target) && !event.target.closest('[data-time-open]')) close(false); });
  document.addEventListener('keydown', event => {
    if (!popup) return;
    if (event.key === 'Escape') {event.preventDefault();event.stopImmediatePropagation();close();}
    else if (event.key === 'Tab') {
      const controls = [...popup.querySelectorAll('button:not(:disabled),input,select')];
      const first = controls[0], last = controls.at(-1);
      if (event.shiftKey && document.activeElement === first) {event.preventDefault();last.focus();}
      else if (!event.shiftKey && document.activeElement === last) {event.preventDefault();first.focus();}
    }
  }, true);
  document.addEventListener('hide.bs.modal', () => close(false));
  addEventListener('resize', position);addEventListener('scroll', position, true);
  const init = () => ui.initTimePickers();
  document.addEventListener('DOMContentLoaded', init);
  new MutationObserver(init).observe(document.documentElement, {childList: true, subtree: true, attributes: true, attributeFilter: ['disabled']});
})();
