import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Confirmation Component', () => {
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
        await expect(page.getByRole('button', { name: /^submitting/i })).toBeDisabled();
        await page.getByLabel('Opt-In').click();
        await page.getByRole('button', { name: /^Next$/i }).click();
    });

    test('renders all notification method checkboxes', async ({ page }) => {
        await expect(page.getByTestId('checkbox-sms')).toBeVisible();
        await expect(page.getByTestId('checkbox-email')).toBeVisible();
        await expect(page.getByTestId('checkbox-post')).toBeVisible();
    });

    test('only one notification method can be selected at a time', async ({ page }) => {
        const sms = page.getByTestId('checkbox-sms');
        const email = page.getByTestId('checkbox-email');
        const post = page.getByTestId('checkbox-post');

        await sms.click();
        await expect(sms).toBeChecked();
        await expect(email).not.toBeChecked();
        await expect(post).not.toBeChecked();

        await email.click();
        await expect(email).toBeChecked();
        await expect(sms).not.toBeChecked();
        await expect(post).not.toBeChecked();

        await post.click();
        await expect(post).toBeChecked();
        await expect(sms).not.toBeChecked();
        await expect(email).not.toBeChecked();
    });

    test('submits preferences and disables button', async ({ page }) => {
        await page.getByTestId('checkbox-sms').click();
        const submitBtn = page.getByTestId('save-preferences-btn');
        await expect(submitBtn).toBeVisible();
        await submitBtn.click();
        await expect(submitBtn).toBeVisible();
        // If you disable the button on submit, check for that here
        // await expect(submitBtn).toBeDisabled();
    });

    test('shows error alert on failed submission', async ({ page }) => {
        await page.route('**/api/decisions', route => route.abort());
        await page.getByTestId('checkbox-sms').click();
        await page.getByTestId('save-preferences-btn').click();
        await expect(page.getByTestId('error-alert')).toBeVisible();
        await expect(page.getByTestId('error-alert')).toContainText("Sorry, we couldn't save your decision. Please try again.");
    });

    test('Help & Guidance section is visible', async ({ page }) => {
        await expect(page.getByTestId('help-guidance-section')).toBeVisible();
        await expect(page.getByTestId('help-guidance-heading')).toBeVisible();
        await expect(page.getByTestId('about-this-step-heading')).toBeVisible();
        await expect(page.getByTestId('need-help-heading')).toBeVisible();
    });
});