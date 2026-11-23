(() => {
    let dialog;
    let opener;
    document.addEventListener('click', event => {
        const link = event.target.closest('a[data-file-preview]');
        if (!link || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey ||
            typeof HTMLDialogElement !== 'function' || new URL(link.href).origin !== location.origin) return;
        const type = link.dataset.filePreview;
        if (!['image/png', 'image/jpeg', 'application/pdf'].includes(type)) return;
        event.preventDefault();
        if (!dialog) {
            dialog = document.createElement('dialog');
            dialog.className = 'file-preview';
            dialog.setAttribute('aria-labelledby', 'file-preview-title');
            dialog.innerHTML = '<header><h2 id="file-preview-title"></h2><button type="button" class="btn-close" aria-label="Đóng bản xem trước"></button></header><div class="file-preview-body"></div><footer><a class="btn btn-outline-primary" data-preview-tab target="_blank" rel="noopener">Mở ở tab mới</a><a class="btn btn-outline-primary" data-preview-download>Tải xuống</a></footer>';
            dialog.querySelector('button').addEventListener('click', () => dialog.close());
            dialog.addEventListener('close', () => {
                dialog.querySelector('.file-preview-body').replaceChildren();
                if (opener?.isConnected) opener.focus();
            });
            document.body.append(dialog);
        }
        opener = link;
        const name = link.dataset.fileName || link.textContent.trim() || 'Tệp đính kèm';
        const isImage = type.startsWith('image/');
        const content = document.createElement(isImage ? 'img' : 'iframe');
        content[isImage ? 'alt' : 'title'] = name;
        const body = dialog.querySelector('.file-preview-body');
        if (isImage) content.addEventListener('error', () => {
            if (!body.contains(content)) return;
            const message = document.createElement('p');
            message.className = 'm-3';
            message.textContent = 'Chưa xem được ảnh này. Bạn có thể thử lại hoặc tải tệp xuống.';
            body.replaceChildren(message);
        }, { once: true });
        content.src = link.href;
        dialog.querySelector('h2').textContent = name;
        body.replaceChildren(content);
        const newTab = dialog.querySelector('[data-preview-tab]');
        newTab.href = link.href;
        newTab.hidden = isImage;
        const download = dialog.querySelector('[data-preview-download]');
        download.href = link.dataset.fileDownload || link.href.split('?')[0];
        dialog.showModal();
    });
})();
