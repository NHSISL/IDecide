export function loadRecaptchaScript(siteKey: string): Promise<void> {
    return new Promise((resolve, reject) => {
        if (!siteKey) {
            reject(new Error("siteKey is required for reCAPTCHA."));
            return;
        }
        if (document.querySelector(`script[src*="recaptcha/api.js"]`)) {
            resolve();
            return;
        }
        const script = document.createElement("script");
        script.src = `https://www.google.com/recaptcha/api.js?render=${siteKey}&badge=bottomleft`;
        script.async = true;
        script.onload = () => resolve();
        script.onerror = () => reject();
        document.head.appendChild(script);
    });
}