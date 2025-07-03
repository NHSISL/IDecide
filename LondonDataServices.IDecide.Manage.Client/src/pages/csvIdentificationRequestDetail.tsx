import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import CsvIdentificationRequestDetail from "../components/csvIdentificationRequest/csvIdentificationRequestDetail"

export const CsvIdentificationRequestDetailPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="ICSV Identification Request Detail">
                </BreadCrumbBase>
                <div className="mt-3">
                    <CsvIdentificationRequestDetail />
                </div>
            </section>
        </Container>
    )
}