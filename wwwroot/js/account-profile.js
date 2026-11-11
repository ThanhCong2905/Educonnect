(() => {
  document.addEventListener('error', event => {
    const image = event.target;
    if (!(image instanceof HTMLImageElement) || !image.closest('.avatar')) return;
    const initial = image.parentElement.querySelector('[data-avatar-initial]');
    if (initial) { image.hidden = true; initial.hidden = false; }
    else image.replaceWith(document.createTextNode(image.dataset.avatarFallback || '?'));
  }, true);
  const refresh = form => {
    const file = form.elements.namedItem('AnhDaiDienFile')?.files[0];
    const remove = form.elements.namedItem('XoaAnhDaiDien')?.value === 'true';
    const image = document.querySelector('[data-avatar-preview]');
    const initial = document.querySelector('[data-avatar-initial]');
    if (!image || !initial) return;
    const source = file ? URL.createObjectURL(file) : remove ? null : image.dataset.currentSrc;
    const removeButton = document.querySelector('[data-avatar-remove]');
    if (removeButton) removeButton.disabled = !source;
    const status = document.querySelector('[data-avatar-status]');
    if (status) {
      status.hidden = !file && !remove;
      status.textContent = file ? 'Ảnh mới chưa được lưu.' : remove ? 'Ảnh sẽ được xóa khi bạn lưu thông tin.' : '';
    }
    image.hidden = !source;
    initial.hidden = !!source;
    if (source) image.src = source;
    else image.removeAttribute('src');
    if (file) image.onload = () => URL.revokeObjectURL(source);
  };
  document.addEventListener('change', event => {
    const field = event.target;
    const form = field.form;
    if (!form?.hasAttribute('data-account-profile') || !['AnhDaiDienFile', 'XoaAnhDaiDien'].includes(field.name)) return;
    const picker = form.elements.namedItem('AnhDaiDienFile');
    const remove = form.elements.namedItem('XoaAnhDaiDien');
    if (field === picker && picker.files.length && remove) remove.value = 'false';
    refresh(form);
  });
  document.addEventListener('click', event => {
    const choose = event.target.closest('[data-avatar-choose]');
    const remove = event.target.closest('[data-avatar-remove]');
    if (choose) document.querySelector('[data-avatar-file]')?.click();
    if (remove) {
      const picker = document.querySelector('[data-avatar-file]');
      const field = picker.form.elements.namedItem('XoaAnhDaiDien');
      picker.value = '';
      field.value = 'true';
      picker.dispatchEvent(new Event('change', { bubbles: true }));
    }
  });
})();
