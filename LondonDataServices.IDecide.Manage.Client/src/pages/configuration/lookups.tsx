import { Container } from "react-bootstrap";
import BreadCrumbBase from "../../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import LookupTable from "../../components/configurations/lookups/lookupTable";

export const Lookups = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/configuration/home"
                    backLink="Configuration Home"
                    currentLink="Lookups">
                </BreadCrumbBase>
                <div className="mt-3">
                    <LookupTable allowedToAdd={true} allowedToEdit={true} allowedToDelete={true} />
                </div>
            </section>
        </Container>
    )
}