import { Container } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import ImpersonationContextDetailAdd from "../components/impersonationContext/impersonationContextDetailAdd";

export const ImpersonationProjectAddPage = () => {

    return (
        <>
            <Container fluid className="mt-4">
                <section>
                    <BreadCrumbBase
                        link="/project"
                        backLink="Projects"
                        currentLink="Add Project">
                    </BreadCrumbBase>
                    <div className="mt-3">
                        <ImpersonationContextDetailAdd />
                    </div>
                </section>
            </Container>
        </>
    )
}