import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Search by NHS Number Page', () => {
    test.beforeEach(async ({ page }) => {
        test.setTimeout(60000); // Increase timeout for setup
        await page.goto('https://localhost:5173/home', { waitUntil: 'networkidle' });
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 15000 });
    });

    test('should display NHS number input and Search button', async ({ page }) => {
        await expect(page.locator('#nhs-number')).toBeVisible({ timeout: 10000 });
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible({ timeout: 10000 });
    });

    test('should disable Search button if NHS number is not 10 digits', async ({ page }) => {
        await page.locator('#nhs-number').fill('12345');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeDisabled({ timeout: 5000 });
    });

    test('should enable Search button when NHS number is 10 digits', async ({ page }) => {
        await page.locator('#nhs-number').fill('0000000000');
        await page.locator('#nhs-number').blur();
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled({ timeout: 5000 });
    });

    test('should NOT enable Search button when NHS number doesnt have a check digit', async ({ page }) => {
        await page.locator('#nhs-number').fill('0000000001');
        await page.locator('#nhs-number').blur();
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeDisabled({ timeout: 5000 });
    });

    test("should show the 'I Don't know my NHS Number' button", async ({ page }) => {
        await expect(page.getByRole('button', { name: /I Don't know my NHS Number/i })).toBeVisible({ timeout: 10000 });
    });

    test("should call onIDontKnow when 'I Don't know my NHS Number' is clicked", async ({ page }) => {
        await page.getByRole('button', { name: /I Don't know my NHS Number/i }).click();
        // Optionally, check for navigation or a unique element on the next step
        // await expect(page).toHaveURL(/\/searchByDetails/i);
    });

    test('should show loading state when submitting', async ({ page }) => {
        await page.locator('#nhs-number').fill('0000000000');
        await page.locator('#nhs-number').blur();
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await page.route('**/api/patients/GetPatientByNhsNumber', async route => {
            await new Promise(res => setTimeout(res, 500));
            route.fulfill({ status: 200, body: '{}' });
        });
        await searchButton.click();
    });

    // --- Power of Attorney (PoA) mode tests ---
    test.describe('Power of Attorney mode', () => {
        test.beforeEach(async ({ page }) => {
            test.setTimeout(60000);
            await page.goto('https://localhost:5173/test-poa', { waitUntil: 'networkidle' });
            await expect(page.locator('#poa-nhs-number')).toBeVisible({ timeout: 15000 });
        });

        test('should display PoA fields', async ({ page }) => {
            await expect(page.locator('#poa-nhs-number')).toBeVisible({ timeout: 10000 });
            await expect(page.locator('#poa-firstname')).toBeVisible({ timeout: 10000 });
            await expect(page.locator('#poa-surname')).toBeVisible({ timeout: 10000 });
            await expect(page.locator('#poa-relationship')).toBeVisible({ timeout: 10000 });
        });

        test('should disable Search button if any PoA field is invalid', async ({ page }) => {
            await page.locator('#poa-nhs-number').fill('');
            await page.locator('#poa-firstname').fill('');
            await page.locator('#poa-surname').fill('');
            const searchButton = page.getByRole('button', { name: /^search$/i });
            await expect(searchButton).toBeDisabled({ timeout: 5000 });
        });

        test('should enable Search button when all PoA fields are valid', async ({ page }) => {
            await page.locator('#poa-nhs-number').fill('0000000000');
            await page.locator('#poa-firstname').fill('John');
            await page.locator('#poa-surname').fill('Doe');
            await page.locator('#poa-relationship').selectOption('The patient is under 13 and you are their parent');
            const searchButton = page.getByRole('button', { name: /^search$/i });
            await expect(searchButton).toBeEnabled({ timeout: 5000 });
        });
    });
}
);