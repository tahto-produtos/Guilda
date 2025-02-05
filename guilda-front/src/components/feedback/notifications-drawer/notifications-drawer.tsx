import StarBorder from "@mui/icons-material/StarBorder";
import CalendarToday from "@mui/icons-material/CalendarToday";
import Close from "@mui/icons-material/Close";
import NotificationsOutlined from "@mui/icons-material/NotificationsOutlined";
import {
    Button,
    Checkbox,
    Drawer,
    IconButton,
    Pagination,
    Stack,
    Typography,
    useScrollTrigger,
    useTheme,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { useLoadingState } from "src/hooks";
import { FilterMyNotificationUseCase } from "src/modules/notifications/use-cases/filter-my-notification.use-case";
import { ListMyNotificationUseCase } from "src/modules/notifications/use-cases/list-my-notification.use-case";
import { NotificationModel } from "src/typings/models/notification.model";
import { capitalizeText } from "src/utils/capitalizeText";
import Link from "next/link";
import { format } from "date-fns";
import { MarkNotificationReadUseCase } from "src/modules/notifications/use-cases/mark-notification-read.use-case";
import { NotificationsContext } from "src/contexts/notifications-provider/notifications.context";
import { ConfirmModal } from "../confirm-modal";
import { GeneralDeleteNotificationsUseCase } from "src/modules/notifications/use-cases/general-cleaning-notification.use-case";
import { GeneralDeleteUserNotificationsUseCase } from "src/modules/notifications/use-cases/general-cleaning-user-notification.use-case";
import { usePagination } from "src/hooks/use-pagination/use-pagination";

interface NotificationsDrawerProps {
    open: boolean;
    onClose: () => void;
}

export function NotificationsDrawer(props: NotificationsDrawerProps) {
    const { onClose, open } = props;
    const { getCountNotifications } = useContext(NotificationsContext);

    const theme = useTheme();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [isOpenDelete2, setIsOpenDelete2] = useState(false);
    const [filters, setFilters] = useState<{ cod: number; name: string }[]>([]);
    const [selectedFilter, setSelectedFilter] = useState<{
        cod: number;
        name: string;
    } | null>(null);
    const [notifications, setNotifications] = useState<NotificationModel[]>([]);

    const [selectedNotifications, setSelectedNotifications] = useState<
        number[]
    >([]);

    const { handleChange, page, setPage, setTotalPages, totalPages } =
        usePagination(1);

    async function getFilters() {
        startLoading();

        new FilterMyNotificationUseCase()
            .handle()
            .then((data) => {
                setFilters(data);
                setSelectedFilter(data[1]);
            })
            .catch(() => {
                toast.error("Erro ao listar notificações.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        getFilters();
    }, []);

    async function listMyNotification() {
        if (!filters || !selectedFilter) return;
        startLoading();

        new ListMyNotificationUseCase()
            .handle({
                cod: selectedFilter.cod,
                name: selectedFilter.name,
                limit: 10,
                page: page,
            })
            .then((data) => {
                setNotifications(data.returnListNotifications);
                setTotalPages(data.TOTALPAGES);

                if (open && selectedFilter.name.toLowerCase() == "não lido") {
                    new MarkNotificationReadUseCase()
                        .handle({
                            ids: data.returnListNotifications.map(
                                (item: any) => item.codNotification
                            ),
                        })
                        .then(() => {
                            getCountNotifications();
                            setSelectedNotifications([]);
                        })
                        .catch(() => {})
                        .finally(() => {});
                }
            })
            .catch(() => {
                toast.error("Erro ao listar notificações.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        filters && open && selectedFilter && listMyNotification();
    }, [open, filters, selectedFilter, page]);

    function handleClean() {
        new GeneralDeleteUserNotificationsUseCase()
            .handle({
                idNotification: selectedNotifications,
            })
            .then(() => {
                toast.success(`Limpeza realizada com sucesso!`);
                setIsOpenDelete2(false);
                listMyNotification();
            })
            .catch(() => {
                toast.error("Erro ao apagar.");
            })
            .finally(() => {});
    }

    function markAsRead() {
        new MarkNotificationReadUseCase()
            .handle({
                ids: notifications.map((item) => item.codNotification),
            })
            .then(() => {
                getCountNotifications();
            })
            .catch(() => {})
            .finally(() => {});
    }

    function handleOnSelect(id: number) {
        let newArr = [...selectedNotifications];

        if (newArr.includes(id)) {
            newArr = newArr.filter((item) => item !== id);
        } else {
            newArr.push(id);
        }

        setSelectedNotifications(newArr);
    }

    return (
        <Drawer open={open} anchor={"right"} onClose={onClose}>
            <Stack
                width={"100vw"}
                minWidth={"100%"}
                maxWidth={"622px"}
                direction={"column"}
                height={"100vh"}
            >
                <Stack
                    width={"100%"}
                    py={"24px"}
                    px={"24px"}
                    justifyContent={"space-between"}
                    direction={"row"}
                    alignItems={"center"}
                >
                    <Stack direction={"row"} gap={"10px"}>
                        <NotificationsOutlined />
                        <Typography variant="h2">Notificações</Typography>
                    </Stack>
                    <IconButton onClick={onClose}>
                        <Close sx={{ color: theme.palette.text.primary }} />
                    </IconButton>
                </Stack>
                <Stack height={"100%"} px={"24px"}>
                    <Stack gap={"14px"}>
                        <Typography
                            variant="h3"
                            fontWeight={"400"}
                            fontSize={"14px"}
                        >
                            Filtros
                        </Typography>
                        <Stack direction={"row"} gap={"16px"} flexWrap={"wrap"}>
                            {filters.map((filter, index) => {
                                const isSelected =
                                    selectedFilter && selectedFilter == filter;

                                return (
                                    <ActionButton
                                        onClick={() =>
                                            setSelectedFilter(filter)
                                        }
                                        border={
                                            isSelected
                                                ? "solid 1px #000"
                                                : undefined
                                        }
                                        key={index}
                                        title={filter.name}
                                    />
                                );
                            })}
                        </Stack>
                    </Stack>

                    <Stack direction={"column"} py={"60px"} width={"100%"}>
                        <Stack
                            direction={"row"}
                            justifyContent={"flex-end"}
                            width={"100%"}
                        >
                            {selectedNotifications.length > 0 && (
                                <Button
                                    color="error"
                                    variant="contained"
                                    size="small"
                                    onClick={() => setIsOpenDelete2(true)}
                                >
                                    Apagar selecionados
                                </Button>
                            )}
                            <ConfirmModal
                                onClose={() => setIsOpenDelete2(false)}
                                onConfirm={handleClean}
                                open={isOpenDelete2}
                            />
                            <Button
                                color="secondary"
                                sx={{
                                    fontWeight: "700",
                                    color: "secondary.main",
                                }}
                                onClick={() => {
                                    setSelectedNotifications(
                                        notifications.map(
                                            (item) => item.codNotification
                                        )
                                    );
                                }}
                            >
                                Selecionar todos
                            </Button>
                            <Button
                                color="error"
                                sx={{ fontWeight: "700", color: "error.main" }}
                                onClick={() => {
                                    setSelectedNotifications([]);
                                }}
                            >
                                Remover seleções
                            </Button>
                        </Stack>
                        {notifications.map((notification, index) => {
                            return (
                                <Stack
                                    key={index}
                                    width={"100%"}
                                    alignItems={"center"}
                                    direction={"row"}
                                    borderBottom={`solid 1px ${theme.palette.grey[300]}`}
                                    py={"20px"}
                                >
                                    <Stack
                                        direction={"row"}
                                        alignItems={"center"}
                                        gap={"16px"}
                                        width={"100%"}
                                    >
                                        <ProfileImage
                                            name={notification.nameUser}
                                            image={notification.pictureUser}
                                            width="50px"
                                            height="50px"
                                        />
                                        <Stack gap={"8px"}>
                                            <Typography
                                                variant="body1"
                                                fontWeight={"600"}
                                            >
                                                {capitalizeText(
                                                    notification.nameUser
                                                )}
                                            </Typography>
                                            <Typography variant="body1">
                                                {capitalizeText(
                                                    notification.hierarchyUser
                                                )}
                                            </Typography>
                                        </Stack>
                                    </Stack>
                                    <Stack width={"100%"} gap={"5px"}>
                                        <Typography
                                            fontSize={"12px"}
                                            color={"primary"}
                                            fontWeight={"700"}
                                        >
                                            {notification.categoria}
                                        </Typography>
                                        <Typography
                                            sx={{
                                                overflowWrap: "break-word",
                                                width: "200px",
                                            }}
                                        >
                                            {notification.texto}
                                        </Typography>
                                        {notification?.files?.length > 0 &&
                                            notification.files.map(
                                                (file, index) => (
                                                    <Link
                                                        href={file.url}
                                                        key={index}
                                                        target="_blank"
                                                    >
                                                        <Typography
                                                            fontSize={"12px"}
                                                            fontWeight={"700"}
                                                            color={"secondary"}
                                                        >
                                                            Baixar arquivo{" "}
                                                            {index + 1}
                                                        </Typography>
                                                    </Link>
                                                )
                                            )}
                                    </Stack>
                                    <Typography sx={{ pr: "10px" }}>
                                        {format(
                                            new Date(
                                                notification.dateNotification
                                            ),
                                            "dd/MM/yyyy"
                                        )}
                                    </Typography>
                                    <Stack
                                        alignItems={"center"}
                                        minWidth={"48px"}
                                        height={"48px"}
                                        borderRadius={"16px"}
                                        bgcolor={theme.palette.primary.main}
                                        justifyContent={"center"}
                                    >
                                        {notification.categoria ==
                                        "Campanha" ? (
                                            <StarBorder
                                                sx={{
                                                    color: theme.palette
                                                        .background.default,
                                                }}
                                            />
                                        ) : (
                                            <CalendarToday
                                                sx={{
                                                    color: theme.palette
                                                        .background.default,
                                                }}
                                            />
                                        )}
                                    </Stack>
                                    <Checkbox
                                        onClick={() =>
                                            handleOnSelect(
                                                notification.codNotification
                                            )
                                        }
                                        checked={selectedNotifications.includes(
                                            notification.codNotification
                                        )}
                                    />
                                </Stack>
                            );
                        })}
                    </Stack>
                    <Pagination
                        count={totalPages || 0}
                        page={page}
                        onChange={handleChange}
                        disabled={isLoading}
                    />
                </Stack>
            </Stack>
        </Drawer>
    );
}
