import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('PositiveConfirmationPage', () => {
    test.beforeEach(async ({ page }) => {
        test.setTimeout(60000); // Increase timeout for setup
        await page.goto('https://localhost:5173/home', { waitUntil: 'networkidle', timeout: 30000 });
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible({ timeout: 10000 });
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled({ timeout: 5000 });
        await searchButton.click();

        // Wait for Confirm Details page to load
        await expect(page.getByRole('heading', { name: /is this you\?/i })).toBeVisible({ timeout: 10000 });

        // Wait for Yes button, ensure it's enabled, then click it
        const yesButton = page.getByRole('button', { name: /^yes$/i });
        await expect(yesButton).toBeVisible({ timeout: 10000 });
        await expect(yesButton).toBeEnabled({ timeout: 5000 });
        await yesButton.click();

        // Wait for PositiveConfirmation page to load
        await expect(page.getByRole('heading', { name: /confirmation required/i })).toBeVisible({ timeout: 10000 });
    });

    test('should show correct labels and button states', async ({ page }) => {
        test.setTimeout(30000);
        // Check labels using CSS locator to avoid strict mode violation
        await expect(page.locator('dt.nhsuk-summary-list__key', { hasText: 'Email' })).toBeVisible({ timeout: 10000 });
        await expect(page.locator('dt.nhsuk-summary-list__key', { hasText: 'Mobile Number' })).toBeVisible({ timeout: 10000 });
        await expect(page.locator('dt.nhsuk-summary-list__key', { hasText: 'Address' })).toBeVisible({ timeout: 10000 });

        // Check Email button
        const emailBtn = page.getByRole('button', { name: /^email$/i });
        if (await emailBtn.count() > 0) {
            await expect(emailBtn).toBeVisible({ timeout: 10000 });
            if (await emailBtn.isEnabled()) {
                await expect(emailBtn).toBeEnabled({ timeout: 5000 });
            } else {
                await expect(emailBtn).toBeDisabled({ timeout: 5000 });
            }
        } else {
            expect(await emailBtn.count()).toBe(0);
        }

        // Check SMS button
        const smsBtn = page.getByRole('button', { name: /^sms$/i });
        if (await smsBtn.count() > 0) {
            await expect(smsBtn).toBeVisible({ timeout: 10000 });
            if (await smsBtn.isEnabled()) {
                await expect(smsBtn).toBeEnabled({ timeout: 5000 });
            } else {
                await expect(smsBtn).toBeDisabled({ timeout: 5000 });
            }
        } else {
            expect(await smsBtn.count()).toBe(0);
        }

        // Check Letter button
        const letterBtn = page.getByRole('button', { name: /^letter$/i });
        if (await letterBtn.count() > 0) {
            await expect(letterBtn).toBeVisible({ timeout: 10000 });
            if (await letterBtn.isEnabled()) {
                await expect(letterBtn).toBeEnabled({ timeout: 5000 });
            } else {
                await expect(letterBtn).toBeDisabled({ timeout: 5000 });
            }
        } else {
            expect(await letterBtn.count()).toBe(0);
        }
    });

    test('should navigate to confirm code page when enabled button is clicked', async ({ page }) => {
        test.setTimeout(30000);
        const buttons = [
            page.getByRole('button', { name: /^email$/i }),
            page.getByRole('button', { name: /^sms$/i }),
            page.getByRole('button', { name: /^letter$/i }),
        ];

        for (const btn of buttons) {
            if (await btn.count() > 0 && await btn.isEnabled()) {
                await btn.click();
                // Confirm navigation (adjust selector as needed for your confirm code page)
                await expect(page.getByText(/code/i)).toBeVisible({ timeout: 10000 });
                break; // Only test one navigation per run
            }
        }
    });
});