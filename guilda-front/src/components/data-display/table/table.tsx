import { InputAdornment, LinearProgress, TextField } from "@mui/material";
import { Box } from "@mui/system";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import PaginationComponent from "src/components/navigation/pagination/pagination";
import { useDebounce } from "src/hooks";
import { TableDataModel } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { NoResultsOverlay } from "./fragments/no-results-overlay";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import { DateUtils } from "src/utils";
import { formatISO, isValid } from "date-fns";
import Search from "@mui/icons-material/Search";
import { useTheme } from "@emotion/react";
import { lightTheme } from "src/theme/material-ui/themes";
import { grey } from "@mui/material/colors";
import styled from "@emotion/styled";
import { uuid } from "uuidv4";

const defaultPaginationLimit = 10;

export interface TableProps<T = any> {
    columns: GridColDef[];
    isLoading?: boolean;
    hideSearchInput?: boolean;
    paginationLimit?: number;
    tableData: any;
    getTableProps?: any;
    hidePagination?: boolean;
    searchInputLabel?: string;
    hideDatePicker?: boolean;
    enableCodeSearch?: boolean;
}

export function Table<T = any>({
    tableData,
    columns,
    isLoading,
    hideSearchInput,
    paginationLimit,
    getTableProps,
    hidePagination,
    searchInputLabel,
    hideDatePicker,
    enableCodeSearch,
}: TableProps<T>) {
    const [rowCountState, setRowCountState] = useState(100);
    const [searchValue, setSearchValue] = useState<string>("");
    const [searchCode, setSearchCode] = useState<string>("");
    const [pageNumber, setPageNumber] = useState<number>(1);
    const [totalItems, setTotalItems] = useState<number>(0);
    const debouncedSearchTerm: string = useDebounce<string>(searchValue, 300);
    const debouncedSearchCode: string = useDebounce<string>(searchCode, 300);
    const [startDate, setStartDate] = useState<dateFns | null>(null);
    const [endDate, setEndDate] = useState<dateFns | null>(null);
    const theme = useTheme();

    useEffect(() => {
        setRowCountState((prevRowCountState) => prevRowCountState);
    }, [setRowCountState]);

    useEffect(() => {
        setTotalItems(tableData?.totalItems);
    }, [tableData]);

    function sendTableProps() {
        if (getTableProps) {
            const pagination: PaginationModel = {
                limit: paginationLimit
                    ? paginationLimit
                    : defaultPaginationLimit,
                offset:
                    (pageNumber - 1) *
                    (paginationLimit
                        ? paginationLimit
                        : defaultPaginationLimit),
                searchText: searchValue,
                startDate:
                    startDate && isValid(startDate)
                        ? formatISO(new Date(startDate.toString()))
                        : "",
                endDate:
                    endDate && isValid(endDate)
                        ? DateUtils.formatDateIsoEndOfDay(endDate)
                        : "",
                code: parseInt(searchCode),
            };
            getTableProps(pagination);
        }
    }

    useEffect(() => {
        sendTableProps();
    }, [debouncedSearchTerm, pageNumber, debouncedSearchCode]);

    useEffect(() => {
        (startDate === null || isValid(startDate)) &&
            (endDate === null || isValid(endDate)) &&
            sendTableProps();
    }, [startDate, endDate]);

    return (
        <Box sx={{ display: "flex", flexDirection: "column", gap: "20px" }}>
            <Box display={"flex"} gap={2}>
                {enableCodeSearch && (
                    <TextField
                        label={"Código"}
                        value={searchCode}
                        onChange={(e) => {
                            setPageNumber(1);
                            setSearchCode(e.target.value);
                        }}
                        size={"small"}
                        sx={{ width: "230px" }}
                    />
                )}
                {hideSearchInput === true ? null : (
                    <TextField
                        name={"search"}
                        label={
                            searchInputLabel ? searchInputLabel : "Pesquisar"
                        }
                        value={searchValue}
                        onChange={(e) => {
                            setPageNumber(1);
                            setSearchValue(e.target.value);
                        }}
                        size={"small"}
                        style={{ width: "100%" }}
                        InputProps={{
                            endAdornment: (
                                <InputAdornment position="end">
                                    <Search sx={{ color: grey[500] }} />
                                </InputAdornment>
                            ),
                        }}
                    />
                )}

                {hideDatePicker === true ? null : (
                    <>
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="De"
                                value={startDate}
                                onChange={(newValue) => setStartDate(newValue)}
                                slotProps={{
                                    textField: {
                                        size: "small",
                                        sx: {
                                            minWidth: "180px",
                                            svg: {
                                                color: grey[500],
                                            },
                                        },
                                    },
                                }}
                            />
                        </LocalizationProvider>
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="Até"
                                value={endDate}
                                onChange={(newValue) => setEndDate(newValue)}
                                slotProps={{
                                    textField: {
                                        size: "small",
                                        sx: {
                                            minWidth: "180px",
                                            svg: {
                                                color: grey[500],
                                            },
                                        },
                                    },
                                }}
                            />
                        </LocalizationProvider>
                    </>
                )}
            </Box>

            <DataGrid
                columns={columns}
                rows={tableData?.items?.length > 0 ? tableData.items : []}
                hideFooter
                disableColumnFilter
                disableRowSelectionOnClick
                autoHeight
                rowCount={rowCountState}
                localeText={{
                    noResultsOverlayLabel: "Nenhum resultado encontrado",
                }}
                paginationMode="server"
                loading={isLoading}
                slots={{
                    loadingOverlay: LinearProgress,
                }}
                getRowId={(row) => {
                    const uuidv4 = uuid();
                    return uuidv4;
                }}
            />
            {hidePagination === true ? null : (
                <PaginationComponent
                    totalItems={totalItems}
                    limit={
                        paginationLimit
                            ? paginationLimit
                            : defaultPaginationLimit
                    }
                    page={pageNumber}
                    getPage={(page: number) => setPageNumber(page)}
                />
            )}
        </Box>
    );
}
