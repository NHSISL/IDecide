import { useState } from 'react';
import ReIdentificationBroker from '../../brokers/apiBroker.reidentification';
import { AccessRequest } from '../../models/accessRequest/accessRequest';
import { ReIdRecord } from '../../types/ReIdRecord';
import { getPseudo, isHx } from '../../helpers/hxHelpers';
import { toast } from 'react-toastify';
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';
import { useQueryClient } from '@tanstack/react-query';

export const reIdentificationService = {
    useRequestReIdentification: () => {
        const [loading, setIsLoading] = useState(false);
        const [data, setData] = useState<ReIdRecord[]>([]);
        const [isAuthorised, setIsAuthorised] = useState(true);
        const [pseudosRequested, setPseudosRequested] = useState<string[]>([]);
        const { supportContactEmail } = useFrontendConfiguration();

        const submit = async (identificationRequest: AccessRequest) => {

            const broker = new ReIdentificationBroker();

            if (identificationRequest && identificationRequest.identificationRequest && identificationRequest.identificationRequest?.identificationItems) {
                const reIdRecords: ReIdRecord[] = identificationRequest.identificationRequest?.identificationItems.map(ii => {
                    return {
                        identifier: getPseudo(ii.identifier),
                        pseudo: ii.identifier,
                        hasAccess: ii.hasAccess,
                        nhsnumber: "",
                        loading: true,
                        rowNumber: ii.rowNumber,
                        isHx: isHx(ii.identifier)
                    }
                }).filter(ri => { return pseudosRequested.findIndex(pr => pr == ri.pseudo) === -1 });

                setPseudosRequested([...pseudosRequested, ...reIdRecords.map(ri => ri.pseudo)])

                if (identificationRequest.identificationRequest.identificationItems.length === 0) {
                    return;
                }

                setIsLoading(true);
                identificationRequest.identificationRequest.identificationItems = reIdRecords;

                await broker.PostReIdentificationAsync(identificationRequest)
                    .then((response) => {
                        setData((data) => {
                            const responseItems = response.identificationRequest?.identificationItems;
                            if (!responseItems) {
                                return data;
                            }

                            const itemsToCache: ReIdRecord[] = responseItems.map(x => {
                                const r = reIdRecords.find(cachedRecord => cachedRecord.rowNumber === x.rowNumber);
                                if (!r) {
                                    return;
                                }
                                return {
                                    ...r,
                                    hasAccess: x.hasAccess,
                                    nhsnumber: x.identifier,
                                    loading: false,
                                }
                            }).filter(x => x !== undefined);

                            return [...data, ...itemsToCache]

                        })
                    }).catch((error) => {
                        let errorMessage = "";
                        if (error.response?.status === 401) {
                            setIsAuthorised(false);
                            errorMessage = `You are not authorised to make this request, please contact ${supportContactEmail} to request access.`;
                        } else {
                            errorMessage = `Something went wrong. Please contact ${supportContactEmail} for support.`;
                        }
                        toast.error(`${errorMessage}`, {
                            position: "top-right",
                            autoClose: 5000,
                            closeOnClick: true,
                            hideProgressBar: false,
                            pauseOnHover: true,
                        });

                        throw error;
                    }).finally(() => {
                        setIsLoading(false);
                    })
            }
        }

        const cleardata = () => {
            setPseudosRequested([]);
            setData([]);
        }

        return {
            submit,
            loading,
            data,
            cleardata,
            isAuthorised
        };
    },

    useRequestReIdentificationCsv: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        return {
            submit: (csvIdentificationRequest: AccessRequest) => {
                setIsLoading(true);
                return broker.PostReIdentificationCsvAsync(csvIdentificationRequest).finally(() => {
                    setIsLoading(false);
                })
            },
            loading
        };
    },

    useRequestReIdentificationImpersonation: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        const queryClient = useQueryClient();

        return {
            submit: (csvIdentificationRequest: AccessRequest) => {
                setIsLoading(true);
                return broker.PostReIdentificationImpersonationAsync(csvIdentificationRequest)
                    .then(() => {
                        queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetAll"] });
                    })
                    .finally(() => {
                        setIsLoading(false);
                    });
            },
            loading
        };
    },

    useRequestReIdentificationImpersonationGenerateTokens: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        return {
            submit: (impersonationContextId: string) => {
                setIsLoading(true);
                return broker.PostImpersonationContextGenerateTokensAsync(impersonationContextId).finally(() => {
                    setIsLoading(false);
                })
            },
            loading
        };
    },

    useRequestReIdentificationImpersonationApproval: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        const queryClient = useQueryClient();
        return {
            submitApproval: (impersonationContextId: string, isApproved: boolean) => {
                setIsLoading(true);
                return broker.PostReIdentificationImpersonationApprovalAsync(impersonationContextId, isApproved)
                    .then(() => {
                        queryClient.invalidateQueries({ queryKey: ["GetAllImpersonationById", { impersonationId: impersonationContextId }] });
                        queryClient.invalidateQueries({ queryKey: ["ImpersonationContextGetAll"] });
                    })
                    .finally(() => {
                        setIsLoading(false);
                    })
            },
            loading
        };
    },

    useGetCsvIdentificationRequestById: (id: string, reason: string) => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        const [data, setData] = useState<AccessRequest | null>(null);
        const [filename, setFilename] = useState<string>('reidentification.csv');
        const [error, setError] = useState<Error | null>(null);

        const fetch = () => {
            setIsLoading(true);
            return broker.GetCsvIdentificationRequestByIdAsync(id, reason)
                .then(result => {
                    setData(result.data);
                    setFilename(result.filename);
                    setError(null);
                })
                .catch(err => {
                    setError(err);
                    setData(null);
                    throw err;
                })
                .finally(() => {
                    setIsLoading(false);
                });
        };

        return {
            fetch,
            loading,
            data,
            filename,
            error,
        };
    }
}
