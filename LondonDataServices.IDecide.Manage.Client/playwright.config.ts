import { defineConfig, devices } from '@playwright/test';

const webServerCommand =
    'dotnet run --project ../LondonDataServices.IDecide.Manage.Server/' +
    (process.env.CI ? ' --launch-profile CI' : '');

export default defineConfig({
    testDir: './tests',
    fullyParallel: true,
    forbidOnly: !!process.env.CI,
    retries: process.env.CI ? 2 : 0,
    workers: process.env.CI ? 1 : undefined,
    reporter: 'html',
    use: {
        baseURL: 'https://localhost:5174',
        ignoreHTTPSErrors: true,
        headless: true,
        screenshot: 'off',
        video: 'off',
        trace: 'on-first-retry',
        actionTimeout: 10000,
        navigationTimeout: 20000,
    },

    projects: [
        { name: 'setup', testMatch: /.*\.setup\.ts/ },
        {
            name: 'Google Chrome',
            use: {
                ...devices['Desktop Chrome'],
                channel: 'chrome',
                headless: true,
            },
            dependencies: ['setup'],
        },
    ],

    webServer: {
        command: webServerCommand,
        url: 'https://localhost:5174',
        reuseExistingServer: !process.env.CI,
        timeout: 120 * 1000,
        ignoreHTTPSErrors: true,
        stdout: 'pipe',
    },
});