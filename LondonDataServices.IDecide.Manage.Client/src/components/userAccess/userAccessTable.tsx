import { FunctionComponent, useMemo, useState } from "react";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Alert, Button, Card, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleExclamation, faDatabase, faRefresh } from "@fortawesome/free-solid-svg-icons";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { Link } from "react-router-dom";
import securityPoints from "../../securityMatrix";
import { SecuredComponent } from "../securitys/securedComponents";
import SearchBase from "../bases/search/SearchBase";
import { debounce } from "lodash";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import moment from "moment";

type UserAccessTableProps = object;

const UserAccessTable: FunctionComponent<UserAccessTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);

    const {
        mappedUserAccess: userAccessRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = userAccessViewService.useGetDistinctUsersOdata(debouncedTerm);

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
                <SearchBase id="search" label="Search ods" value={searchTerm} placeholder="Search User Table"
                    onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>


            <Card>
                <Card.Header>
                    <div className="d-flex justify-content-between align-items-center">
                        <SecuredComponent allowedRoles={securityPoints.userAccess.add}>
                            <Link to="/userAccess/newUser">
                                <Button><FontAwesomeIcon icon={faDatabase} className="me-2" /> Add New User</Button>
                            </Link>
                        </SecuredComponent>
                        <Alert variant="danger" className="p-2 ms-3 mb-0"><FontAwesomeIcon icon={faCircleExclamation} size="lg" />&nbsp; Any modification to data on this screen is audited.</Alert>
                    </div>
                </Card.Header>
                <Card.Body>
                    <Container fluid className="infiniteScrollContainer">
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>
                            <Table striped bordered hover variant="light">
                                <thead>
                                    <tr>
                                        <th>Display Name</th>
                                        <th>Email</th>
                                        <th>Date Created</th>
                                        <th className="text-center">Action(s)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {isLoading || showSpinner ? (
                                        <tr>
                                            <td colSpan={3} className="text-center">
                                                <SpinnerBase />
                                            </td>
                                        </tr>
                                    ) : (
                                            <>
                                               
                                            {userAccessRetrieved && userAccessRetrieved?.map(d => <tr key={d.id}>
                                                <td>{d.displayName}</td>
                                                <td>{d.email}</td>
                                                <td>{moment(d.createdDate?.toString()).format("Do-MMM-yyyy")}</td>
                                                <td className="text-center">
                                                    <SecuredComponent allowedRoles={securityPoints.userAccess.edit}>
                                                        <Link to={`/userAccess/${d.UserId}`} >
                                                            <Button size="sm">
                                                                Edit Access
                                                            </Button>
                                                        </Link>
                                                    </SecuredComponent>
                                                </td>
                                            </tr>)}

                                            <tr>
                                                <td colSpan={4} className="text-center">
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
                </Card.Body>
            </Card>

        </>
    );
};

export default UserAccessTable;