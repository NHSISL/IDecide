import { Container } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import CsvReIdentificationDetail from "../components/csvReIdentification/csvReIdentificationDetail";

export const CsvReIdentificationPage = () => {

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/"
                    backLink="Home"
                    currentLink="Dataset Re-Identification">
                </BreadCrumbBase>
                <div className="mt-3">
                    <CsvReIdentificationDetail />
                </div>
            </section>
        </Container>
    )
}