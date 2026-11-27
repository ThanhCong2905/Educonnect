const { chromium } = require('playwright');
const fs = require('node:fs');
const assert = require('node:assert/strict');

const base = process.env.AUTH_TEST_URL || 'http://127.0.0.1:5299';
assert.ok(['127.0.0.1', 'localhost'].includes(new URL(base).hostname));

(async () => {
    const profile = JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json'));
    const chat = JSON.parse(fs.readFileSync('tests/ChatAttachments/bin/Debug/net10.0/results.json'));
    assert.ok(profile.database.startsWith('EduConnect_Verification_'));
    assert.equal(profile.database, chat.database);
    const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
    const results = [];
    try {
        for (const [role, id] of [['GiaSu', profile.tutor], ['HocSinh', profile.learner], ['PhuHuynh', chat.reader], ['QuanTri', profile.admin]]) {
            const home = role === 'QuanTri' ? '/QuanTri/QuanTri/Index' : '/';
            const denied = role === 'QuanTri' ? '/NhuCau/NhuCau/Index' : '/QuanTri/QuanTri/Index';
            for (const [name, target, expected] of [
                ['Trang chủ', '', home],
                ['Đích cũ không còn truy cập được', '/KetNoi/KetNoi/ChiTiet/73785156-b597-4dba-aa35-e1a29ebfdbad', home],
                ['Đích hợp lệ được giữ', '/TaiKhoan/HoSo', '/TaiKhoan/HoSo'],
                ['Đích khác quyền', denied, home]
            ]) {
                const context = await browser.newContext();
                try {
                    const page = await context.newPage();
                    await page.goto(base + '/TaiKhoan/DangNhap' + (target ? '?returnUrl=' + encodeURIComponent(target) : ''));
                    await page.locator('[name="Email"]').fill(id.replaceAll('-', '') + '@example.test');
                    await page.locator('[name="MatKhau"]').fill('EduConnect kiểm thử 2026!');
                    const submitted = page.waitForResponse(r => r.request().method() === 'POST' && r.url().includes('/TaiKhoan/DangNhap'));
                    await page.getByRole('button', { name: 'Đăng nhập', exact: true }).click();
                    assert.equal((await submitted).status(), 302);
                    await page.waitForURL(base + expected);
                    await page.waitForLoadState('networkidle');
                    assert.equal((await page.request.get(page.url())).status(), 200);
                    assert.equal((await page.request.get(base + '/khong-co-trang-nay', { maxRedirects: 0 })).status(), 404);
                    results.push({ role, name, status: 'Pass', url: page.url() });
                    console.log('PASS', role, name);
                } finally { await context.close(); }
            }
        }
    } finally { await browser.close(); }
    fs.writeFileSync('.codex-local/login-return-browser-results.json', JSON.stringify({ database: profile.database, results }, null, 2));
})().catch(error => { console.error(error); process.exitCode = 1; });
