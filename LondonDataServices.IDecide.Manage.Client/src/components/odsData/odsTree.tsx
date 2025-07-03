import { FunctionComponent, useEffect, useState, useRef } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { OdsTreeElement } from "./odsTreeElement";
import { Form, Spinner } from "react-bootstrap";

type OdsTreeProps = {
    rootId: string;
    selectedRecords: OdsData[];
    setSelectedRecords: (selectedRecords: OdsData[]) => void;
    readonly: boolean;
    showRoot?: boolean
};

const OdsTree: FunctionComponent<OdsTreeProps> = ({ rootId, selectedRecords, setSelectedRecords, readonly = false, showRoot = false }) => {

    const { data: rootRecord, isLoading } = odsDataService.useRetrieveAllOdsData(`?filter=Id eq ${rootId}`)
    const { data } = odsDataService.useGetOdsChildren(rootId);
    const tree = useRef<HTMLDivElement>(null);
    const rootItem = useRef<HTMLInputElement>(null);
    const [rootItemDisabled, setRootItemDisabled] = useState(false)

    const addSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords.filter(o => o.organisationCode != odsRecord.organisationCode)])
    }

    useEffect(() => {
        if (!tree.current) {
            return
        }
        const indeterminate = tree.current.querySelectorAll<HTMLInputElement>('input:indeterminate');

        for (let i = 0; i < indeterminate.length; i++) {
            indeterminate[i].indeterminate = false;
            if (!indeterminate[i].dataset)
                continue;
            const path = indeterminate[i].dataset.odsHierarchy;
            if (!path) {
                continue;
            }

            if (selectedRecords.find(x => x.odsHierarchy.startsWith(path))) {
                indeterminate[i].indeterminate = true;
            } else {
                indeterminate[i].indeterminate = false;
            }
        }

        if (rootItem && rootItem.current && rootRecord && rootRecord[0] && selectedRecords.find(x => x.odsHierarchy.startsWith(rootRecord[0].odsHierarchy) && x.id !== rootRecord[0].id)) {
            rootItem.current.indeterminate = true;
            setRootItemDisabled(true);
        } else {
            setRootItemDisabled(false);
        }
    }, [selectedRecords, rootId, rootRecord]);


    if (isLoading || !rootRecord) {
        return <Spinner />
    }
   

    const rootSelected = (): boolean => {
        return selectedRecords.filter((x: OdsData) => x.organisationCode == rootRecord[0].organisationCode).length > 0
    }

    return (
        <div ref={tree}>
            {showRoot && <Form>

                <Form.Check
                    ref={rootItem}
                    data-ods-hierarchy={rootRecord[0].odsHierarchy}
                    disabled={rootItemDisabled}
                    inline
                    checked={rootSelected()}
                    onChange={(e) => {
                        if (e.target.checked) {
                            addSelectedRecord(rootRecord[0]);
                        } else {
                            removeSelectedRecord(rootRecord[0]);
                        }
                    }} />

                <span>{rootRecord[0].organisationName}({rootRecord[0].organisationCode})</span>
            </Form>
            }

            {data && data.map((element: OdsData) => {
                return <span key={element.id} >
                    <OdsTreeElement
                        node={element}
                        addSelectedRecord={addSelectedRecord}
                        removeSelectedRecord={removeSelectedRecord}
                        parentSelected={false}
                        selectedRecords={selectedRecords}
                        readonly={readonly}
                    ></OdsTreeElement>
                </span>;
            }
            )}
        </div>
    );
};

export default OdsTree;

