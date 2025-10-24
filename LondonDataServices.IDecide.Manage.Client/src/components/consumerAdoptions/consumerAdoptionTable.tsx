import React from "react";
import { Card, Container, Table } from "react-bootstrap";
import { ConsumerAdoption } from "../../models/consumerAdoptions/consumerAdoption";
import { useEffect, useState } from "react";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import ConsumerAdoptionRow from "./consumerAdoptionRow";
import { consumerAdoptionViewService } from "../../services/views/consumerAdoptionViewService";

type Page = {
    data: ConsumerAdoption[];
};

type ConsumerAdoptionTableProps = {
    decisionId?: string;
};

const ConsumerAdoptionTable = ({ decisionId }: ConsumerAdoptionTableProps) => {
    if (!decisionId) {
        return null;
    }

    const [showSpinner] = useState(false);
    const [mappedConsumerAdoption, setMappedConsumerAdoption] = useState<Array<ConsumerAdoption>>([]);
    const [totalPages, setTotalPages] = useState<number>(0);

    const {
        pages,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage
    } = consumerAdoptionViewService.useGetAllConsumerAdoptions(decisionId);

    useEffect(() => {
        if (pages && Array.isArray(pages)) {
            const allData = extractAllData(pages);
            setMappedConsumerAdoption(allData);
            updateTotalPages(pages, allData);
        }
    }, [pages]);

    const extractAllData = (pages: Page[]): ConsumerAdoption[] => {
        return pages.flatMap(page => {
            if (Array.isArray(page.data)) {
                return page.data;
            } else {
                return [];
            }
        });
    };

    const updateTotalPages = (pages: Page[], uniqueAccessAudit: Array<ConsumerAdoption>) => {
        const itemsPerPage = pages[0]?.data.length || 1;
        const totalItems = uniqueAccessAudit.length;
        setTotalPages(Math.ceil(totalItems / itemsPerPage));
    };

    const hasNoMorePages = (): boolean => {
        return !isLoading && !hasNextPage;
    };

    return (
        <>
            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header>
                        <span style={{ fontSize: "30px" }}>Consumer Adoptions</span>
                    </Card.Header>
                    <Card.Body>
                        <InfiniteScroll
                            loading={isLoading || showSpinner}
                            hasNextPage={hasNextPage || false}
                            loadMore={fetchNextPage}
                        >
                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th className="text-center">Consumer</th>
                                        <th className="text-center">Decison</th>
                                        <th className="text-center">Decison Datetime</th>
                                        <th className="text-center">Adoption DateTime</th>
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
                                            {(
                                                mappedConsumerAdoption && mappedConsumerAdoption.map(
                                                    (consumerAdoption: ConsumerAdoption) => (
                                                        <ConsumerAdoptionRow
                                                            key={consumerAdoption.id}
                                                            consumerAdoption={consumerAdoption}
                                                        />
                                                    )
                                                )
                                            )}
                                            <tr>
                                                <td colSpan={6} className="text-center">
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

export default ConsumerAdoptionTable;