import { Box, Checkbox, Chip, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { useState } from "react";

export const PermissionsAccordionGroup = (props: {
    group: string;
    profilePermissions: Array<Number>;
    updatePermissions: any;
    permissions: Array<any>;
}) => {
    const { group, profilePermissions, updatePermissions, permissions } = props;

    const [isOpen, setIsOpen] = useState<boolean>(false);

    const groupPermissions = permissions.filter(
        (item) => item.resource === group
    );

    const groupPermissionsIds = groupPermissions.map((item) => {
        return item.id;
    });

    const handleCheckboxOnChange = (e: any, id: number) => {
        if (e.target.checked === true) {
            updatePermissions().Add(id);
        } else {
            updatePermissions().Remove(id);
        }
    };

    const handleGroupSelect = (e: any) => {
        if (e.target.checked === true) {
            for (let i = 0; i < groupPermissionsIds.length; i++) {
                if (
                    profilePermissions.includes(groupPermissionsIds[i]) ===
                    false
                ) {
                    updatePermissions().Add(groupPermissionsIds[i]);
                }
            }
        } else {
            updatePermissions().RemoveGroup(groupPermissionsIds);
        }
    };

    return (
        <Box
            sx={{ border: `solid 1px ${grey[200]}` }}
            display={"flex"}
            flexDirection={"column"}
            borderRadius={1}
        >
            <Box sx={{ backgroundColor: grey[300] }} py={1} px={1}>
                <Box
                    sx={{
                        display: "flex",
                        alignItem: "center",
                    }}
                >
                    <Checkbox
                        checked={groupPermissionsIds.every((item) =>
                            profilePermissions.includes(item)
                        )}
                        indeterminate={
                            groupPermissionsIds.some(
                                (item) => profilePermissions.indexOf(item) >= 0
                            ) &&
                            groupPermissionsIds.every((item) =>
                                profilePermissions.includes(item)
                            ) === false
                        }
                        onChange={(event) => handleGroupSelect(event)}
                    />
                    <Typography
                        sx={{
                            display: "flex",
                            alignItems: "center",
                            width: "100%",
                            cursor: "pointer",
                        }}
                        onClick={() => setIsOpen(!isOpen)}
                    >
                        {props.group}
                    </Typography>
                </Box>
            </Box>
            <Box sx={{}}>
                {isOpen &&
                    groupPermissions.map((item, index) => (
                        <Box
                            key={index}
                            sx={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "space-between",
                                borderTop: `solid 1px ${grey[200]}`,
                                py: 1,
                                px: 1,
                                paddingLeft: 5,
                            }}
                        >
                            <Box
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                }}
                            >
                                <Checkbox
                                    onChange={(event) =>
                                        handleCheckboxOnChange(event, item.id)
                                    }
                                    checked={profilePermissions.includes(
                                        item.id
                                    )}
                                    size="small"
                                />
                                <Typography
                                    sx={{
                                        display: "flex",
                                        alignItems: "center",
                                    }}
                                >
                                    {item.action}
                                </Typography>
                            </Box>
                        </Box>
                    ))}
            </Box>
        </Box>
    );
};
