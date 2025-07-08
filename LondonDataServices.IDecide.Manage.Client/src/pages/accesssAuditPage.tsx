import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import AccessAuditTable from "../components/accessAudits/accessAuditTable"

export const AccessAuditPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="Access Audit">
                </BreadCrumbBase>
                <div className="mt-3">
                    <AccessAuditTable />
                </div>
            </section>
        </Container>
    )
}