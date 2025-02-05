import {
    Button,
    CircularProgress,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { useLoadingState } from "src/hooks";
import { ListFeedUseCase } from "../../use-cases/list-feed.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { Post } from "src/typings/models/post.model";
import { toast } from "react-toastify";
import { PostItem } from "src/components/data-display/post/post";
import { LoadingButton } from "@mui/lab";

interface FeedProps {
    userFeedId?: number;
}

export function Feed(props: FeedProps) {
    const { userFeedId } = props;
    const { personaInfo } = useContext(UserPersonaContext);
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [posts, setPosts] = useState<Post[]>([]);
    const theme = useTheme();
    const [tab, setTab] = useState<number>(0);
    const [page, setPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(0);

    const FEED_POST_LIMIT = 5;
    
    useEffect(() => {
        setPosts([]);
    }, [tab]);

    function loadFeed() {
        if (!personaInfo || !myUser) return;

        startLoading();

        new ListFeedUseCase()
            .handle({
                feed: userFeedId ? true : tab == 0 ? false : true,
                feedTahto: userFeedId ? false : tab == 0 ? true : false,
                specificUserId: userFeedId ? userFeedId : 0,
                isAdm: myUser.isAdmin,
                userConfigPost: {
                    client: personaInfo.client,
                    group: personaInfo.group,
                    hierarchy: personaInfo.hierarchy,
                    homeOrFloor: personaInfo.homeOrFloor,
                    period: personaInfo.period,
                    sector: personaInfo.sector,
                    subsector: personaInfo.subsector,
                },
                limit: FEED_POST_LIMIT,
                page: 1,
            })
            .then((data) => {
                setTotalPages(data.TOTALPAGES);
                setPosts(data.returnListsPosts);
                setPage(1);
            })
            .catch(() => {
                toast.error("Falha ao carregar o feed");
            })
            .finally(() => {
                finishLoading();
            });
    }

    function loadMoreFeed(toPage: number) {
        if (!personaInfo || !myUser) return;

        new ListFeedUseCase()
            .handle({
                feed: userFeedId ? true : tab == 0 ? false : true,
                feedTahto: userFeedId ? false : tab == 0 ? true : false,
                specificUserId: userFeedId ? userFeedId : 0,
                isAdm: myUser.isAdmin,
                userConfigPost: {
                    client: personaInfo.client,
                    group: personaInfo.group,
                    hierarchy: personaInfo.hierarchy,
                    homeOrFloor: personaInfo.homeOrFloor,
                    period: personaInfo.period,
                    sector: personaInfo.sector,
                    subsector: personaInfo.subsector,
                },
                limit: FEED_POST_LIMIT,
                page: toPage,
            })
            .then((data) => {
                setTotalPages(data.TOTALPAGES);
                setPosts((posts) => [...posts, ...data.returnListsPosts]);
            })
            .catch(() => {
                toast.error("Falha ao carregar o feed");
            })
            .finally(() => {});
    }

    useEffect(() => {
        personaInfo && myUser && loadFeed();
    }, [personaInfo, myUser, tab, userFeedId]);

    function loadMorePosts() {
        if (page < totalPages) {
            setPage((prevPage) => prevPage + 1);
            loadMoreFeed(page + 1);
        }
    }

    if (isLoading)
        return (
            <>
                <CircularProgress />
            </>
        );

    return (
        <ContentCard sx={{ flexDirection: "column", gap: "28px" }}>
            {!userFeedId && (
                <Stack gap={"40px"} direction={"row"}>
                    <Stack
                        width={"100%"}
                        height={"49px"}
                        bgcolor={
                            tab == 0
                                ? theme.palette.secondary.main
                                : theme.palette.background.default
                        }
                        color={
                            tab == 0
                                ? theme.palette.background.default
                                : theme.palette.grey[300]
                        }
                        border={
                            tab == 0
                                ? `solid 1px ${theme.palette.secondary.main}`
                                : `solid 1px ${theme.palette.grey[400]}`
                        }
                        borderRadius={"8px"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        onClick={() => setTab(0)}
                    >
                        <Typography
                            variant="body1"
                            fontWeight={"600"}
                            fontSize={"16px"}
                        >
                            Tahto comunica
                        </Typography>
                    </Stack>
                    <Stack
                        width={"100%"}
                        height={"49px"}
                        bgcolor={
                            tab == 1
                                ? theme.palette.secondary.main
                                : theme.palette.background.default
                        }
                        color={
                            tab == 1
                                ? theme.palette.background.default
                                : theme.palette.grey[300]
                        }
                        border={
                            tab == 1
                                ? `solid 1px ${theme.palette.secondary.main}`
                                : `solid 1px ${theme.palette.grey[400]}`
                        }
                        borderRadius={"8px"}
                        justifyContent={"center"}
                        alignItems={"center"}
                        onClick={() => setTab(1)}
                    >
                        <Typography
                            variant="body1"
                            fontWeight={"600"}
                            fontSize={"16px"}
                        >
                            Feed de publicações
                        </Typography>
                    </Stack>
                </Stack>
            )}

            <Stack gap={"24px"}>
                {posts.map((post) => (
                    <PostItem key={post.cod} data={post} loadFeed={loadFeed} />
                ))}
            </Stack>
            <Stack
                direction={"row"}
                justifyContent={"center"}
                alignItems={"cener"}
            >
                {page < totalPages && (
                    <LoadingButton
                        variant="outlined"
                        sx={{ width: "100%", maxWidth: "300px" }}
                        onClick={loadMorePosts}
                        loading={isLoading}
                    >
                        Ver mais
                    </LoadingButton>
                )}
            </Stack>
        </ContentCard>
    );
}
