import { Table } from "../../../../components";
import { Indicator, OrderReport, TableDataModel } from "../../../../typings";
import { listGroupTableNewColumns } from "./order-report.table-columns";

interface OrderReportTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
    hideSearchInput?: boolean;
    hideDatePicker?: boolean;
    enableCodeSearch?: boolean;
}

export function OrderReportTable({
    tableData,
    getTableProps,
    isLoading,
    enableCodeSearch,
    hideSearchInput,
    hideDatePicker,
}: OrderReportTableProps) {
    return (
        <Table<OrderReport>
            tableData={tableData}
            columns={listGroupTableNewColumns}
            getTableProps={getTableProps}
            isLoading={isLoading}
            enableCodeSearch={enableCodeSearch}
            hideSearchInput={hideSearchInput}
            hideDatePicker={hideDatePicker}
        />
    );
}
