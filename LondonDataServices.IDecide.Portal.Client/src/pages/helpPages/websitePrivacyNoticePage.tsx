import { Card, Col, Container, ListGroup, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

const WebsitePrivacyNoticePage = () => {
    const navigate = useNavigate();
    const { t: translate } = useTranslation();

    return (
        <>
            <Container style={{ padding: 20 }}>
                <button
                    className="nhsuk-back-link mt-4"
                    type="button"
                    onClick={() => navigate(-1)}
                >
                    <span className="nhsuk-back-link__arrow" aria-hidden="true">&#8592;</span>
                    {translate("websitePrivacyNotice.back", "Back")}
                </button>
                <Row className="justify-content-center">
                    <Col md={10} lg={8}>
                        <Card>
                            <Card.Body>
                                <Card.Title as="h1">
                                    {translate("websitePrivacyNotice.title", "Website Privacy Notice")}
                                </Card.Title>
                                <Card.Text>
                                    {translate(
                                        "websitePrivacyNotice.intro",
                                        "This privacy notice explains how we collect, use and protect the personal information you provide when you use this website. It is written for patients and members of the public and follows clear, plain language so you can quickly understand what we do with your data."
                                    )}
                                </Card.Text>

                                <h2 className="h5">
                                    {translate("websitePrivacyNotice.whoWeAreTitle", "Who we are")}
                                </h2>
                                <p>
                                    {translate(
                                        "websitePrivacyNotice.whoWeAre",
                                        "The ONE London Opt In / Opt Out portal is provided on behalf of NHS organisations across London. The portal enables patients to exercise their right to choose how their confidential information is used for purposes beyond their individual care. The service is managed by the London Data Service (LDS) in partnership with NHS London Integrated Care Boards."
                                    )}
                                </p>

                                <h2 className="h5">
                                    {translate("websitePrivacyNotice.whatWeCollectTitle", "What information we collect")}
                                </h2>
                                <ListGroup>
                                    <ListGroup.Item>
                                        <strong>
                                            {translate("websitePrivacyNotice.personalDetailsLabel", "Personal details:")}
                                        </strong>
                                        {translate(
                                            "websitePrivacyNotice.personalDetails",
                                            " Information such as your NHS number, name, date of birth, and postcode are collected to verify your identity and apply your preference correctly."
                                        )}
                                    </ListGroup.Item>
                                </ListGroup>

                                <h2 className="h5 mt-4">
                                    {translate("websitePrivacyNotice.howWeUseTitle", "How we use your information")}
                                </h2>
                                <p>
                                    {translate(
                                        "websitePrivacyNotice.howWeUseIntro",
                                        "The information you provide is used solely to:"
                                    )}
                                </p>
                                <ul>
                                    <li>
                                        {translate(
                                            "websitePrivacyNotice.howWeUse1",
                                            "verify your identity to process your opt-in or opt-out request,"
                                        )}
                                    </li>
                                    <li>
                                        {translate(
                                            "websitePrivacyNotice.howWeUse2",
                                            "record and maintain your current data sharing preference,"
                                        )}
                                    </li>
                                    <li>
                                        {translate(
                                            "websitePrivacyNotice.howWeUse3",
                                            "support auditing and compliance with NHS information governance standards,"
                                        )}
                                    </li>
                                    <li>
                                        {translate(
                                            "websitePrivacyNotice.howWeUse4",
                                            "ensure the website is secure and functioning properly."
                                        )}
                                    </li>
                                </ul>

                                <h2 className="h5">
                                    {translate("websitePrivacyNotice.updatesTitle", "Updates to this notice")}
                                </h2>
                                <p>
                                    {translate(
                                        "websitePrivacyNotice.updates",
                                        "This notice may be updated from time to time to reflect changes in legislation or the way the service operates. The latest version will always be available on this page."
                                    )}
                                </p>

                                <h2 className="h5 mt-4">
                                    {translate("websitePrivacyNotice.nhsLoginTitle", "NHS Login")}
                                </h2>
                                <p>
                                    {translate(
                                        "websitePrivacyNotice.nhsLoginIntro",
                                        "If you access our service using NHS England services (such as NHS login), please note the following:"
                                    )}
                                </p>
                                <p>
                                    {translate(
                                        "websitePrivacyNotice.nhsLoginDetails.part1",
                                        "Please note that if you access our service using your NHS login details, the identity verification services are managed by NHS England. NHS England is the controller for any personal information you provided to NHS England to get an NHS login account and verify your identity, and uses that personal information solely for that single purpose. For this personal information, our role is a “processor” only and we must act under the instructions provided by NHS England (as the “controller”) when verifying your identity. To see NHS login’s Privacy Notice and Terms and Conditions, please "
                                    )}
                                    <a
                                        href="https://www.nhs.uk/nhs-services/online-services/nhs-login/privacy-notice/"
                                        target="_blank"
                                        rel="noopener noreferrer"
                                    >
                                        {translate("websitePrivacyNotice.nhsLoginDetails.linkText", "click here")}
                                    </a>
                                    {translate(
                                        "websitePrivacyNotice.nhsLoginDetails.part2",
                                        ". This restriction does not apply to the personal information you provide to us separately."
                                    )}
                                </p>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </>
    );
};

export default WebsitePrivacyNoticePage;