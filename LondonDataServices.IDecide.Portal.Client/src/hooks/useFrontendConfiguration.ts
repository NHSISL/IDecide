import { useEffect, useState } from "react";
import FrontendConfigurationBroker, { FrontendConfiguration } from "../brokers/apiBroker.frontendConfigurationBroker";
import { useQuery } from "@tanstack/react-query";

export function useFrontendConfiguration() {
    const [configuration, setConfiguration] = useState<FrontendConfiguration>({} as FrontendConfiguration);

    const broker = new FrontendConfigurationBroker();

    const query = useQuery({
        queryKey: ["FrontendConfiguration"],
        queryFn: async () => await broker.GetFrontendConfigruationAsync(),
        staleTime: Infinity
    });

    useEffect(() => {
        if (query && query.data) {
            setConfiguration(query.data);
        }
    },[query,query.data]);
        
    return { configuration, ...configuration }
}