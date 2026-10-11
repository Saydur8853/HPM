import asyncio
import os
from playwright.async_api import async_playwright

async def main():
    os.makedirs("d:/Shamim Sir/HPM-Web/manual_assets/screenshots", exist_ok=True)
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(viewport={"width": 1440, "height": 900})
        page = await context.new_page()

        print("Navigating to login page...")
        await page.goto("http://localhost:5180/login", wait_until="networkidle")
        await page.screenshot(path="d:/Shamim Sir/HPM-Web/manual_assets/screenshots/01_login_page.png")
        print("Captured login page")

        # Try to fill login
        await page.fill('input[name="UserName"]', "admin")
        await page.fill('input[name="Password"]', "Admin@PrimeHpm2026!")
        await page.click('button[type="submit"]')
        await page.wait_for_load_state("networkidle")
        await asyncio.sleep(2)

        print("Current URL after login:", page.url)
        await page.screenshot(path="d:/Shamim Sir/HPM-Web/manual_assets/screenshots/02_after_login.png")

        await browser.close()

if __name__ == "__main__":
    asyncio.run(main())
