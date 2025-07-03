import  debounce from "lodash/debounce";
import { FunctionComponent, useMemo, useState } from "react";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase } from "@fortawesome/free-solid-svg-icons";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/ImpersonationContextView";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { impersonationContextViewService } from "../../services/views/impersonationContext/impersonationContextViewService";
import SearchBase from "../bases/inputs/SearchBase";
import ImpersonationContextRow from "./impersonationContextRow";

type ImpersonationContextTableProps = object;

const ImpersonationContextTable: FunctionComponent<ImpersonationContextTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);


    const {
        mappedImpersonationContexts: impersonationContextRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
    } = impersonationContextViewService.useGetAllImpersonationContexts(
        debouncedTerm
    );

    const handleSearchChange = (value: string) => {
        setSearchTerm(value);
        handleDebounce(value);
    };

    const handleDebounce = useMemo(
        () =>
            debounce((value: string) => {
                setDebouncedTerm(value);
            }, 500),
        []
    );

    const hasNoMorePages = () => {
        return false;
        //return !isLoading && data?.pages.at(-1)?.nextPage === undefined;
    };

    return (
        <>
            <SearchBase id="search" label="Search lookups" value={searchTerm} placeholder="Search Impersonation Table"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
            <br />

            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> Impersonation</Card.Header>
                    <Card.Body>
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light">
                                <thead>
                                    <tr>
                                        <th>Requester FirstName</th>
                                        <th>Requester LastName</th>
                                        <th>Requester Email</th>
                                        <th>Recipient FirstName</th>
                                        <th>Recipient LastName</th>
                                        <th>Recipient Email</th>
                                        <th>Reason</th>
                                        <th>Approval</th>
                                        <th>Action(s)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading || showSpinner ? (
                                        <tr>
                                            <td colSpan={6} className="text-center">
                                                <SpinnerBase />
                                            </td>
                                        </tr>
                                    ) : (
                                        <>
                                            {impersonationContextRetrieved?.map(
                                                (impersonationContextView: ImpersonationContextView) => (
                                                    <ImpersonationContextRow
                                                        key={impersonationContextView.id.toString()}
                                                        impersonationContext={impersonationContextView}
                                                    />
                                                )
                                            )}
                                            <tr>
                                                <td colSpan={7} className="text-center">
                                                    <InfiniteScrollLoader
                                                        loading={isLoading || isFetchingNextPage}
                                                        spinner={<SpinnerBase />}
                                                        noMorePages={hasNoMorePages()}
                                                        noMorePagesMessage={<>-- No more pages --</>}
                                                    />
                                                    </td>
                                            </tr>
                                        </>
                                    )}
                                </tbody>
                            </Table>
                        </InfiniteScroll>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default ImpersonationContextTable;