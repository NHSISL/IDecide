import { ErrorBase } from "../../../types/ErrorBase";
export interface ILookupApiErrors extends ErrorBase {
    id: string[];
    name: string[];
    value: string[];
    createdDate: string[];
    createdBy: string[];
    updatedBy: string[];
    updatedDate: string[];
}

export const LookupApiErrors: ILookupApiErrors = {
    hasErrors: false,
    id:[],
    name: [],
    value: [],
    createdDate: [],
    createdBy:[],
    updatedBy:[],
    updatedDate:[]
};
