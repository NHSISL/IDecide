/* eslint-disable @typescript-eslint/no-unused-vars */
import { defineConfig, devices } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const authFile = path.join(__dirname, './playwright/.auth/user.json');

const webServerCommand =
    'dotnet run --project ../LondonDataServices.IDecide.Portal.Server/LondonDataServices.IDecide.Portal.Server.csproj' +
    (process.env.CI ? ' --environment ASPNETCORE_ENVIRONMENT=ContinuousIntegration' : '');

export default defineConfig({
    testDir: './tests',
    reporter: 'html',
    workers: process.env.CI ? 2 : 8, // Use fewer workers in CI to avoid resource contention

    webServer: {
        command: webServerCommand,
        url: 'https://localhost:5173',
        reuseExistingServer: !process.env.CI,
        timeout: 60 * 1000, // Reduce timeout for faster failure if server doesn't start
        ignoreHTTPSErrors: true,
        stdout: 'pipe'
    },
    use: {
        ignoreHTTPSErrors: true,
        baseURL: 'http://localhost:5173',
        headless: true,
        screenshot: 'off', // Disable screenshots unless debugging
        video: 'off',      // Disable video unless debugging
        trace: 'off',      // Disable trace unless debugging
        actionTimeout: 10000, // Fail fast on slow actions
        navigationTimeout: 20000, // Fail fast on slow navigation
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
        },
    ],
});