import { useCallback } from "react";

interface Props {
    setApiError: (msg: string | JSX.Element) => void;
    configuration: {
        helpdeskContactEmail: string;
        helpdeskContactNumber: string;
    };
}

export function useApiErrorHandlerChecks({ setApiError, configuration }: Props) {
    return useCallback((errorTitle: string) => {
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
        }
    }, [setApiError, configuration]);
}