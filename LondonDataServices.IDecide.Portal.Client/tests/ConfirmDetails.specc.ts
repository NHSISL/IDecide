import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('ConfirmDetailsPage', () => {
    test.beforeEach(async ({ page }) => {
        test.setTimeout(60000); // Increase timeout for setup
        await page.goto('https://localhost:5173/home', { waitUntil: 'networkidle' });
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible({ timeout: 10000 });
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled({ timeout: 5000 });
        await searchButton.click();
        // Wait for Confirm Details page to load using direct locator
        await expect(page.locator('h4', { hasText: 'Is this you?' })).toBeVisible({ timeout: 15000 });
    });

    test('should be on the Confirm Details step', async ({ page }) => {
        await expect(page.locator('h4', { hasText: 'Is this you?' })).toBeVisible({ timeout: 10000 });
    });

    test('should display all required labels', async ({ page }) => {
        await expect(page.getByText('Name', { exact: true })).toBeVisible({ timeout: 10000 });
        await expect(page.getByText('Email', { exact: true })).toBeVisible({ timeout: 10000 });
        await expect(page.getByText('Mobile Number', { exact: true })).toBeVisible({ timeout: 10000 });
        await expect(page.getByText('Address', { exact: true })).toBeVisible({ timeout: 10000 });
    });

    test('should display Yes, No, and code buttons', async ({ page }) => {
        await expect(page.getByRole('button', { name: /^yes$/i })).toBeVisible({ timeout: 10000 });
        await expect(page.getByRole('button', { name: /^no$/i })).toBeVisible({ timeout: 10000 });
        await expect(page.getByRole('button', { name: /we have already been sent a code/i })).toBeVisible({ timeout: 10000 });
    });

    test('Yes button should be enabled', async ({ page }) => {
        const yesButton = page.getByRole('button', { name: /^yes$/i });
        await expect(yesButton).toBeEnabled({ timeout: 5000 });
    });

    test('No button should be enabled', async ({ page }) => {
        const noButton = page.getByRole('button', { name: /^no$/i });
        await expect(noButton).toBeEnabled({ timeout: 5000 });
    });

    test('Code button should be enabled', async ({ page }) => {
        const codeButton = page.getByRole('button', { name: /we have already been sent a code/i });
        await expect(codeButton).toBeEnabled({ timeout: 5000 });
    });

    // Uncomment and adjust these tests if you want to check navigation/step changes
    // test('should move to Positive Confirmation when Yes is clicked', async ({ page }) => {
    //     const yesButton = page.getByRole('button', { name: /^yes$/i });
    //     await yesButton.click();
    //     await expect(page.getByRole('heading', { name: /confirmation required/i })).toBeVisible({ timeout: 10000 });
    // });

    // test('should go back a step when No is clicked', async ({ page }) => {
    //     const noButton = page.getByRole('button', { name: /^no$/i });
    //     await noButton.click();
    //     await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 10000 });
    //     await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible({ timeout: 10000 });
    // });

    // test('should go to confirmCode component when code button is clicked', async ({ page }) => {
    //     const codeButton = page.getByRole('button', { name: /we have already been sent a code/i });
    //     await codeButton.click();
    //     await expect(page.getByText(/confirmation required/i)).toBeVisible({ timeout: 10000 });
    //     // Optionally check for code input field:
    //     // await expect(page.locator('#code-input')).toBeVisible({ timeout: 10000 });
    // });
});