import { test, expect } from '@playwright/test';

test.beforeEach(async ({ page }) => {
    await page.goto('https://localhost:5173/home');
});

test('has title', async ({ page }) => {
    await expect(page).toHaveTitle("Re-Identification Portal");
});

test('has navigation', async ({ page }) => {
    await expect(page.getByRole('link', { name: 'Re-Identify Dataset' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'My Dataset Worklist' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Re-identify Single Patient' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Report Re-identification' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Projects' })).toBeVisible();
});