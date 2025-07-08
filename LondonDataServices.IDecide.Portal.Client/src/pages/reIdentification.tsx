import { Container } from "react-bootstrap";
import ReIdentificationDetail from "../components/reIdentification/reIdentificationDetail";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";

export const ReIdentificationPage = () => {

    return (
        <>
            <Container fluid className="mt-4">
                <section>
                    <BreadCrumbBase
                        link="/"
                        backLink="Home"
                        currentLink="Reidentify Single Patient">
                    </BreadCrumbBase>
                    <div className="mt-3">
                        <ReIdentificationDetail />
                    </div>
                </section>
            </Container>
        </>
    )
}