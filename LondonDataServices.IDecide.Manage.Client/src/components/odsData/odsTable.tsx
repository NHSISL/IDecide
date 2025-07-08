import { FunctionComponent, useMemo, useState } from "react";
import { Button, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import OdsRow from "./odsRow";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { OdsDataView } from "../../models/views/components/odsData/odsDataView";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { odsDataViewService } from "../../services/views/odsData/odsDataViewService";
import { faRefresh } from "@fortawesome/free-solid-svg-icons";

type OdsTableProps = object;

const OdsTable: FunctionComponent<OdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);

    const {
        mappedOdsData: odsRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = odsDataViewService.useGetAllOdsData(
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
        return !hasNextPage;
    };

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };

    return (
        <>
            <InputGroup className="mb-3">
                <SearchBase id="search" label="Search ods" value={searchTerm} placeholder="Search ODS"
                    onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>

            <Container fluid className="infiniteScrollContainer">
                <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                    <Table striped bordered hover variant="light" responsive>
                        <thead>
                            <tr>
                                <th>Organisation Name</th>
                                <th>Organisation Code</th>
                                <th>Has Children</th>
                                <th>Relationship With Parent StartDate</th>
                                <th>Relationship With Parent EndDate</th>
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
                                    {odsRetrieved?.map(
                                        (odsDataView: OdsDataView) => (
                                            <OdsRow
                                                key={odsDataView.id}
                                                ods={odsDataView}
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
            </Container>
        </>
    );
};

export default OdsTable;