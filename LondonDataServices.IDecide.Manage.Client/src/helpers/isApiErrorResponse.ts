interface ApiErrorResponse {
    response: {
        data?: {
            title?: string;
            message?: string;
        };
        statusText?: string;
    };
}
export function isApiErrorResponse(error: unknown): error is ApiErrorResponse {
    return (
        typeof error === "object" &&
        error !== null &&
        "response" in error &&
        typeof (error as { response?: unknown }).response === "object"
    );
}