import { Table } from "../../../../components";
import { Group, TableDataModel } from "../../../../typings";
import { listGroupTableColumns } from "./list-group.table-columns";

interface ListGroupsTableProps {
    tableData?: TableDataModel;
    getTableProps?: any;
    hidePagination?: boolean;
    hideSearchInput?: boolean;
    isLoading?: boolean;
}

export function ListGroupsTable({
    tableData,
    getTableProps,
    hidePagination,
    hideSearchInput,
    isLoading,
}: ListGroupsTableProps) {
    return (
        <Table<Group>
            tableData={tableData}
            columns={listGroupTableColumns}
            getTableProps={getTableProps}
            hidePagination={hidePagination}
            hideSearchInput={hideSearchInput}
            hideDatePicker={true}
            isLoading={isLoading}
        />
    );
}
