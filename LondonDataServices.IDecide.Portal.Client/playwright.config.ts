import { defineConfig, devices } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const authFile = path.join(__dirname, './playwright/.auth/user.json');

export default defineConfig({
    testDir: './tests',
    reporter: 'html',
    webServer: {
        command: 'dotnet run -p ..\\LondonDataServices.IDecide.Portal.Server\\LondonDataServices.IDecide.Portal.Server.csproj' + (process.env.CI ? ' --environment ASPNETCORE_ENVIRONMENT=ContinuousIntegration' : ''),
        url: 'https://localhost:5173',
        reuseExistingServer: true,
        ignoreHTTPSErrors: true,
    },
    use: {
        baseURL: 'http://localhost:5173',
    },
    projects: [
        { name: 'setup', testMatch: /.*\.setup\.ts/ },
        {
            name: 'Google Chrome',
            use: {
                ...devices['Desktop Chrome'],
                channel: 'chrome',
                headless: true, // <-- Add this line
            },
        },
    ],
});