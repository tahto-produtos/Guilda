import { FeedbackOutlined, HomeOutlined } from "@mui/icons-material";
import {
    Breadcrumbs,
    Divider,
    Link,
    Stack,
    Typography,
    useTheme,
} from "@mui/material";
import { useRouter } from "next/router";
import { useState } from "react";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useDebounce, useLoadingState } from "src/hooks";
import { MyFeedbackTable } from "src/modules/feedback/fragments/my-feedback-table";
import { getLayout } from "src/utils";

export default function MyFeedbackView() {
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();

    const theme = useTheme();

    return (
        <ContentCard sx={{ p: 0 }}>
            <Stack
                width={"100%"}
                height={"80px"}
                sx={{
                    borderTopLeftRadius: "16px",
                    borderTopRightRadius: "16px",
                }}
                bgcolor={theme.palette.secondary.main}
                pl={"80px"}
                justifyContent={"center"}
            >
                <Breadcrumbs
                    aria-label="breadcrumb"
                    sx={{
                        color: theme.palette.background.default,
                    }}
                >
                    <Link
                        underline="hover"
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.background.default}
                        href="/"
                    >
                        <HomeOutlined
                            sx={{
                                mr: 0.5,
                                color: theme.palette.background.default,
                            }}
                        />
                    </Link>
                    <Link
                        sx={{
                            display: "flex",
                            alignItems: "center",

                            textDecoration: "none",
                        }}
                        color={theme.palette.background.default}
                    >
                        <Typography fontWeight={"700"}>
                            Meus Feedbacks
                        </Typography>
                    </Link>
                </Breadcrumbs>
            </Stack>
            <ContentArea sx={{ py: " 40px" }}>
                <Stack px={"40px"}>
                    <PageTitle
                        icon={<FeedbackOutlined sx={{ fontSize: "40px" }} />}
                        title="Meus Feedbacks"
                        loading={isLoading}
                    ></PageTitle>
                    <Divider />
                    <Stack direction={"column"} gap={"20px"} mt={"20px"}>
                        <MyFeedbackTable />
                    </Stack>
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

MyFeedbackView.getLayout = getLayout("private");
