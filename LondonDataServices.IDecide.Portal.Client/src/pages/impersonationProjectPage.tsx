import { Container } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import ImpersonationContextProjectTable from "../components/impersonationContext/impersonationContextProjectTable";

export const ImpersonationProjectPage = () => {

    return (
        <>
            <Container fluid className="mt-4">
                <section>
                    <BreadCrumbBase
                        link="/"
                        backLink="Home"
                        currentLink="Projects">
                    </BreadCrumbBase>
                    <div className="mt-3">
                        <ImpersonationContextProjectTable />
                    </div>
                </section>
            </Container>
        </>
    )
}