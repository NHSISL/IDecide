import { test, expect, Page } from '@playwright/test';
import testData from './testData/testDataForSingle.json' assert { type: 'json' };

async function navigateToReIdentifySinglePatient(page: Page) {
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await link.waitFor({ state: 'visible' });
    await expect(link).toBeVisible();
    await link.click();

    const header = page.getByText('Reidentify Single Patient').nth(1);
    await header.waitFor({ state: 'visible' });
    await expect(header).toHaveText('Reidentify Single Patient');
}

async function fillReIdentifyForm(page: Page, pseudoNumber: string, reason: string) {
    const pseudoNHSNumberInput = page.getByPlaceholder('Pseudo Number');
    await pseudoNHSNumberInput.fill(pseudoNumber);

    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: reason });

    const getNhsNumberButton = page.getByRole('button', { name: 'Get Nhs Number' });
    await getNhsNumberButton.click();
}


test.describe('Re-Identify Single Patient Tests', () => {
    
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    testData.forEach(({ validHexNumber, reason, expectedText}) => {
        test(`has Access to Patient: ${validHexNumber}`, async ({ page }) => {
            await navigateToReIdentifySinglePatient(page);
            await fillReIdentifyForm(page, validHexNumber, reason);

            const startOverButton = page.getByRole('button', { name: 'Start Over' });
            await startOverButton.waitFor({ state: 'visible'})
            await expect(startOverButton).toBeVisible({ timeout: 5000 });

            const nhsNumberText = page.getByText(expectedText);
            await nhsNumberText.waitFor({ state: 'visible' });
            await expect(nhsNumberText).toBeVisible({ timeout: 5000 });

            const hidingInText = page.getByText('Hiding in:');
            await hidingInText.waitFor({ state: 'visible' });
            await expect(hidingInText).toBeVisible({ timeout: 5000 });

            await startOverButton.click();
            const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
            await expect(link).toBeVisible({ timeout: 5000 });
        });
    });

    testData.forEach(({ inValidHexNumber, reason, inValidExpectedText}) => {
        test(`has No Access to Patient: ${inValidHexNumber}`, async ({ page }) => {
        await navigateToReIdentifySinglePatient(page);
        await fillReIdentifyForm(page, inValidHexNumber, reason);

        const startOverButton = page.getByRole('button', { name: 'Start Over' });
        await startOverButton.waitFor({ state: 'visible' });
        await expect(startOverButton).toBeVisible({ timeout: 5000 });

        const noAccessText = page.getByText(inValidExpectedText);
        await noAccessText.waitFor({ state: 'visible', timeout: 20000 })
        await expect(noAccessText).toBeVisible({ timeout: 5000 });

        await startOverButton.click();
        const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
        await expect(link).toBeVisible({ timeout: 5000 });

        });
    });

    test('verifies No Access to Non-NHS Number', async ({ page }) => {
        await navigateToReIdentifySinglePatient(page);
        await fillReIdentifyForm(page, '111111XXXX', 'Direct patient care');

        const startOverButton = page.getByRole('button', { name: 'Start Over' });
        await expect(startOverButton).toBeVisible({ timeout: 5000 });

        const noAccessText = page.getByText('It appears you tried to re-identify a patient you don\'t have access to.Please');
        await expect(noAccessText).toBeVisible({ timeout: 5000 });

        await startOverButton.click();
        const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
        await expect(link).toBeVisible({ timeout: 5000 });

    });

    test('verifies No Access to Non-NHS Length Number', async ({ page }) => {
        await navigateToReIdentifySinglePatient(page);
        await fillReIdentifyForm(page, '11111', 'Direct patient care');

        //No popup to say why??
        // fill out all and make sure button isnt active
    });

    testData.forEach(({ validHexNumber, reason, expectedText}) => {
        test(`Has Access - to valid Hex Numbers: ${validHexNumber}`, async ({ page }) => {
            await navigateToReIdentifySinglePatient(page);
            await fillReIdentifyForm(page, validHexNumber, reason);

            const startOverButton = page.getByRole('button', { name: 'Start Over' });
            await startOverButton.waitFor({ state: 'visible', timeout: 10000 })
            await expect(startOverButton).toBeVisible({ timeout: 5000 });

            const nhsNumberText = page.getByText(expectedText);
            await nhsNumberText.waitFor({ state: 'visible' });
            await expect(nhsNumberText).toBeVisible({ timeout: 5000 });

            const hidingInText = page.getByText('Hiding in:');
            await hidingInText.waitFor({ state: 'visible' });
            await expect(hidingInText).toBeVisible({ timeout: 5000 });

            await startOverButton.click();
            const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
            await expect(link).toBeVisible({ timeout: 5000 });

        });
    });

    testData.forEach(({ inValidHexNumber, reason, inValidExpectedText}) => {
        test(`verifies No Access to Non-NHS Hex Number Number: ${inValidHexNumber}`, async ({ page }) => {
            await navigateToReIdentifySinglePatient(page);
            await fillReIdentifyForm(page, inValidHexNumber, reason);

            const startOverButton = page.getByRole('button', { name: 'Start Over' });
            await startOverButton.waitFor({ state: 'visible' });
            await expect(startOverButton).toBeVisible({ timeout: 5000 });

            const noAccessText = page.getByText(inValidExpectedText);
            await noAccessText.waitFor({ state: 'visible', timeout: 20000 })
            await expect(noAccessText).toBeVisible({ timeout: 5000 });

            await startOverButton.click();
            const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
            await expect(link).toBeVisible();

        });
    });

    testData.forEach(({ validPseudoNumber, reason, expectedText}) => {
        test(`Has Access -  Re-identify with pseudoNumber: ${validPseudoNumber}`, async ({ page }) => {
            const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
            await expect(link).toBeVisible();
            await link.click();

            const pseudoNHSNumberInput = page.getByPlaceholder('Pseudo Number');
            await pseudoNHSNumberInput.fill(validPseudoNumber);

            const reasonDropdown = page.getByRole('combobox');
            await reasonDropdown.selectOption({ label: reason });

            const getNhsNumberButton = page.getByRole('button', { name: 'Get Nhs Number' });
            await getNhsNumberButton.click();

            // Assert the expected output
            page.getByText(expectedText);

        });
    });
});