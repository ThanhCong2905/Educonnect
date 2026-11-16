window.EduConnect = window.EduConnect || {};
window.EduConnect.initClassCalendar = (root = document) => {
    root.querySelectorAll('[data-week-calendar]').forEach(calendar => {
        if (calendar.dataset.ready) return;
        calendar.dataset.ready = 'true';
        const panel = calendar.querySelector('[data-lesson-panel]');
        let trigger = null;

        function close(restoreFocus = false) {
            const previous = trigger;
            trigger?.setAttribute('aria-expanded', 'false');
            trigger = null;
            panel.hidden = true;
            panel.replaceChildren();
            if (restoreFocus) previous?.focus();
        }

        function open(link) {
            const template = document.getElementById(link.dataset.lessonPreview);
            if (!template) return;
            close();
            trigger = link;
            panel.replaceChildren(template.content.cloneNode(true));
            panel.hidden = false;
            link.setAttribute('aria-expanded', 'true');
            position();
            panel.focus({ preventScroll: true });
        }

        function position() {
            if (!trigger) return;
            const source = trigger.getBoundingClientRect();
            if (!source.width && !source.height) { close(); return; }
            const box = panel.getBoundingClientRect();
            let left = source.right + 10;
            if (left + box.width > innerWidth - 12) left = source.left - box.width - 10;
            panel.style.left = Math.max(12, Math.min(left, innerWidth - box.width - 12)) + 'px';
            panel.style.top = Math.max(12, Math.min(source.top, innerHeight - box.height - 12)) + 'px';
        }

        calendar.addEventListener('click', event => {
            const link = event.target.closest('[data-lesson-preview]');
            if (link && !event.ctrlKey && !event.metaKey && !event.shiftKey && !event.altKey) {
                if (!document.getElementById(link.dataset.lessonPreview)) return;
                event.preventDefault();
                if (trigger === link) close(true);
                else open(link);
            }
            if (event.target.closest('[data-close-lesson]')) close(true);
        });
        document.addEventListener('click', event => {
            if (trigger && !panel.contains(event.target) && !event.target.closest('[data-lesson-preview]')
                && !event.target.closest('.modal')) close();
        });
        document.addEventListener('keydown', event => {
            if (event.key === 'Escape' && trigger && !document.querySelector('.modal.show')) {
                event.preventDefault();
                close(true);
            }
        });
        panel.addEventListener('focusout', () => {
            setTimeout(() => {
                if (trigger && !panel.contains(document.activeElement) && document.activeElement !== trigger
                    && !document.querySelector('.modal.show')) close();
            }, 0);
        });
        window.addEventListener('resize', position);
        window.addEventListener('scroll', position, true);
    });
};
