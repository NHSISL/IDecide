import { test, expect, Page } from '@playwright/test';

const baseUrl = 'https://localhost:5173/report/fake/fake/fake';

test.beforeEach(async ({ page }) => {
    await page.goto(baseUrl);
});

async function selectReasonAndLaunchReport(page: Page, reason: string) {
    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: reason });

    const acceptTandC = page.getByRole('button', { name: 'Accept' });
    if (await acceptTandC.isVisible()) {
        await acceptTandC.click();
    }

    const launchReportButton = page.getByRole('button', { name: 'Launch Report' });
    await launchReportButton.click();
}

async function checkToastHeader(page: Page) {
    const toastHeader = page.locator('.toast-header');
    await expect(toastHeader).toContainText('Re-identifications');

    const closeButton = page.locator('button[aria-label="Close"]');
    await expect(closeButton).toBeVisible();
}

test('has title', async ({ page }) => {
    await expect(page).toHaveTitle("Re-Identification Portal");
});

test('has reasonForReIdentify', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");
});

test('has singleIdentityPopup', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const singleIdentityButton = page.getByRole('cell', { name: 'Test single record click: 1111151000' });
    await singleIdentityButton.click();
    await page.waitForTimeout(2000);
    await singleIdentityButton.click();

    await checkToastHeader(page);

    const cards = page.locator('.toast-body .card');
    await expect(cards).toHaveCount(1);

    const firstCard = cards.nth(0);
    await expect(firstCard).toContainText('Pseudo: 1111151000 NHS: DEC1111151');

    const buttonElement = page.getByRole('button', { name: 'show reidentification history' });
    await expect(buttonElement).toBeVisible();
});

test('has identityColumnWithMultipleRecords', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const multipleRecordsButton = page.getByRole('cell', { name: 'Test multiple record click: 1111192000,1111209000' });
    await multipleRecordsButton.click();
    await page.waitForTimeout(2000);
    await multipleRecordsButton.click();

    await checkToastHeader(page);

    const copyAllText = page.locator('.toast-header span', { hasText: 'CopyAll' });
    await expect(copyAllText).toBeVisible();

    const cards = page.locator('.toast-body .card');
    await expect(cards).toHaveCount(2);

    const firstCard = cards.nth(0);
    await expect(firstCard).toContainText('Pseudo: 1111192000 NHS: DEC1111192');

    const secondCard = cards.nth(1);
    await expect(secondCard).toContainText('Pseudo: 1111209000 NHS: DEC1111209');

    const buttonElement = page.getByRole('button', { name: 'show reidentification history' });
    await expect(buttonElement).toBeVisible();
});

test('has singleIdentityValueColumnPopup', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const singleIdentityButton = page.getByRole('cell', { name: 'Test single record click: 1111129000' });
    await singleIdentityButton.click();
    await page.waitForTimeout(2000);
    await singleIdentityButton.click();

    await checkToastHeader(page);

    const cards = page.locator('.toast-body .card');
    await expect(cards).toHaveCount(1);

    const firstCard = cards.nth(0);
    await expect(firstCard).toContainText('Pseudo: 1111129000 NHS: DEC1111129');

    const buttonElement = page.getByRole('button', { name: 'show reidentification history' });
    await expect(buttonElement).toBeVisible();
});

test('has valueColumnWithMultipleRecords', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const multipleRecordsButton = page.getByRole('cell', { name: 'Test multiple record click: 1111131000,1111112000' });
    await multipleRecordsButton.click();
    await page.waitForTimeout(2000);
    await multipleRecordsButton.click();

    await checkToastHeader(page);

    const copyAllText = page.locator('.toast-header span', { hasText: 'CopyAll' });
    await expect(copyAllText).toBeVisible();

    const cards = page.locator('.toast-body .card');
    await expect(cards).toHaveCount(2);

    const firstCard = cards.nth(0);
    await expect(firstCard).toContainText('Pseudo: 1111131000 NHS: DEC1111131');

    const secondCard = cards.nth(1);
    await expect(secondCard).toContainText('Pseudo: 1111112000 NHS: DEC1111112');

    const buttonElement = page.getByRole('button', { name: 'show reidentification history' });
    await expect(buttonElement).toBeVisible();
});

test('has identityColomnWithoutValidKey', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const multipleRecordsButton = page.getByRole('cell', { name: 'Test multiple record click: 1111131000,1111112000' });
    await multipleRecordsButton.click();
    await page.waitForTimeout(2000);
    await multipleRecordsButton.click();

    const toastContainer = page.locator('.toast-container');
    await expect(toastContainer).toBeHidden();
});

test('has valueColomnWithoutValidColomn', async ({ page }) => {
    await selectReasonAndLaunchReport(page, "Direct patient care");

    const multipleRecordsButton = page.getByRole('cell', { name: 'Test multiple record click: 1111137000,1111200000' });
    await multipleRecordsButton.click();
    await page.waitForTimeout(2000);
    await multipleRecordsButton.click();

    const toastContainer = page.locator('.toast-container');
    await expect(toastContainer).toBeHidden();
});
