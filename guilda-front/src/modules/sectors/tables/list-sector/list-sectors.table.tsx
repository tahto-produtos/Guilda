import { Sector, TableDataModel } from "src/typings";
import { Table } from "../../../../components";
import { listSectorTableColumns } from "./list-sectors.table-columns";

interface ListSectorTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
}

export function ListSectorsTable({
    tableData,
    getTableProps,
    isLoading,
}: ListSectorTableProps) {
    return (
        <Table<Sector>
            tableData={tableData}
            columns={listSectorTableColumns}
            getTableProps={getTableProps}
            isLoading={isLoading}
        />
    );
}
