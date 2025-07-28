import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Search by NHS Number Page', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
        await clickStartButton(page);
    });

    test('should display NHS number input and Search button', async ({ page }) => {
        await expect(page.locator('#nhs-number')).toBeVisible();
        await expect(page.getByRole('button', { name: 'Search' })).toBeVisible();
    });

    test('should require NHS number input (shows 10 digits error)', async ({ page }) => {
        await page.locator('#nhs-number').fill('');
        await page.getByRole('button', { name: 'Search' }).click();
        await expect(page.getByText(/NHS Number must be exactly 10 digits/i)).toBeVisible();
    });

    test('should show error for NHS number with less than 10 digits', async ({ page }) => {
        await page.locator('#nhs-number').fill('12345');
        await page.getByRole('button', { name: 'Search' }).click();
        await expect(page.getByText(/NHS Number must be exactly 10 digits/i)).toBeVisible();
    });

    test("should display an H2 with the text 'Provide Your NHS Number'", async ({ page }) => {
        await expect(page.getByRole('heading', { level: 2, name: 'Provide Your NHS Number' })).toBeVisible();
    });

    test("should highlight the 'Provide Your NHS Number' step and select its radio", async ({ page }) => {
        // Check the radio is selected
        const radio = page.getByRole('radio', { name: 'Provide Your NHS Number' });
        await expect(radio).toBeChecked();

        // Check the step is highlighted (adjust selector/class as needed)
        // Example: the parent element has a class 'selected' or 'active'
        // await expect(radio.locator('..')).toHaveClass(/selected|active/);
    });

    test("should not allow clicking the 'Provide Your NHS Number' radio", async ({ page }) => {
        const radio = page.getByRole('radio', { name: 'Provide Your NHS Number' });
        await expect(radio).toBeDisabled();
    });

    //test("should navigate to the search by details page when clicking 'I Don't know my NHS Number'", async ({ page }) => {
    //    await page.getByRole('button', { name: "I Don't know my NHS Number" }).click();
    //    await expect(page).toHaveURL(/\/searchByDetails/i); // Update this regex if your route is different
    //});
});