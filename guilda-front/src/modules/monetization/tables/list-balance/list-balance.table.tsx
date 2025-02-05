import { Table } from "../../../../components";
import { Indicator, TableDataModel } from "../../../../typings";
import { listBalanceTableColumns } from "./list-balance.table-columns";

interface ListBalanceTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
}

export function ListBalanceTable({
    tableData,
    getTableProps,
    isLoading,
}: ListBalanceTableProps) {
    return (
        <Table
            tableData={tableData}
            columns={listBalanceTableColumns}
            getTableProps={getTableProps}
            isLoading={isLoading}
            hideDatePicker={true}
            hideSearchInput={true}
            hidePagination={true}
        />
    );
}
