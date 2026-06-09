import { test as setup } from '@playwright/test';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);

setup('authenticate', async () => {
    setup.skip(true, 'Skipped temporarily');
});