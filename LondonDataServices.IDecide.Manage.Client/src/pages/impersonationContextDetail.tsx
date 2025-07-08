import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import ImpersonationContextDetail from "../components/impersonationContext/impersonationContextDetail"

export const ImpersonationContextDetailPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="Impersonation Context Detail">
                </BreadCrumbBase>
                <div className="mt-3">
                    <p>Role Needs to be in Security Matrix and Azure AD against user.</p>
                    <ImpersonationContextDetail />
                </div>
            </section>
        </Container>
    )
}