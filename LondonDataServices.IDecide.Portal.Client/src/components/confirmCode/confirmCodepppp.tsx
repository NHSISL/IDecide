import { Patient } from "../../models/patients/patient";
import { ConfirmCode } from "./ConfirmCode";

export interface ConfirmCodeProps {
    createdPatient: Patient;
}

export default ConfirmCode;