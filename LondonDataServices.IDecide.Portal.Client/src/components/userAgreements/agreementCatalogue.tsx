import { agreement_v0_1 } from "./versions/agreement_v0_1"
import { agreement_v1_0 } from "./versions/agreement_v1_0";

export type Agreement = { 
    type: string;
    version: string;
    text: JSX.Element;
};

export const useAgreementCatalogue = () => {

    const agreements : Agreement[] = [ 
        agreement_v0_1(),
        agreement_v1_0(),
    ]

    return {
        getAgreement: (type: string, version: string) => { 
            const agreement = agreements.filter(x => x.type === type && x.version === version);
            if(agreement.length === 1) {
                return agreement[0].text;
            }

            throw new Error(`Agreement not found for type: ${type} and version: ${version}`);
        }
    }
}