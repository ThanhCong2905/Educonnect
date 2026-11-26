const assert = require('node:assert/strict');

module.exports = async function signIn(page, base, userId) {
    const url = new URL(base);
    assert.ok(['127.0.0.1', 'localhost'].includes(url.hostname), 'Fixture login requires a local test server.');
    await page.context().clearCookies();
    await page.goto(base + '/TaiKhoan/DangNhap');
    const token = await page.locator('input[name="__RequestVerificationToken"]').first().inputValue();
    const response = await page.request.post(base + '/TaiKhoan/DangNhap', {
        form: {
            Email: userId.replaceAll('-', '').toLowerCase() + '@example.test',
            MatKhau: 'EduConnect kiểm thử 2026!',
            __RequestVerificationToken: token
        },
        maxRedirects: 0
    });
    assert.equal(response.status(), 302, 'Fixture account could not sign in.');
    await page.goto(base + '/');
};
