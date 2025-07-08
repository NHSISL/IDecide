import { Button, Card, CardBody, CardFooter, CardHeader, Col, Container, Row, Spinner } from "react-bootstrap"
import { Link, useNavigate, useParams } from "react-router-dom"
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase"
import OdsTree from "../odsData/odsTree";
import { useEffect, useState } from "react";
import { OdsData } from "../../models/odsData/odsData";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { UserAccess } from "../../models/userAccess/userAccess";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { userAccessService } from "../../services/foundations/userAccessService";
import { toastError } from "../../brokers/toastBroker.error";
import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import OdsSearch from "../odsData/odsSearch";

export const UserAccessEdit = () => {
    const { UserId } = useParams();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const [selectedOrganisation, setSelectedOrganisation] = useState<OdsData | undefined>();
    const { data, isLoading: isUserAccessLoading } = userAccessViewService.useGetAccessForUser(UserId);
    const [selectedUser, setSelectedUser] = useState<UserAccess>();
    const [odsSearchString, setOdsSearchString] = useState("");
    const { data: rootRecord, isLoading: isOdsDataLoading } = odsDataService.useRetrieveAllOdsData(odsSearchString);
    const { mutateAsync: createUserAccess } = userAccessService.useCreateUserAccess();
    const { mutateAsync: deleteUserAccess } = userAccessService.useRemoveUserAccess();
    const [searchString, setSearchString] = useState(`?filter=OrganisationCode eq 'Root'`);
    const { data: odsRoot } = odsDataService.useRetrieveAllOdsData(searchString);
    const [rootId, setRootId] = useState("");

    const navigate = useNavigate();

    useEffect(() => {
        if (odsRoot) {
            setRootId(odsRoot[0].id);
        }
    }, [odsRoot]);

    useEffect(() => {
        if (!selectedOrganisation) {
            setSearchString(`?filter=OrganisationCode eq 'Root'`);
        } else {
            setSearchString(`?filter=OrganisationCode eq '${selectedOrganisation?.organisationCode}'`);
        }
    }, [selectedOrganisation])

    useEffect(() => {
        if (data && data?.length) {
            setSelectedUser(data[0]);
            const orgcodes = data.map(d => d.orgCode);
            setOdsSearchString(`?filter=OrganisationCode in (${orgcodes.map(x => `'${x}'`).join(',')})`)
        }
    }, [data])

    useEffect(() => {
        if (rootRecord) {
            setSelectedOdsRecords(rootRecord)
        }
    }, [rootRecord])

    const saveRecord = async () => {
        if (selectedUser) {
            const ua = {
                ...new UserAccess(),
                ...selectedUser,
                UserId: selectedUser.UserId,
                email: selectedUser.email,
                activeFrom: new Date(),
                orgCodes: selectedOdsRecords.map(x => x.organisationCode)
            }
            try {
                await createUserAccess(ua);
                navigate("/userAccess");
            } catch (error) {
                if (error instanceof Error) {
                    toastError(error.message);
                }
            }
        }
    }

    const deleteUser = async () => {
        if (data) {
            await data.forEach(async (ua) => {
                await deleteUserAccess(ua.id);
            });

            navigate("/userAccess");
        }
    }

    const removeOdsCode = (odsRecord: OdsData) => {
        setSelectedOdsRecords([...selectedOdsRecords.filter(o => o.organisationCode != odsRecord.organisationCode)])
    }

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/userAccess"
                    backLink="User Access"
                    currentLink="Edit User">
                </BreadCrumbBase>
                <div className="mt-3">
                    {isUserAccessLoading || isOdsDataLoading ? (
                        <Spinner animation="border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </Spinner>
                    ) : (
                        <Card>
                            <CardHeader>
                                Edit Account For:
                            </CardHeader>
                            <CardBody>
                                {selectedUser && <>
                                    <div>display Name: {selectedUser.displayName}</div>
                                    <div>Job Title: {selectedUser.jobTitle}</div>
                                    <div>Mail: {selectedUser.email}</div>
                                    <div>UPN: {selectedUser.userPrincipalName}</div>
                                    {selectedOrganisation && <>{selectedOrganisation.organisationCode} </>}
                                    <div style={{ paddingTop: "10px" }}>
                                            <Row>
                                                <Col xl={3} className="mt-3">
                                                <Card>
                                                    <CardHeader>Search</CardHeader>
                                                        <CardBody>
                                                            <OdsSearch selectedRecords={selectedOdsRecords} selectedOrganisation={selectedOrganisation} setSelectedOrganisation={(organisation: OdsData | undefined) => { setSelectedOrganisation(organisation) }} />
                                                    </CardBody>
                                                </Card>
                                            </Col>
                                                <Col xl={6} className="mt-3">
                                                <Card>
                                                    <CardHeader>
                                                        Organisations:
                                                        </CardHeader>
                                                        <CardBody className="text-nowrap">
                                                            {rootId &&
                                                                <OdsTree readonly={false} rootId={rootId} selectedRecords={selectedOdsRecords} setSelectedRecords={setSelectedOdsRecords} showRoot={selectedOrganisation !== undefined} />
                                                        }
                                                    </CardBody>
                                                </Card>
                                            </Col>
                                                <Col xl={3} className="mt-3">
                                                <Card>
                                                    <CardHeader>
                                                        Selected Records:
                                                    </CardHeader>
                                                    <CardBody>
                                                        {selectedOdsRecords.length === 0 && <div>none</div>}
                                                        {selectedOdsRecords.map(r => <div>
                                                            <FontAwesomeIcon icon={faTimes} color="red" onClick={() => removeOdsCode(r)} />
                                                            &nbsp;
                                                            <span>{r.organisationName} ({r.organisationCode})</span>
                                                        </div>
                                                        )}

                                                    </CardBody>
                                                </Card>
                                            </Col>
                                        </Row>
                                    </div>
                                </>
                                }
                            </CardBody>

                            <CardFooter>
                                {selectedOdsRecords.length === 0 ?
                                    <Button onClick={deleteUser}>Remove User</Button>
                                    :
                                    <Button onClick={saveRecord}>Save</Button>
                                }

                                <Link to="/userAccess" style={{ paddingLeft: '10px' }}>
                                    <Button variant="secondary">Cancel</Button>
                                </Link>
                            </CardFooter>
                        </Card>
                    )}
                </div>
            </section>
        </Container>
    )
}