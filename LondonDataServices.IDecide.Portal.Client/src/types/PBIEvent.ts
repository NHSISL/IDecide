type Target = {
    column: string,
    table: string,
    measure: string
}
export type PBIIdentity = {
    target: Target,
    equals: number
}

export type PBIValues = {
    target: Target,
    value: string,
    formattedValue: string
}

export type PBIEvent = {
    dataPoints : Array<{
        identity : Array<PBIIdentity>
        values: Array<PBIValues>
    }>
}