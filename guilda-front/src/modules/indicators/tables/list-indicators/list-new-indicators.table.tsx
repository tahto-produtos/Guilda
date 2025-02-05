import { Table } from "../../../../components";
import { Indicator, TableDataModel } from "../../../../typings";
import { listGroupTableNewColumns } from "./list-new-indicators.table-columns";

interface ListNewIndicatorTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    isLoading?: boolean;
    enableCodeSearch?: boolean;
    hideSearchInput?: boolean;
    hideDatePicker?: boolean;
}

export function ListNewIndicatorsTable({
    tableData,
    getTableProps,
    isLoading,
    enableCodeSearch,
    hideSearchInput,
    hideDatePicker,
}: ListNewIndicatorTableProps) {
    return (
        <Table<Indicator>
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
