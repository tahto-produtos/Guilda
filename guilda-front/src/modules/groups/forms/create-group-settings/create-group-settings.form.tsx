import { useEffect, useState } from "react";
import { Stack } from "@mui/material";

import { Group, HistoryIndicatorSector } from "../../../../typings";
import { MetricMinMaxForm, MetricMinMaxSchema } from "../metric-min-max";

interface CreateGroupSettingsFormProps {
    disabled: boolean;
    metrics: HistoryIndicatorSector[];
    groups: Group[];
    onMetricsChange: (group: Group, metrics: MetricMinMaxSchema) => void;
}

export function CreateGroupSettingsForm({
    disabled,
    metrics,
    groups,
    onMetricsChange,
}: CreateGroupSettingsFormProps) {
    const [metricsByGroupId, setMetricsByGroupId] = useState<
        Map<number, MetricMinMaxSchema>
    >(new Map());

    useEffect(() => {
        const metricsByGroupMap = new Map<number, MetricMinMaxSchema>();

        for (const group of groups) {
            const metric = metrics.find((item) => item.id === group.id);

            if (metric) {
                metricsByGroupMap.set(group.id, {
                    min: metric.metrics?.metricMin,
                    max: metric.metrics?.metricMax,
                });
            }

            setMetricsByGroupId(metricsByGroupMap);
        }
    }, [metrics, groups]);

    return (
        <Stack>
            {groups.map((group, index) => (
                <MetricMinMaxForm
                    disabled={disabled}
                    id={`create-metrics-form-${index}`}
                    key={index}
                    group={group}
                    onSubmit={(metrics) => onMetricsChange(group, metrics)}
                    initialValues={metricsByGroupId.get(group.id)}
                />
            ))}
        </Stack>
    );
}
