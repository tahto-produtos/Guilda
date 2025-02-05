import {
    Box,
    Button,
    CardMedia,
    Divider,
    IconButton,
    Popover,
    Stack,
    Typography,
    capitalize,
    darken,
    useTheme,
} from "@mui/material";
import { Post } from "src/typings/models/post.model";
import { ProfileImage } from "../profile-image/profile-image";
import { capitalizeText } from "src/utils/capitalizeText";
import FavoriteBorder from "@mui/icons-material/FavoriteBorder";
import AddCommentOutlined from "@mui/icons-material/AddCommentOutlined";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { CreateLikeUseCase } from "src/modules/post/create-like.use-case";
import { toast } from "react-toastify";
import { useEffect, useState, useContext } from "react";
import { useLoadingState } from "src/hooks";
import { PostComments } from "./comments/post-comments";
import ThumbUpOutlined from "@mui/icons-material/ThumbUpOutlined";
import ReplyOutlined from "@mui/icons-material/ReplyOutlined";
import { CreatePostUseCase } from "src/modules/post/create-post.use-case";
import { RepostModal } from "./repost-modal/repost-modal";
import { useRouter } from "next/router";
import { format } from "date-fns";
import MoreVert from "@mui/icons-material/MoreVert";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import Star from "@mui/icons-material/Star";
import { Delete } from "@mui/icons-material";
import { DeletePostUseCase } from "src/modules/post/delete-post.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";

interface PostItemProps {
    data: Post;
    loadFeed: () => void;
}

export function PostItem(props: PostItemProps) {
    const { data } = props;
    const router = useRouter();
    const theme = useTheme();
    const { myUser } = useContext(UserInfoContext);
    const [isReactedByMe, setIsReactedByMe] = useState<boolean>(
        data.myReaction.amount > 0
    );
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [isOpenComments, setIsOpenComments] = useState<boolean>(false);
    const [isDeleted, setIsDeleted] = useState<boolean>(false);

    const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);
    const open = Boolean(anchorEl);
    const [reactionImage, setReactionImage] = useState<string | null>(
        props?.data?.myReaction?.linkIconSelected || null
    );

    const [isVisibilityOpen, setIsVisibilityOpen] = useState(false);

    const [isOpenRepost, setIsOpenRepost] = useState<boolean>(false);
    const [isOpenModalDeletePost, setIsOpenModalDeletePost] =
        useState<boolean>(false);

    const isRepost = Boolean(data.idPostReference);
    const isAllowComment = data.allowComment == 1;
    const isAllowDeletePost = data.canDeletePost;

    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        if (isReactedByMe) {
            return handleDislike(1);
        }

        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const reactions = data.reactions;

    async function handleLike(idReaction: number) {
        startLoading();

        await new CreateLikeUseCase()
            .handle({
                postId: data.cod,
                isLike: true,
                idReaction,
            })
            .then((data) => {
                setIsReactedByMe(true);
                setReactionImage(data.link);
            })
            .catch(() => {
                toast.error("Falha ao curtir o post.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function handleDislike(idReaction: number) {
        startLoading();

        await new CreateLikeUseCase()
            .handle({
                postId: data.cod,
                isLike: false,
                idReaction,
            })
            .then((data) => {
                setIsReactedByMe(false);
            })
            .catch(() => {
                toast.error("Falha ao curtir o post.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    const handleRepost = async (msg: string) => {
        startLoading();

        await new CreatePostUseCase()
            .handle({
                message: msg,
                codPostReference: data.cod,
            })
            .then((data) => {
                toast.success("Repostado com sucesso!");
            })
            .catch(() => {
                toast.error("Falha ao repostar o post.");
            })
            .finally(() => {
                finishLoading();
            });
    };

    const handleDeletePost = async () => {
        if (!myUser) return;
        startLoading();

        await new DeletePostUseCase()
            .handle({
                idPost: data.cod,
                isAdm: myUser.isAdmin,
            })
            .then((data) => {
                toast.success("Post excluido com sucesso!");
                setIsOpenModalDeletePost(false);
                props.loadFeed();
            })
            .catch((error) => {
                console.error("Falha ao excluir o post.", error);
                toast.error("Falha ao excluir o post.");
            })
            .finally(() => {
                finishLoading();
            });
    };

    function parseDateString(dateString: any) {
        const [datePart, timePart] = dateString.split(' ');
        const [day, month, year] = datePart.split('/').map(Number);
        const [hours, minutes, seconds] = timePart.split(':').map(Number);

        return new Date(year, month - 1, day, hours, minutes, seconds);
    }

    return (
        <Stack
            sx={{
                flexDirection: "column",
                gap: "30px",
                p: "40px 32px",
                border: `solid 1px ${theme.palette.grey[300]}`,
                borderRadius: "8px",
                bgcolor: theme.palette.grey[100],
            }}
        >
            {isRepost && (
                <Stack
                    width={"100%"}
                    direction={"row"}
                    gap={"10px"}
                    alignItems={"center"}
                >
                    <ReplyOutlined color="secondary" />
                    <Typography
                        variant="body1"
                        fontSize={"12px"}
                        fontWeight={"700"}
                    >
                        {capitalizeText(data.nameUser)} compartilhou um post
                    </Typography>
                </Stack>
            )}

            <Stack
                alignItems={"center"}
                gap={"24px"}
                direction={"row"}
                justifyContent={"space-between"}
            >
                <Stack direction={"row"} gap={"16px"} alignItems={"center"}>
                    <ProfileImage
                        image={data.imageUser}
                        name={data.nameUser}
                        width="50px"
                        height="50px"
                    />
                    <Stack gap={"8px"}>
                        <Typography variant="body1" fontWeight={"600"}>
                            {capitalizeText(data.nameUser)}
                        </Typography>
                        <Typography variant="body1" fontWeight={"400"}>
                            {capitalizeText(data.hierarchyUser)}
                        </Typography>
                    </Stack>
                </Stack>

                <Typography
                    variant="body1"
                    fontWeight={"400"}
                    fontSize={"12px"}
                    flexDirection={"row"}
                    display={"flex"}
                    alignItems={"center"}
                >
                    {data.highlight == 1 && (
                        <Stack
                            direction={"row"}
                            gap={"5px"}
                            alignItems={"center"}
                            mr={"10px"}
                        >
                            <Star color="warning" />{" "}
                        </Stack>
                    )}

                    {data.timeAgo} -{" "}


                    {/* {format(new Date(data.datePost.toString()), "dd/MM/yyyy")} */}
                    {format(parseDateString(data.datePost.toString()), "dd/MM/yyyy")}
                    <IconButton onClick={() => setIsVisibilityOpen(true)}>
                        {data.visibility && myUser?.isAdmin && <MoreVert />}
                    </IconButton>
                </Typography>
            </Stack>
            <Divider />
            <Stack py={"0px"} gap={"20px"}>
                <Typography>{data.post}</Typography>
            </Stack>
            <Stack
                direction={"row"}
                width={"100%"}
                gap={"20px"}
                flexWrap={"wrap"}
            >
                {data.files.length > 0 &&
                    data.files.map((file, index) => {
                        const isVideo = file.url.toUpperCase().endsWith(".MP4") || 
                                        file.url.toUpperCase().endsWith(".MOV") ||
                                        file.url.toUpperCase().endsWith(".WMV") ||
                                        file.url.toUpperCase().endsWith(".AVI");
                        return isVideo ? (
                            <video
                                key={index}
                                controls
                                preload="metadata" // Adiciona preload="none" para impedir o carregamento automático
                                style={{
                                    width: "100%",
                                    objectFit: "contain",
                                }}
                            >
                                <source src={file.url} type="video/mp4" />
                                Seu navegador não suporta a reprodução de vídeos.
                            </video>
                        ) : (
                            <CardMedia
                                component="img"
                                image={file.url}
                                key={index}
                                sx={{
                                    width: "100%",
                                    objectFit: "contain",
                                }}
                            />
                        );
                    })}
            </Stack>
            {isRepost && (
                <Stack
                    width={"100%"}
                    direction={"column"}
                    gap={"20px"}
                    border={`solid 1px ${theme.palette.grey[300]}`}
                    p={"20px"}
                    borderRadius={"8px"}
                >
                    <Stack
                        gap={"24px"}
                        direction={"row"}
                        justifyContent={"space-between"}
                        alignItems={"center"}
                    >
                        <Stack
                            direction={"row"}
                            gap={"16px"}
                            alignItems={"center"}
                        >
                            <ProfileImage
                                image={data.imageUserReference}
                                name={data.nameReference}
                                width="50px"
                                height="50px"
                            />
                            <Stack gap={"8px"}>
                                <Typography variant="body1" fontWeight={"600"}>
                                    {capitalizeText(data.nameReference)}
                                </Typography>
                                <Typography variant="body1" fontWeight={"400"}>
                                    {capitalizeText(
                                        data.hierarchyuserReference
                                    )}
                                </Typography>
                            </Stack>
                        </Stack>
                    </Stack>
                    <Divider />
                    <Stack py={"0px"}>
                        <Typography>{data.postReference}</Typography>
                    </Stack>
                    <Stack
                        direction={"row"}
                        width={"100%"}
                        gap={"20px"}
                        flexWrap={"wrap"}
                    >
                        {data.filesReference.length > 0 &&
                            data.filesReference.map((file, index) => (
                                <CardMedia
                                    component="img"
                                    image={file.url}
                                    key={index}
                                    sx={{
                                        width: "100%",
                                        objectFit: "contain",
                                    }}
                                />
                            ))}
                    </Stack>
                </Stack>
            )}
            <Stack
                direction={"row"}
                gap={"24px"}
                justifyContent={"space-between"}
            >
                <Stack direction={"row"} gap={"24px"}>
                    <ActionButton
                        title={isReactedByMe ? "Descurtir" : "Curtir"}
                        isActive={isReactedByMe}
                        icon={
                            isReactedByMe && reactionImage ? (
                                <CardMedia
                                    component="img"
                                    image={reactionImage}
                                    sx={{
                                        width: "20px",
                                        objectFit: "contain",
                                    }}
                                />
                            ) : (
                                <FavoriteBorder />
                            )
                        }
                        loading={isLoading}
                        onClick={handleClick as () => void}
                    />
                    <Popover
                        open={open}
                        anchorEl={anchorEl}
                        onClose={handleClose}
                        anchorOrigin={{
                            vertical: "bottom",
                            horizontal: "left",
                        }}
                        PaperProps={{ sx: { borderRadius: "100px" } }}
                    >
                        <Stack
                            direction={"row"}
                            gap={"20px"}
                            px={"15px"}
                            py={"10px"}
                        >
                            <Stack
                                direction={"row"}
                                alignItems={"center"}
                                gap={"5px"}
                                sx={{ cursor: "pointer" }}
                                onClick={() => {
                                    handleLike(1);
                                    handleClose();
                                }}
                            >
                                <ThumbUpOutlined />
                                <Typography fontWeight={"500"}>
                                    Curti
                                </Typography>
                            </Stack>
                            <Stack
                                direction={"row"}
                                alignItems={"center"}
                                gap={"5px"}
                                sx={{ cursor: "pointer" }}
                                onClick={() => {
                                    handleLike(2);
                                    handleClose();
                                }}
                            >
                                <FavoriteBorder />
                                <Typography fontWeight={"500"}>Amei</Typography>
                            </Stack>
                        </Stack>
                    </Popover>
                    {isAllowComment && (
                        <ActionButton
                            title="Comentários"
                            icon={<AddCommentOutlined />}
                            onClick={() => setIsOpenComments(!isOpenComments)}
                        />
                    )}
                    <ActionButton
                        title="Compartilhar"
                        icon={<ReplyOutlined />}
                        onClick={() => setIsOpenRepost(true)}
                    />
                    {isAllowDeletePost && (
                        <ActionButton
                            title="Excluir"
                            icon={<Delete />}
                            onClick={() =>
                                setIsOpenModalDeletePost(!isOpenModalDeletePost)
                            }
                        />
                    )}
                    <ConfirmModal
                        onClose={() => setIsOpenModalDeletePost(false)}
                        open={isOpenModalDeletePost}
                        onConfirm={handleDeletePost}
                        text="Tem certeza que deseja deletar o post?"
                    />
                </Stack>
                <Stack direction={"row"} gap={"20px"}>
                    {data.reactions.map((react, index) => (
                        <Stack
                            key={index}
                            direction={"row"}
                            alignItems={"center"}
                            gap={"5px"}
                        >
                            <CardMedia
                                component="img"
                                image={react.linkIcon}
                                sx={{
                                    width: 20,
                                    height: 20,
                                    objectFit: "contain",
                                }}
                            />
                            <Typography fontWeight={"500"}>
                                {react.amount}
                            </Typography>
                        </Stack>
                    ))}
                </Stack>
            </Stack>
            <RepostModal
                isOpen={isOpenRepost}
                onClose={() => setIsOpenRepost(false)}
                onConfirm={(msg) => handleRepost(msg)}
            />
            {isOpenComments && <PostComments codPost={data.cod} />}
            <BaseModal
                width={"540px"}
                open={isVisibilityOpen}
                title={`Visibilidade`}
                onClose={() => setIsVisibilityOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <Typography>{data.visibility}</Typography>
                </Box>
            </BaseModal>
        </Stack>
    );
}
