export function loadRecaptchaScript(siteKey: string): Promise<void> {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`script[src*="recaptcha/api.js"]`)) {
            resolve();
            return;
        }
        const script = document.createElement("script");
        script.src = `https://www.google.com/recaptcha/api.js?render=${siteKey}`;
        script.async = true;
        script.onload = () => resolve();
        script.onerror = () => reject();
        document.body.appendChild(script);
    });
}