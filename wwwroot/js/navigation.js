(() => {
  const ui = window.EduConnect;
  const back = document.querySelector('[data-page-back]');
  const backLabels = /^(Quay lại|Về lớp học|Về quản lý hồ sơ|Về lớp học của tôi|← Buổi học|← Bài tập của lớp|← Danh sách buổi học)$/;
  let leaving = false;
  let pending;
  ui.confirmPageLeave = () => {
    if (pending) return pending;
    const previous = document.activeElement;
    const element = document.createElement('div');
    element.className = 'modal fade';
    element.tabIndex = -1;
    element.setAttribute('aria-labelledby', 'leave-page-title');
    element.innerHTML = '<div class="modal-dialog modal-dialog-centered"><div class="modal-content"><div class="modal-header"><h2 class="modal-title fs-5" id="leave-page-title">Rời trang này?</h2><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button></div><div class="modal-body"><p>Thay đổi chưa lưu sẽ bị mất.</p></div><div class="modal-footer"><button type="button" class="btn btn-primary" data-bs-dismiss="modal">Tiếp tục chỉnh sửa</button><button type="button" class="btn btn-outline-danger" data-leave-confirm>Rời trang</button></div></div></div>';
    document.body.append(element);
    const modal = new bootstrap.Modal(element);
    pending = new Promise(resolve => {
      let accepted = false;
      element.querySelector('[data-leave-confirm]').onclick = () => { accepted = true; modal.hide(); };
      element.addEventListener('hidden.bs.modal', () => {
        modal.dispose(); element.remove(); pending = null;
        previous?.focus(); resolve(accepted);
      }, { once: true });
    });
    modal.show();
    return pending;
  };
  function configureBack() {
    if (!back) return;
    const local = [...document.querySelectorAll('main a[href]')].find(a => !a.closest('.modal') && backLabels.test(a.textContent.trim()));
    if (local) {
      const url = new URL(local.href, location.href);
      if (url.origin === location.origin) { back.href = url.href; local.hidden = true; }
    } else if (back.dataset.parentPage !== 'true' && document.referrer) {
      const referrer = new URL(document.referrer);
      if (referrer.origin === location.origin && referrer.href !== location.href
          && !/\/TaiKhoan\/(DangNhap|DangXuat|DangKy|QuenMatKhau|DatLaiMatKhau|Anh)(\/|$)/i.test(referrer.pathname))
        back.href = referrer.href;
    }
  }
  document.addEventListener('DOMContentLoaded', configureBack);
  document.addEventListener('click', async event => {
    const link = event.target.closest('a[href]');
    if (!link || event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || link.target === '_blank' || link.hasAttribute('download')) return;
    if (link.closest('.modal') && backLabels.test(link.textContent.trim())) {
      event.preventDefault(); bootstrap.Modal.getInstance(link.closest('.modal'))?.hide(); return;
    }
    if (link.hasAttribute('data-modal') || link.getAttribute('href').startsWith('#') || !ui.hasUnsavedChanges?.()) return;
    event.preventDefault();
    if (await ui.confirmPageLeave()) { leaving = true; location.assign(link.href); }
  });
  const allowed = new WeakSet();
  document.addEventListener('submit', async event => {
    const form = event.target;
    if (form.method.toLowerCase() !== 'get') return;
    if (allowed.delete(form)) {
      leaving = true;
      queueMicrotask(() => { if (event.defaultPrevented) leaving = false; });
      return;
    }
    if (!ui.hasUnsavedChanges?.()) return;
    event.preventDefault(); event.stopImmediatePropagation();
    if (await ui.confirmPageLeave()) {
      allowed.add(form); form.requestSubmit(event.submitter);
    }
  }, true);
  window.addEventListener('pageshow', () => { leaving = false; });
  window.addEventListener('beforeunload', event => {
    if (!leaving && ui.hasUnsavedChanges?.()) { event.preventDefault(); event.returnValue = ''; }
  });
})();
