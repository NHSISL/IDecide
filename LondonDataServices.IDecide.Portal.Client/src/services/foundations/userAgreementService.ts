import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import UserAgreementBroker from '../../brokers/apiBroker.userAgreement';
import { UserAgreement } from '../../models/userAgreement/userAgreement';

export const userAgreementService = {

    useRetrieveAgreement: (userEntraId: string, agreementVersion?: string, agreementType?: string) => {
        const broker = new UserAgreementBroker();

        return useQuery({
            queryKey: ["UserAgreement", { userid: userEntraId, version: agreementVersion }],
            queryFn: () => broker.UserHasSignedAgreement(userEntraId, agreementVersion, agreementType),
            staleTime: Infinity
        });
    },

    useAcceptAgreement: () => {
        const broker = new UserAgreementBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (userAgreement: UserAgreement) => {
                return broker.SignAgreement(userAgreement);
            }, onSuccess: () => {
                queryClient.invalidateQueries({ queryKey: ["UserAgreement"] });
            }
        });
    }
}