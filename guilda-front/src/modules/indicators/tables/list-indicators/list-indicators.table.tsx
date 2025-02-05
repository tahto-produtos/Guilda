import { Table } from "../../../../components";
import { Indicator, TableDataModel } from "../../../../typings";
import { listGroupTableColumns } from "./list-indicators.table-columns";

interface ListIndicatorTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
    enableCodeSearch?: boolean;
}

export function ListIndicatorsTable({
    tableData,
    getTableProps,
    isLoading,
    enableCodeSearch,
}: ListIndicatorTableProps) {
    return (
        <Table<Indicator>
            tableData={tableData}
            columns={listGroupTableColumns}
            getTableProps={getTableProps}
            isLoading={isLoading}
            enableCodeSearch={enableCodeSearch}
        />
    );
}
