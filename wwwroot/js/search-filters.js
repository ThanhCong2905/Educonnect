(() => {
    document.addEventListener('click', event => {
        if (event.target.closest('.time-picker')) return;
        if (!matchMedia('(min-width: 992px)').matches) return;
        document.querySelectorAll('.filter-more[open]').forEach(panel => {
            if (!panel.contains(event.target)) panel.open = false;
        });
    });
    document.addEventListener('keydown', event => {
        if (event.key !== 'Escape') return;
        const panel = event.target.closest('.filter-more[open]');
        if (!panel) return;
        panel.open = false;
        panel.querySelector('summary').focus();
    });
    document.addEventListener('change', event => {
        if (!event.target.matches('[data-search-scope] [data-scope-subject]')) return;
        if (event.target.selectedOptions[0]?.dataset.integrated === 'true') {
            event.target.form.querySelector('.filter-more').open = true;
        }
    });
})();
