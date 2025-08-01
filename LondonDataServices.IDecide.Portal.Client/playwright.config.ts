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
        command: "npm run dev", // or "yarn dev" if you use yarn
        url: 'https://localhost:5173/home',
        reuseExistingServer: true,
        ignoreHTTPSErrors: true,
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