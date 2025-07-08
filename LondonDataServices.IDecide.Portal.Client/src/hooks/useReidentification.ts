import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../models/accessRequest/accessRequest";
import { useState } from "react";
import { ReIdRecord } from "../types/ReIdRecord";
import { reIdentificationService } from "../services/foundations/reIdentificationService";
import { IdentificationItem } from "../models/ReIdentifications/IdentificationItem";

type TrackedAccessRequest = {
    called: boolean,
    completed: boolean,
    accessRequest: AccessRequest,
}

export function useReidentification(reason: string) {
    const [isLoading, setIsLoading] = useState(false);
    const [lastPseudo, setLastPseudo] = useState<ReIdRecord>();
    const { accounts } = useMsal();
    const { submit, data, cleardata, isAuthorised } = reIdentificationService.useRequestReIdentification();

    const processRequest = async (request: TrackedAccessRequest) => {
        if (request.accessRequest.identificationRequest) {
            setIsLoading(true);
            try {
                await submit(request.accessRequest);
            } catch (e) {
                console.log(e);
            } finally {
                setIsLoading(false);
            }
            
        }
    }

    const reidentify = async (pseudoNumbers: string[]) => {
        const acc = accounts[0];


        const identificationItems = pseudoNumbers.map((ps): IdentificationItem => {
            return {
                rowNumber: crypto.randomUUID(),
                identifier: ps,
                hasAccess: false
            }
        });

        const trackedAccessRequest: TrackedAccessRequest = {
            called: false,
            completed: false,
            accessRequest: {
                csvIdentificationRequest: undefined,
                impersonationContext: undefined,
                identificationRequest: {
                    id: crypto.randomUUID(),
                    identificationItems: identificationItems,
                    displayName: acc.name || "",
                    email: acc.username,
                    reason: reason,
                    organisation: "TODO"
                }
            }
        }

        await processRequest(trackedAccessRequest)
    }

    function clearList() {
        setLastPseudo(undefined);
        cleardata();
    }

    return { reidentify, reidentifications: data, lastPseudo, clearList, isLoading, isAuthorised }
}


