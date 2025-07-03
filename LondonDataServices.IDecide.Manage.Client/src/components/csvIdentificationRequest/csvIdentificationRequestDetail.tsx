import React, { FunctionComponent, useState, useEffect } from "react";
import { CsvIdentificationRequestView } from "../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";
import { csvIdentificationRequestViewService } from "../../services/views/csvIdentificationRequest/csvIdentificationRequestViewService";
import CsvIdentificationRequestDetailCard from "./csvIdentificationRequestDetailCard";
import { useParams } from "react-router-dom";

type CsvIdentificationRequestDetailProps = {
    children?: React.ReactNode;
};

const CsvIdentificationRequestDetail: FunctionComponent<CsvIdentificationRequestDetailProps> = (props) => {
    const {
        children
    } = props;

    const { CsvIdentificationRequestId } = useParams();

    let csvIdentificationRequestRetrieved: CsvIdentificationRequestView | undefined

    if (CsvIdentificationRequestId) {
        const { mappedCsvIdentificationRequest } = csvIdentificationRequestViewService.useGetCsvIdentificationRequestById(CsvIdentificationRequestId);
        csvIdentificationRequestRetrieved = mappedCsvIdentificationRequest
    }

    const [csvIdentificationRequest, setCsvIdentificationRequest] = useState<CsvIdentificationRequestView>();
    const [mode, setMode] = useState<string>('VIEW');

    useEffect(() => {
        if (CsvIdentificationRequestId !== "") {
            setCsvIdentificationRequest(csvIdentificationRequestRetrieved);
            setMode('VIEW');
        }
        if (CsvIdentificationRequestId === "" || CsvIdentificationRequestId === undefined) {
            //setCsvIdentificationRequest(new CsvIdentificationRequestView(crypto.randomUUID(), "", "", "", "", new Date(), new Date()))
            setMode('ADD');
        }
    }, [CsvIdentificationRequestId, csvIdentificationRequestRetrieved]);

    return (
        <div>
            {csvIdentificationRequest !== undefined && (
                <div>
                    <CsvIdentificationRequestDetailCard
                        key={CsvIdentificationRequestId}
                        csvIdentificationRequest={csvIdentificationRequest}
                        mode={mode}
                        //onAdd={handleAdd}
                        //onUpdate={handleUpdate}
                        //onDelete={handleDelete}
                    >
                        {children}
                    </CsvIdentificationRequestDetailCard>
                </div>
            )}
        </div>
    );
};

export default CsvIdentificationRequestDetail;