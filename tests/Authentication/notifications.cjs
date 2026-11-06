const {chromium}=require('playwright');
const assert=require('node:assert/strict');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'});
 for(const width of [1280,390]){
  const page=await browser.newPage({viewport:{width,height:900},reducedMotion:'reduce'});
  await page.goto((process.env.AUTH_TEST_URL || 'http://127.0.0.1:5299') + '/TaiKhoan/DangNhap');
  await page.evaluate(()=>EduConnect.toast('Đã tạo tài khoản. Bạn có thể đăng nhập bằng email vừa đăng ký.'));
  const notice=page.locator('[data-notification]');
  assert.ok(await notice.isVisible());
  const messageBox=await notice.boundingBox();
  const card=await page.locator('.account-card').boundingBox();
  assert.ok(messageBox.y+messageBox.height<=card.y,'Không che biểu mẫu');
  await page.screenshot({path:'.backups/login/auth-results/notification-'+width+'.png',fullPage:true});
  await page.mouse.move(0,880);
  await page.waitForTimeout(5200);
  assert.ok(await notice.isHidden(),'Tự tắt sau 5 giây');
  const after=await page.locator('.account-card').boundingBox();
  assert.equal(after.y,card.y,'Không nhảy biểu mẫu khi tắt');
  await page.evaluate(()=>EduConnect.toast('Đã lưu thay đổi.'));
  await notice.hover(); await page.waitForTimeout(5200);
  assert.ok(await notice.isVisible(),'Tạm dừng khi đang đọc');
  await page.locator('[data-notification-close]').click();
  assert.ok(await notice.isHidden(),'Đóng thủ công');
  assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth));
  await page.close();
 }
 await browser.close();console.log('PASS auto-hide 5s, no overlap, stable form, hover pause, manual close, desktop/mobile.');
})().catch(e=>{console.error(e);process.exitCode=1});
