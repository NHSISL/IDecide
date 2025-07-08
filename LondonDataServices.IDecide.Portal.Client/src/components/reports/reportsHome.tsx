import { FunctionComponent, useEffect, useState } from "react";
import { Alert, Button, Card, Col, Container, Dropdown, DropdownButton, Navbar, Row } from "react-bootstrap";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import ReportsReasonPage from "./ReportsReasonPage";
import ReportsLaunchPage from "./reportsLaunchPage";
import axios, { AxiosError } from "axios";
import { AuthenticationResult, InteractionRequiredAuthError, RedirectRequest, SilentRequest } from "@azure/msal-browser";
import { IReportEmbedConfiguration } from "embed";
import ReportDeveloperTools from "./reportDeveloperTools";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { useParams } from "react-router-dom";
import LoginUnAuthorisedComponent from "../layouts/loginUnauth";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";

const ReportsHome: FunctionComponent = () => {
    const { accounts, instance } = useMsal();
    const { reportGroupId, reportId, reportPage, tenantId } = useParams();
    const [toastPostion, setToastPosition] = useState<ToastPosition>("bottom-end")
    const [showDeveloperTools, setShowDeveloperTools] = useState("");
    const [reidReason, setReidReason] = useState("");
    const [isLaunched, setIsLaunched] = useState(false);
    const [reportConfig, setReportConfig] = useState<IReportEmbedConfiguration>();
    const [developerEvents, setDeveloperEvents] = useState<DeveloperEvents[]>([]);
    const [lastEvent, setLastEvent] = useState<DeveloperEvents>();
    const [noAccess, setNoAccess] = useState(false);
    const [toastHidden, setToastHidden] = useState(false);
    const { configuration } = useFrontendConfiguration();
    const [launchError, setlaunchError] = useState("");

    const aquireAccessToken = async () => {
        await instance.initialize();

        let request: SilentRequest = {
            scopes: ["https://analysis.windows.net/powerbi/api/Report.Read.All"],
        };

        if (tenantId) {
            request = {
                ...request,
                authority: "https://login.microsoftonline.com/" + tenantId
            }
        }

        try {
            const token = await instance.acquireTokenSilent(request);
            return token;
        } catch (error) {
            if (error instanceof InteractionRequiredAuthError) {
                // fallback to interaction when silent call fails
                const redirectRequest: RedirectRequest = {
                    ...request,
                    redirectUri: window.location.href
                };

                await instance.acquireTokenRedirect(redirectRequest);
            } else {
                console.log(error);
                throw error; // rethrow the error after logging it
            }
        }
    }

    const aquireReportEmbeddingUrl = async (accessToken: AuthenticationResult) => {
        return axios.get("https://api.powerbi.com/v1.0/myorg/reports/" + reportId,
            {
                headers: {
                    "Authorization": "Bearer " + accessToken.accessToken
                }
            }
        ).then(response => {
            return response.data.embedUrl;
        });
    }

    const refreshToken = async () => {

        const at = await aquireAccessToken();
        if (at) {
            try {
                const reportUrl = await aquireReportEmbeddingUrl(at);
                setReportConfig({
                    type: 'report',
                    embedUrl: reportUrl,
                    accessToken: at.accessToken
                } as IReportEmbedConfiguration);
                setIsLaunched(true);

                if (at.expiresOn) {
                    const refreshMs = at.expiresOn.getTime() - (new Date()).getTime();
                    console.info(`Token refreshed, setting timeout for ${at.expiresOn} - ${refreshMs} ms.`)
                    setTimeout(() => {
                        refreshToken();
                    }, refreshMs)
                }
            } catch (err) {
                const error = err as Error | AxiosError;

                if (axios.isAxiosError(error)) {
                    if (error.response && error.response.status === 401) {
                        setNoAccess(true);
                        return;
                    }
                    if (error.response && error.response.status === 404) {
                        setlaunchError(
                            `Please check the settings you have provided:\n
    - Tenant ID: ${tenantId}\n
    - Group ID: ${reportGroupId}\n
    - Report ID: ${reportId}\n
An error has occurred (${error.response.data.error.code})`
                        );
                        return;
                    }
                }

                throw err;
            }
        }
    }

    const launch = async () => {
        if (reportGroupId == "fake") {
            setReportConfig({
                type: 'fake'
            })
            setIsLaunched(true);
        } else {
            refreshToken();
        }
    }

    useEffect(() => {
        if (lastEvent && showDeveloperTools) {
            setDeveloperEvents((developerEvents) => [...developerEvents, lastEvent]);
        }
    }, [showDeveloperTools, lastEvent])

    const raiseEvent = (event: DeveloperEvents) => {
        setLastEvent(event)
    }

    return (
        <Container className="min-vh-100 d-flex flex-column p-0 m-0" fluid>
            <Row className="m-0">
                <Navbar style={{ padding: 1, backgroundColor: configuration?.bannerColour || "#f8f9fa" }} >
                    <Container fluid>
                        <Navbar.Brand style={{ fontSize: "1em", padding: 0 }}>
                            <Card.Link href="/" style={{ color: "black", textDecoration: "none" }}>
                                LDS Re-Identification Portal
                                {configuration?.environment !== "Live" && <>&nbsp;({configuration?.environment})</>}
                            </Card.Link>
                        </Navbar.Brand>
                        {toastHidden && <Button onClick={() => setToastHidden(false)}>Show reidentification window</Button>}
                        {accounts.length > 0 &&
                            <AuthenticatedTemplate>
                                <Navbar.Text style={{ padding: 1 }}>
                                    <DropdownButton title={accounts[0].username} size="sm">
                                        <Dropdown.Item onClick={() => instance.logout()}>Logout</Dropdown.Item>
                                        <Dropdown.Divider />
                                        <Dropdown.Header>Position Re-id</Dropdown.Header>
                                        <Dropdown.Item onClick={() => { setToastPosition("top-start") }}>Top-Left</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("top-end") }}>Top-Right</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("bottom-end") }}>Bottom-Right</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("bottom-start") }}>Bottom-Left</Dropdown.Item>
                                        {/** <Dropdown.Divider />
                                        <Dropdown.Item onClick={() => { setShowDeveloperTools("end"); }}>Show Developer Tools</Dropdown.Item>*/}
                                    </DropdownButton>
                                </Navbar.Text>
                            </AuthenticatedTemplate>
                        }
                    </Container>
                </Navbar>
            </Row>
            <Row className="flex-grow-1 align-items-center m-0">
                <Col>
                    <UnauthenticatedTemplate>
                        <LoginUnAuthorisedComponent />
                    </UnauthenticatedTemplate>
                    <AuthenticatedTemplate>
                        {!noAccess && !isLaunched && <ReportsReasonPage launchReport={launch} reidReason={reidReason} setReidReason={setReidReason} launchError={launchError} />}


                        {noAccess && accounts.length && <Card>
                            <Card.Header>No Access </Card.Header>
                            <Card.Body>
                                <Alert variant="info" className="mb-0">
                                    <p>You do not have access to this report with the account:</p>
                                    <p>{accounts[0].username} ({accounts[0].name}).</p>
                                    <p>Please contact <a href="mailto:isl.support@nhs.net">isl.support@nhs.net</a> for access.</p>
                                </Alert>
                                <Button onClick={() => { instance.logout(); setNoAccess(true); }}>Logout</Button>
                            </Card.Body>
                        </Card>}
                        {isLaunched && reportConfig &&
                            <ReportsLaunchPage
                                reportConfig={reportConfig}
                                addDeveloperEvent={raiseEvent}
                                toastPostion={toastPostion}
                                activePage={reportPage}
                                toastHidden={toastHidden}
                                reidReason={reidReason}
                                hideToast={() => setToastHidden(true)}
                            />
                        }
                    </AuthenticatedTemplate>
                </Col>
            </Row>
            <ReportDeveloperTools
                developerToolsLocation={showDeveloperTools}
                setDeveloperToolsLocation={setShowDeveloperTools}
                eventsList={developerEvents} />
        </Container>
    )
}

export default ReportsHome