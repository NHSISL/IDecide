// Helper function to click the Start button on the homepage
export async function clickStartButton(page) {
    await page.getByTestId('start-login-button').click();
}

export async function clickStartAnotherPersonButton(page) {
    await page.getByTestId('start-another-person-button').click();
}

export async function fillPoaFields(
    page: import('@playwright/test').Page,
    firstName = 'John',
    surname = 'Doe'
) {
    await page.locator('#poa-firstname').fill(firstName);
    await page.locator('#poa-surname').fill(surname);

    // Get all enabled options (excluding the disabled placeholder)
    const options = await page.locator('#poa-relationship option:not([disabled])').all();
    // Pick a random option (skipping the first if it's a placeholder)
    const randomIndex = Math.floor(Math.random() * (options.length - 1)) + 1;
    const randomValue = await options[randomIndex].getAttribute('value');
    await page.locator('#poa-relationship').selectOption(randomValue!);
}

// Helper function to populate NHS login credentials
export async function fillNhsLoginCredentials(
    page: import('@playwright/test').Page,
    email = 'testuserlive@demo.signin.nhs.uk',
    password = 'Passw0rd$1'
) {
    await page.locator('input[type="email"]').fill(email);
    await page.locator('input[type="password"]').fill(password);
}

// Helper function to click the Continue button
export async function clickContinueButton(page: import('@playwright/test').Page) {
    await page.getByRole('button', { name: /continue/i }).click();
}

// Helper function to fill the OTP code
export async function fillOtpCode(
    page: import('@playwright/test').Page,
    otpCode = '190696'
) {
    await page.locator('#otp-input').fill(otpCode);
}

// Helper function to check the "Remember this device" checkbox
export async function checkRememberDeviceCheckbox(page: import('@playwright/test').Page) {
    await page.locator('#rmd').check();
}

// Helper function to handle OTP if prompted and click continue
export async function handleOtpIfPrompted(
    page: import('@playwright/test').Page,
    otpCode = '190696'
) {
    // Wait for navigation after login to settle
    await page.waitForLoadState('networkidle');

    const currentUrl = page.url();

    // Check if we're locked out due to too many OTP attempts
    if (currentUrl.includes('/otp-attempts-exceeded')) {
        throw new Error(
            'OTP attempts exceeded. NHS Login has locked you out for 15 minutes. ' +
            'Please wait before running tests again.'
        );
    }

    // Check if we're on the OTP page by URL
    const isOnOtpPage = currentUrl.includes('/enter-mobile-code');

    if (isOnOtpPage) {
        const otpInput = page.locator('#otp-input');
        const continueButton = page.getByRole('button', { name: /continue/i });

        // Wait for OTP input to be visible
        await otpInput.waitFor({ state: 'visible', timeout: 10000 });

        // Check for existing error before attempting to fill OTP
        const errorAlert = page.locator('div[role="alert"]:has-text("Error")');
        const hasExistingError = await errorAlert.isVisible().catch(() => false);

        if (hasExistingError) {
            const errorText = await errorAlert.textContent();
            throw new Error(
                `OTP page shows existing error: ${errorText}. ` +
                'Tests aborted to prevent lockout. Please resolve the error before continuing.'
            );
        }

        // Fill OTP and remember device
        await otpInput.fill(otpCode);
        await page.locator('#rmd').check();

        // Click continue button
        await continueButton.click();

        // Wait for navigation away from OTP page
        await page.waitForLoadState('networkidle');

        const newUrl = page.url();

        // Check if we got locked out after submission
        if (newUrl.includes('/otp-attempts-exceeded')) {
            throw new Error(
                'Invalid OTP code caused lockout. NHS Login has locked you out for 15 minutes. ' +
                `The OTP code "${otpCode}" appears to be incorrect.`
            );
        }

        // Check if we're still on the OTP page with an error
        if (newUrl.includes('/enter-mobile-code')) {
            const postSubmitError = await errorAlert.isVisible().catch(() => false);
            if (postSubmitError) {
                const errorText = await errorAlert.textContent();
                throw new Error(
                    `OTP submission failed: ${errorText}. ` +
                    'Tests aborted to prevent further failed attempts and lockout.'
                );
            }
        }
    }
}