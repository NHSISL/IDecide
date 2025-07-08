import { PBIEvent } from "./PBIEvent";

export type ReportObject = {
    on: (event: string, callback: (event: CustomEvent<PBIEvent>) => void) => void;
    off: (event: string) => void;
    getPages: () => Promise<{ displayName: string, visibility: number }[]>
    setPage: (pageName: string) => void;
}