const { chromium } = require('playwright');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { execFileSync } = require('node:child_process');

const base = process.env.AUTH_TEST_URL || 'http://127.0.0.1:5299';
const database = process.env.AUTH_TEST_DATABASE;
assert.ok(database?.startsWith('EduConnect_Verification_'), 'Chỉ chạy trên database kiểm thử.');
const evidence = path.resolve('.backups/login/auth-results');
fs.mkdirSync(evidence, { recursive: true });
const results = [];
const suffix = crypto.randomUUID().replaceAll('-', '');
const password = 'Học cùng EduConnect 2026!';
const replacementPassword = 'Mật khẩu mới của EduConnect!';
const accounts = {};

function sql(query) {
    const output = execFileSync('sqlcmd', ['-S', 'lpc:CONGVJP', '-d', database, '-E', '-C', '-I', '-b', '-h', '-1', '-w', '65535', '-f', '65001', '-Q', 'SET NOCOUNT ON; ' + query], { encoding: 'utf8' }).trim();
    return output;
}

function rows(query) {
    return JSON.parse(sql(query + ' FOR JSON PATH'));
}

async function token(client, route) {
    const response = await client.get(base + route);
    assert.equal(response.status(), 200, route);
    const html = await response.text();
    const match = html.match(/name="__RequestVerificationToken"[^>]*value="([^"]+)"/);
    assert.ok(match, 'Biểu mẫu có antiforgery token.');
    return match[1];
}

async function post(client, route, form, csrfRoute = '/TaiKhoan/DangNhap', ajax = false) {
    const csrf = await token(client, csrfRoute);
    return client.post(base + route, {
        form: { ...form, __RequestVerificationToken: csrf },
        headers: ajax ? { 'X-Requested-With': 'XMLHttpRequest' } : {},
        maxRedirects: 0
    });
}

async function run(id, name, action) {
    const start = Date.now();
    let status = 'Pass';
    let actual;
    try { actual = await action() || 'Các assertion trong ca đều đạt.'; }
    catch (error) { status = 'Fail'; actual = error.stack; }
    results.push({ id, name, status, actual, durationMs: Date.now() - start });
    fs.writeFileSync(path.join(evidence, 'results.json'), JSON.stringify({ database, base, results }, null, 2));
    console.log(`${status} ${id} ${name}`);
}

(async () => {
    const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
    const guest = await browser.newContext();
    const guestClient = guest.request;
    try {
        await run('AUTH-01', 'Database chuẩn có 37 bảng và view tài khoản', async () => {
            assert.equal(Number(sql('SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped=0')), 37);
            assert.equal(Number(sql("SELECT COUNT(*) FROM sys.views WHERE name='NguoiThamGia'")), 1);
            assert.equal(Number(sql("SELECT COUNT(*) FROM sys.tables WHERE name='NguoiThamGia'")), 0);
        });
        await run('AUTH-02', 'Khách phải đăng nhập trước khi mở nghiệp vụ', async () => {
            for (const route of ['/', '/NhuCau/NhuCau/Tao', '/TimKiem/TimKiem/Index', '/HocTap/TinNhan/Index']) {
                const response = await guestClient.get(base + route, { maxRedirects: 0 });
                assert.equal(response.status(), 302);
                assert.ok(response.headers().location.includes('/TaiKhoan/DangNhap'));
            }
        });
        await run('AUTH-03', 'AJAX và SignalR từ khách trả 401', async () => {
            assert.equal((await guestClient.get(base + '/TimKiem/TimKiem/Index', { headers: { 'X-Requested-With': 'XMLHttpRequest' } })).status(), 401);
            assert.equal((await guestClient.post(base + '/cap-nhat/negotiate?negotiateVersion=1')).status(), 401);
        });
        await run('AUTH-04', 'Đăng ký thiếu CSRF bị chặn', async () => {
            assert.equal((await guestClient.post(base + '/TaiKhoan/DangKy', { form: { Email: 'no-token@example.test' } })).status(), 400);
        });
        await run('AUTH-05', 'Đăng ký không được tự cấp vai trò quản trị', async () => {
            const email = `admin-${suffix}@example.test`;
            const response = await post(guestClient, '/TaiKhoan/DangKy', { HoTen: 'Không cấp quyền', Email: email, VaiTro: 'QuanTri', MatKhau: password, XacNhanMatKhau: password }, '/TaiKhoan/DangKy');
            assert.equal(response.status(), 200);
            assert.equal(Number(sql(`SELECT COUNT(*) FROM AspNetUsers WHERE Email='${email}'`)), 0);
        });
        await run('AUTH-06', 'Mật khẩu phổ biến bị từ chối', async () => {
            const response = await post(guestClient, '/TaiKhoan/DangKy', { HoTen: 'Mật khẩu phổ biến', Email: `weak-${suffix}@example.test`, VaiTro: 'HocSinh', MatKhau: 'passwordpassword', XacNhanMatKhau: 'passwordpassword' }, '/TaiKhoan/DangKy');
            assert.equal(response.status(), 200);
            assert.ok((await response.text()).includes('PasswordPolicy') === false);
            assert.equal(Number(sql(`SELECT COUNT(*) FROM AspNetUsers WHERE Email='weak-${suffix}@example.test'`)), 0);
        });
        for (const role of ['HocSinh', 'PhuHuynh', 'GiaSu']) {
            await run('REGISTER-' + role, 'Đăng ký và đăng nhập ' + role, async () => {
                const email = `${role.toLowerCase()}-${suffix}@example.test`;
                const response = await post(guestClient, '/TaiKhoan/DangKy', { HoTen: suffix + ' ' + role, Email: email, VaiTro: role, MatKhau: password, XacNhanMatKhau: password }, '/TaiKhoan/DangKy');
                assert.equal(response.status(), 302);
                const user = rows(`SELECT u.Id,u.HoTen,u.PasswordHash,r.Name AS VaiTro FROM AspNetUsers u JOIN AspNetUserRoles ur ON ur.UserId=u.Id JOIN AspNetRoles r ON r.Id=ur.RoleId WHERE u.Email='${email}'`)[0];
                assert.ok(user?.Id);
                assert.equal(user.VaiTro, role);
                assert.ok(user.PasswordHash && user.PasswordHash !== password);
                assert.equal(Number(sql(`SELECT COUNT(*) FROM ${role} WHERE TaiKhoanId='${user.Id}'`)), 1);
                const context = await browser.newContext();
                accounts[role] = { email, id: user.Id, context, request: context.request };
                const login = await post(context.request, '/TaiKhoan/DangNhap', { Email: email, MatKhau: password });
                assert.equal(login.status(), 302);
                const home = await context.request.get(base + '/');
                assert.equal(home.status(), 200);
                assert.ok(!(await home.text()).includes('Chuyển vai trò'));
                const cookie = (await context.cookies()).find(c => c.name === 'EduConnect.Identity');
                assert.ok(cookie?.httpOnly);
                assert.equal(cookie.sameSite, 'Lax');
            });
        }
        const learner = accounts.HocSinh;
        const parent = accounts.PhuHuynh;
        const tutor = accounts.GiaSu;
        await run('AUTH-10', 'Email trùng khác hoa thường chỉ có một tài khoản', async () => {
            const response = await post(guestClient, '/TaiKhoan/DangKy', { HoTen: 'Đăng ký trùng', Email: learner.email.toUpperCase(), VaiTro: 'GiaSu', MatKhau: password, XacNhanMatKhau: password }, '/TaiKhoan/DangKy');
            assert.equal(response.status(), 200);
            assert.equal(Number(sql(`SELECT COUNT(*) FROM AspNetUsers WHERE NormalizedEmail='${learner.email.toUpperCase()}'`)), 1);
        });
        await run('AUTH-11', 'Học sinh không mở được trang quản trị', async () => {
            const response = await learner.request.get(base + '/QuanTri/HoSo/Index', { maxRedirects: 0 });
            assert.equal(response.status(), 302);
            assert.ok(response.headers().location.includes('KhongCoQuyen'));
        });
        await run('AUTH-12', 'Gia sư không tạo nhu cầu học', async () => {
            assert.equal((await tutor.request.get(base + '/NhuCau/NhuCau/Tao', { headers: { 'X-Requested-With': 'XMLHttpRequest' } })).status(), 403);
        });
        await run('AUTH-13', 'Lưu hồ sơ và công khai theo cấu hình demo', async () => {
            const input = {
                HoTen: suffix + ' GiaSu', TrinhDo: 'Đại học', ChuyenMon: 'Toán', KinhNghiem: '3 năm', KhuVuc: 'Hà Nội', MoTa: 'Dạy Toán lớp 10', DayTrucTuyen: 'true',
                'MonDay[0].MonHocId': '1', 'MonDay[0].PhamViDay': 'Lớp 10–12', 'MonDay[0].HocPhiDeXuat': '150000', 'MonDay[0].DonViHocPhi': 'VND/Gio'
            };
            const saved = await post(tutor.request, '/NhuCau/HoSoGiaSu/Luu', input, '/NhuCau/HoSoGiaSu/Sua');
            assert.equal(saved.status(), 302);
            const profile = rows(`SELECT Id,RowVersion FROM HoSoGiaSu WHERE GiaSuId='${tutor.id}'`)[0];
            assert.ok(profile?.Id);
            tutor.profileId = profile.Id;
            const publish = await post(tutor.request, '/NhuCau/HoSoGiaSu/TrangThai', { operation: 'Hien', rowVersion: profile.RowVersion }, '/NhuCau/HoSoGiaSu/Sua');
            assert.equal(publish.status(), 302);
            const search = await learner.request.get(base + '/TimKiem/TimKiem/Tim?TuKhoa=' + suffix);
            assert.equal(search.status(), 200);
            assert.ok((await search.text()).toLowerCase().includes(profile.Id.toLowerCase()));
        });
        await run('AUTH-14', 'Tạo nhu cầu lấy đúng chủ từ danh tính đăng nhập', async () => {
            const form = {
                MonHocIds: '1', MucTieu: suffix + ' nhu cầu', KhoiLop: 'Lớp 10', SoBuoiTuan: '2', HinhThucHoc: 'TrucTuyen', BanNhap: 'false', HocPhiDuKien: '150000', DonViHocPhi: 'VND/Gio',
                'LichRanh[0].ThuTrongTuan': '1', 'LichRanh[0].GioBatDau': '19:00', 'LichRanh[0].GioKetThuc': '20:00', ChuSoHuuId: tutor.id
            };
            const saved = await post(learner.request, '/NhuCau/NhuCau/TaoNhuCauHoc', form, '/NhuCau/NhuCau/Tao');
            assert.equal(saved.status(), 302);
            const need = rows(`SELECT Id,ChuSoHuuId FROM NhuCauHoc WHERE MucTieu=N'${suffix} nhu cầu'`)[0];
            assert.equal(need?.ChuSoHuuId.toLowerCase(), learner.id.toLowerCase());
            learner.needId = need.Id;
            const response = await tutor.request.get(base + '/TimKiem/TimKiem/Tim?TuKhoa=' + suffix);
            assert.ok((await response.text()).toLowerCase().includes(need.Id.toLowerCase()));
        });
        await run('AUTH-15', 'Người khác không sửa được nhu cầu', async () => {
            assert.equal((await parent.request.get(base + '/NhuCau/NhuCau/Sua/' + learner.needId)).status(), 404);
        });
        await run('AUTH-16', 'Đường dẫn đổi người dùng mẫu đã đóng', async () => {
            const response = await post(learner.request, '/Home/Chon', { id: tutor.id }, '/NhuCau/NhuCau/Tao');
            assert.equal(response.status(), 404);
            assert.ok((await (await learner.request.get(base + '/')).text()).toLowerCase().includes(learner.id.toLowerCase()));
        });
        await run('AUTH-17', 'Quên mật khẩu có thông báo chung', async () => {
            for (const address of [learner.email, `missing-${suffix}@example.test`]) {
                const response = await post(guestClient, '/TaiKhoan/QuenMatKhau', { Email: address }, '/TaiKhoan/QuenMatKhau');
                assert.equal(response.status(), 200);
                assert.ok((await response.text()).includes('DaGuiLienKet') === false);
                assert.ok((await response.text()).includes('30 ph'));
            }
        });
        await run('AUTH-18', 'Hộp thư Development đọc được thư đặt lại mật khẩu', async () => {
            const page = await guest.newPage();
            await page.goto(base + '/TaiKhoan/HopThuThu');
            const card = page.locator('.account-mail').filter({ hasText: learner.email });
            assert.equal(await card.count(), 1);
            learner.resetLink = await card.locator('a').getAttribute('href');
            assert.ok(learner.resetLink.startsWith(base));
            await page.screenshot({ path: path.join(evidence, 'development-mailbox.png'), fullPage: true });
            await page.close();
        });
        await run('AUTH-19', 'Đặt lại mật khẩu thu hồi cookie cũ', async () => {
            const link = new URL(learner.resetLink);
            learner.oldCookies = await learner.context.cookies();
            const response = await post(guestClient, '/TaiKhoan/DatLaiMatKhau', { Email: learner.email, Ma: link.searchParams.get('ma'), MatKhau: replacementPassword, XacNhanMatKhau: replacementPassword }, link.pathname + link.search);
            assert.equal(response.status(), 302);
            const old = await learner.request.get(base + '/', { maxRedirects: 0 });
            assert.equal(old.status(), 302);
            const sessionCount = Number(sql(`SELECT COUNT(*) FROM PhienDangNhap WHERE TaiKhoanId='${learner.id}' AND ThuHoiLuc IS NULL`));
            assert.equal(sessionCount, 0);
        });
        await run('AUTH-20', 'Liên kết đặt lại chỉ dùng một lần', async () => {
            const link = new URL(learner.resetLink);
            const response = await post(guestClient, '/TaiKhoan/DatLaiMatKhau', { Email: learner.email, Ma: link.searchParams.get('ma'), MatKhau: password, XacNhanMatKhau: password }, link.pathname + link.search);
            assert.equal(response.status(), 200);
            const html = await response.text();
            assert.ok(html.includes('validation-summary-errors'));
            assert.ok(!html.includes('value="' + password + '"'));
        });
        await run('AUTH-21', 'Mật khẩu cũ bị từ chối, mật khẩu mới đăng nhập được', async () => {
            const old = await post(guestClient, '/TaiKhoan/DangNhap', { Email: learner.email, MatKhau: password });
            assert.equal(old.status(), 200);
            const fresh = await post(learner.request, '/TaiKhoan/DangNhap', { Email: learner.email, MatKhau: replacementPassword });
            assert.equal(fresh.status(), 302);
        });
        await run('AUTH-22', 'Đăng xuất thu hồi phiên và cookie sao chép', async () => {
            const clone = await browser.newContext();
            await clone.addCookies(await parent.context.cookies());
            const page = await parent.context.newPage();
            await page.goto(base + '/TaiKhoan/QuenMatKhau');
            await page.addScriptTag({ url: base + '/lib/signalr/signalr.min.js' });
            const connectedUser = await page.evaluate(async () => {
                window.authHub = new signalR.HubConnectionBuilder().withUrl('/cap-nhat').build();
                window.authHub.onclose(() => { window.authHubClosed = true; });
                await window.authHub.start();
                return window.authHub.invoke('KiemTraPhien');
            });
            assert.equal(connectedUser.toLowerCase(), parent.id.toLowerCase());
            const response = await post(parent.request, '/TaiKhoan/DangXuat', {}, '/');
            assert.equal(response.status(), 302);
            await page.waitForFunction(() => window.authHubClosed === true, { timeout: 10000 });
            await page.close();
            assert.equal((await clone.request.get(base + '/', { maxRedirects: 0 })).status(), 302);
            assert.equal(Number(sql(`SELECT COUNT(*) FROM PhienDangNhap WHERE TaiKhoanId='${parent.id}' AND ThuHoiLuc IS NULL`)), 0);
            await clone.close();
        });
        await run('AUTH-23', 'Phiên hết hạn sau 30 phút không hoạt động', async () => {
            sql(`UPDATE PhienDangNhap SET TaoLuc=DATEADD(minute,-40,SYSDATETIMEOFFSET()),HoatDongCuoiLuc=DATEADD(minute,-31,SYSDATETIMEOFFSET()) WHERE TaiKhoanId='${tutor.id}' AND ThuHoiLuc IS NULL`);
            assert.equal((await tutor.request.get(base + '/', { maxRedirects: 0 })).status(), 302);
        });
        await run('AUTH-24', 'Tài khoản bị khóa không dùng lại phiên đăng nhập', async () => {
            sql(`UPDATE AspNetUsers SET TrangThai='BiKhoa' WHERE Id='${learner.id}'`);
            assert.equal((await learner.request.get(base + '/', { maxRedirects: 0 })).status(), 302);
            sql(`UPDATE AspNetUsers SET TrangThai='HoatDong' WHERE Id='${learner.id}'`);
        });
        await run('AUTH-25', 'Chặn returnUrl bên ngoài ứng dụng', async () => {
            const response = await post(guestClient, '/TaiKhoan/DangNhap', { Email: learner.email, MatKhau: replacementPassword, ReturnUrl: 'https://example.org/redirect' });
            assert.equal(response.status(), 302);
            assert.equal(response.headers().location, '/');
        });
        await run('AUTH-26', 'Năm lần sai tạo thời gian chờ, không khóa tài khoản', async () => {
            sql('DELETE FROM TrangThaiThuDangNhap');
            const context = await browser.newContext();
            for (let i = 0; i < 5; i++) {
                const response = await post(context.request, '/TaiKhoan/DangNhap', { Email: learner.email, MatKhau: 'Mật khẩu sai để kiểm thử' });
                assert.equal(response.status(), 200);
            }
            const before = rows(`SELECT LoaiKhoa,SoLanSai,ChoDenLuc FROM TrangThaiThuDangNhap WHERE GiaTriKhoa='${learner.email.toUpperCase()}' OR LoaiKhoa='DiaChiIp'`);
            assert.equal(before.length, 2);
            assert.ok(before.every(x => x.SoLanSai === 5 && x.ChoDenLuc));
            await post(context.request, '/TaiKhoan/DangNhap', { Email: learner.email, MatKhau: replacementPassword });
            const after = rows(`SELECT LoaiKhoa,SoLanSai,ChoDenLuc FROM TrangThaiThuDangNhap WHERE GiaTriKhoa='${learner.email.toUpperCase()}' OR LoaiKhoa='DiaChiIp'`);
            assert.deepEqual(before, after);
            assert.equal(rows(`SELECT TrangThai FROM AspNetUsers WHERE Id='${learner.id}'`)[0].TrangThai, 'HoatDong');
            sql('DELETE FROM TrangThaiThuDangNhap');
            await context.close();
        });
        await run('AUTH-27', 'Trang đăng nhập hiển thị được trên điện thoại', async () => {
            const context = await browser.newContext({ viewport: { width: 390, height: 844 } });
            const page = await context.newPage();
            await page.goto(base + '/TaiKhoan/DangNhap');
            assert.equal(await page.locator('h1').innerText(), 'Đăng nhập');
            assert.ok(await page.locator('button[type="submit"]').isVisible());
            assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth));
            await page.screenshot({ path: path.join(evidence, 'login-mobile.png'), fullPage: true });
            await context.close();
        });
    } finally {
        await browser.close();
    }
    const failed = results.filter(x => x.status === 'Fail');
    console.log(JSON.stringify({ total: results.length, passed: results.length - failed.length, failed: failed.length }));
    process.exitCode = failed.length ? 1 : 0;
})().catch(error => { console.error(error); process.exitCode = 1; });
