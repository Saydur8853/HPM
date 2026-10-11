import asyncio
from playwright.async_api import async_playwright

async def main():
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        page = await browser.new_page(viewport={"width": 1440, "height": 900})

        print("Navigating to /setup...")
        await page.goto("http://localhost:5180/setup", wait_until="networkidle")
        await asyncio.sleep(2)
        print("URL after /setup:", page.url)

        # Print page title and text
        title = await page.title()
        print("Page title:", title)

        content = await page.content()
        print("Has 'Configure your hospital':", "Configure your hospital" in content)
        print("Has 'Sign In':", "Sign In" in content or "login" in page.url)

        await browser.close()

if __name__ == "__main__":
    asyncio.run(main())
