import { logoutBroker } from "../../brokers/apiBroker.logoutBroker";

export async function logoutService(): Promise<void> {
    const response = await logoutBroker();
    if (!response.ok) {
        throw new Error("Logout failed");
    }
}