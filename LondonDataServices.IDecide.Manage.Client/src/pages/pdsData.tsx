import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import PdsTable from "../components/pdsData/pdsTable"

export const PdsData = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="Pds Data">
                </BreadCrumbBase>
                <div className="mt-3">
                    <PdsTable />
                </div>
            </section>
        </Container>
    )
}