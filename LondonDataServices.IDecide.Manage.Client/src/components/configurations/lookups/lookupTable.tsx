import { FunctionComponent, useMemo, useState } from "react";
import debounce from "lodash/debounce";
import { Card, Container, Table } from "react-bootstrap";
import LookupRow from "./lookupRow";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { lookupViewService } from "../../../services/views/lookups/lookupViewService";
import SearchBase from "../../bases/inputs/SearchBase";
import { SpinnerBase } from "../../bases/spinner/SpinnerBase";
import LookupRowAdd from "./lookupRowAdd";
import LookupRowNew from "./lookupRowNew";
import { ApiError } from "../../../types/apiError";
import { ILookupApiErrors, LookupApiErrors } from "./lookupApiErrors";

type LookupTableProps = {
    allowedToAdd: boolean;
    allowedToEdit: boolean;
    allowedToDelete: boolean;
}

const LookupTable: FunctionComponent<LookupTableProps> = (props) => {
    const {
        allowedToAdd,
        allowedToEdit,
        allowedToDelete,
    } = props;

    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [addMode, setAddMode] = useState<boolean>(false);
    const [addApiError, setAddApiError] = useState<ILookupApiErrors>(LookupApiErrors);

    const { mappedLookups: lookupsRetrieved, isLoading }
        = lookupViewService.useGetAllLookups(debouncedTerm);

    const handleSearchChange = (value: string) => {
        setSearchTerm(value);
        handleDebounce(value);
    }

    const handleAddState = () => {
        setAddMode(!addMode);
    };

    const addLookup = lookupViewService.useCreateLookup();
    const handleAddNew = (lookup: LookupView) => {
        return addLookup.mutate(lookup, {
            onSuccess: () => {
                setAddMode(false);
            },
            onError: (error: ApiError) => {
                setAddApiError(error?.response?.data?.errors as ILookupApiErrors);
            }
        });
    };

    const updateLookup = lookupViewService.useUpdateLookup();
    const handleUpdate = (lookup: LookupView) => {
        return updateLookup.mutateAsync(lookup);
    }

    const deleteLookup = lookupViewService.useRemoveLookup();
    const handleDelete = (lookup:LookupView) => {
        return deleteLookup.mutateAsync(lookup.id);
    }

    const handleDebounce = useMemo(
        () => debounce((value: string) => {
            setDebouncedTerm(value)
        }, 500)
        , [])

    return (
        <>
            <Container fluid>
                <SearchBase id="search" label="Search lookups" value={searchTerm} placeholder="Search lookups"
                    onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <br />
                <Card>
                    <Card.Header as="h5">
                        Lookup Configuration
                    </Card.Header>
                    <Card.Body>
                        {isLoading && <> <SpinnerBase />.</>}
                        <Table striped bordered hover variant="light" responsive>
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Value</th>
                                    <th>Group</th>
                                    <th>Created By</th>
                                    <th>Created When</th>
                                    <th>Action(s)</th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    allowedToAdd &&
                                    <>
                                        {addMode === false && (<LookupRowNew onAdd={handleAddState} />)}

                                        {addMode === true && (
                                            <LookupRowAdd
                                                onCancel={handleAddState}
                                                onAdd={handleAddNew}
                                                apiError={addApiError} />)}
                                    </>
                                }
                                {
                                    lookupsRetrieved?.map((lookup: LookupView) =>
                                        <LookupRow
                                            key={lookup.id?.toString()}
                                            lookup={lookup}
                                            allowedToEdit={allowedToEdit}
                                            allowedToDelete={allowedToDelete}
                                            onUpdate={handleUpdate}
                                            onDelete={handleDelete}/>
                                    )}
                            </tbody>
                        </Table>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
}

export default LookupTable;