import { test, expect } from '@playwright/test';

test.describe('Footer', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    test('should display all footer links with correct text and href', async ({ page }) => {
        const links = [
            { text: /Copyright/i, href: '/copyright/' },
            { text: /About Us/i, href: '/about/' },
            { text: /Contact us/i, href: '/contact/' },
            { text: /Website privacy notice/i, href: '/websitePrivacyNotice/' },
            { text: /Accessibility statement/i, href: '/accessibilityStatement/' },
            { text: /Cookie use/i, href: '/cookieUse/' },
        ];

        for (const link of links) {
            const anchor = page.getByRole('link', { name: link.text });
            await expect(anchor).toHaveAttribute('href', link.href);
        }
    });

    test('should display the OneLondon logo with correct src and alt', async ({ page }) => {
        const logo = page.getByAltText('OneLondon Logo');
        await expect(logo).toBeVisible();
        await expect(logo).toHaveAttribute('src', '/Picture1.png');
        await expect(logo).toHaveClass(/footer-logo/);
    });

    test('should have the logo inside a span with correct class', async ({ page }) => {
        const logo = await page.getByAltText('OneLondon Logo');
        const parent = await logo.evaluateHandle(node => node.parentElement);
        const className = await parent.evaluate(node => node.className);
        expect(className).toContain('footer-logos');
    });

    test('footer container should have correct class', async ({ page }) => {
        // Assumes the footer has role="contentinfo" (HTML5 <footer>)
        const footer = await page.getByRole('contentinfo');
        const container = await footer.evaluateHandle(node => node.parentElement);
        const className = await container.evaluate(node => node.className);
        expect(className).toContain('footer-center');
    });
});