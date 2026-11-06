(() => {
    'use strict';
    const rail = document.querySelector('[data-notification-rail]');
    if (!rail) return;
    const notification = rail.querySelector('[data-notification]');
    const text = rail.querySelector('[data-notification-text]');
    let timer;

    function dismiss() {
        clearTimeout(timer);
        notification.hidden = true;
        text.textContent = '';
    }

    function scheduleDismiss() {
        clearTimeout(timer);
        if (notification.hidden || notification.matches(':hover') || notification.contains(document.activeElement)) return;
        timer = setTimeout(dismiss, 5000);
    }

    function toast(message) {
        if (!message) return;
        clearTimeout(timer);
        text.textContent = message;
        notification.hidden = false;
        scheduleDismiss();
    }

    notification.querySelector('[data-notification-close]').addEventListener('click', dismiss);
    notification.addEventListener('mouseenter', () => clearTimeout(timer));
    notification.addEventListener('mouseleave', scheduleDismiss);
    notification.addEventListener('focusin', () => clearTimeout(timer));
    notification.addEventListener('focusout', () => setTimeout(scheduleDismiss, 0));
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) clearTimeout(timer);
        else scheduleDismiss();
    });
    window.addEventListener('pageshow', scheduleDismiss);
    window.EduConnect = window.EduConnect || {};
    window.EduConnect.toast = toast;
    const pending = sessionStorage.getItem('educonnect:toast');
    if (pending) {
        sessionStorage.removeItem('educonnect:toast');
        toast(pending);
    } else scheduleDismiss();
})();
