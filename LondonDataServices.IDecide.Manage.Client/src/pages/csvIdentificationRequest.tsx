import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import CsvIdentificationRequestTable from "../components/csvIdentificationRequest/csvIdentificationRequestTable"

export const CsvIdentificationRequestPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="CSV Identification Request">
                </BreadCrumbBase>
                <div className="mt-3">
                    <CsvIdentificationRequestTable />
                </div>
            </section>
        </Container>
    )
}