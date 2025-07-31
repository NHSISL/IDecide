// Helper function to click the Start button on the homepage
export async function clickStartButton(page) {
    await page.getByRole('button', { name: 'Start' }).click();
}