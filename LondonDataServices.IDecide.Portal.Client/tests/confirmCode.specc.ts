import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('ConfirmCodePage', () => {
    test.beforeEach(async ({ page }) => {
        test.setTimeout(60000);
        await page.goto('https://localhost:5173/home', { waitUntil: 'networkidle', timeout: 30000 });
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible({ timeout: 10000 });
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled({ timeout: 5000 });
        await searchButton.click();

        await expect(page.getByRole('heading', { name: /is this you\?/i })).toBeVisible({ timeout: 10000 });

        const yesButton = page.getByRole('button', { name: /^yes$/i });
        await expect(yesButton).toBeVisible({ timeout: 10000 });
        await expect(yesButton).toBeEnabled({ timeout: 5000 });
        await yesButton.click();

        await expect(page.getByRole('heading', { name: /confirmation required/i })).toBeVisible({ timeout: 10000 });

        const buttons = [
            page.getByRole('button', { name: /^Email$/i }),
            page.getByRole('button', { name: /^SMS$/i }),
            page.getByRole('button', { name: /^Letter$/i }),
        ];

        for (const btn of buttons) {
            if (await btn.count() > 0 && await btn.isEnabled()) {
                await btn.click();
                await expect(page.getByText(/enter code/i)).toBeVisible({ timeout: 10000 });
                break;
            }
        }
    });

    test('should show error for code less than 5 digits', async ({ page }) => {
        await page.locator('#code').fill('123');
        await page.getByRole('button', { name: /^submit$/i }).click();
        await expect(page.locator('#code-error')).toHaveText(/Enter the 5 digit code sent to you/i, { timeout: 5000 });
    });

    test('should not allow non-numeric input', async ({ page }) => {
        await page.locator('#code').fill('12ab!');
        await expect(page.locator('#code')).toHaveValue('12');
    });

    test('should disable submit button while submitting', async ({ page }) => {
        await page.locator('#code').fill('12345');
        await page.getByRole('button', { name: /^submit$/i }).click();
        await expect(page.getByRole('button', { name: /^submitting/i })).toBeDisabled();
    });

    test('should show error for invalid code', async ({ page }) => {
        await page.locator('#code').fill('99999');
        await page.getByRole('button', { name: /^submit$/i }).click();
        // Wait for error message
        await expect(page.locator('#code-error')).toHaveText(/Invalid code/i);
    });

    //test('should show Power of Attorney details if present', async ({ page }) => {
    //    // If your test setup allows, simulate powerOfAttourney in context
    //    // Example:
    //    // await page.evaluate(() => window.__TEST__POA__ = true);
    //    await expect(page.getByText(/Power of Attorney Details/i)).toBeVisible();
    //});

    test('should show Help & Guidance section', async ({ page }) => {
        await expect(page.getByRole('heading', { name: /Help & Guidance/i })).toBeVisible();
        await expect(page.getByText(/How do I get my code/i)).toBeVisible();
        await expect(page.getByText(/What if I enter the wrong code/i)).toBeVisible();
    });
});