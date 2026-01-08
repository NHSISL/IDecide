let recaptchaPromise: Promise<void> | null = null;

export function loadRecaptchaScript(siteKey: string): Promise<void> {
    if ((window as any).grecaptcha && (window as any).grecaptcha.execute) {
        return Promise.resolve();
    }

    if (recaptchaPromise) {
        return recaptchaPromise;
    }

    recaptchaPromise = new Promise((resolve, reject) => {
        const checkReady = () => {
            if ((window as any).grecaptcha && (window as any).grecaptcha.execute) {
                resolve();
                return true;
            }
            return false;
        };

        if (checkReady()) {
            return;
        }

        const existingScript = document.querySelector('script[src*="recaptcha/api.js"]');
        if (existingScript) {
            existingScript.addEventListener('load', () => {
                // Poll for grecaptcha availability after script load
                const interval = setInterval(() => {
                    if (checkReady()) {
                        clearInterval(interval);
                    }
                }, 50);
            });
            return;
        }

        const script = document.createElement('script');
        script.src = `https://www.google.com/recaptcha/api.js?render=${siteKey}`;
        script.async = true;
        script.defer = true;
        script.onload = () => {
            // Poll for grecaptcha availability after script load
            const interval = setInterval(() => {
                if (checkReady()) {
                    clearInterval(interval);
                }
            }, 50);
        };
        script.onerror = reject;
        document.body.appendChild(script);

        // Timeout after 10 seconds
        setTimeout(() => {
            if (!(window as any).grecaptcha) {
                reject(new Error("reCAPTCHA failed to load within timeout."));
            }
        }, 10000);
    });

    return recaptchaPromise;
}