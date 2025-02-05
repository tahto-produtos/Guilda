import { InputAdornment, TextField, Typography, useTheme } from "@mui/material";
import { Box } from "@mui/system";
import { useEffect, useState } from "react";
import { Group, HistoryIndicatorSector } from "src/typings";

interface GroupMonetizationSettingsProps {
    groups: Group[];
    disabled: boolean;
    historyIndicatorSector?: HistoryIndicatorSector[];
    getMonetizationValues: (
        group: Group,
        monetizationValue: Number | string
    ) => void;
}

const GroupInput = ({
    group,
    disabled,
    onMonetizationSettingsChange,
    defaultValue,
}: {
    group: Group;
    disabled: boolean;
    defaultValue?: number;
    onMonetizationSettingsChange: (monetizationValue: Number | string) => void;
}) => {
    const theme = useTheme();
    const [monetizationValue, setMonetizationValue] = useState<Number>(
        defaultValue ? defaultValue : 0
    );

    useEffect(() => {
        defaultValue !== undefined && setMonetizationValue(defaultValue);
    }, [defaultValue]);

    return (
        <Box display={"flex"} justifyContent={"space-between"} gap={1}>
            <TextField
                disabled={disabled}
                type={"number"}
                label={`Monetização do ${group.name}`}
                value={monetizationValue}
                onChange={(e) => {
                    setMonetizationValue(parseInt(e.target.value));
                    onMonetizationSettingsChange(parseInt(e.target.value));
                }}
                size={"medium"}
                fullWidth
                InputProps={{
                    startAdornment: (
                        <InputAdornment position="start">
                            <Typography
                                color={
                                    disabled
                                        ? theme.palette.grey["500"]
                                        : undefined
                                }
                                variant={"h3"}
                                width={"50px"}
                            >
                                {group.name}
                            </Typography>
                        </InputAdornment>
                    ),
                }}
            />
        </Box>
    );
};

export default function GroupMonetizationSettings({
    groups,
    disabled,
    getMonetizationValues,
    historyIndicatorSector,
}: GroupMonetizationSettingsProps) {
    const handleMonetizationSettingsChange = (
        group: Group,
        monetizationValue: Number | string
    ) => {
        getMonetizationValues(group, monetizationValue);
    };

    return (
        <Box display={"flex"} flexDirection={"column"} gap={3} mb={5}>
            {groups.map((group, index) => (
                <GroupInput
                    key={index}
                    group={group}
                    disabled={disabled}
                    onMonetizationSettingsChange={(monetizationValue) =>
                        handleMonetizationSettingsChange(
                            group,
                            monetizationValue
                        )
                    }
                    defaultValue={
                        historyIndicatorSector?.find(
                            (item) => item.metrics?.groupId === group.id
                        )
                            ? historyIndicatorSector?.find(
                                  (item) => item.metrics?.groupId === group.id
                              )?.metrics.monetization
                            : 0
                    }
                />
            ))}
        </Box>
    );
}
