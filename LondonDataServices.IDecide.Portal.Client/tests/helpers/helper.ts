// Helper function to click the Start button on the homepage
export async function clickStartButton(page) {
    await page.getByTestId('start-login-button').click();
}
export async function clickStartAnotherPersonButton(page) {
    await page.getByTestId('start-another-person-button').click();
}
export async function fillPoaFields(page: import('@playwright/test').Page, firstName = 'John', surname = 'Doe') {
    await page.locator('#poa-firstname').fill(firstName);
    await page.locator('#poa-surname').fill(surname);

    // Get all enabled options (excluding the disabled placeholder)
    const options = await page.locator('#poa-relationship option:not([disabled])').all();
    // Pick a random option (skipping the first if it's a placeholder)
    const randomIndex = Math.floor(Math.random() * (options.length - 1)) + 1;
    const randomValue = await options[randomIndex].getAttribute('value');
    await page.locator('#poa-relationship').selectOption(randomValue!);
}