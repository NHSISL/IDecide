import { useCallback } from "react";
import { logoutService } from "../services/foundations/logoutService";

export function useLogout() {
    return useCallback(async () => {
        try {
            await logoutService();
            window.location.href = '/';
        } catch (error) {
            console.error(error);
        }
    }, []);
}