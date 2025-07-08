import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import UserAccessTable from "../components/userAccess/userAccessTable"

export const UserAccessPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="User Access">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h3>User Access</h3>
                    <p>Use this screen to assign reidentification users to ODS organisations, this will allow an end user to reidentify patients that are registered to the ODS code and patients registered to all children ODS organisations.</p>
                    <UserAccessTable/>
                </div>
            </section>
        </Container>
    )
}