import Send from "@mui/icons-material/Send";
import DeleteOutline from "@mui/icons-material/DeleteOutline";
import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  Chip,
  Divider,
  Drawer,
  IconButton,
  InputAdornment,
  Pagination,
  Select,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { CreateNotificationModal } from "src/modules/notifications/fragments/create-notification-modal";
import { ListLibraryNotifications } from "src/modules/notifications/use-cases/list-library-notifications.use-case";
import { DeleteHobbyUseCase } from "src/modules/personas/use-cases/delete-hobby.use-case";
import { ListHobbyUseCase } from "src/modules/personas/use-cases/list-hobby.use-case";
import { Hobby } from "src/typings/models/hobby.model";
import { LibraryNotification } from "src/typings/models/library-notification.model";
import { getLayout } from "src/utils";
import { SendNotificationUseCase } from "src/modules/notifications/use-cases/send-notification.use-case";
import { DeleteNotificationsUseCase } from "src/modules/notifications/use-cases/delete-notification.use-case";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import { ActiveNotificationsUseCase } from "src/modules/notifications/use-cases/active-notification.use-case";
import {
  DatePicker,
  LocalizationProvider,
  TimePicker,
} from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, set, setWeek } from "date-fns";
import { GeneralDeleteNotificationsUseCase } from "src/modules/notifications/use-cases/general-cleaning-notification.use-case";
import { usePagination } from "src/hooks/use-pagination/use-pagination";
import PaginationComponent from "src/components/navigation/pagination/pagination";
import Link from "next/link";
import { EditNotificationUseCase } from "src/modules/notifications/use-cases/edit-notification.use-case";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import abilityFor from "src/utils/ability-for";
import { FilterList, NotificationsOutlined } from "@mui/icons-material";
import { capitalizeText } from "src/utils/capitalizeText";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";

export default function NotificationsView() {
  const { myPermissions } = useContext(PermissionsContext);

  const [notifications, setNotifications] = useState<LibraryNotification[]>([]);
  const [selectedToEdit, setSelectedToEdit] =
    useState<LibraryNotification | null>(null);
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [searchText, setSearchText] = useState<string>("");
  const debouncedSearchText: string = useDebounce<string>(searchText, 400);
  const router = useRouter();
  const [isOpenNew, setIsOpenNew] = useState<boolean>(false);
  const { personaShowUser } = useContext(UserPersonaContext);
  const [isLoadingSend, setIsLoadingSend] = useState<boolean>(false);
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
  const [isOpenDelete, setIsOpenDelete] = useState<number | null>(null);
  const [isOpenDelete1, setIsOpenDelete1] = useState(false);
  const [isOpenDelete2, setIsOpenDelete2] = useState(false);
  const [fromHour, setFromHour] = useState<Date | null>(null);
  const [toHour, setToHour] = useState<Date | null>(null);

  const [titleEdit, setTitleEdit] = useState<string>("");
  const [msgEdit, setMsgEdit] = useState<string>("");
  const [startDateEdit, setStartDateEdit] = useState<Date | null>(null);
  const [endDateEdit, setEndDateEdit] = useState<Date | null>(null);
  const [fromHourEdit, setFromHourEdit] = useState<Date | null>(null);
  const [toHourEdit, setToHourEdit] = useState<Date | null>(null);
  const [notificationType, setNotificationType] = useState<any[]>([
    {
      IDGDA_NOTIFICATION_TYPE: 5,
      TYPE: "Informação",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 6,
      TYPE: "Evento",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 3,
      TYPE: "Campanha",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 10,
      TYPE: "Novidade",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 11,
      TYPE: "Comunicados internos",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 12,
      TYPE: "Atualização de status",
    },
    {
      IDGDA_NOTIFICATION_TYPE: 13,
      TYPE: "Lembretes e alertas",
    },
  ]);
  const [notificationTypeSelected, setNotificationTypeSelected] =
    useState<any>();

  // filters
  const [createdAtFrom, setCreatedAtFrom] = useState<dateFns | null>(null);
  const [createdAtTo, setCreatedAtTo] = useState<dateFns | null>(null);
  const [endedAtFrom, setEndedAtFrom] = useState<dateFns | null>(null);
  const [nameCreator, setNameCreator] = useState<string>("");
  const [scheduling, setScheduling] = useState<boolean>(false);
  const [startedAtFrom, setStartedAtFrom] = useState<dateFns | null>(null);
  const [startedAtTo, setStartedAtTo] = useState<dateFns | null>(null);
  const [word, setWord] = useState<string>("");
  ///////

  const [isOpenFilter, setIsOpenFilter] = useState<boolean>(false);

  const { handleChange, page, setPage, setTotalPages, totalPages } =
    usePagination();
  const LIMIT = 10;

  const [selectedNotifications, setSelectedNotifications] = useState<number[]>(
    []
  );

  function handleOnSelect(id: number) {
    let newArr = [...selectedNotifications];

    if (newArr.includes(id)) {
      newArr = newArr.filter((item) => item !== id);
    } else {
      newArr.push(id);
    }

    setSelectedNotifications(newArr);
  }

  useEffect(() => {
    if (selectedToEdit) {
      setTitleEdit(selectedToEdit.title);
      setMsgEdit(selectedToEdit.notification);
      setStartDateEdit(
        selectedToEdit.startedAt ? new Date(selectedToEdit.startedAt) : null
      );
      setEndDateEdit(
        selectedToEdit.endedAt ? new Date(selectedToEdit.endedAt) : null
      );
      setFromHourEdit(
        selectedToEdit.startedAt ? new Date(selectedToEdit.startedAt) : null
      );
      setToHourEdit(
        selectedToEdit.endedAt ? new Date(selectedToEdit.endedAt) : null
      );
    }
  }, [selectedToEdit]);

  const [isOpenAtivate, setIsOpenActivate] = useState<{
    active: boolean;
    id: number;
  } | null>(null);

  const theme = useTheme();

  async function listNitificationsLibrary() {
    startLoading();

    new ListLibraryNotifications()
      .handle({
        limit: LIMIT,
        page: page,
        idCreator: 0,
        title: searchText,
        createdAtFrom: createdAtFrom
          ? format(new Date(createdAtFrom.toString()), "yyyy-MM-dd")
          : "",
        createdAtTo: createdAtTo
          ? format(new Date(createdAtTo.toString()), "yyyy-MM-dd")
          : "",
        endedAtFrom: endedAtFrom
          ? format(new Date(endedAtFrom.toString()), "yyyy-MM-dd")
          : "",
        nameCreator: nameCreator,
        scheduling: scheduling,
        type: notificationTypeSelected ? notificationTypeSelected.TYPE : "",
        startedAtFrom: startedAtFrom
          ? format(new Date(startedAtFrom.toString()), "yyyy-MM-dd")
          : "",
        startedAtTo: startedAtTo
          ? format(new Date(startedAtTo.toString()), "yyyy-MM-dd")
          : "",
        word: word,
      })
      .then((data) => {
        setTotalPages(data.totalpages);
        setNotifications(data.LibraryNotification);
      })
      .catch(() => {
        toast.error("Erro ao listar.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    listNitificationsLibrary();
  }, [
    debouncedSearchText,
    createdAtFrom,
    createdAtTo,
    endedAtFrom,
    nameCreator,
    scheduling,
    startedAtFrom,
    startedAtTo,
    word,
    page,
    notificationTypeSelected,
  ]);

  async function handleDelete() {
    if (!isOpenDelete) return;

    startLoading();

    new DeleteNotificationsUseCase()
      .handle({
        IDGDA_NOTIFICATION: isOpenDelete,
        VALIDADETED: true,
      })
      .then(() => {
        listNitificationsLibrary();
        toast.success("Notificação apagada com sucesso!");
      })
      .catch(() => {
        toast.error("Erro ao apagar o hobby.");
      })
      .finally(() => {
        finishLoading();
      });
  }

  function handleSend(cod: number) {
    setIsLoadingSend(true);
    new SendNotificationUseCase()
      .handle({
        NOTIFICATION_ID: cod,
      })
      .then((data) => {
        toast.success("Enviado com sucesso!");
      })
      .catch(() => {
        toast.error("Erro ao listar.");
      })
      .finally(() => {
        setIsLoadingSend(false);
      });
  }

  function handleActive(active: boolean, id: number) {
    const fromHourDateTime =
      fromHour &&
      startDate &&
      set(startDate, {
        hours: fromHour.getHours(),
        minutes: fromHour.getMinutes(),
      });

    const toHourDateTime =
      toHour &&
      endDate &&
      set(endDate, {
        hours: toHour.getHours(),
        minutes: toHour.getMinutes(),
      });

    new ActiveNotificationsUseCase()
      .handle({
        ACTIVATED: active == true ? 1 : 0,
        ENDED_AT: toHourDateTime
          ? format(toHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        STARTED_AT: fromHourDateTime
          ? format(fromHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        NOTIFICATION_ID: id,
      })
      .then(() => {
        toast.success(`${active ? "Ativado" : "Desativado"} com sucesso!`);
        listNitificationsLibrary();
        setIsOpenActivate(null);
      })
      .catch(() => {
        toast.error("Erro ao ativar.");
      })
      .finally(() => {
        setIsLoadingSend(false);
      });
  }

  function handleClean(deleteAll: boolean) {
    new GeneralDeleteNotificationsUseCase()
      .handle({
        ids: deleteAll
          ? notifications.map((i) => i.codNotification)
          : selectedNotifications,
      })
      .then(() => {
        toast.success(`Limpeza realizada com sucesso!`);
        listNitificationsLibrary();
        setIsOpenDelete1(false);
        setIsOpenDelete2(false);
        setSelectedNotifications([]);
      })
      .catch(() => {
        toast.error("Erro ao apagar.");
      })
      .finally(() => {
        setIsLoadingSend(false);
      });
  }

  function handleEdit() {
    if (!selectedToEdit) return;

    const fromHourDateTime =
      fromHourEdit &&
      startDateEdit &&
      set(startDateEdit, {
        hours: fromHourEdit.getHours(),
        minutes: fromHourEdit.getMinutes(),
      });

    const toHourDateTime =
      toHourEdit &&
      endDateEdit &&
      set(endDateEdit, {
        hours: toHourEdit.getHours(),
        minutes: toHourEdit.getMinutes(),
      });

    new EditNotificationUseCase()
      .handle({
        ACTIVATED: selectedToEdit.active as 0 | 1,
        ENDED_AT: toHourDateTime
          ? format(toHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        STARTED_AT: fromHourDateTime
          ? format(fromHourDateTime, "yyyy-MM-dd HH:mm")
          : "",
        NOTIFICATION: msgEdit,
        NOTIFICATION_ID: selectedToEdit.codNotification,
        TITLE: titleEdit,
      })
      .then(() => {
        toast.success(`Editado com sucesso!`);
        listNitificationsLibrary();
        setSelectedToEdit(null);
      })
      .catch(() => {
        toast.error("Erro ao editar.");
      })
      .finally(() => {});
  }

  return (
    <ContentCard>
      <ContentArea>
        <PageTitle
          icon={<NotificationsOutlined sx={{ fontSize: "36px" }} />}
          title="Gerenciar Notificações"
          loading={isLoading}
        >
          <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"16px"}
            width={"100%"}
            justifyContent={"flex-end"}
          >
            <TextField
              label="Busque por notificações"
              size="small"
              sx={{
                maxWidth: "250px",
                width: "100%",
              }}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <SearchIcon
                      width={12}
                      height={12}
                      color={theme.palette.text.primary}
                    />
                  </InputAdornment>
                ),
              }}
            />
            <Button variant="contained" onClick={() => setIsOpenNew(true)}>
              Criar notificação
            </Button>
            <Button variant="contained" onClick={() => setIsOpenFilter(true)}>
              Filtrar
            </Button>
          </Stack>
        </PageTitle>
        <Divider />
        <Stack direction={"column"} gap={"20px"} mt={"20px"}>
          <Stack direction={"row"} justifyContent={"flex-end"} width={"100%"}>
            {selectedNotifications.length > 0 && (
              <Button
                color="error"
                variant="contained"
                size="small"
                onClick={() => setIsOpenDelete1(true)}
              >
                Apagar selecionados
              </Button>
            )}
            <ConfirmModal
              onClose={() => setIsOpenDelete1(false)}
              onConfirm={() => handleClean(false)}
              open={isOpenDelete1}
            />
            <Button
              color="secondary"
              sx={{ fontWeight: "700", color: "secondary.main" }}
              onClick={() => {
                setSelectedNotifications(
                  notifications.map((item) => item.codNotification)
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
          {notifications.length > 0 || isLoading ? (
            notifications.map((item, index) => (
              <Box
                key={index}
                border={`solid 1px ${
                  item.sendedAt
                    ? theme.palette.secondary.main
                    : theme.palette.primary.main
                }`}
                display={"flex"}
                width={"100%"}
                borderRadius={"16px"}
                p={"24px"}
                alignItems={"center"}
                justifyContent={"space-between"}
              >
                <Box
                  display={"flex"}
                  alignItems={"center"}
                  gap={2}
                  width={"100%"}
                >
                  <Box width={"100%"}>
                    <Stack
                      width={"100%"}
                      justifyContent={"space-between"}
                      alignItems={"center"}
                      direction={"row"}
                    >
                      <Stack>
                        <Typography
                          variant="body2"
                          fontSize={"16px"}
                          fontWeight={"600"}
                          fontFamily={"Montserrat"}
                        >
                          {capitalizeText(item.title || "")}
                        </Typography>
                        <Typography
                          variant="body2"
                          fontSize={"13px"}
                          color={"text.secondary"}
                          mt={"8px"}
                          maxWidth={"500px"}
                          sx={{
                            overflowWrap: "break-word",
                          }}
                        >
                          {item.notification}
                        </Typography>
                        <Typography
                          variant="body2"
                          fontSize={"14px"}
                          mt={"8px"}
                        >
                          Criado em:{" "}
                          {format(new Date(item.createdAt), "dd/MM/yyyy")} -
                          Inícia em:{" "}
                          {item.startedAt
                            ? format(new Date(item.startedAt), "dd/MM/yyyy")
                            : "-"}{" "}
                          - Finaliza em:{" "}
                          {item.endedAt
                            ? format(new Date(item.endedAt), "dd/MM/yyyy")
                            : "-"}{" "}
                          - Enviado em:{" "}
                          {item.sendedAt
                            ? format(new Date(item.sendedAt), "dd/MM/yyyy")
                            : "-"}
                        </Typography>
                      </Stack>
                      <Box display={"flex"} gap={1} alignItems={"center"}>
                        <Stack
                          direction={"row"}
                          gap={"10px"}
                          alignItems={"center"}
                        >
                          {/* {abilityFor(myPermissions).can(
                    "Editar Notificacao",
                    "Notificacao"
                  ) &&
                    item.edit == 1 && ( */}
                          {item.edit == 1 && (
                            <Button
                              variant="outlined"
                              color="inherit"
                              onClick={() => setSelectedToEdit(item)}
                            >
                              Editar
                            </Button>
                          )}
                          {item.active == 1 ? (
                            <Button
                              variant="contained"
                              color="error"
                              onClick={() =>
                                setIsOpenActivate({
                                  active: false,
                                  id: item.codNotification,
                                })
                              }
                            >
                              Desativar
                            </Button>
                          ) : (
                            <Button
                              variant="contained"
                              color="success"
                              onClick={() =>
                                setIsOpenActivate({
                                  active: true,
                                  id: item.codNotification,
                                })
                              }
                            >
                              Ativar
                            </Button>
                          )}
                          {/* {abilityFor(myPermissions).can(
                    "Enviar Notificacao",
                    "Notificacao"
                  ) && ( */}
                          <IconButton
                            size="small"
                            onClick={() => handleSend(item.codNotification)}
                            disabled={isLoadingSend}
                          >
                            <Send fontSize="small" />
                          </IconButton>
                          {/*  )} */}
                          {/* {abilityFor(myPermissions).can(
                    "Limpar Notificacao",
                    "Notificacao"
                  ) && ( */}
                          <IconButton
                            size="small"
                            onClick={() =>
                              setIsOpenDelete(item.codNotification)
                            }
                          >
                            <DeleteOutline fontSize="small" />
                          </IconButton>
                          {/* )} */}
                          <Checkbox
                            onClick={() => handleOnSelect(item.codNotification)}
                            checked={selectedNotifications.includes(
                              item.codNotification
                            )}
                          />
                        </Stack>
                      </Box>
                    </Stack>
                    <Stack
                      direction={"row"}
                      alignItems={"center"}
                      mt={"30px"}
                      gap={"83px"}
                    >
                      <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"9px"}
                      >
                        <ProfileImage
                          width="50px"
                          height="50px"
                          name={item.nameCreator}
                        />
                        <Typography fontSize={"14px"} fontWeight={"600"}>
                          {capitalizeText(item.nameCreator || "Não informado")}
                        </Typography>
                      </Stack>
                      <Stack direction={"column"} gap={"9px"}>
                        <Typography fontSize={"14px"} fontWeight={"400"}>
                          Status
                        </Typography>
                        <Typography
                          fontSize={"14px"}
                          fontWeight={"700"}
                          color={item.sendedAt ? "secondary" : "primary"}
                        >
                          {item.sendedAt ? "Enviado" : "Não enviado"}
                        </Typography>
                      </Stack>
                    </Stack>
                    {/* <Typography
                                            variant="body2"
                                            fontSize={"13px"}
                                            mt={1}
                                            fontWeight={"500"}
                                        >
                                            Criado por: {quiz.nameCreator}{" "}
                                        </Typography>
                                        <Chip
                                            size="small"
                                            sx={{ mt: "10px" }}
                                            label={quiz.status}
                                        /> */}
                  </Box>
                </Box>
              </Box>
            ))
          ) : (
            <EmptyState
              title={
                searchText
                  ? `Não encontramos notificações com o nome "${searchText}"`
                  : `Nenhuma notificação encontrada`
              }
            />
          )}
          <Pagination
            count={totalPages || 0}
            page={page}
            onChange={handleChange}
            size="small"
          />
        </Stack>
        <Stack direction={"row"} mt={"20px"}>
          <Button
            variant="contained"
            color="error"
            onClick={() => setIsOpenDelete2(true)}
          >
            Apagar todas as notificações do resultado da busca
          </Button>
        </Stack>
      </ContentArea>
      <ConfirmModal
        onClose={() => setIsOpenDelete2(false)}
        onConfirm={() => handleClean(true)}
        open={isOpenDelete2}
      />
      <ConfirmModal
        onClose={() => setIsOpenDelete(null)}
        onConfirm={handleDelete}
        open={isOpenDelete}
      />
      {isOpenNew && (
        <CreateNotificationModal
          open={isOpenNew}
          onClose={() => setIsOpenNew(false)}
          refresh={listNitificationsLibrary}
        />
      )}
      {Boolean(isOpenAtivate) && isOpenAtivate && (
        <BaseModal
          width={"600px"}
          hideHeader
          open={Boolean(isOpenAtivate)}
          onClose={() => setIsOpenActivate(null)}
        >
          <Box
            sx={{
              padding: "20px 20px",
            }}
            gap={"10px"}
            display={"flex"}
            flexDirection={"column"}
          >
            <Stack direction={"row"} gap={"16px"}>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="Inicia em"
                  value={startDate}
                  onChange={(newValue) => setStartDate(newValue)}
                  slotProps={{
                    textField: {
                      sx: {
                        fontSize: "12px",
                        width: "230px",
                        height: "38px",
                        "& .MuiInputBase-input": {
                          fontSize: "12px",
                        },
                        svg: {
                          color: theme.palette.grey["500"],
                        },
                      },
                    },
                  }}
                />
              </LocalizationProvider>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <TimePicker
                  label="dás"
                  value={fromHour}
                  onChange={(newValue) => setFromHour(newValue)}
                />
              </LocalizationProvider>
            </Stack>
            <Stack direction={"row"} gap={"16px"}>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  label="Finaliza em"
                  value={endDate}
                  onChange={(newValue) => setEndDate(newValue)}
                  slotProps={{
                    textField: {
                      sx: {
                        fontSize: "12px",
                        width: "230px",
                        height: "38px",
                        "& .MuiInputBase-input": {
                          fontSize: "12px",
                        },
                        svg: {
                          color: theme.palette.grey["500"],
                        },
                      },
                    },
                  }}
                />
              </LocalizationProvider>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <TimePicker
                  label="ás"
                  value={toHour}
                  onChange={(newValue) => setToHour(newValue)}
                />
              </LocalizationProvider>
            </Stack>
            <Box display={"flex"} justifyContent={"center"} gap={2} mt={3}>
              <Button
                onClick={() => setIsOpenActivate(null)}
                size={"large"}
                variant={"outlined"}
              >
                Cancelar
              </Button>
              <Button
                onClick={() =>
                  handleActive(isOpenAtivate.active, isOpenAtivate.id)
                }
                variant={"contained"}
                size={"large"}
              >
                Confirmar
              </Button>
            </Box>
          </Box>
        </BaseModal>
      )}
      <BaseModal
        width={"560px"}
        title="Editar"
        open={Boolean(selectedToEdit)}
        onClose={() => setSelectedToEdit(null)}
      >
        <Box
          sx={{
            padding: "20px 20px",
            display: "flex",
            flexDirection: "column",
            gap: "20px",
          }}
        >
          <TextField
            fullWidth
            label="Título"
            value={titleEdit}
            onChange={(e) => setTitleEdit(e.target.value)}
          />
          <TextField
            fullWidth
            label="Mensagem"
            value={msgEdit}
            onChange={(e) => setMsgEdit(e.target.value)}
          />
          <Stack direction={"row"} gap={"16px"}>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="Inicia em"
                value={startDateEdit}
                onChange={(newValue) => setStartDateEdit(newValue)}
                slotProps={{
                  textField: {
                    sx: {
                      fontSize: "12px",
                      width: "230px",
                      height: "38px",
                      "& .MuiInputBase-input": {
                        fontSize: "12px",
                      },
                      svg: {
                        color: theme.palette.grey["500"],
                      },
                    },
                  },
                }}
              />
            </LocalizationProvider>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <TimePicker
                label="dás"
                value={fromHourEdit}
                onChange={(newValue) => setFromHourEdit(newValue)}
              />
            </LocalizationProvider>
          </Stack>
          <Stack direction={"row"} gap={"16px"}>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="Finaliza em"
                value={endDateEdit}
                onChange={(newValue) => setEndDateEdit(newValue)}
                slotProps={{
                  textField: {
                    sx: {
                      fontSize: "12px",
                      width: "230px",
                      height: "38px",
                      "& .MuiInputBase-input": {
                        fontSize: "12px",
                      },
                      svg: {
                        color: theme.palette.grey["500"],
                      },
                    },
                  },
                }}
              />
            </LocalizationProvider>

            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <TimePicker
                label="ás"
                value={toHourEdit}
                onChange={(newValue) => setToHourEdit(newValue)}
              />
            </LocalizationProvider>
          </Stack>
          <Box display={"flex"} justifyContent={"center"} gap={2} mt={3}>
            <Button
              onClick={() => setSelectedToEdit(null)}
              size={"large"}
              variant={"outlined"}
            >
              Cancelar
            </Button>
            <Button variant={"contained"} size={"large"} onClick={handleEdit}>
              Salvar
            </Button>
          </Box>
        </Box>
      </BaseModal>
      <Drawer
        open={isOpenFilter}
        anchor={"right"}
        onClose={() => setIsOpenFilter(false)}
        PaperProps={{
          sx: {
            borderTopLeftRadius: "16px",
            borderBottomLeftRadius: "16px",
          },
        }}
      >
        <Stack
          width={"100vw"}
          minWidth={"100%"}
          maxWidth={"430px"}
          direction={"column"}
          height={"100vh"}
          gap={"20px"}
          py={"40px"}
          px={"35px"}
        >
          <Typography
            fontSize={"20px"}
            fontWeight={"600"}
            alignItems={"center"}
            gap={"10px"}
            display={"flex"}
            flexDirection={"row"}
          >
            <FilterList />
            Filtar
          </Typography>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Criado em (de)"
              value={createdAtFrom}
              onChange={(newValue) => setCreatedAtFrom(newValue)}
              sx={{ width: "100%" }}
              slotProps={{
                textField: {
                  sx: {
                    width: "100%",
                    fontSize: "12px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Criado em (até)"
              value={createdAtTo}
              onChange={(newValue) => setCreatedAtTo(newValue)}
              sx={{ width: "100%" }}
              slotProps={{
                textField: {
                  sx: {
                    width: "100%",
                    fontSize: "12px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Iniciado em (de)"
              value={startedAtFrom}
              onChange={(newValue) => setStartedAtFrom(newValue)}
              sx={{ width: "100%" }}
              slotProps={{
                textField: {
                  sx: {
                    width: "100%",
                    fontSize: "12px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Iniciado em (até)"
              value={startedAtTo}
              onChange={(newValue) => setStartedAtTo(newValue)}
              sx={{ width: "100%" }}
              slotProps={{
                textField: {
                  sx: {
                    width: "100%",
                    fontSize: "12px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Finalizado em (até)"
              value={endedAtFrom}
              onChange={(newValue) => setEndedAtFrom(newValue)}
              sx={{ width: "100%" }}
              slotProps={{
                textField: {
                  sx: {
                    width: "100%",
                    fontSize: "12px",
                    "& .MuiInputBase-input": {
                      fontSize: "12px",
                    },
                    svg: {
                      color: theme.palette.grey["500"],
                    },
                  },
                },
              }}
            />
          </LocalizationProvider>
          <TextField
            fullWidth
            label="Palavra"
            value={word}
            onChange={(e) => setWord(e.target.value)}
          />
          <TextField
            fullWidth
            label="Nome do criador"
            value={nameCreator}
            onChange={(e) => setNameCreator(e.target.value)}
          />
          <Stack
            direction={"row"}
            alignItems={"center"}
            gap={"10px"}
            onClick={() => setScheduling(!scheduling)}
          >
            <Typography>Agendado:</Typography>
            <Checkbox checked={scheduling} />
          </Stack>
          <Autocomplete
            fullWidth
            disableClearable={false}
            value={notificationTypeSelected}
            options={notificationType}
            getOptionLabel={(option) => option.TYPE}
            onChange={(event, value) => {
              setNotificationTypeSelected(value);
            }}
            filterOptions={(x) => x}
            filterSelectedOptions
            renderInput={(props) => (
              <TextField {...props} label={"Tipo de notificação"} />
            )}
            renderOption={(props, option) => {
              return (
                <li {...props} key={option.id}>
                  {option.TYPE}
                </li>
              );
            }}
            isOptionEqualToValue={(option, value) =>
              option.IDGDA_NOTIFICATION_TYPE === value.IDGDA_NOTIFICATION_TYPE
            }
            sx={{ m: 0 }}
          />
        </Stack>
      </Drawer>
    </ContentCard>
  );
}

NotificationsView.getLayout = getLayout("private");
