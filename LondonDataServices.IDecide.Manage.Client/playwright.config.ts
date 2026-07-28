import { defineConfig, devices } from '@playwright/test';

const webServerUrl = process.env.CI ? 'https://localhost:7043' : 'https://localhost:5174';

const webServerCommand = process.env.CI
    ? 'dotnet run --project ../LondonDataServices.IDecide.Manage.Server/ --launch-profile CI --no-build'
    : 'dotnet run --project ../LondonDataServices.IDecide.Manage.Server/';

export default defineConfig({
    testDir: './tests',
    fullyParallel: true,
    forbidOnly: !!process.env.CI,
    retries: process.env.CI ? 2 : 0,
    workers: process.env.CI ? 1 : undefined,
    reporter: 'html',
    use: {
        baseURL: webServerUrl,
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
        url: webServerUrl,
        reuseExistingServer: !process.env.CI,
        timeout: 10 * 60 * 1000,
        ignoreHTTPSErrors: true,
        stdout: 'pipe',
    },
});