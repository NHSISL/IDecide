import { Card, Container, Table, Button } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import { Consumer } from "../../models/consumers/consumer";
import { useEffect, useMemo, useState } from "react";
import { consumerViewService } from "../../services/views/consumerViewService";
import { debounce } from "lodash";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import ConsumerRow from "./consumerRow";
import ConsumerRowEdit from "./consumerRowEdit";
import SearchBase from "../bases/search/SearchBase";

type Page = {
    data: Consumer[];
};

const ConsumerTable = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);
    const [mappedConsumer, setMappedConsumer] = useState<Array<Consumer>>([]);
    const [totalPages, setTotalPages] = useState<number>(0);
    const [addConsumerClicked, setAddConsumerClicked] = useState<boolean>(false);
    const [editConsumer, setEditConsumer] = useState<Consumer | null>(null);

    const {
        pages,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = consumerViewService.useGetAllConsumers(debouncedTerm);

    useEffect(() => {
        if (pages && Array.isArray(pages)) {
            const allData = extractAllData(pages);
            setMappedConsumer(allData);
            updateTotalPages(pages, allData);
        }
    }, [pages]);

    const extractAllData = (pages: Page[]): Consumer[] => {
        return pages.flatMap(page => {
            if (Array.isArray(page.data)) {
                return page.data;
            } else {
                return [];
            }
        });
    };

    const updateTotalPages = (pages: Page[], uniqueAccessAudit: Array<Consumer>) => {
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

    const handleAddConsumer = () => {
        setAddConsumerClicked(true);
    };

    const handleCancelAddConsumer = () => {
        setAddConsumerClicked(false);
    };

    const handleEditConsumer = (consumer: Consumer) => {
        setEditConsumer(consumer);
    };

    const handleCancelEditConsumer = () => {
        setEditConsumer(null);
    };

    const addConsumer = consumerViewService.useCreatePatientConsumer();
    const handleAddConsumerSave = (consumer: Consumer) => {
        alert("Pow");
        addConsumer.mutate(consumer, {
            onSuccess: () => {
                setAddConsumerClicked(false);
                refetch();
            }
        });
    };

    const updateConsumer = consumerViewService.useUpdateConsumer();
    const handleSaveEditConsumer = async (consumer: Consumer) => {
        if (consumer.id) {
            updateConsumer.mutate(consumer, {
                onSuccess: () => {
                    setEditConsumer(null);
                    refetch();
                }
            });
        }
    };

    const deleteConsumer = consumerViewService.useRemoveConsumer();
    const handleDeleteConsumer = (id: string) => {
        alert(`Delete Consumer with ID: ${id}`);
        deleteConsumer.mutateAsync(id);
        refetch();
    };

    return (
        <>
            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header>
                        <FontAwesomeIcon icon={faDatabase} className="me-2" />
                        Consumers
                        <Button
                            variant="primary"
                            className="float-end"
                            onClick={handleAddConsumer}
                        >
                            Add Consumer
                        </Button>
                    </Card.Header>
                    <Card.Body>
                        <SearchBase
                            id="search"
                            label="Search Consumers"
                            value={searchTerm}
                            placeholder="Search Consumers"
                            onChange={(e) => { handleSearchChange(e.currentTarget.value); }}
                        />
                        <br />
                        {/* Show ConsumerRow in Add mode if Add Consumer was clicked */}
                        {addConsumerClicked && (
                            <ConsumerRow
                                action="add"
                                onCancel={handleCancelAddConsumer}
                                onSave={handleAddConsumerSave}
                            />
                        )}
                        <InfiniteScroll
                            loading={isLoading || showSpinner}
                            hasNextPage={hasNextPage || false}
                            loadMore={fetchNextPage}
                        >
                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Contact Email</th>
                                        <th>Contact Number</th>
                                        <th>Contact Name</th>
                                        <th>Entra Id</th>
                                        <th className="text-center">Actions</th>
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
                                            {editConsumer ? (
                                                <tr>
                                                    <td colSpan={6}>
                                                        <ConsumerRowEdit
                                                            consumer={editConsumer}
                                                            onCancel={handleCancelEditConsumer}
                                                            onSave={handleSaveEditConsumer}
                                                        />
                                                    </td>
                                                </tr>
                                            ) : (
                                                mappedConsumer && mappedConsumer.map(
                                                    (consumer: Consumer) => (
                                                        <ConsumerRow
                                                            key={consumer.id}
                                                            consumer={consumer}
                                                            action="view"
                                                            onEdit={() => handleEditConsumer(consumer)}
                                                            onDelete={handleDeleteConsumer}
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

export default ConsumerTable;