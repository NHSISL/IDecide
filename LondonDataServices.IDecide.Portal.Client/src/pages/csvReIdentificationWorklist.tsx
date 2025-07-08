import { Container } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import CsvIdentificationWorklistTable from "../components/csvIdentificationWorklist/csvIdentificationWorklistTable";

export const CsvReIdentificationWorklistPage = () => {

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/"
                    backLink="Home"
                    currentLink="CSV Worklist">
                </BreadCrumbBase>
                <div className="mt-3">
                    <CsvIdentificationWorklistTable />
                </div>
            </section>
        </Container>
    )
}