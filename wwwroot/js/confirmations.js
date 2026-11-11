(() => {
    const approved = new WeakSet();
    let pending;
    let dialog;

    function submit(form, button) {
        approved.add(form);
        form.requestSubmit(button ?? undefined);
    }

    function openInForm(message, form, button) {
        const previous = form.querySelector('[data-action-confirmation]');
        if (previous) {
            previous.querySelector('button').focus();
            return;
        }
        const confirmation = document.createElement('div');
        confirmation.dataset.actionConfirmation = '';
        confirmation.className = 'discard-warning';
        const text = document.createElement('p');
        text.textContent = message;
        const actions = document.createElement('div');
        actions.className = 'actions';
        const back = document.createElement('button');
        back.type = 'button';
        back.className = 'btn btn-outline-primary';
        back.textContent = 'Kiểm tra lại';
        back.addEventListener('click', () => { confirmation.remove(); button?.focus(); });
        const accept = document.createElement('button');
        accept.type = 'button';
        accept.className = 'btn btn-primary';
        accept.textContent = 'Xác nhận';
        accept.addEventListener('click', () => { confirmation.remove(); submit(form, button); });
        actions.append(back, accept);
        confirmation.append(text, actions);
        (button?.closest('.actions') ?? form.lastElementChild)?.before(confirmation);
        back.focus();
    }

    function open(message, form, button) {
        if (form.closest('.modal.show')) {
            openInForm(message, form, button);
            return;
        }
        if (!dialog) {
            dialog = document.createElement('div');
            dialog.className = 'modal fade';
            dialog.tabIndex = -1;
            dialog.setAttribute('aria-labelledby', 'action-confirm-title');
            dialog.innerHTML = '<div class="modal-dialog modal-dialog-centered"><div class="modal-content"><div class="modal-header"><h2 class="modal-title fs-5" id="action-confirm-title">Xác nhận thao tác</h2><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button></div><div class="modal-body"><p data-confirm-text></p></div><div class="modal-footer"><button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">Quay lại</button><button type="button" class="btn btn-primary" data-confirm-accept>Tiếp tục</button></div></div></div>';
            document.body.append(dialog);
            dialog.querySelector('[data-confirm-accept]').addEventListener('click', () => {
                const task = pending;
                pending = null;
                bootstrap.Modal.getInstance(dialog).hide();
                if (task?.form.isConnected) submit(task.form, task.button);
            });
            dialog.addEventListener('hidden.bs.modal', () => {
                pending?.button?.focus();
                pending = null;
            });
        }
        pending = { form, button };
        dialog.querySelector('[data-confirm-text]').textContent = message;
        bootstrap.Modal.getOrCreateInstance(dialog).show();
    }

    document.addEventListener('submit', event => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (approved.delete(form)) return;
        let message = form.dataset.confirm;
        if (!message && form.hasAttribute('data-confirm-fee') && event.submitter?.value !== 'true') {
            const amount = Number(form.elements.namedItem('HocPhiDuKien')?.value);
            const unit = form.elements.namedItem('DonViHocPhi')?.value;
            if (amount > 0 && ['VND/Gio', 'VND/Buoi'].includes(unit)) {
                message = `Bạn đăng học phí ${new Intl.NumberFormat('vi-VN').format(amount)} đồng/${unit === 'VND/Gio' ? 'giờ' : 'buổi'}. Thông tin này đã đúng chưa?`;
            }
        }
        if (!message || !window.bootstrap) return;
        event.preventDefault();
        event.stopImmediatePropagation();
        open(message, form, event.submitter);
    }, true);
})();
