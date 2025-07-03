import debounce from "lodash/debounce";
import { FunctionComponent, useMemo, useState } from "react";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Button, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRefresh } from "@fortawesome/free-solid-svg-icons";
import { CsvIdentificationRequestView } from "../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { csvIdentificationRequestViewService } from "../../services/views/csvIdentificationRequest/csvIdentificationRequestViewService";
import SearchBase from "../bases/inputs/SearchBase";
import CsvIdentificationRequestRow from "./csvIdentificationRequestRow";

type CsvIdentificationRequestTableProps = object;

const CsvIdentificationRequestTable: FunctionComponent<CsvIdentificationRequestTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);


    const {
        mappedCsvIdentificationRequests: csvIdentificationRequestRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = csvIdentificationRequestViewService.useGetAllCsvIdentificationRequests(
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
        return true;
    };

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };


    return (
        <>
            <InputGroup className="mb-3">
                <SearchBase id="search" label="Search CSV Requests" value={searchTerm} placeholder="Search Csv Identitfation Requests"
                    onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>
            <Container fluid className="infiniteScrollContainer">

                <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>
                    <Table striped bordered hover variant="light">
                        <thead>
                            <tr>
                                <th>Requester Name</th>
                                <th>Requester Email</th>
                                <th>Recipient Name</th>
                                <th>Reason</th>
                                <th>FileName</th>
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
                                    {csvIdentificationRequestRetrieved?.map(
                                        (csvIdentificationRequestView: CsvIdentificationRequestView) => (
                                            <CsvIdentificationRequestRow
                                                key={csvIdentificationRequestView.id.toString()}
                                                csvIdentificationRequest={csvIdentificationRequestView}
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

export default CsvIdentificationRequestTable;