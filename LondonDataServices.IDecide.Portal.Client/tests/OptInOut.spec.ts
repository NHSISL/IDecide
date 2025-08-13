import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('OptInOutPage', () => {
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

        await page.locator('#code').fill('12345');
        await page.getByRole('button', { name: /^submit$/i }).click();
        // Now on OptInOptOut page
    });

    test('should render both Opt-In and Opt-Out options', async ({ page }) => {
        await expect(page.getByLabel(/Opt-In/i)).toBeVisible();
        await expect(page.getByLabel(/Opt-Out/i)).toBeVisible();
    });

    test('should allow selecting Opt-In', async ({ page }) => {
        await page.getByLabel('Opt-In').click();
        const optInRadio = page.locator('input[type="radio"][value="optin"]');
        await expect(optInRadio).toBeChecked();
    });

    test('should allow selecting Opt-Out', async ({ page }) => {
        await page.getByLabel('Opt-Out').click();
        const optOutRadio = page.locator('input[type="radio"][value="optout"]');
        await expect(optOutRadio).toBeChecked();
    });

    test('should show error if no option is selected and Next is clicked', async ({ page }) => {
        await page.getByRole('button', { name: /^Next$/i }).click();
        await expect(page.locator('.nhsuk-error-message')).toHaveText(/Please select an option to continue/i);
    });

    test('should clear error when an option is selected after error', async ({ page }) => {
        await page.getByRole('button', { name: /^Next$/i }).click();
        await expect(page.locator('.nhsuk-error-message')).toHaveText(/Please select an option to continue/i);
        await page.getByLabel('Opt-In').click();
        await expect(page.locator('.nhsuk-error-message')).toBeHidden();
    });

    test('should submit and go to next step when Opt-In is selected', async ({ page }) => {
        await page.getByLabel('Opt-In').click();
        await page.getByRole('button', { name: /^Next$/i }).click();
        // You may want to check for navigation or next step indicator here
        // Example: await expect(page.getByText(/next step/i)).toBeVisible();
    });

    test('should submit and go to next step when Opt-Out is selected', async ({ page }) => {
        await page.getByLabel('Opt-Out').click();
        await page.getByRole('button', { name: /^Next$/i }).click();
        // You may want to check for navigation or next step indicator here
    });

    //test('should show Power of Attorney details if present', async ({ page }) => {
       
    //});

    test('should show Help & Guidance section', async ({ page }) => {
        await expect(page.getByRole('heading', { name: /Help & Guidance/i })).toBeVisible();
        await expect(page.getByText(/Whats does Opt-Out do/i)).toBeVisible();
        await expect(page.getByText(/Choosing to opt out of sharing your data/i)).toBeVisible();
    });
});