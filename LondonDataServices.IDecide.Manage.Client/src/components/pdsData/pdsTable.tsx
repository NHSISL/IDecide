import { FunctionComponent, useMemo, useState } from "react";
import { Button, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import PdsRow from "./pdsRow";
import { pdsDataViewService } from "../../services/views/pdsData/pdsDataViewService";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { faRefresh } from "@fortawesome/free-solid-svg-icons";

type PdsTableProps = object;

const PdsTable: FunctionComponent<PdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);
    const [getTestPatients, setGetTestPatients] = useState<boolean>(false);

    const {
        mappedPdsData: pdsRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = pdsDataViewService.useGetAllPdsData(
        getTestPatients,
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
        return hasNextPage;
    };

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };

    const toggleTestPatients = () => {
        setGetTestPatients(prevState => !prevState);
    };

    return (
        <>
        <Container fluid>
            <InputGroup className="mb-3">
            <SearchBase id="search" label="Search pds" value={searchTerm} placeholder="Search PDS Table By NHS Number"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>

            <div className="d-flex justify-content-end mb-2">
                <Button onClick={toggleTestPatients} variant="outline-dark">
                    {getTestPatients ? "Show All Patients" : "Show Only Test Patients"}
                </Button>
            </div>
            </Container>
            <Container fluid className="infiniteScrollContainer">
               
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Pseudo Identifier</th>
                                        <th>Pseudo Identifier Hex Number</th>
                                        <th>Organisation Code</th>
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
                                                {pdsRetrieved?.map(
                                                    (pdsDataView: PdsDataView) => (
                                                        <PdsRow
                                                            key={pdsDataView.id}
                                                            pds={pdsDataView}
                                                    />
                                                )
                                            )}
                                            <tr>
                                                    <td colSpan={7} className="text-center">
                                                    <InfiniteScrollLoader
                                                        loading={isFetchingNextPage}
                                                        spinner={<SpinnerBase />}
                                                        noMorePages={!hasNoMorePages()}
                                                        noMorePagesMessage={<>-- No more pages --</>}
                                                    />
                                                </td>
                                            </tr>
                                        </>
                                    )}
                                </tbody>
                            </Table>
                        </InfiniteScroll>
            </Container>
        </>
    );
};

export default PdsTable;