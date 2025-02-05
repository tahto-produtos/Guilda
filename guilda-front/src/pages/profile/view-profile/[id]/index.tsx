import {
  Button,
  Skeleton,
  Typography,
  useTheme,
  Stack,
  Divider,
  Box,
  alpha,
  Breadcrumbs,
  Link,
  Modal,
} from "@mui/material";
import { useRouter } from "next/router";
import React, { useContext, useEffect, useState, useRef } from "react";
import { toast } from "react-toastify";
import { useLoadingState } from "src/hooks";
import { getLayout } from "src/utils";
import { ContentCard } from "../../../../components/surfaces/content-card/content-card";
import { UserPersonaContext } from "../../../../contexts/user-persona/user-persona.context";
import { ProfileImage } from "../../../../components/data-display/profile-image/profile-image";
import Chip from "@mui/material/Chip";
import { ListFeedUseCase } from "../../../../modules/home-v2/use-cases/list-feed.use-case";
import { PostItem } from "../../../../components/data-display/post/post";
import { Post } from "../../../../typings/models/post.model";
import { BaseModal } from "../../../../components/feedback";
import { ListFollowUseCase } from "../../../../modules/follow/use-cases/list-follow.use-case";
import { FriendItem } from "../../../../modules/home-v2/fragments/friend-item/friend-item";
import { Friend } from "../../../../typings/models/friend.model";
import EditOutlined from "@mui/icons-material/EditOutlined";
import { PersonaShowUserUseCase } from "../../../../modules/personas/use-cases/person-show-user";
import { PersonaShowUser } from "../../../../typings";
import AddBoxOutlined from "@mui/icons-material/AddBoxOutlined";
import { CreateFollowUseCase } from "../../../../modules/follow/use-cases/create-follow.use-case";
import { Feed } from "src/modules/home-v2/fragments/feed/feed";
import { capitalizeText } from "src/utils/capitalizeText";
import LocationOn from "@mui/icons-material/LocationOn";
import BadgeOutlined from "@mui/icons-material/BadgeOutlined";
import { ListFriendUseCase } from "src/modules/follow/use-cases/list-friend-list.use-case";
import HomeOutlined from "@mui/icons-material/HomeOutlined";
import { title } from "process";

export default function ViewProfile() {
  const router = useRouter();
  const { id } = router.query;

  const { personaShowUser } = useContext(UserPersonaContext);
  const [open, setOpen] = useState(false);
const heightAll = "75px";

  const [modalContent, setModalContent] = useState<string | React.ReactNode>('');
  const [modalTitle, setModalTitle] = useState('');
  
  const handleOpen = (title: string, content: string) => {
    setModalTitle(title);
    setModalContent(content);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setModalContent('');
    setModalTitle('');
  };





  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const theme = useTheme();
  const [personaShowUserView, setPersonaShowUserView] =
    useState<PersonaShowUser | null>(personaShowUser);

  const [isTruncated, setIsTruncated] = useState(false);
  const [isTruncated2, setIsTruncated2] = useState(false);

  const textRef = useRef<HTMLSpanElement>(null); // Definindo o tipo do ref
  const textRef2 = useRef<HTMLSpanElement>(null); // Definindo o tipo do ref

  const checkTruncation = (tref: React.RefObject<HTMLSpanElement>, setTruncated: React.Dispatch<React.SetStateAction<boolean>>) => {
    if (tref.current) {
      const scrollWidth = tref.current.scrollWidth;
      const clientWidth = tref.current.clientWidth;
      console.log("Scroll Width:", scrollWidth);
      console.log("Client Width:", clientWidth);

      const isOverflowing = scrollWidth > clientWidth;


      setTruncated(isOverflowing);
    }
  };

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      checkTruncation(textRef, setIsTruncated);
      checkTruncation(textRef2, setIsTruncated2);
    }, 100); // Atraso de 100ms

    window.addEventListener('resize', () => {
      checkTruncation(textRef, setIsTruncated);
      checkTruncation(textRef2, setIsTruncated2);
    });

    return () => {
      clearTimeout(timeoutId);
      window.removeEventListener('resize', () => {
        checkTruncation(textRef, setIsTruncated);
        checkTruncation(textRef2, setIsTruncated2);
      });
    };
  }, [personaShowUserView?.OBJETIVO]);


  const [page, setPage] = useState<number>(1);
  const [isOpenModalFollowing, setIsOpenModalFollowing] =
    useState<boolean>(false);
  const [isOpenModalFollowers, setIsOpenModalFollowers] =
    useState<boolean>(false);
  const [followings, setFollowings] = useState<Friend[]>([]);
  const [followers, setFollowers] = useState<Friend[]>([]);
  const [follow, setFollow] = useState<boolean>(false);
  const [showInfos, setShowInfos] = useState<boolean>(false);
  const [showFeed, setShowFeed] = useState<boolean>(false);
  const [showAge, setShowAge] = useState<boolean>(
    personaShowUser && personaShowUser?.IDADE == 1 ? true : false
  );
  const [friends, setFriends] = useState<Friend[]>([]);

  const FEED_POST_LIMIT = 5;

  function getFriends() {
    if (!personaShowUserView) return;
    startLoading();

    new ListFriendUseCase()
      .handle({
        friend: "",
        tierList: true,
        personaId: personaShowUserView.ID_PERSON_ACCOUNT,
      })
      .then((data) => {
        setFriends(data);
      })
      .catch(() => {
        //
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    personaShowUserView && getFriends();
  }, [personaShowUserView]);

  async function getPersonaShowUser() {
    await new PersonaShowUserUseCase()
      .handle({ id: Number(id) })
      .then((data) => {
        setFollow(data[0].FOLLOWED_BY_ME);
        setPersonaShowUserView({
          ...data[0],
          ID_PERSON_ACCOUNT: Number(id),
        });
        setShowAge(data[0].IDADE == 1 ? true : false);
      })
      .catch(() => { });
  }

  useEffect(() => {
    if (personaShowUser?.ID_PERSON_ACCOUNT != Number(id)) {
      getPersonaShowUser();
    } else {
      setPersonaShowUserView(personaShowUser);
    }
  }, [id, personaShowUser]);

  useEffect(() => {
    if (personaShowUser?.ID_PERSON_ACCOUNT == Number(id)) {
      setShowInfos(true);
      setShowFeed(true);
    } else if (personaShowUserView?.ISPUBLIC == 1) {
      setShowInfos(true);
      setShowFeed(true);
    } else if (personaShowUserView?.ISPUBLIC == 0) {
      setShowInfos(false);
      if (follow) {
        setShowFeed(true);
      }
    }
  }, [id, personaShowUserView]);

  const handleClickModalFollowing = () => {
    setIsOpenModalFollowing(!isOpenModalFollowing);
  };

  const handleClickModalFollowers = () => {
    setIsOpenModalFollowers(!isOpenModalFollowers);
  };

  function getFollowings(follow: boolean) {
    startLoading();

    new ListFollowUseCase()
      .handle({
        follow: follow,
        limit: 1000,
        page: 1,
        filterName: "",
        idPersona: personaShowUserView?.ID_PERSON_ACCOUNT || 0,
      })
      .then((data) => {
        if (follow) {
          setFollowings(data.listsFollows);
        } else {
          setFollowers(data.listsFollows);
        }
      })
      .catch(() => {
        //
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    getFollowings(true);
  }, [personaShowUserView]);

  useEffect(() => {
    getFollowings(false);
  }, [personaShowUserView]);

  const handleClickFollowUnfollow = async () => {
    if (!personaShowUserView) return null;
    startLoading();

    await new CreateFollowUseCase()
      .handle({
        follow: !personaShowUserView?.FOLLOWED_BY_ME,
        idFollowed: personaShowUserView?.ID_PERSON_ACCOUNT,
      })
      .then(() => {
        if (personaShowUserView?.ISPUBLIC == 1) {
          setShowInfos(true);
          setShowFeed(true);
        } else if (personaShowUserView?.ISPUBLIC == 0) {
          setShowInfos(false);
          if (!follow) {
            setShowFeed(true);
          }
        }
        setFollow(!follow);
      })
      .catch(() => {
        toast.error("Falha ao seguir o usuário");
      })
      .finally(() => {
        finishLoading();
      });
  };

  if (!personaShowUserView || !id) return null;

  return (
    <div
      style={{
        width: "100%",
        display: "flex",
        flexDirection: "column",
        backgroundColor: theme.palette.background.paper,
        borderRadius: "16px",
      }}
    >
      <div
        style={{
          width: "100%",
          height: "225px",
          backgroundColor: "#2FAC9F",
          borderRadius: "13px 13px 0 0",
        }}
      >
        <Stack
          width={"100%"}
          sx={{ borderRadius: "13px 13px 0 0" }}
          px={"24px"}
          height={"72px"}
          alignItems={"center"}
          direction={"row"}
          bgcolor={alpha(theme.palette.background.default, 0.2)}
        >
          <Breadcrumbs aria-label="breadcrumb">
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
              underline="hover"
              sx={{ display: "flex", alignItems: "center" }}
              color={theme.palette.background.default}
            >
              <Typography fontWeight={"700"}>
                {personaShowUserView?.NOME_SOCIAL || personaShowUserView.NOME}
              </Typography>
            </Link>
          </Breadcrumbs>
        </Stack>
      </div>
      {/* <Image
                src={Logo}
                alt="Tahto"
                width={150}
                height={80}
                style={{ marginBottom: 30 }}
                
            /> */}
      <ContentCard>
        {personaShowUserView ? (
          <ProfileImage
            width="110px"
            height="110px"
            marginTop="-80px"
            color={theme.palette.grey[200]}
            image={personaShowUserView.FOTO}
          />
        ) : (
          <Skeleton variant="circular" width={"110px"} height={"110px"} />
        )}
      </ContentCard>
      {personaShowUserView ? (
        <ContentCard sx={{ flexDirection: "column", gap: "28px" }}>
          <Stack gap={"10px"} direction={"row"}>
            <Stack width={"25%"} height={heightAll} gap={"7px"}>
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {personaShowUserView?.NOME_SOCIAL || personaShowUserView.NOME}
              </Typography>
              <Typography variant="subtitle2" fontSize={"12px"}>
                {personaShowUserView?.QUEM_E}
              </Typography>
            </Stack>
            <Stack
              width={"50%"}
              height={heightAll}
              //border={`solid 1px ${theme.palette.grey[400]}`}
              //borderRadius={"8px"}
              gap={"7px"}
            >
              <Typography
                variant="body1"
                fontWeight={"600"}
                fontSize={"15px"}

              >
                {showInfos ? "Localização:" : ""}
              </Typography>
              <Stack direction={"row"} gap={"5px"} alignItems={"center"}>
                <LocationOn color="secondary" />
                <Typography
                  variant="body1"
                  fontSize={"16px"}
                  fontWeight={"500"}
                  color={"secondary"}
                >

                </Typography>
              </Stack>
            </Stack>
            {/*             <Stack
              width={"40%"}
              height={"49px"}
              //border={`solid 1px ${theme.palette.grey[400]}`}
              //borderRadius={"8px"}
              gap={"7px"}
            >
              <Typography
                variant="body1"
                fontWeight={"400"}
                fontSize={"14px"}
                color={"text.secondary"}
              >
                {showInfos ? "Matrícula:" : ""}
              </Typography>
              <Stack direction={"row"} gap={"5px"} alignItems={"center"}>
                <BadgeOutlined color="secondary" />
                <Typography
                  variant="body1"
                  fontSize={"16px"}
                  fontWeight={"500"}
                  color={"secondary"}
                >
                  {showInfos ? personaShowUserView?.BC : ""}
                </Typography>
              </Stack>
            </Stack> */}
            {/*             <Stack
              width={"100%"}
              height={"49px"}
              //border={`solid 1px ${theme.palette.grey[400]}`}
              //borderRadius={"8px"}
              justifyContent={"center"}
              alignItems={"right"}
            ></Stack> */}
            <Stack
              //minWidth={"fit-content"}
              width={"25%"}
              height={heightAll}
              //border={`solid 1px ${theme.palette.grey[400]}`}
              //borderRadius={"8px"}
              justifyContent={"center"}
              alignItems={"right"}
            >
              {personaShowUser?.ID_PERSON_ACCOUNT == Number(id) ? (
                <Button
                  variant="outlined"
                  //title={"Editar Perfil"}
                  //isActive={true}
                  sx={{
                    alignItems: "center",
                    display: "flex",
                    gap: "10px",
                  }}
                  onClick={() =>
                    router.push(
                      `/profile/edit-profile/${personaShowUser?.ID_PERSON_ACCOUNT}`
                    )
                  }
                >
                  Editar conta
                </Button>
              ) : (
                <Button
                  variant="contained"
                  //title={"Editar Perfil"}
                  //isActive={false}
                  sx={{
                    alignItems: "center",
                    display: "flex",
                    gap: "10px",
                  }}
                  onClick={() => handleClickFollowUnfollow()}
                >
                  <AddBoxOutlined />
                  {follow ? "Seguindo" : "Seguir"}
                </Button>
              )}
            </Stack>
          </Stack>

          <Stack gap={"10px"} direction={"row"}>
            <Stack
              width={"25%"}
              height={heightAll}
            //border={`solid 1px ${theme.palette.grey[400]}`}
            //borderRadius={"8px"}
            //alignItems={"center"}
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                Nome:
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {personaShowUserView?.NOME}
              </Typography>
            </Stack>
            <Stack
              width={"25%"}
              height={heightAll}
            //border={`solid 1px ${theme.palette.grey[400]}`}
            //borderRadius={"8px"}
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showInfos ? "Motivações:" : ""}
              </Typography>
              <Stack direction="row" alignItems="center" justifyContent="space-between" width="100%">

              <Typography
                ref={textRef2}
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
                sx={{
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap', // Garante que o texto seja truncado com "..."
                  flexGrow: 1, // O texto ocupa o espaço restante
                }}
              >
                {showInfos ? personaShowUserView?.MOTIVACOES : ""}
              </Typography>
              {isTruncated2 && (<Button
                onClick={() => handleOpen('Motivações', personaShowUserView?.MOTIVACOES)}
                size="small"
                sx={{
                  minWidth: 'fit-content',
                  color: 'secondary.main',
                  fontSize: '12px',
                  paddingTop: '0px',
                  textTransform: 'none', // Remover capitalização automática do botão
                  '&:hover': {
                    color: 'red',
                  },
                  marginLeft: '8px' // Espaçamento entre o texto e o botão
                }}
              >
                Ver mais
              </Button>
              )}
              </Stack>
            </Stack>
            <Stack
              width={"25%"}
              height={heightAll} // Definir a altura consistente para todos os campos
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showInfos ? "Objetivos:" : ""}
              </Typography>

              <Stack direction="row" alignItems="center" justifyContent="space-between" width="100%">
                <Typography
                  ref={textRef}
                  variant="subtitle2"
                  fontSize={"12px"}
                  color={"secondary"}
                  sx={{
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap', // Garante que o texto seja truncado com "..."
                    flexGrow: 1, // O texto ocupa o espaço restante
                  }}
                >
                  {showInfos ? personaShowUserView?.OBJETIVO : ""}
                </Typography>
                {isTruncated && (<Button
                  onClick={() => handleOpen('Objetivos', personaShowUserView?.OBJETIVO)}
                  size="small"
                  sx={{
                    minWidth: 'fit-content',
                    color: 'secondary.main',
                    fontSize: '12px',
                    paddingTop: '0px',
                    textTransform: 'none', // Remover capitalização automática do botão
                    '&:hover': {
                      color: 'red',
                    },
                    marginLeft: '8px' // Espaçamento entre o texto e o botão
                  }}
                >
                  Ver mais
                </Button>
                )}
              </Stack>

            </Stack>
            <Stack width={"25%"} height={heightAll}>
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showAge ? "Idade:" : ""}
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {showAge ? personaShowUserView?.IDADE_CALCULADA : ""}
              </Typography>
            </Stack>
            {/*<Stack width={"40%"} height={"49px"}>
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showInfos ? "E-mail:" : ""}
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {showInfos ? personaShowUserView?.EMAIL : ""}
              </Typography>
            </Stack>*/}
            <Modal
              open={open}
              onClose={handleClose}
            >
              <Stack
                sx={{
                  position: 'absolute',
                  top: '50%',
                  left: '50%',
                  transform: 'translate(-50%, -50%)',
                  width: 400,
                  bgcolor: 'background.paper',
                  border: '2px solid #000',
                  boxShadow: 24,
                  p: 4,
                }}
              >
                <Typography
                  variant="h6"
                  component="h2"
                >
                  {modalTitle}
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                >
                  {modalContent}
                </Typography>
                <Button onClick={handleClose}>Fechar</Button>
              </Stack>
            </Modal>
          </Stack>

          <Stack gap={"10px"} direction={"row"}>
            <Stack
              width={"25%"}
              height={heightAll}
            //border={`solid 1px ${theme.palette.grey[400]}`}
            //borderRadius={"8px"}
            //alignItems={"center"}
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                Hobby:
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {personaShowUserView?.HOBBIES?.map((hobby) => hobby.HOBBY).join(
                  "; "
                )}
              </Typography>
            </Stack>
            {/* <Stack
              width={"40%"}
              height={"49px"}
              //border={`solid 1px ${theme.palette.grey[400]}`}
              //borderRadius={"8px"}
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showInfos ? "Telefone:" : ""}
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {showInfos ? personaShowUserView?.TELEFONE : ""}
              </Typography>
            </Stack> */}

            <Stack
              width={"25%"}
              height={heightAll}
            //border={`solid 1px ${theme.palette.grey[400]}`}
            //borderRadius={"8px"}
            >
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                {showInfos ? "UF:" : ""}
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {showInfos ? personaShowUserView?.UF : ""}
              </Typography>
            </Stack>
            <Stack width={"25%"} height={heightAll}>
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                Setor:
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {personaShowUserView?.SETOR}
              </Typography>
            </Stack>
            <Stack width={"25%"} height={heightAll}>
              <Typography
                variant="subtitle1"
                fontWeight={"600"}
                fontSize={"15px"}
              >
                Cargo:
              </Typography>
              <Typography
                variant="subtitle2"
                fontSize={"12px"}
                color={"secondary"}
              >
                {personaShowUserView?.CARGO}
              </Typography>
            </Stack>
          </Stack>
        </ContentCard>
      ) : null}

      <Divider />
      <ContentCard sx={{ flexDirection: "column", gap: "28px", mt: "20px" }}>
        <Stack gap={"40px"} direction={"row"}>
          <Stack width={"100%"}>
            <Typography variant="h2" fontSize={"24px"} ml={"24px"}>
              Postagens
            </Typography>
            {showFeed && (
              <Feed userFeedId={personaShowUserView.ID_PERSON_ACCOUNT} />
            )}
          </Stack>
          <Divider
            orientation="vertical"
            sx={{
              height: "100%",
              width: "7px",
              bgcolor: theme.palette.secondary.main,
              borderRadius: "18px",
            }}
          />
          <Stack
            direction={"column"}
            minWidth={"300px"}
            gap={"24px"}
          //border={`solid 1px ${theme.palette.grey[400]}`}
          //borderRadius={"8px"}
          >
            <Typography variant="h2" fontSize={"16px"}>
              Amigos
            </Typography>
            <Stack gap={"20px"}>
              {friends.map((item, index) => (
                <FriendItem data={item} key={index} nameIsLink={false} />
              ))}
            </Stack>

            <Stack
              width={"100%"}
              direction={"row"}
              gap={"26px"}
              sx={{ cursor: "pointer", mt: "40px" }}
            >
              <Stack onClick={() => handleClickModalFollowing()}>
                <Typography
                  variant="h1"
                  fontSize={"30px"}
                  color={"secondary"}
                  fontWeight={"700"}
                >
                  {personaShowUserView?.FOLLOWING}
                </Typography>
                <Typography variant="body1">Seguindo</Typography>
              </Stack>
              <BaseModal
                title={"Seguindo"}
                open={isOpenModalFollowing}
                onClose={() => handleClickModalFollowing()}
                fullWidth={true}
              >
                <Box
                  mt={3}
                  display={"flex"}
                  flexDirection={"column"}
                  gap={"30px"}
                >
                  {followings.map((item, index) => (
                    <FriendItem data={item} key={index} nameIsLink={true} />
                  ))}
                </Box>
              </BaseModal>
              <Divider
                orientation="vertical"
                sx={{
                  height: "100%",
                  width: "2px",
                  bgcolor: theme.palette.secondary.main,
                  borderRadius: "18px",
                }}
              />
              <Stack onClick={() => handleClickModalFollowers()}>
                <Typography
                  variant="h1"
                  fontSize={"30px"}
                  fontWeight={"700"}
                  color={"secondary"}
                >
                  {personaShowUserView?.FOLLOWERS}
                </Typography>
                <Typography variant="body1">Seguidores</Typography>
              </Stack>
              <BaseModal
                title={"Seguidores"}
                open={isOpenModalFollowers}
                onClose={() => handleClickModalFollowers()}
                fullWidth={true}
              >
                <Box
                  mt={3}
                  display={"flex"}
                  flexDirection={"column"}
                  gap={"30px"}
                >
                  {followers.map((item, index) => (
                    <FriendItem data={item} key={index} nameIsLink={true} />
                  ))}
                </Box>
              </BaseModal>
            </Stack>
          </Stack>
        </Stack>
      </ContentCard>
    </div>
  );
}

ViewProfile.getLayout = getLayout("private");
