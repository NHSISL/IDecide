import { test, expect } from '@playwright/test';

test.describe('Landing Page', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/');
    });

    test('should display the London Data Service logo', async ({ page }) => {
        await expect(
            page.getByRole('img', { name: 'London Data Service logo' })
        ).toBeVisible();
    });

    test('should display the service name', async ({ page }) => {
        await expect(page.getByText('London Data Service')).toBeVisible();
    });

    test('should display the Local Data Opt-Out heading', async ({ page }) => {
        await expect(
            page.getByText('Local Data Opt-Out', { exact: true })
        ).toBeVisible();
    });

    test('should display the welcome message', async ({ page }) => {
        await expect(
            page.getByText('Welcome to the One London Local Data Opt-Out Management Portal.')
        ).toBeVisible();
    });

    test('should display the sign in prompt', async ({ page }) => {
        await expect(page.getByText('Please sign in to continue.')).toBeVisible();
    });

    test('should display the Sign in button', async ({ page }) => {
        await expect(page.getByRole('button', { name: 'Sign in' })).toBeVisible();
    });
});
