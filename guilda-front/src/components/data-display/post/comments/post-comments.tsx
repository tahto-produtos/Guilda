import { LoadingButton } from "@mui/lab";
import {
    Button,
    Divider,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { useLoadingState } from "src/hooks";
import { CreateCommentUseCase } from "src/modules/post/create-comment.use-case";
import { ListCommentUseCase } from "src/modules/post/list-comment.use-case";
import { PersonaComment } from "src/typings/models/persona-comments.model";
import { ProfileImage } from "../../profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import { DeleteComment } from "./use-cases/delete-comment";
import { UserInfoContext } from "src/contexts/user-context/user.context";

interface PostCommentsProps {
    codPost: number;
}

export function PostComments(props: PostCommentsProps) {
    const { codPost } = props;
    const theme = useTheme();
    const { myUser } = useContext(UserInfoContext);
    const [comment, setComment] = useState<string>("");
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [comments, setComments] = useState<PersonaComment[]>([]);
    const [page, setPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);

    const COMMENT_PAGE_LIMIT = 5;

    async function handleCreateComment() {
        startLoading();

        await new CreateCommentUseCase()
            .handle({
                codPost,
                comment: comment,
            })
            .then((data) => {
                listComments();
            })
            .catch((e) => {
                const msg = e?.response?.data?.Message;
                toast.error(msg || "Falha ao curtir o post.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function listComments() {
        if(!myUser) return;
        startLoading();

        await new ListCommentUseCase()
            .handle({
                codPost,
                isAdm: myUser.isAdmin,
                limit: COMMENT_PAGE_LIMIT,
                page: 1,
            })
            .then((data) => {
                setTotalPages(data.TOTALPAGES);
                setComments(data.listsComments);
                setPage(1);
            })
            .catch(() => {
                toast.error("Falha ao listar os comentários.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        listComments();
    }, []);

    async function listMoreComments(toPage: number) {
        if(!myUser) return;
        startLoading();

        await new ListCommentUseCase()
            .handle({
                codPost,
                isAdm: myUser.isAdmin,
                limit: COMMENT_PAGE_LIMIT,
                page: toPage,
            })
            .then((data) => {
                setTotalPages(data.TOTALPAGES);
                setComments((comments) => [...comments, ...data.listsComments]);
            })
            .catch(() => {
                toast.error("Falha ao listar os comentários.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    function loadMorePosts() {
        if (page < totalPages) {
            setPage((prevPage) => prevPage + 1);
            listMoreComments(page + 1);
        }
    }

    async function deleteComment(id: number) {
        if(!myUser) return;
        await new DeleteComment()
            .handle({
                idComment: id,
                isAdm: myUser?.isAdmin,
            })
            .then((data) => {
                toast.success("Comentário apagado.");
            })
            .catch(() => {
                toast.error("Falha ao apagar o comentário.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <Stack width={"100%"} direction={"column"} gap={"16px"}>
            <Stack direction={"row"} gap={"16px"}>
                <TextField
                    label="Escrever comentário"
                    fullWidth
                    value={comment}
                    onChange={(e) => setComment(e.target.value)}
                />
                <LoadingButton
                    variant="contained"
                    size="large"
                    onClick={handleCreateComment}
                    loading={isLoading}
                >
                    Enviar
                </LoadingButton>
            </Stack>
            {comments.map((item, index) => {
                return (
                    <Stack
                        key={index}
                        direction={"column"}
                        width={"100%"}
                        py={"20px"}
                        px={"20px"}
                        border={`solid 1px ${theme.palette.grey[300]}`}
                        borderRadius={"8px"}
                        gap={"10px"}
                    >
                        <Stack
                            direction={"row"}
                            justifyContent={"space-between"}
                            alignItems={"center"}
                        >
                            <Stack
                                direction={"row"}
                                gap={"8px"}
                                alignItems={"center"}
                            >
                                <ProfileImage
                                    name={item.name}
                                    image={item.url}
                                    height="40px"
                                    width="40px"
                                />
                                <Stack>
                                    <Typography variant="body1">
                                        {capitalizeText(item.name)}
                                    </Typography>
                                    <Typography variant="body2">
                                        {item.hierarchy}
                                    </Typography>
                                </Stack>
                            </Stack>
                            <Typography
                                variant="body1"
                                fontWeight={"400"}
                                fontSize={"12px"}
                            >
                                {item.timeAgo}
                            </Typography>
                        </Stack>
                        <Divider />
                        <Stack direction={"row"} py={"15px"}>
                            <Typography>{item.comment}</Typography>
                        </Stack>
                        <Stack direction={"row"}>
                            {item.canDeleteComment &&
                            item.canDeleteComment == true ? (
                                <Button
                                    size="small"
                                    variant="contained"
                                    color="error"
                                    onClick={() => deleteComment(item.id)}
                                >
                                    Apagar comentário
                                </Button>
                            ) : null}
                        </Stack>
                    </Stack>
                );
            })}
            {page < totalPages && (
                <LoadingButton
                    variant="outlined"
                    onClick={loadMorePosts}
                    loading={isLoading}
                    fullWidth
                >
                    Ver mais comentários
                </LoadingButton>
            )}
        </Stack>
    );
}
