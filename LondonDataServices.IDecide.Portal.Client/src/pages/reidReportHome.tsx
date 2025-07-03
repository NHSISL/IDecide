import { Container, Row, Col } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import ReportHelper from "../components/reports/reportHelper";

export const ReIdReportHome = () => {
    return (
        <>
            <Container fluid className="mt-4">
                <section>
                    <BreadCrumbBase
                        link="/"
                        backLink="Home"
                        currentLink="Reports">
                    </BreadCrumbBase>

                    <Row className="mt-3">
                        <Col md={6} className="col1">
                            <ReportHelper />
                        </Col>
                        <Col md={6} className="col2">

                            <div className="bg-light p-3">
                                <h5>Report URL Generator Help</h5>
                                <p>The One London re-identification portal provides 3 types of integration into to BI tools:</p>
                                <ol>
                                    <li>Seemless integration to re-identify one patient within Power BI.</li>
                                    <li>Seemless integration to re-identify a group of patients within Power BI.</li>
                                    <li>A popup web page that can be integrated into any web based BI tool that can re-identify a single patient.</li>
                                </ol>
                                <p>This page is intended for Analysts and provides instructions and tools to enable this integration.</p>

                                <h5>Power BI Single Patient Re-identification</h5>
                                <p> For Power BI single re-identification, check each step below for information on how to complete the setup:</p>
                                <ol>
                                    <li>Prepare the report dataset for re-identfication by including the Pseudo identifier.</li>
                                    <li>Publish the report to Power BI server and extract the report identifers.</li>
                                    <li>Generate unique link to re-identification portal for report.</li>
                                </ol>

                                <h5>Power BI Multiple Patient Re-identification </h5>
                                <p>For Power BI multiple re-identification, check each step below for information on how to complete the setup:</p>
                                <ol>
                                    <li>Create a measure on Power Bi that includes a comma seperated list of Pseudo identifers.</li>
                                    <li>Publish the report to Power BI server and extract the report identifers.</li>
                                    <li>Generate unique link to re-identification portal for report.</li>
                                </ol>

                                <h5>Re-identification Popup</h5>
                                <p>The re-identification service provides a URL that can be direct linked to from Power BI or
                                    Tableau this will provide a single record re-identification, this can either be launched
                                    in a new tab or hosted within a webview or iframe within the BI tool:</p>
                                <ol>
                                    <li>Understand how to construct the re-identification URL</li>
                                    <li>Power BI generate a weblink</li>
                                    <li>Tableau generate a weblink</li>
                                </ol>
                            </div>
                        </Col>
                    </Row>
                </section>
            </Container>
        </>
    )
}