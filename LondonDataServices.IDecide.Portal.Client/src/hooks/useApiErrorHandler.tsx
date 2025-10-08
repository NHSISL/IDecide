/* eslint-disable @typescript-eslint/no-explicit-any */
import { useCallback } from "react";

interface ApiErrorHandlerProps {
    setError: (msg: string) => void;
    setApiError: (msg: string | JSX.Element) => void;
    setLoading: (loading: boolean) => void;
    translate: (key: string) => string;
    configuration: {
        helpdeskContactEmail: string;
        helpdeskContactNumber: string;
    };
}

export function useApiErrorHandler({
    setError,
    setApiError,
    setLoading,
    translate,
    configuration
}: ApiErrorHandlerProps) {
    return useCallback((error: any) => {
        const errorData = error?.response?.data;
        const errorTitle = errorData?.title;

        if (errorTitle === "The patient is marked as sensitive.") {
            setApiError(
                <>
                    There is an issue with this patient record. For further assistance please e-mail{" "}
                    <a
                        href={`mailto:${configuration.helpdeskContactEmail}`}
                        style={{ textDecoration: "underline" }}
                    >
                        {configuration.helpdeskContactEmail}
                    </a>
                    {" or leave a voicemail with the One London Service desk on "}
                    <a
                        href={`tel:${configuration.helpdeskContactNumber}`}
                        style={{ textDecoration: "underline" }}
                    >
                        {configuration.helpdeskContactNumber}
                    </a>
                    {" for a call back and assistance."}
                </>
            );
        } else if (errorTitle === "The provided captcha token is invalid.") {
            setApiError(
                <>
                    There was a problem verifying you are not a robot. Please try again, or contact support at{" "}
                    <a
                        href={`mailto:${configuration.helpdeskContactEmail}`}
                        style={{ textDecoration: "underline" }}
                    >
                        {configuration.helpdeskContactEmail}
                    </a>
                    {" or leave a voicemail with the One London Service desk on "}
                    <a
                        href={`tel:${configuration.helpdeskContactNumber}`}
                        style={{ textDecoration: "underline" }}
                    >
                        {configuration.helpdeskContactNumber}
                    </a>
                    {" for a call back and assistance."}
                </>
            );
        } else if (errorTitle) {
            setError(errorTitle);
        } else if (error?.response?.status === 400) {
            setError(translate("errors.400"));
        } else if (error?.response?.status === 404) {
            setError(translate("errors.404"));
        } else {
            setError(translate("SearchBySHSNumber.errorCreatePatient"));
        }
        setLoading(false);
    }, [setError, setApiError, setLoading, translate, configuration]);
}