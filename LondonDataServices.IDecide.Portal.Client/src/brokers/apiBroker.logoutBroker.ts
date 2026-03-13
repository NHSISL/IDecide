export async function logoutBroker(): Promise<Response> {
    return fetch('/logout', { method: 'POST' });
}