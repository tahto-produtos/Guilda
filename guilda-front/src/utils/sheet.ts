import * as XLSX from "xlsx";

interface ExportOptions {
    extension?: "csv" | "xlsx";
    autoSizeColumns?: boolean;
}

export class SheetBuilder {
    private readonly wb: XLSX.WorkBook;
    private currentSheetName: string;

    constructor(worksheet: string = "Sheet1") {
        this.wb = XLSX.utils.book_new();
        this.currentSheetName = worksheet;
        this.createWorksheet(worksheet);
    }

    public createWorksheet(name: string) {
        if (this.wb.Sheets[name]) {
            throw new Error(`Worksheet "${name}" already exists.`);
        }
        this.wb.SheetNames.push(name);
        this.wb.Sheets[name] = {};
        this.selectWorksheet(name);
        return this;
    }

    public deleteWorksheet(name: string) {
        if (this.wb.SheetNames.includes(name)) {
            delete this.wb.Sheets[name];
            this.wb.SheetNames.splice(this.wb.SheetNames.indexOf(name), 1);
            if (this.currentSheetName === name) {
                this.currentSheetName = this.wb.SheetNames[0];
            }
        }
        return this;
    }

    public selectWorksheet(name: string) {
        this.currentSheetName = name;
        if (!this.wb.SheetNames.includes(name)) {
            const newWs = XLSX.utils.aoa_to_sheet([]);
            this.wb.SheetNames.push(name);
            this.wb.Sheets[name] = newWs;
        }
        return this;
    }

    public append(rows: string[][], options?: XLSX.SheetAOAOpts) {
        const ws = this.getWorksheet(this.getCurrentWorksheetName());
        const range = XLSX.utils.decode_range(ws["!ref"] || "A1:A1");
        const newRange = {
            s: { c: 0, r: range.e.r + 1 },
            e: { c: rows[0].length - 1, r: range.e.r + rows.length + 1 },
        };

        XLSX.utils.sheet_add_aoa(ws, rows, {
            ...options,
            origin: newRange.s,
        });

        let ref = newRange;
        newRange.s.r = newRange.s.r - 1;

        ws["!ref"] = XLSX.utils.encode_range(ref);
        return this;
    }

    public setHeader(headers: string[], options?: XLSX.SheetAOAOpts) {
        const ws = this.getWorksheet(this.getCurrentWorksheetName());
        XLSX.utils.sheet_add_aoa(ws, [headers], { origin: 0, ...options });
        return this;
    }

    public exportAs(filename: string, options?: ExportOptions) {
        if (this.wb.SheetNames.length === 0) {
            throw new Error("Workbook has no worksheets.");
        }

        const extension = options?.extension ?? "xlsx";
        const autoSizeColumns = options?.autoSizeColumns || true;

        if (autoSizeColumns) {
            this.autoSizeColumns();
        }

        XLSX.writeFile(this.wb, `${filename}.${extension}`, {
            bookType: extension,
        });

        return this;
    }

    public autoSizeColumns() {
        const ws = this.getWorksheet(this.currentSheetName);
        if (!ws) {
            throw new Error(
                `Worksheet "${this.currentSheetName}" does not exist.`
            );
        }

        const range = XLSX.utils.decode_range(ws["!ref"]!);
        const cols = [];
        for (let col = range.s.c; col <= range.e.c; col++) {
            let max = 0;
            for (let row = range.s.r + 1; row <= range.e.r; row++) {
                const cell = ws[XLSX.utils.encode_cell({ r: row, c: col })];
                if (cell && cell.v) {
                    const value = cell.w || cell.v.toString();
                    max = Math.max(max, value.length);
                }
            }
            const width = Math.ceil((max + 2) * 1.2);
            cols[col] = { wch: width };
            const headerCell =
                ws[XLSX.utils.encode_cell({ r: range.s.r, c: col })];
            if (headerCell) {
                headerCell.s = Object.assign({}, headerCell.s, { wch: width });
            } else {
                ws[XLSX.utils.encode_cell({ r: range.s.r, c: col })] = {
                    s: { wch: width },
                };
            }
        }
        ws["!cols"] = cols;
        return this;
    }

    public getWorksheet(name: string) {
        const ws = this.wb.Sheets[name];
        if (!ws) {
            throw new Error(`Worksheet "${name}" does not exist`);
        }
        return ws;
    }

    public getCurrentWorksheetName(): string {
        return this.currentSheetName;
    }
}
