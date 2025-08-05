import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('PositiveConfirmationPage', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible();
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible();
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled();
        await searchButton.click();

        // Wait for Confirm Details page to load
        await expect(page.getByRole('heading', { name: /is this you\?/i })).toBeVisible({ timeout: 10000 });

        // Wait for Yes button, ensure it's enabled, then click it
        const yesButton = page.getByRole('button', { name: /^yes$/i });
        await expect(yesButton).toBeVisible();
        await expect(yesButton).toBeEnabled();
        await yesButton.click();
    });

   
});