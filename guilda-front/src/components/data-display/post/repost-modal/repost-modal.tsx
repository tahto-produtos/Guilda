import { Box, Button, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { BaseModal } from "src/components/feedback";

interface RepostModalProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: (msg: string) => void;
}

export function RepostModal(props: RepostModalProps) {
    const { isOpen, onClose, onConfirm } = props;
    const [msg, setMsg] = useState<string>("");

    return (
        <BaseModal
            width={"540px"}
            open={isOpen}
            title={`Repostar`}
            onClose={onClose}
        >
            <Stack gap={"20px"}>
                {/* <TextField
                    value={msg}
                    onChange={(e) => setMsg(e.target.value)}
                    label="O que você está pensando?"
                /> */}
                <Button variant="contained" onClick={() => onConfirm(msg)}>
                    Respostar
                </Button>
            </Stack>
        </BaseModal>
    );
}
