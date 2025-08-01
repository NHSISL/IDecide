import { defineConfig, devices } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const authFile = path.join(__dirname, './playwright/.auth/user.json');
const webServerCommand = 'dotnet run --project ../LondonDataServices.IDecide.Portal.Server/LondonDataServices.IDecide.Portal.Server.csproj' + (process.env.CI ? ' --environment ASPNETCORE_ENVIRONMENT=ContinuousIntegration' : '');
console.log(webServerCommand);

export default defineConfig({
    testDir: './tests',
    reporter: 'html',
    webServer: {
        command: webServerCommand,
        url: 'https://localhost:5173',
        reuseExistingServer: !process.env.CI,
        ignoreHTTPSErrors: true,
        stdout: 'pipe'
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