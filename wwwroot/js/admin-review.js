(() => {
    document.addEventListener('click', async event => {
        const button = event.target.closest('[data-admin-copy]');
        if (!button) return;
        const status = button.closest('details').querySelector('[data-copy-status]');
        try {
            await navigator.clipboard.writeText(button.dataset.adminCopy);
            status.textContent = 'Đã sao chép.';
        } catch {
            status.textContent = 'Hãy chọn và sao chép giá trị trực tiếp.';
        }
    });
    const report = document.querySelector('[data-admin-report-result]');
    if (report) {
        const result = report.elements.namedItem('KetQua');
        const measure = report.querySelector('[data-admin-report-measure]');
        const update = () => {
            measure.hidden = result.value !== 'XuLy';
            measure.querySelector('select').disabled = measure.hidden;
        };
        result.addEventListener('change', update);
        update();
    }

    const form = document.querySelector('[data-profile-review]');
    const dialog = form?.querySelector('[data-review-dialog]');
    if (!dialog) return;
    const reason = dialog.querySelector('textarea');
    const summary = dialog.querySelector('[data-review-summary]');
    const submit = dialog.querySelector('[data-review-submit]');
    const fields = [...form.querySelectorAll('[data-review-field]')];
    let opener;
    let rejecting = false;

    function updateDialog() {
        const checked = fields.filter(field => field.checked);
        const incomplete = checked.some(field => !document.getElementById(field.getAttribute('aria-controls')).querySelector('textarea').value.trim());
        summary.textContent = incomplete ? 'Điền góp ý cho các mục đã chọn trước khi gửi.'
            : checked.length ? 'Sẽ gửi góp ý cho ' + checked.length + ' mục đã chọn.'
            : rejecting ? 'Lý do này sẽ hiển thị cho gia sư.' : 'Chọn ít nhất một mục cần sửa trên hồ sơ.';
        submit.disabled = incomplete || (rejecting ? !reason.value.trim() : !checked.length);
    }

    for (const field of fields) {
        const panel = document.getElementById(field.getAttribute('aria-controls'));
        const input = panel.querySelector('textarea');
        if (input.value.trim()) field.checked = true;
        const toggle = () => {
            panel.hidden = !field.checked;
            input.disabled = !field.checked;
            if (field.checked && document.activeElement === field) input.focus();
        };
        field.addEventListener('change', toggle);
        toggle();
    }

    reason.addEventListener('input', updateDialog);
    for (const button of form.querySelectorAll('[data-review-open]')) {
        button.addEventListener('click', () => {
            opener = button;
            rejecting = button.dataset.reviewOpen === 'reject';
            const title = rejecting ? 'Từ chối hồ sơ' : 'Yêu cầu chỉnh sửa';
            dialog.querySelector('h2').textContent = title;
            dialog.querySelector('[data-review-reason]').hidden = !rejecting;
            reason.disabled = !rejecting;
            reason.required = rejecting;
            submit.textContent = rejecting ? 'Xác nhận từ chối' : 'Gửi yêu cầu chỉnh sửa';
            submit.value = rejecting ? 'TuChoi' : 'CanChinhSua';
            submit.className = rejecting ? 'btn btn-outline-danger' : 'btn btn-primary';
            updateDialog();
            dialog.showModal();
            if (rejecting) reason.focus();
        });
    }
    for (const button of dialog.querySelectorAll('[data-review-close]')) {
        button.addEventListener('click', () => dialog.close());
    }
    dialog.addEventListener('close', () => {
        reason.required = false;
        reason.disabled = true;
        opener?.focus();
    });
})();
