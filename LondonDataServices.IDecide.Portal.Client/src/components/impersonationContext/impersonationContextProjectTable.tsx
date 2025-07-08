import { debounce } from "lodash";
import { FunctionComponent, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Button, Container, Table, InputGroup } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faRefresh } from "@fortawesome/free-solid-svg-icons";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/ImpersonationContextView";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { impersonationContextViewService } from "../../services/views/impersonationContext/impersonationContextViewService";
import SearchBase from "../bases/inputs/SearchBase";
import ImpersonationContextProjectRow from "./impersonationContextProjectRow";

type ImpersonationContextProjectTableProps = object;

const ImpersonationContextProjectTable: FunctionComponent<ImpersonationContextProjectTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);
    const navigate = useNavigate();

    const {
        mappedImpersonationContexts: impersonationContextRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
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
    };

    const handleAddProjectClick = () => {
        navigate("/addProject");
    };

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };

    return (
        <div className="projectTableView">
            <Container className="mt-4">
                <InputGroup className="mb-3">
                    <SearchBase id="search" label="Search lookups" value={searchTerm} placeholder="Search Projects"
                        onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />

                    <Button variant="outline-secondary" onClick={handleRefresh}>
                        <FontAwesomeIcon icon={faRefresh} />
                    </Button>
                </InputGroup>
                <Button variant="success" className="mb-3" onClick={handleAddProjectClick}>
                    <FontAwesomeIcon icon={faPlus} /> Add Project
                </Button>
            </Container>
            <Container className="infiniteScrollContainer">
                {showSpinner ? (
                    <SpinnerBase />
                ) : (
                    impersonationContextRetrieved && impersonationContextRetrieved.length > 0 && (
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>
                            <Table striped bordered hover variant="light">
                                <thead>
                                    <tr>
                                        <th>Project</th>
                                        <th>Status</th>
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
                                                    <ImpersonationContextProjectRow
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
                    )
                )}
            </Container>
        </div>
    );
};

export default ImpersonationContextProjectTable;