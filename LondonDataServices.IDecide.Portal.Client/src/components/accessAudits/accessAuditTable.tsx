import { FunctionComponent, useMemo, useState, useEffect } from "react";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AccessAuditRow from "./accessAuditRow";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { AccessAudit } from "../../models/accessAudit/accessAudit";
import { accessAuditViewService } from "../../services/views/accessAudit/accessAuditViewService";

type AccessAuditTableProps = {
    requestId: string;
};

type Page = {
    data: AccessAudit[];
};

const AccessAuditTable: FunctionComponent<AccessAuditTableProps> = ({ requestId }) => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);
    const [mappedAccessAudit, setMappedAccessAudit] = useState<Array<AccessAudit & { count: number, okCount: number, notOkCount: number }>>([]);
    const [totalPages, setTotalPages] = useState<number>(0);

    const {
        pages,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = accessAuditViewService.useGetAllAccessAuditByRequestId(
        debouncedTerm, requestId
    );

    useEffect(() => {
        refetch();
    }, [requestId, refetch]);

    useEffect(() => {
        if (pages && Array.isArray(pages)) {
            const allData = extractAllData(pages);
            const filteredData = filterData(allData);
            const groupedData = groupDataByTransactionId(filteredData);
            const uniqueAccessAudit = mapToUniqueAccessAudit(groupedData);

            setMappedAccessAudit(uniqueAccessAudit);
            updateTotalPages(pages, uniqueAccessAudit);
        }
    }, [pages]);

    const extractAllData = (pages: Page[]): AccessAudit[] => {
        return pages.flatMap(page => {
            if (Array.isArray(page.data)) {
                return page.data;
            } else {
                return [];
            }
        });
    };

    const filterData = (data: AccessAudit[]): AccessAudit[] => {
        return data.filter(audit => audit.transactionId !== "00000000-0000-0000-0000-000000000000");
    };

    const groupDataByTransactionId = (data: AccessAudit[]): { [key: string]: AccessAudit[] } => {
        return data.reduce((acc, audit) => {
            const transactionIdKey = audit.transactionId;

            if (!acc[transactionIdKey]) {
                acc[transactionIdKey] = [];
            }

            acc[transactionIdKey].push(audit);
            return acc;
        }, {} as { [key: string]: AccessAudit[] });
    };

    const mapToUniqueAccessAudit = (groupedData: { [key: string]: AccessAudit[] }): Array<AccessAudit & { count: number, okCount: number, notOkCount: number }> => {
        return Object.values(groupedData).map(group => {
            const okCount = group.filter(audit => audit.hasAccess === true && audit.auditType === 'NECS Access').length;
            const notOkCount = group.filter(audit => audit.hasAccess === false && audit.auditType === 'PDS Access').length;

            return {
                ...group[0],
                count: group.length,
                okCount,
                notOkCount
            };
        });
    };

    const updateTotalPages = (pages: Page[], uniqueAccessAudit: Array<AccessAudit & { count: number, okCount: number, notOkCount: number }>) => {
        const itemsPerPage = pages[0]?.data.length || 1;
        const totalItems = uniqueAccessAudit.length;
        setTotalPages(Math.ceil(totalItems / itemsPerPage));
    };

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

    const hasNoMorePages = (): boolean => {
        return !isLoading && !hasNextPage;
    };

    return (
        <>
            <br /> <br />
            <Container className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" />Download History</Card.Header>
                    <Card.Body>

                        <SearchBase id="search" label="Search accessAudit" value={searchTerm} placeholder="Search Download History"
                            onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                        <br />

                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Downloaded Date</th>
                                        <th>Counts</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading || showSpinner ? (
                                        <tr>
                                            <td colSpan={4} className="text-center">
                                                <SpinnerBase />
                                            </td>
                                        </tr>
                                    ) : (
                                        <>
                                            {mappedAccessAudit && mappedAccessAudit.map(
                                                (accessAudit: AccessAudit & { count: number, okCount: number, notOkCount: number }) => (
                                                    <AccessAuditRow
                                                        key={accessAudit.id}
                                                        accessAudit={accessAudit}
                                                        okCount={accessAudit.okCount}
                                                        notOkCount={accessAudit.notOkCount}
                                                    />
                                                )
                                            )}
                                            <tr>
                                                <td colSpan={4} className="text-center">
                                                    <InfiniteScrollLoader
                                                        loading={isFetchingNextPage}
                                                        spinner={<SpinnerBase />}
                                                        noMorePages={!hasNoMorePages()}
                                                        noMorePagesMessage={<>-- No more pages --</>}
                                                        totalPages={totalPages}
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

export default AccessAuditTable;