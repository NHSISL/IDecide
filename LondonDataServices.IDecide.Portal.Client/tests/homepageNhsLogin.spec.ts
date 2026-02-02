import { test, expect } from '@playwright/test';
import {
    clickStartButton,
    fillNhsLoginCredentials,
    clickContinueButton,
    handleOtpIfPrompted
} from './helpers/helper';

test.describe('Home Page Nhs Login', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    test('should display the Start button', async ({ page }) => {
        await expect(page.getByTestId('start-login-button')).toBeVisible();
    });

    test('should navigate to NHS login and complete authentication', async ({ page }) => {
        await clickStartButton(page);
        await expect(page).toHaveURL('https://access.sandpit.signin.nhs.uk/login');

        await fillNhsLoginCredentials(page);
        await clickContinueButton(page);

        await handleOtpIfPrompted(page);

        // Verify we're back on the app after authentication
        await expect(page).toHaveURL(/https:\/\/localhost:5173/);
    });

    test('should display Confirm Details page with user information', async ({ page }) => {
        await clickStartButton(page);
        await expect(page).toHaveURL('https://access.sandpit.signin.nhs.uk/login');

        await fillNhsLoginCredentials(page);
        await clickContinueButton(page);

        await handleOtpIfPrompted(page);

        // Wait for redirect back to app
        await page.waitForURL('https://localhost:5173/nhs-optOut');

        // Verify the Next button is visible on the Confirm Details page
        await expect(page.getByRole('button', { name: /next/i })).toBeVisible();

        // Verify the summary list contains the expected user details
        const summaryList = page.locator('dl.nhsuk-summary-list');
        await expect(summaryList).toBeVisible();

        // Verify Name field
        await expect(
            summaryList
                .locator('.nhsuk-summary-list__row', { has: page.locator('dt:has-text("Name")') })
                .locator('dd.nhsuk-summary-list__value')
        ).toHaveText('Mona, MILLAR');

        // Verify Email field
        await expect(
            summaryList
                .locator('.nhsuk-summary-list__row', { has: page.locator('dt:has-text("Email")') })
                .locator('dd.nhsuk-summary-list__value')
        ).toHaveText('testuserlive@demo.signin.nhs.uk');

        // Verify Mobile Number field
        await expect(
            summaryList
                .locator(
                    '.nhsuk-summary-list__row',
                    { has: page.locator('dt:has-text("Mobile Number")') }
                )
                .locator('dd.nhsuk-summary-list__value')
        ).toHaveText('+447887510886');
    });

    test('should navigate to Make your Choice page after clicking Next button', async ({ page }) => {
        await clickStartButton(page);
        await expect(page).toHaveURL('https://access.sandpit.signin.nhs.uk/login');

        await fillNhsLoginCredentials(page);
        await clickContinueButton(page);

        await handleOtpIfPrompted(page);

        // Wait for redirect back to app
        await page.waitForURL('https://localhost:5173/nhs-optOut');

        // Click the Next button on the Confirm Details page
        await page.getByRole('button', { name: /next/i }).click();

        // Wait for navigation to complete
        await page.waitForLoadState('networkidle');

        // Verify that we're on the Make your Choice page (still same route)
        await expect(page).toHaveURL('https://localhost:5173/nhs-optOut');

        // Verify page content has changed to Make your Choice
        await expect(page.locator('h4:has-text("Make your Choice")')).toBeVisible();
    });
});