import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import OdsTable from "../components/odsData/odsTable"

export const OdsData = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="ODS Data">
                </BreadCrumbBase>
                <div className="mt-3">
                    <OdsTable />
                </div>
            </section>
        </Container>
    )
}