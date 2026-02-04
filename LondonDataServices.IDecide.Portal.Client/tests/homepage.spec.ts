import { test, expect } from '@playwright/test';
import { clickStartButton } from './helpers/helper';

test.describe('Home Page', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    test('should display the main heading', async ({ page }) => {
        await expect(
            page.getByRole('heading', { name: 'Welcome to the London Secure Data Environment Data Portal' })
        ).toBeVisible();
    });

    test('should display the Start button', async ({ page }) => {
        await expect(page.getByTestId('start-login-button')).toBeVisible();
    });

    test('should navigate to /optOut when Start button is clicked', async ({ page }) => {
        await clickStartButton(page);
        await expect(page).toHaveURL('https://access.sandpit.signin.nhs.uk/login');
    });

    test('should display all expandable section headers', async ({ page }) => {
        const headers = [
            "What is the London Data Service?",
            "How will the London Data Service and the London Analytics Platform use my data?",
            "How do I stop the London Data Service and London Analytics Platform using my data?",
            "Where can I see the privacy notices for the London Data Service and London Analytics Platform?"
        ];
        for (const header of headers) {
            await expect(page.getByRole('button', { name: header })).toBeVisible();
        }
    });

    test('should expand and collapse each expandable section', async ({ page }) => {
        const headers = [
            "What is the London Data Service?",
            "How will the London Data Service and the London Analytics Platform use my data?",
            "How do I stop the London Data Service and London Analytics Platform using my data?",
            "Where can I see the privacy notices for the London Data Service and London Analytics Platform?"
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
        const beforeYouStartSection = page.locator('section').filter({ hasText: 'Before you start' });

        await expect(beforeYouStartSection).toBeVisible();
        await expect(beforeYouStartSection).toContainText(
            "You'll need your 10-digit NHS number or your full name, postcode and date of birth so that we can identify you."
        );
        await expect(beforeYouStartSection).toContainText(
            "We will be sending an e-mail, SMS text message or letter to the contact details you have registered with your GP."
        );
    });

    test('should show correct content when expanding "What is the London Data Service?"', async ({ page }) => {
        const header = "What is the London Data Service?";
        const sectionButton = page.getByRole('button', { name: header });
        await sectionButton.click();
        await expect(
            page.getByText(
                'The London Data Service securely collects patient information from a range of healthcare ' +
                'locations across London such as GP surgeries and hospitals. It then organises and stores this ' +
                'information ready to be used by other approved systems such as the London Care Record and London ' +
                'Analytics Platform to improve patient care and support healthier communities across London.'
            )
        ).toBeVisible();
    });

    test(
        'should show correct content when expanding "How do I stop the London Data Service and the London ' +
        'Analytics Platform using my data?"',
        async ({ page }) => {
            const header = "How do I stop the London Data Service and London Analytics Platform using my data?";
            const sectionButton = page.getByRole('button', { name: header });
            await sectionButton.click();
            await expect(
                page.getByText(
                    "You can use this portal to tell us that you do not want your data used for secondary purposes " +
                    "such as healthcare planning, population health management and research. To register your details " +
                    "to 'opt-out' click the button above."
                )
            ).toBeVisible();
            await expect(
                page.getByText(
                    "Telling us that you don't want your data shared with healthcare professionals who will be " +
                    "treating you is a different process. To do that please e-mail "
                )
            ).toBeVisible();
        }
    );

    test(
        'should show correct content when expanding "Where can I see the privacy Notices for the London Data ' +
        'Service and the London Analytics Platform?"',
        async ({ page }) => {
            const header = "Where can I see the privacy notices for the London Data Service and London Analytics Platform?";
            const sectionButton = page.getByRole('button', { name: header });
            await sectionButton.click();
            await expect(page.getByText('The London Data Service and London Analytics Platform Privacy Notices are available')).toBeVisible();
        }
    );
});