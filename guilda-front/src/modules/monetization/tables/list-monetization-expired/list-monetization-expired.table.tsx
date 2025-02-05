import { Table } from "../../../../components";
import { TableDataModel } from "../../../../typings";
import { listMonetizationExpiredTableColumns } from "./list-monetization-expired.table-columns";

interface ListMonetizationExpiredTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
}

export function ListMonetizationExpiredTable({
    tableData,
    getTableProps,
    isLoading,
}: ListMonetizationExpiredTableProps) {
    return (
        <Table
            tableData={tableData}
            columns={listMonetizationExpiredTableColumns}
            getTableProps={getTableProps}
            isLoading={isLoading}
            hideDatePicker={true}
            hideSearchInput={true}
            hidePagination={true}
        />
    );
}
