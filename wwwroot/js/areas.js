(() => {
    let catalog;

    function loadCatalog() {
        catalog ??= fetch('/data/vietnam-wards.json', { credentials: 'same-origin' })
            .then(response => {
                if (!response.ok) throw new Error('Không tải được danh mục phường / xã.');
                return response.json();
            }).catch(error => {
                catalog = null;
                throw error;
            });
        return catalog;
    }

    document.addEventListener('change', async event => {
        if (!event.target.matches('[data-province]')) return;
        const province = event.target;
        const group = province.closest('[data-area-picker]');
        const ward = group.querySelector('[data-ward]');
        const status = group.querySelector('[data-area-status]');
        const selectedProvince = province.value;
        ward.replaceChildren(new Option(selectedProvince ? 'Đang tải phường / xã…' : 'Chọn tỉnh / thành phố trước', ''));
        ward.disabled = true;
        status.textContent = '';
        if (!selectedProvince) return;

        try {
            const data = await loadCatalog();
            if (province.value !== selectedProvince) return;
            ward.replaceChildren(new Option('Tất cả phường / xã', ''));
            for (const name of data.provinces[selectedProvince] ?? []) ward.add(new Option(name, name));
            ward.disabled = false;
        } catch {
            if (province.value !== selectedProvince) return;
            ward.replaceChildren(new Option('Chưa tải được danh mục', ''));
            status.textContent = 'Chọn lại tỉnh / thành phố để tải danh sách phường / xã.';
        }
    });
})();
