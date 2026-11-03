const { chromium } = require('playwright');
const assert = require('node:assert/strict');
const fs = require('node:fs');

(async () => {
  const ids = JSON.parse(fs.readFileSync('tests/DesignContracts/bin/Debug/net10.0/results.json', 'utf8'));
  assert.ok(ids.database.startsWith('EduConnect_Verification_'));
  const base = process.env.EDU_TEST_URL || 'http://127.0.0.1:5283';
  const browser = await chromium.launch({ headless: true, executablePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe' });
  const context = await browser.newContext();
  const page = await context.newPage(), errors = [], checks = [];
  context.on('page', p => p.on('pageerror', e => errors.push(e.message)));
  page.on('pageerror', e => errors.push(e.message));
  function pass(name) { checks.push(name); console.log('PASS ' + name); }
  async function save(p, status = 200) {
    const response = p.waitForResponse(r => r.request().method() === 'POST' && /\/(TaoNhuCauHoc|CapNhat)/.test(r.url()));
    await p.locator('button[name="BanNhap"][value="true"]').click();
    assert.equal((await response).status(), status);
  }
  try {
    await page.goto(base);
    const token = await page.locator('input[name="__RequestVerificationToken"]').first().inputValue();
    assert.equal((await page.request.post(base + '/Home/Chon', { form: { id: ids.learner, __RequestVerificationToken: token } })).status(), 200);
    const query = '?TuKhoa=' + ids.tutor;
    await page.goto(base + '/TimKiem/TimKiem/Tim' + query);
    assert.equal(await page.locator('.candidate').count(), 1);
    await page.goto(base + '/TimKiem/TimKiem/Index' + query);
    assert.equal(await page.locator('.candidate').count(), 1);
    pass('Canonical and legacy search URLs render the same results');

    await page.locator('.candidate .actions a').click();
    await page.waitForURL('**/NhuCau/HoSoGiaSu/ChiTiet?*');
    assert.equal(await page.locator('.profile-hero h1').innerText(), 'Contract tutor ' + ids.tutor);
    assert.ok((await page.locator('nav[aria-label="Điều hướng chính"] a.selected').innerText()).includes('Tìm'));
    const publicUrl = page.url();
    const partial = await page.request.get(publicUrl, { headers: { 'X-Requested-With': 'XMLHttpRequest', 'X-Edu-Modal': 'true' } });
    assert.equal(partial.status(), 200); assert.ok(!(await partial.text()).includes('<html'));
    const legacy = await page.request.get(base + '/TimKiem/TimKiem/ChiTiet/' + ids.hid);
    assert.equal(legacy.status(), 200); assert.ok(legacy.url().includes('HoSoGiaSu/ChiTiet'));
    pass('Public profile action, partial rendering, navigation and old URL work');

    await page.goto(base + `/TimKiem/TimKiem/SoSanh?ids=${ids.need}&ids=${ids.hid}&ids=${ids.need}`);
    assert.equal(await page.locator('thead th').count(), 3);
    assert.ok((await page.locator('thead th').nth(1).innerText()).includes('Contract learner'));
    assert.ok((await page.locator('thead th').nth(2).innerText()).includes('Contract tutor'));
    pass('IReadOnlyList query binding and comparison order work');

    await page.goto(base + '/NhuCau/NhuCau/Sua');
    assert.ok((await page.locator('form.form-wide').getAttribute('action')).includes('TaoNhuCauHoc'));
    await page.locator('[name="MonHocIds"][value="1"]').check();
    const goal = 'Browser contract ' + Date.now();
    await page.locator('[name="MucTieu"]').fill(goal);
    await save(page); await page.waitForURL(/\/NhuCau\/NhuCau(?:\/Index)?$/);
    const edit = page.locator('.card').filter({ hasText: goal }).locator('a[href*="/Sua/"]');
    const editUrl = new URL(await edit.getAttribute('href'), base).href;
    pass('Create form posts to the new action and keeps existing AJAX behavior');

    await page.goto(editUrl);
    const stalePage = await context.newPage(); await stalePage.goto(editUrl);
    assert.ok((await page.locator('form.form-wide').getAttribute('action')).includes('CapNhat'));
    assert.equal(await page.locator('[name="MaNhuCau"]').inputValue(), editUrl.split('/').pop());
    await page.locator('[name="MucTieu"]').fill(goal + ' updated');
    await save(page); await page.waitForURL(/\/NhuCau\/NhuCau(?:\/Index)?$/);
    await page.goto(editUrl);
    assert.equal(await page.locator('[name="MucTieu"]').inputValue(), goal + ' updated');
    pass('Update form binds Base64 to byte rowVersion and persists the edit');

    await stalePage.locator('[name="MucTieu"]').fill(goal + ' stale');
    await save(stalePage, 422);
    await stalePage.locator('.ui-error-summary').waitFor();
    assert.equal(await stalePage.locator('[name="MucTieu"]').inputValue(), goal + ' stale');
    await page.reload(); assert.equal(await page.locator('[name="MucTieu"]').inputValue(), goal + ' updated');
    pass('Concurrent stale form shows an error, preserves draft and prevents overwrite');

    const form = page.locator('form.form-wide');
    const data = await form.evaluate(f => [...new FormData(f)].filter(([, v]) => typeof v === 'string'));
    const body = new URLSearchParams(data); body.set('RowVersion', 'invalid-version'); body.set('BanNhap', 'true');
    const invalid = await page.request.post(new URL(await form.getAttribute('action'), base).href, {
      data: body.toString(), headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'X-Requested-With': 'XMLHttpRequest' }
    });
    assert.equal(invalid.status(), 422); assert.equal((await invalid.json()).ok, false);
    pass('Malformed rowVersion returns validation feedback rather than a server error');

    assert.deepEqual(errors, []);
    fs.writeFileSync('tests/DesignContracts/bin/Debug/net10.0/browser-results.json', JSON.stringify({ passed: checks.length, checks, errors }, null, 2));
  } finally { await browser.close(); }
})().catch(e => { console.error(e); process.exitCode = 1; });
