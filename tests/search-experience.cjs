const { chromium } = require('playwright');
const fs = require('node:fs');
const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const signIn = require('./sign-in.cjs');
const base = process.env.EDU_TEST_URL || 'http://127.0.0.1:5299';
const database = process.env.EDU_FULL_TEST_DATABASE;
assert.ok(database?.startsWith('EduConnect_Verification_'));
assert.ok(['127.0.0.1', 'localhost'].includes(new URL(base).hostname));
const output = '.codex-local/search-experience';
fs.mkdirSync(output, { recursive: true });
const sql = query => execFileSync('sqlcmd', ['-S','lpc:CONGVJP','-d',database,'-E','-C','-I','-b','-h','-1','-w','65535','-f','65001','-Q','SET NOCOUNT ON; '+query], { encoding:'utf8' }).trim();

(async () => {
    const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
    const checks = [], errors = [];
    const context = await browser.newContext({ viewport:{ width:1600, height:1100 } });
    const page = await context.newPage();
    page.on('pageerror', error => errors.push(error.message));
    const pass = name => { checks.push(name); console.log('PASS', name); fs.writeFileSync(output+'/results.json', JSON.stringify({ database, checks, errors },null,2)); };
    async function select(name, value) {
        if(!await page.locator('[name="'+name+'"]').isVisible())await page.locator('.filter-more>summary').click();
        const response = page.waitForResponse(r => r.request().headers()['x-edu-partial'] === 'true' && r.url().includes('/TimKiem/'));
        await page.locator('[name="'+name+'"]').selectOption(value);
        assert.equal((await response).status(), 200);
        await page.waitForFunction(() => !document.getElementById('search-content').hasAttribute('aria-busy'));
    }
    try {
        await page.goto(base+'/TimKiem/TimKiem/GiaSu');
        await page.evaluate(() => window.searchDocument = 'same-document');
        await select('MonHocId','1');
        assert.equal(await page.locator('[name="MonHocId"]').inputValue(), '1');
        await select('KhoiLop','Lớp 12');
        assert.equal(await page.evaluate(() => window.searchDocument), 'same-document');
        assert.ok(new URL(page.url()).searchParams.get('KhoiLop') === 'Lớp 12');
        pass('Môn và khối tự lọc qua AJAX, không tải lại trang');
        await page.locator('input[form="compare-form"][name="ids"]').nth(0).check();
        await page.locator('input[form="compare-form"][name="ids"]').nth(1).check();
        await select('SoSaoToiThieu','5');
        assert.equal(await page.locator('input[form="compare-form"]:checked').count(),2);
        assert.match(await page.locator('#compare-status').innerText(),/2\/3/);
        await page.locator('[data-compare-clear]').click();
        assert.equal(await page.locator('input[form="compare-form"]:checked').count(),0);
        pass('Lọc số sao giữ lựa chọn so sánh kể cả hồ sơ ra khỏi trang kết quả');

        await select('KhuVuc','Hà Nội');
        await page.locator('[data-ward]:enabled').waitFor();
        await page.waitForFunction(() => document.querySelector('[data-ward]').options.length > 1);
        const ward = await page.locator('[data-ward] option').nth(1).getAttribute('value');
        await select('PhuongXa',ward);
        assert.equal(await page.locator('[data-province]').inputValue(), 'Hà Nội');
        assert.equal(await page.locator('[data-ward]').inputValue(), ward);
        pass('Tỉnh và phường tự lọc, giữ danh sách địa giới phụ thuộc');
        const missing='khong-co-ket-qua-'+Date.now();
        const empty=page.waitForResponse(r=>r.request().headers()['x-edu-partial']==='true'&&new URL(r.url()).searchParams.get('TuKhoa')===missing);
        await page.locator('[name="TuKhoa"]').fill(missing);
        assert.equal((await empty).status(),200);
        await page.getByRole('heading',{name:'Chưa có kết quả phù hợp'}).waitFor();
        assert.equal(await page.evaluate(()=>window.searchDocument),'same-document');
        pass('Từ khóa tự lọc sau khi ngừng gõ, trạng thái rỗng rõ ràng');

        await page.goto(base+'/TimKiem/TimKiem/GiaSu?SapXep=DanhGia');
        await page.screenshot({ path:output+'/search-desktop.png', fullPage:true });
        const ids = await page.locator('.candidate[data-candidate-id]').evaluateAll(nodes => nodes.slice(0,2).map(node => node.dataset.candidateId));
        await page.goto(base+'/TimKiem/TimKiem/GiaSu');
        const unrated = await page.locator('.candidate').filter({hasText:'Chưa có đánh giá'}).first().getAttribute('data-candidate-id');
        assert.ok(unrated&&!ids.includes(unrated));
        ids.push(unrated);
        assert.equal(ids.length,3);
        await page.goto(base+'/TimKiem/TimKiem/SoSanh?'+ids.map(id=>'ids='+id).join('&'));
        assert.equal((await page.locator('thead th[data-comparison-id]').count()),3);
        const widths = await page.locator('thead th[data-comparison-id]').evaluateAll(nodes=>nodes.map(node=>node.getBoundingClientRect().width));
        assert.ok(Math.max(...widths)-Math.min(...widths)<2);
        assert.equal(await page.getByRole('rowheader',{name:'Mục tiêu',exact:true}).count(),0);
        assert.equal(await page.getByRole('rowheader',{name:'Khối lớp',exact:true}).count(),0);
        await page.getByRole('rowheader',{name:'Môn và phạm vi dạy',exact:true}).waitFor();
        await page.getByRole('rowheader',{name:'Kinh nghiệm',exact:true}).waitFor();
        assert.ok(await page.locator('.comparison-badges').getByText('Đánh giá cao nhất',{exact:true}).count()>0);
        await page.screenshot({ path:output+'/comparison-desktop.png', fullPage:true });
        pass('Ba cột so sánh bằng nhau, tiêu chí đúng Gia sư và nhãn đánh giá có dữ liệu');

        await page.setViewportSize({ width:390,height:844 });
        assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
        assert.ok(await page.locator('.table-responsive').evaluate(node=>node.scrollWidth>node.clientWidth));
        pass('So sánh trên điện thoại cuộn trong bảng, không vỡ khung trang');
        await page.goto(base+'/TimKiem/TimKiem/GiaSu');
        assert.ok((await page.locator('.candidate').first().boundingBox()).y<700);
        await page.getByRole('button',{name:'Bộ lọc',exact:true}).click();
        await page.locator('#search-filter-drawer.show').waitFor();
        await select('MonHocId','1');
        assert.equal(await page.locator('#search-filter-drawer.show').count(),1);
        assert.equal(await page.locator('.offcanvas-backdrop').count(),1);
        await page.screenshot({ path:output+'/filters-mobile.png', fullPage:true });
        await page.locator('.filter-show-results').click();
        await page.locator('.offcanvas-backdrop').waitFor({ state:'detached' });
        assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth+1),false);
        pass('Bộ lọc mobile mở bằng phễu, chọn là lọc và đóng về kết quả');
        await page.getByRole('button',{name:'Bộ lọc',exact:true}).click();
        const clear = page.waitForResponse(r=>r.request().headers()['x-edu-partial']==='true'&&r.url().includes('/TimKiem/'));
        await page.locator('[data-search-clear]').click();
        assert.equal((await clear).status(),200);
        await page.waitForFunction(()=>!document.getElementById('search-content').hasAttribute('aria-busy'));
        assert.equal(await page.locator('.offcanvas-backdrop').count(),0);
        assert.equal(await page.locator('[name="MonHocId"]').inputValue(),'');
        pass('Xóa bộ lọc trong ngăn mobile không để lại nền khóa trang');
        await page.getByRole('button',{name:'Bộ lọc',exact:true}).click();
        const previous=page.waitForResponse(r=>r.request().headers()['x-edu-partial']==='true'&&r.url().includes('/TimKiem/'));
        await page.goBack();
        assert.equal((await previous).status(),200);
        await page.locator('.offcanvas-backdrop').waitFor({state:'detached'});
        assert.equal(await page.locator('[name="MonHocId"]').inputValue(),'1');
        pass('Quay lại khi ngăn lọc đang mở khôi phục tiêu chí và không kẹt nền');

        const fixture = JSON.parse(fs.readFileSync('tests/ProfileWorkflow/bin/Debug/net10.0/profile-results.json'));
        assert.equal(fixture.database,database);
        await page.setViewportSize({ width:1900,height:1100 });
        await signIn(page,base,fixture.tutor);
        await page.getByRole('heading',{name:'Người học phù hợp',exact:true}).waitFor();
        assert.equal(await page.locator('.candidate').count(),3);
        assert.ok((await page.getByRole('link',{name:'Xem tất cả',exact:true}).getAttribute('href')).includes('MucPhuHop=1'));
        pass('Trang chủ có dữ liệu phù hợp vẫn giữ gợi ý cá nhân và bộ lọc Xem tất cả');
        await page.goto(base+'/TimKiem/TimKiem/NhuCau');
        const geometry = await page.locator('.candidate').evaluateAll(nodes=>nodes.slice(0,3).map(node=>({price:node.querySelector('.need-summary .price')?.getBoundingClientRect().top,goal:node.querySelector('.candidate-goal')?.getBoundingClientRect().top,actions:node.querySelector('.actions').getBoundingClientRect().top})));
        assert.equal(geometry.length,3);
        for (const key of ['price','goal','actions']) assert.ok(Math.max(...geometry.map(r=>r[key]))-Math.min(...geometry.map(r=>r[key]))<2,key+' alignment');
        await page.screenshot({ path:output+'/needs-desktop.png',fullPage:true });
        pass('Mục tiêu, học phí và thao tác của ba thẻ nhu cầu thẳng hàng');

        for(const role of ['HocSinh','GiaSu','PhuHuynh']) {
            const fresh = await browser.newContext();
            try {
                const p = await fresh.newPage();
                const email='search-home-'+role.toLowerCase()+'-'+Date.now()+'@example.test',password='Học cùng EduConnect 2026!';
                await p.goto(base+'/TaiKhoan/DangKy');
                let token=await p.locator('[name="__RequestVerificationToken"]').first().inputValue();
                const registered=await p.request.post(base+'/TaiKhoan/DangKy',{form:{__RequestVerificationToken:token,DongYChinhSach:'true',HoTen:'Kiểm thử trang chủ',Email:email,VaiTro:role,MatKhau:password,XacNhanMatKhau:password},maxRedirects:0});
                assert.equal(registered.status(),302);
                sql(`UPDATE AspNetUsers SET EmailConfirmed=1 WHERE Email='${email}'`);
                await p.goto(base+'/TaiKhoan/DangNhap');
                token=await p.locator('[name="__RequestVerificationToken"]').first().inputValue();
                assert.equal((await p.request.post(base+'/TaiKhoan/DangNhap',{form:{__RequestVerificationToken:token,Email:email,MatKhau:password},maxRedirects:0})).status(),302);
                await p.goto(base+'/');
                assert.equal(await p.locator('.candidate').count(),6);
                const kind=await p.locator('.candidate').first().getAttribute('class');
                assert.equal(kind.includes('tutor-card'),role!=='GiaSu');
                const viewAll=await p.getByRole('link',{name:'Xem tất cả',exact:true}).getAttribute('href');
                assert.ok(viewAll.includes('SapXep=DanhGia'));
                assert.ok(!viewAll.includes('MucPhuHop=1'));
                if(role==='HocSinh')await p.screenshot({path:output+'/home-new-learner.png',fullPage:true});
                pass('Tài khoản mới '+role+' có sáu gợi ý đúng vai trò và đường Xem tất cả đúng');
            } finally { await fresh.close(); }
        }
        assert.deepEqual(errors,[]);
        pass('Không có lỗi JavaScript trong các luồng tìm kiếm mới');
    } finally { await browser.close(); }
})().catch(error=>{console.error(error);process.exitCode=1;});
