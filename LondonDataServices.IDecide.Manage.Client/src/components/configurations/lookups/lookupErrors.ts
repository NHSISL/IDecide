import { ErrorBase } from "../../../types/ErrorBase";
export interface ILookupErrors extends ErrorBase {
    hasErrors: boolean;
    id: string;
    name: string;
    groupName: string;
    value:string;
    createdDate: string;
    createdBy: string;
    updatedBy: string;
    updatedDate: string;
}


export const LookupErrors: ILookupErrors = {
    hasErrors: false,
    id:"",
    name: "",
    groupName: "",
    value: "",
    createdDate: "",
    createdBy:"",
    updatedBy:"",
    updatedDate:""
};
