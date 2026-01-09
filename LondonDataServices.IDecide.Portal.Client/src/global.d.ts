declare interface Grecaptcha {
    execute(siteKey: string, options: { action: string }): Promise<string>;
    render(container: string | HTMLElement, parameters: object): number;
    getResponse(optWidgetId?: number): string;
    reset(optWidgetId?: number): void;
}
interface Grecaptcha {
    execute(siteKey: string, options: { action: string }): Promise<string>;
}
interface Window {
    grecaptcha?: Grecaptcha;
}