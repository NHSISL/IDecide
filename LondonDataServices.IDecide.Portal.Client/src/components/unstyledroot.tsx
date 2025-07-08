import { Outlet } from "react-router-dom";
import { UserAgreementModal } from "./userAgreements/userAgreements";

export default function UnstyledRoot() {
    return (
        <>
            <Outlet />
            <UserAgreementModal />
        </>
    );
}