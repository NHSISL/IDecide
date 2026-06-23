import { Container, Row, Col } from "react-bootstrap";
import { useTranslation } from "react-i18next";

export const Home = () => {

    const { t: translate } = useTranslation();
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <h3>The London Data Service Opt Out Management Portal</h3>
                    <p>
                        The London Data Service securely collects patient information from a range of healthcare locations around
                        London such as GP surgeries and hospitals.
                        It then organises and stores this information ready to be used by other approved systems.
                    </p>

                    <h1 style={{ fontSize: "1.7rem", marginBottom: "0.7rem" }}>
                        {translate("homepage.title")}
                    </h1>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro1")}</p>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro2")}</p>
                    <p style={{ marginBottom: "0.5rem" }}>{translate("homepage.intro3")}</p>
                    
                </Col>
            </Row>
        </Container>
    );
}