import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import ImpersonationContextTable from "../components/impersonationContext/impersonationContextTable"

export const ImpersonationContext = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="Impersonation Context">
                </BreadCrumbBase>
                <div className="mt-3">
                    <ImpersonationContextTable />
                </div>
            </section>
        </Container>
    )
}