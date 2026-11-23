document.querySelectorAll('[data-password-toggle]').forEach(button => {
    const input = document.getElementById(button.dataset.passwordToggle);
    if (!input) return;
    button.hidden = false;
    button.addEventListener('click', () => {
        const visible = input.type === 'password';
        input.type = visible ? 'text' : 'password';
        button.textContent = visible ? 'Ẩn' : 'Hiện';
        button.setAttribute('aria-label', visible ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');
        button.setAttribute('aria-pressed', String(visible));
    });
});

const verification = document.querySelector('[data-email-verification]');
if (verification) {
    const pending = verification.querySelector('[data-verification-pending]');
    const success = verification.querySelector('[data-verification-success]');
    const form = verification.querySelector('[data-verification-resend]');
    const button = form?.querySelector('button');
    const statusUrl = verification.dataset.statusUrl;
    let readyAt = Date.now() + Number(verification.dataset.waitSeconds) * 1000;
    let submitting = false;
    let checking = false;

    function renderCountdown() {
        if (!button || submitting || pending.hidden) return;
        const remaining = Math.max(0, Math.ceil((readyAt - Date.now()) / 1000));
        button.disabled = remaining > 0;
        button.textContent = remaining > 0 ? `Gửi lại sau ${remaining} giây` : 'Gửi lại thư xác minh';
    }

    async function checkStatus() {
        if (!statusUrl || pending.hidden || document.hidden || checking) return;
        checking = true;
        try {
            const response = await fetch(statusUrl, { cache: 'no-store' });
            if (!response.ok || response.status === 204) return;
            const status = await response.json();
            if (status.daXacMinh) {
                pending.hidden = true;
                success.hidden = false;
                clearInterval(clock);
                clearInterval(polling);
            } else {
                readyAt = Date.now() + status.giayCho * 1000;
                renderCountdown();
            }
        } catch {
            // Keep the local countdown while the network is unavailable.
        } finally {
            checking = false;
        }
    }

    form?.addEventListener('submit', event => {
        if (button.disabled) {
            event.preventDefault();
            return;
        }
        submitting = true;
        button.disabled = true;
        button.textContent = 'Đang gửi…';
    });

    const clock = pending.hidden ? null : setInterval(renderCountdown, 1000);
    const polling = !statusUrl || pending.hidden ? null : setInterval(checkStatus, 5000);
    document.addEventListener('visibilitychange', checkStatus);
    window.addEventListener('pageshow', () => {
        submitting = false;
        renderCountdown();
        checkStatus();
    });
    renderCountdown();
}
