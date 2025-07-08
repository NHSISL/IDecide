export function isHx(hxNumber: string) {
    return hxNumber.indexOf("-") !== -1
}

export function convertHx(hxNumber: string) {
    const cleanedID = hxNumber.replace(/-/g, '');
    const originalHex = cleanedID.split('').reverse().join('');
    return parseInt(originalHex, 16).toString();
}

export function getPseudo(maybeHxNumber: string) {
    return isHx(maybeHxNumber) ? convertHx(maybeHxNumber) : maybeHxNumber;
}