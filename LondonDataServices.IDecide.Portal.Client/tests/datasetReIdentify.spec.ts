import { test, expect } from '@playwright/test';;
import * as path from 'path';
import { fileURLToPath } from 'url';
import testDataForDataset from './testData/testDataForDataset.json' assert { type: 'json' };

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

async function navigateToReIdentifySinglePatient(page: Page) {
    const link = page.getByRole('link', { name: 'Re-Identify Dataset' });
    await expect(link).toBeVisible();
    await link.click();
    const header = page.getByText('Dataset Upload');
    await expect(header).toHaveText('Dataset Upload');
}

async function fillReIdentifyForm(page: Page, recipientEmail: string, reason: string) {
    const emailAddressInput = page.getByPlaceholder('Enter user email address');
    await emailAddressInput.fill(recipientEmail);

    const selectEmailButton = page.getByRole('button', { name: 'Select' });
    await expect(selectEmailButton).toBeVisible();
    await expect(selectEmailButton).toBeEnabled();
    await selectEmailButton.click();

    const hasHeaderCheckbox = page.getByLabel('Has Header Record');
    await expect(hasHeaderCheckbox).toBeVisible();
    if (!(await hasHeaderCheckbox.isChecked())) {
        await hasHeaderCheckbox.check();
    }

    const fileInput = page.getByPlaceholder('Upload CSV');
    await expect(fileInput).toBeVisible();
    await expect(fileInput).toBeEnabled();
    const filePath = path.resolve(__dirname, 'resources/valid.csv');
    await fileInput.setInputFiles(filePath);

    const reasonDropdown = page.getByPlaceholder('Enter a reason');
    await reasonDropdown.fill(reason);

    const colSelectDropdown = page.getByRole('combobox');
    await expect(colSelectDropdown).toBeVisible({ timeout: 5000 });
    await colSelectDropdown.selectOption({ label: "Col-4 - PseudoID" });
}

test.describe('Re-Identify dataset Patient Tests', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    testDataForDataset.forEach(({ recipientEmail, reason }) => {
        test(`Can upload CSV and recieve Email: ${recipientEmail}`, async ({ page }) => {
            await navigateToReIdentifySinglePatient(page);
            await fillReIdentifyForm(page, recipientEmail, reason);

            const sendFileButton = page.getByRole('button', { name: 'Send File' });
            await sendFileButton.click();

            const successMessage = page.getByText('CSV Sent');
            await expect(successMessage).toBeVisible();
        });
    });

    testDataForDataset.forEach(({ invalidRecipientEmail }) => {
        test(`User doesn't exist in Users Table: ${invalidRecipientEmail}`, async ({ page }) => {
            await navigateToReIdentifySinglePatient(page);
            const emailAddressInput = page.getByPlaceholder('Enter user email address');
            await emailAddressInput.fill(invalidRecipientEmail);
            const selectEmailButton = page.getByRole('button', { name: 'Select' });
    
            // Assert that the Select button is not visible or not in the DOM
            await expect(selectEmailButton).toBeHidden();
        });
    });
});