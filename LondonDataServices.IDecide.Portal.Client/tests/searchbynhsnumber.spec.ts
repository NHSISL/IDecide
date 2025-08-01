import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Search by NHS Number Page', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
        await clickStartButton(page);
        await expect(page.locator('#nhs-number')).toBeVisible();
    });

    test('should display NHS number input and Search button', async ({ page }) => {
        await expect(page.locator('#nhs-number')).toBeVisible();
        await expect(page.getByRole('button', { name: /^search$/i })).toBeVisible();
    });

    test('should disable Search button if NHS number is not 10 digits', async ({ page }) => {
        await page.locator('#nhs-number').fill('12345');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeDisabled();
    });

    test('should enable Search button when NHS number is 10 digits', async ({ page }) => {
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await expect(searchButton).toBeEnabled();
    });

    test("should show the 'I Don't know my NHS Number' button", async ({ page }) => {
        await expect(page.getByRole('button', { name: /I Don't know my NHS Number/i })).toBeVisible();
    });

    test("should call onIDontKnow when 'I Don't know my NHS Number' is clicked", async ({ page }) => {
        await page.getByRole('button', { name: /I Don't know my NHS Number/i }).click();
        // Optionally, check for navigation or a unique element on the next step
        // await expect(page).toHaveURL(/\/searchByDetails/i);
    });

    test('should show loading state when submitting', async ({ page }) => {
        await page.locator('#nhs-number').fill('1234567890');
        const searchButton = page.getByRole('button', { name: /^search$/i });
        await page.route('**/api/patients/GetPatientByNhsNumber', async route => {
            await new Promise(res => setTimeout(res, 500));
            route.fulfill({ status: 200, body: '{}' });
        });
        await searchButton.click();
        await expect(page.getByRole('button', { name: /submitting/i })).toBeVisible();
    });

    // --- Power of Attorney (PoA) mode tests ---
    test.describe('Power of Attorney mode', () => {
        test.beforeEach(async ({ page }) => {
            await page.goto('https://localhost:5173/test-poa');
            // No clickStartButton here, as test-poa should render the component directly
            await expect(page.locator('#poa-nhs-number')).toBeVisible();
        });

        test('should display PoA fields', async ({ page }) => {
            await expect(page.locator('#poa-nhs-number')).toBeVisible();
            await expect(page.locator('#poa-firstname')).toBeVisible();
            await expect(page.locator('#poa-surname')).toBeVisible();
            await expect(page.locator('#poa-relationship')).toBeVisible();
        });

        test('should disable Search button if any PoA field is invalid', async ({ page }) => {
            await page.locator('#poa-nhs-number').fill('');
            await page.locator('#poa-firstname').fill('');
            await page.locator('#poa-surname').fill('');
            // Do NOT selectOption('') because the empty option is disabled by default
            const searchButton = page.getByRole('button', { name: /^search$/i });
            await expect(searchButton).toBeDisabled();
        });

        test('should enable Search button when all PoA fields are valid', async ({ page }) => {
            await page.locator('#poa-nhs-number').fill('1234567890');
            await page.locator('#poa-firstname').fill('John');
            await page.locator('#poa-surname').fill('Doe');
            await page.locator('#poa-relationship').selectOption('Parent');
            const searchButton = page.getByRole('button', { name: /^search$/i });
            await expect(searchButton).toBeEnabled();
        });
    });
});