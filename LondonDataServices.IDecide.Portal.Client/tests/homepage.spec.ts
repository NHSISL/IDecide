import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Home Page', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    test('should display the main heading', async ({ page }) => {
        await expect(page.getByRole('heading', { name: 'Welcome to the OneLondon Data Portal' })).toBeVisible();
    });

    test('should display the Start button', async ({ page }) => {
        await expect(page.getByRole('button', { name: 'Start' })).toBeVisible();
    });

    test('should navigate to /optOut when Start button is clicked', async ({ page }) => {
        await clickStartButton(page);
        await expect(page).toHaveURL(/\/optOut/);
    });

    test('should display all expandable section headers', async ({ page }) => {
        const headers = [
            "What is the London Data Service?",
            "How will the London Data Service and the London Analytics Platform use my data?",
            "How do I stop LDS & SDE using my data?",
            "Where can I see the privacy Notices for LDS & SDE?"
        ];
        for (const header of headers) {
            await expect(page.getByRole('button', { name: header })).toBeVisible();
        }
    });

    test('should expand and collapse each expandable section', async ({ page }) => {
        const headers = [
            "What is the London Data Service?",
            "How will the London Data Service and the London Analytics Platform use my data?",
            "How do I stop LDS & SDE using my data?",
            "Where can I see the privacy Notices for LDS & SDE?"
        ];
        for (const header of headers) {
            const sectionButton = page.getByRole('button', { name: header });
            // Expand
            await sectionButton.click();
            // Check expanded content is visible (checks for aria-expanded)
            await expect(sectionButton).toHaveAttribute('aria-expanded', 'true');
            // Collapse
            await sectionButton.click();
            await expect(sectionButton).toHaveAttribute('aria-expanded', 'false');
        }
    });

    test('should show "Before you start" info box', async ({ page }) => {
        await expect(page.getByText('Before you start')).toBeVisible();
        await expect(page.getByText("You'll need your 10-digit NHS Number or your Full name, Postcode & Date Of Birth so that we can identify you.")).toBeVisible();
        await expect(page.getByText("We will be sending an e-mail, SMS text message or letter to the contact details you have registered with your GP. This will help us confirm we are speaking with the right person. You should be confident that your GP has your up-to-date contact details.")).toBeVisible();
    });

    test('should show correct content when expanding "What is the London Data Service?"', async ({ page }) => {
        const header = "What is the London Data Service?";
        const sectionButton = page.getByRole('button', { name: header });
        await sectionButton.click();
        await expect(page.getByText('The London Data Service securely collects patient information from a range of healthcare locations around London such as GP surgeries and hospitals.')).toBeVisible();
        await expect(page.getByText('This securely stored data about patients using London\'s healthcare services can be distributed to analytics and care platforms throughout London such as the London Care Record or the London Analytics Platform.')).toBeVisible();
    });

    test('should show correct content when expanding "How do I stop LDS & SDE using my data?"', async ({ page }) => {
        const header = "How do I stop LDS & SDE using my data?";
        const sectionButton = page.getByRole('button', { name: header });
        await sectionButton.click();
        await expect(page.getByText("Using this portal you can tell us that you don't want your data used for secondary purposes, such as population health planning and research.")).toBeVisible();
        await expect(page.getByText("Telling us that you don't want your data shared with healthcare professionals who will be treating you is a different process.")).toBeVisible();
        await expect(page.getByRole('link', { name: 'NELondonicb.oneLondon.opt-out@nhs.net' })).toBeVisible();
    });

    test('should show correct content when expanding "Where can I see the privacy Notices for LDS & SDE?"', async ({ page }) => {
        const header = "Where can I see the privacy Notices for LDS & SDE?";
        const sectionButton = page.getByRole('button', { name: header });
        await sectionButton.click();
        await expect(page.getByText('The SDE privacy notice is here and the LDS Privacy notice is available here.')).toBeVisible();
    });
});