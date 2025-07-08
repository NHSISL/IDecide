import { useCallback } from "react";

export const useFileChange = (
    setError: React.Dispatch<React.SetStateAction<string[]>>,
    setFileName: React.Dispatch<React.SetStateAction<string>>,
    setHeaderColumns: React.Dispatch<React.SetStateAction<string[]>>,
    setCsvData: React.Dispatch<React.SetStateAction<string | null>>,
    hasHeaderRecord: boolean,
    csvMaxReId?: number
) => {
    const handleFileChange = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            const file = e.target.files?.[0];
            setError([]);

            if (!file) {
                setError(["No file selected. Please upload a .csv file."]);
                return;
            }

            const sizeInBytes = file.size;
            if (sizeInBytes > 1024 * 1024) {
                setError(["File size exceeds 1MB. Please upload a smaller file."]);
                return;
            }

            if (!file.name.endsWith(".csv")) {
                setError(["Please upload a valid .csv file."]);
                return;
            }

            setFileName(file.name);
            const reader = new FileReader();
            const CHUNK_SIZE = 1024 * 1024;
            let offset = 0;
            let csvData = "";

            reader.onload = (event) => {
                const chunk = event.target?.result as string;
                csvData += chunk;
                offset += CHUNK_SIZE;

                if (offset < file.size) {
                    readNextChunk();
                } else {
                    processCsvData(csvData);
                }
            };

            reader.onerror = () => {
                setError(["Failed to read file. Please try again."]);
            };

            const readNextChunk = () => {
                const blob = file.slice(offset, offset + CHUNK_SIZE);
                reader.readAsText(blob);
            };

            const processCsvData = (data: string) => {
                setCsvData(btoa(data));
                const rows = data.split("\n");

                if (csvMaxReId !== undefined) {
                    const rowCount = hasHeaderRecord ? rows.length - 2 : rows.length - 1;
                    if (rowCount > csvMaxReId) {
                        setError(["The CSV file contains more than the set " + csvMaxReId + " row limit for CSV upload."]);
                        return;
                    }
                }

                if (hasHeaderRecord) {
                    const headers = rows.length === 1 ? [rows[0]] : rows[0].split(",");
                    if (headers.length <= 0 || (rows.length > 1 && !/[a-zA-Z]/.test(rows[0]))) {
                        setError(["The CSV file does not contain a valid header row."]);
                        return;
                    }
                    setHeaderColumns(headers);
                } else {
                    const headers = rows.length === 1 ? [rows[1]] : rows[1].split(",");
                    setHeaderColumns(headers);
                }
            };

            readNextChunk();
        },
        [setError, setFileName, setHeaderColumns, setCsvData, hasHeaderRecord, csvMaxReId]
    );

    return { handleFileChange };
};