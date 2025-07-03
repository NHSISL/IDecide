import { Button, ButtonGroup, Card, CardBody, CardFooter, CardHeader, Col, Container, Row, Spinner } from "react-bootstrap"
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase"
import UserSearch from "../UserSearch/UserSearch"
import { useEffect, useState } from "react"
import { User } from "../../models/views/components/Users/Users"
import OdsTree from "../odsData/odsTree"
import { OdsData } from "../../models/odsData/odsData"
import { userAccessService } from "../../services/foundations/userAccessService"
import { UserAccess } from "../../models/userAccess/userAccess"
import { useNavigate } from "react-router-dom"
import { toastError } from "../../brokers/toastBroker.error"
import { odsDataService } from "../../services/foundations/odsDataAccessService"
import { faTimes } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import OdsSearch from "../odsData/odsSearch"

export const UserAccessNew = () => {

    const [selectedUser, setSelectedUser] = useState<User | undefined>();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const navigate = useNavigate();
    const { mutateAsync, isPending, error } = userAccessService.useCreateUserAccess();
    const [selectedOrganisation, setSelectedOrganisation] = useState<OdsData | undefined>();
    const [searchString, setSearchString] = useState(`?filter=OrganisationCode eq 'Root'`);
    const { data: odsRoot } = odsDataService.useRetrieveAllOdsData(searchString);
    const [rootId, setRootId] = useState("");

    useEffect(() => {
        if (!selectedOrganisation) {
            setSearchString(`?filter=OrganisationCode eq 'Root'`);
        } else {
            setSearchString(`?filter=OrganisationCode eq '${selectedOrganisation?.organisationCode}'`);
        }
    }, [selectedOrganisation])

    useEffect(() => {
        if (odsRoot) {
            setRootId(odsRoot[0].id);
        }
    }, [odsRoot]);

    const saveRecord = async () => {
        let ua : UserAccess;

        if(selectedUser){
            ua = {...new UserAccess(),                 
                ...selectedUser,
                UserId: selectedUser.id,
                email: selectedUser.mail,
                activeFrom: new Date(),
                orgCodes: selectedOdsRecords.map(x => x.organisationCode)
            }
            try {
                await mutateAsync(ua);
                navigate("/userAccess");
            } catch(error) {
                if(error instanceof Error) {
                    toastError(error.message);
                    console.log(error);
                }
            }
            
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
                    currentLink="New User">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h1>New User Access</h1>
                    {!selectedUser ? 
                        <UserSearch selectUser={(User) => { setSelectedUser(User)}} />
                    : 
                        <Card>
                            <CardHeader>
                                Create Account For:
                            </CardHeader>
                            <CardBody>
                                <div>display Name: {selectedUser.displayName}</div>
                                <div>Job Title: {selectedUser.jobTitle}</div>
                                <div>Mail: {selectedUser.mail}</div>
                                <div>UPN: {selectedUser.userPrincipalName}</div>
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
                            </CardBody>
                            
                            <CardFooter>
                                <ButtonGroup>
                                    <Button onClick={saveRecord} disabled={isPending}>
                                        { isPending ? <Spinner /> : "Save"}
                                    </Button>
                                    <Button onClick={() => {
                                        setSelectedUser(undefined)
                                        setSelectedOdsRecords([]);
                                    } } variant="secondary">Clear</Button>
                                </ButtonGroup>
                                {error && <div className="bs-warning">{JSON.stringify(error.message)}</div>}
                            </CardFooter>
                        </Card>
                    }
                </div>
            </section>
        </Container>
    )
}