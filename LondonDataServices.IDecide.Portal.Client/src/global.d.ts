declare interface Grecaptcha {
    ready(cb: () => void): void;
    execute(siteKey: string, options: { action: string }): Promise<string>;
    render(container: string | HTMLElement, parameters: object): number;
    getResponse(optWidgetId?: number): string;
    reset(optWidgetId?: number): void;
}
interface Grecaptcha {
    execute(siteKey: string, options: { action: string }): Promise<string>;
}
declare const grecaptcha: Grecaptcha;
interface Window {
    grecaptcha?: Grecaptcha;
}