(() => {
    function scope(option, grade) {
        return !grade || !option?.dataset.gradeFrom ||
            grade >= Number(option.dataset.gradeFrom) && grade <= Number(option.dataset.gradeTo);
    }

    function update(container, changed) {
        const subject = container.querySelector('[data-scope-subject]');
        const grade = container.querySelector('[data-scope-grade]');
        const rowParts = container.querySelectorAll('[data-grade-parts]');
        if (!subject) return;
        const selected = subject.selectedOptions[0];
        if (grade) {
            const gradeNumber = Number(grade.value.replace('Lớp ', ''));
            const resetGrade = changed === subject && container.hasAttribute('data-search-scope');
            for (const option of subject.options) {
                option.disabled = option.hidden = !resetGrade && !scope(option, gradeNumber);
            }
            if (subject.selectedOptions[0]?.disabled) subject.value = '';
            if (resetGrade && !scope(selected, gradeNumber)) grade.value = '';
        }
        const integrated = subject.selectedOptions[0]?.dataset.integrated === 'true';
        const parts = container.querySelector('[data-integrated-parts]');
        if (parts) {
            parts.hidden = !integrated;
            for (const control of parts.querySelectorAll('input, select')) {
                if (!integrated) {
                    if (control.type === 'checkbox') control.checked = false;
                    else control.value = parts.hasAttribute('data-need-parts') ? '0' : '';
                }
                control.disabled = !integrated;
            }
        }
        for (const item of rowParts) {
            const lop = Number(item.dataset.gradeParts);
            const picker = item.querySelector('[data-grade-choice]');
            const available = scope(selected, lop);
            item.hidden = !available;
            picker.disabled = !available;
            if (!available) picker.checked = false;
            const options = item.querySelector('[data-tutor-parts]');
            options.hidden = !(available && picker.checked && integrated);
            for (const control of options.querySelectorAll('input')) {
                control.disabled = options.hidden;
                if (options.hidden) control.checked = false;
            }
        }
    }

    function init(root) {
        root.querySelectorAll('[data-subject-scope]').forEach(container => update(container));
    }

    if (window.EduConnect) window.EduConnect.initSubjectScope = init;

    document.addEventListener('change', event => {
        const container = event.target.closest('[data-subject-scope]');
        if (container) update(container, event.target);
    }, true);
    document.addEventListener('DOMContentLoaded', () => {
        init(document);
        new MutationObserver(changes => {
            for (const change of changes) for (const node of change.addedNodes) {
                if (!(node instanceof HTMLElement)) continue;
                if (node.matches('[data-subject-scope]')) update(node);
                init(node);
            }
        }).observe(document.body, { childList: true, subtree: true });
    });
})();
