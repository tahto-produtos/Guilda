import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  Divider,
  Link,
  IconButton,
  Stack,
  TextField,
  Typography,
  useTheme,
  CardMedia,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Persona } from "src/typings/models/persona.model";
import { EditPersonasUseCase } from "../../../personas/use-cases/edit-personas.use-case";
import { capitalizeText } from "src/utils/capitalizeText";
import { format } from "date-fns";
import { ListHobbyUseCase } from "../../../personas/use-cases/list-hobby.use-case";
import { Hobby } from "../../../../typings/models/hobby.model";
import { ProfileImage } from "../../../../components/data-display/profile-image/profile-image";
import { BaseModal } from "src/components/feedback";
import { removeEmojis } from "src/utils/removeEmojis";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import PictureInPicture from "@mui/icons-material/PictureInPicture";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";

interface EditProfileComponentProps {
  initialState?: Persona;
  idPersona: number;
}

export function EditProfileComponent(props: EditProfileComponentProps) {
  const { initialState, idPersona } = props;

  const { refreshPersona } = useContext(UserPersonaContext);

  const theme = useTheme();
  const [birthDate, setBirthDate] = useState<dateFns | Date | null>(
    initialState?.DATA_NASCIMENTO
      ? new Date(
        new Date(initialState?.DATA_NASCIMENTO).getFullYear(),
        new Date(initialState?.DATA_NASCIMENTO).getMonth(),
        new Date(initialState?.DATA_NASCIMENTO).getDate()
      )
      : null
  );
  const [checked, setChecked] = useState(false);

  const [name, setName] = useState<string>(initialState?.NOME || "");
  const [socialName, setSocialName] = useState<string>(
    initialState?.NOME_SOCIAL || ""
  );

  const [isOpenModalRulesProfileImage, setIsOpenModalRulesProfileImage] =
    useState<boolean>(false);
  const [image, setImage] = useState<File>();
  const [imageShow, setImageShow] = useState<string>(initialState?.FOTO || "");
  const [motivations, setMotivations] = useState<string>(
    initialState?.MOTIVACOES || ""
  );
  const [targets, setTargets] = useState<string>(initialState?.OBJETIVO || "");
  const [hobbies, setHobbies] = useState<Hobby[]>([]);
  const [hobbiesShow, setHobbiesShow] = useState<Hobby[]>([]);
  const [selectedHobbies, setSelectedHobbies] = useState<Hobby[]>(
    initialState?.HOBBIES || []
  );

  const [idade, setIdade] = useState<number>(initialState?.IDADE || 0);

  const fetchImageAsFile = async (imageUrl: string): Promise<File | null> => {
    try {
      const response = await fetch(imageUrl);
      const blob = await response.blob();

      // Extrair o nome do arquivo da URL
      const filename = imageUrl.split('/').pop() || 'default_filename';

      // Extrair o tipo MIME do blob
      const file = new File([blob], filename, { type: blob.type });

      return file;
    } catch (error) {
      console.error("Erro ao carregar a imagem:", error);
      return null;
    }
  };

  const handleCheck = (event: any) => {
    setChecked(event.target.checked);
    console.log(event.target.checked);
  };

  useEffect(() => {
    if (initialState?.FOTO) {
      console.log("Entrou");

      fetchImageAsFile(initialState.FOTO).then((file) => {
        if (file) {
          setImage(file);          
          setImageShow(initialState.FOTO || "");
        }
      });
    }
  }, [initialState?.FOTO]);

  async function listHobbies() {
    new ListHobbyUseCase()
      .handle({
        hobby: "",
      })
      .then((data) => {
        setHobbies(data);
        const notSelectedHobbiesList = data.filter(
          (hobby) =>
            !selectedHobbies.some(
              (selectedHobby) => selectedHobby.HOBBY == hobby.HOBBY
            )
        );
        setHobbiesShow(notSelectedHobbiesList);
      })
      .catch(() => {
        toast.error("Erro ao carregar os hobbies.");
      })
      .finally(() => { });
  }

  useEffect(() => {
    listHobbies();
  }, []);

  useEffect(() => {
    const notSelectedHobbiesList = hobbies.filter(
      (hobby) =>
        !selectedHobbies.some(
          (selectedHobby) => selectedHobby.HOBBY == hobby.HOBBY
        )
    );
    setHobbiesShow(notSelectedHobbiesList);
  }, [selectedHobbies]);

  const handleProfileImage = (event: any) => {
    const files = event.target.files;
    if (files.length > 0) {
      validateImage(files[0] as File).then((isValid) => {
        if (isValid) {
          setImage(files[0] as File);
          console.log("Files" + files[0]);
          setImageShow(URL.createObjectURL(files[0]));
          setIsOpenModalRulesProfileImage(false);
        }
      });
    }
  };

  function validateImage(file: File): Promise<boolean> {
    const MAX_WIDTH = 300;
    const MAX_HEIGHT = 300;
    const MAX_SIZE_MB = 5;
    const MAX_SIZE_BYTES = MAX_SIZE_MB * 1024 * 1024;

    return new Promise((resolve) => {
      if (file.size > MAX_SIZE_BYTES) {
        toast.warning("Tamanho máximo de image é de 5MB");
        resolve(false);
      } else {
        const reader = new FileReader();
        reader.readAsDataURL(file);

        reader.onload = (event) => {
          if (event.target) {
            const img = new Image();
            img.src = event.target.result as string;

            img.onload = () => {
              if (img.width > MAX_WIDTH || img.height > MAX_HEIGHT) {
                toast.warning(
                  "A imagem é muito grande, tamanho máximo de 300 X 300!"
                );
                resolve(false);
              } else {
                resolve(true);
              }
            };
          }
        };
      }
    });
  }

  async function handleSave() {
    console.log(initialState?.DATA_NASCIMENTO);
    await new EditPersonasUseCase()
      .handle({
        BC: initialState?.BC || "",
        CIDADE: Number(initialState?.ID_CIDADE),
        NOME_SOCIAL: socialName,
        DATA_NASCIMENTO: (() => {
          try {
            return initialState?.DATA_NASCIMENTO
              ? format(new Date(initialState?.DATA_NASCIMENTO), "yyyy-MM-dd")
              : "";
          } catch (error) {
            console.error("Erro ao formatar a data:", error);
            return ""; // Mantém o valor vazio ou podes definir outro valor padrão
          }
        })(), // Executa a lógica diretamente na chamada
        EMAIL: initialState?.EMAIL,
        FOTO: initialState?.FOTO,
        HOBBIES: selectedHobbies,
        IDADE: idade,
        IDPERSONAUSER: idPersona.toString(),
        MOTIVACOES: motivations,
        NOME: name,
        OBJETIVO: targets,
        SITE: initialState?.ID_SITE,
        TELEFONE: initialState?.TELEFONE,
        UF: Number(initialState?.ID_UF),
        WHO_IS: initialState?.QUEM_E,
        PROFILE_IMAGE: image,
      })
      .then((data) => {
        toast.success("Salvo com sucesso!");
        refreshPersona();
      })
      .catch((e) => {
        if (e.response.data.Message) {
          toast.error(e.response.data.Message);
        } else {
          toast.error("Erro ao salvar.");
        }
      })
      .finally(() => { });
  }

  const handleRemoveImage = () => {
    setImage(undefined);
    setImageShow("");
  }

  // @ts-ignore
  return (
    <Stack width={"100%"} mt={"25px"}>
      <Stack direction={"column"} gap={"30px"}>
        <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
          <Stack direction={"row"} gap={"80px"}>
            <Stack
              position={"relative"}
              justifyContent={"center"}
              alignItems={"center"}
            >
              <CardMedia
                component="img"
                image={imageShow}
                sx={{
                  width: "273px",
                  borderRadius: "16px",
                  objectFit: "contain",
                }}
              />
              <Stack
                sx={{
                  position: "absolute",
                  bottom: 30,
                }}
                gap={"10px"}
              >

                <Button
                  onClick={() => setIsOpenModalRulesProfileImage(true)}
                  size="small"
                  sx={{
                    backgroundColor: "rgba(255, 255, 255, 0.8)", // Fundo branco com um pouco de transparência
                    color: "black", // Texto escuro para contraste
                    borderRadius: "8px", // Cantos arredondados
                    border: "1px solid #ccc", // Borda suave
                    boxShadow: "0px 4px 6px rgba(0, 0, 0, 0.1)", // Sombra leve
                    padding: "8px 16px", // Ajuste do tamanho do botão
                    "&:hover": {
                      backgroundColor: "rgba(255, 255, 255, 1)", // Fundo mais sólido ao passar o mouse
                    },
                  }}
                >
                  Inserir foto
                </Button>
                <Button
                  onClick={handleRemoveImage}
                  size="small"
                  disabled={!imageShow}
                  sx={{
                    backgroundColor: "rgba(255, 255, 255, 0.8)", // Fundo branco com um pouco de transparência
                    color: "black", // Texto escuro para contraste
                    borderRadius: "8px", // Cantos arredondados
                    border: "1px solid #ccc", // Borda suave
                    boxShadow: "0px 4px 6px rgba(0, 0, 0, 0.1)", // Sombra leve
                    padding: "8px 16px", // Ajuste do tamanho do botão
                    "&:hover": {
                      backgroundColor: "rgba(255, 255, 255, 1)", // Fundo mais sólido ao passar o mouse
                    },
                  }}
                > Remover imagem
                </Button>
              </Stack>
            </Stack>
            <Stack width={"100%"} gap={"40px"}>
              <TextField
                value={`${initialState?.BC} - ${capitalizeText(name)}`}
                label="Nome"
                InputProps={{ readOnly: true }}
              />
              <TextField
                value={socialName}
                onChange={(e) => setSocialName(removeEmojis(e.target.value))}
                label="Nome social"
              />
              <Stack direction={"row"} alignItems={"center"} gap={"30px"}>
                <Stack
                  direction={"row"}
                  alignItems={"center"}
                  gap={"0px"}
                  onClick={() => {
                    if (idade == 1) {
                      setIdade(0);
                    } else {
                      setIdade(1);
                    }
                  }}
                >
                  <Checkbox checked={idade == 1} />
                  <Typography variant="caption" sx={{ textWrap: "nowrap" }}>
                    A sua idade ficará visível?
                  </Typography>
                </Stack>
              </Stack>
            </Stack>
          </Stack>
          <BaseModal
            width={"540px"}
            open={isOpenModalRulesProfileImage}
            title={`Regras de imagem de perfil`}
            onClose={() => setIsOpenModalRulesProfileImage(false)}
          >

<Typography variant="body1" component="div">
        <ol>
          <li>O colaborador somente poderá utilizar imagem com no máximo 5MB e 300 X 300;</li>
          <li>
            Ao fazer o upload de foto ou imagem, o colaborador concorda com a publicação na
            Plataforma Guilda, uso nos termos do artigo 22 da Lei de Direitos de Autor;
          </li>
          <li>
            O Colaborador confirma que os direitos de terceiros não foram violados e é o
            único e exclusivo responsável por todas as reivindicações de terceiros.
          </li>
          <li>
            É vedada a publicação de conteúdo que viole as condições legais, os direitos de
            terceiros ou decência comum;
          </li>
          <li>
            A Tahto se reserva ao direito de eliminar imagens se assim o entender que estão
            inadequadas. Isto é particularmente válido para as publicações que contenham
            qualquer dos seguintes conteúdos:
            <ul>
              <li>
                Imagens depreciativas, difamatórias, injuriosas, abusivas, sexistas,
                homofóbicas, imorais, indecentes, obscenas, extremistas ou racistas ou
                aqueles que glorificam a violência ou que sejam suscetíveis de corromper os
                bons costumes;
              </li>
              <li>Ataques pessoais;</li>
              <li>Spam, publicidade;</li>
              <li>Infrações à marca ou direitos de autor ou outros conteúdos ilegais.</li>
            </ul>
          </li>
          <li>
            A Tahto poderá excluir da Plataforma Guilda todo aquele que não aderir a estas
            regras.
          </li>
          <li>
            Ao fazer o upload de fotos e/ou imagens o colaborador concorda que estas poderão
            ser guardadas, sem quaisquer limitações em termos de espaço, tempo e conteúdo e
            que não haverá nenhum reembolso.
          </li>
          <li>
            O colaborador concorda que este instrumento e está ciente de que o upload de
            foto/imagem em desacordo com o disposto acima incorrerá em infração das normas
            internas desta instituição, gerando minha responsabilidade quanto às perdas e
            danos eventualmente causados, inclusive demissão por justa causa.
          </li>
        </ol>
      </Typography>
      
      <Typography variant="body1" mt={2}>
        Por esta ser a expressão da minha vontade declaro que estou de acordo e ciente de minhas responsabilidades.
      </Typography>

      {/* Checkbox para aceitar os termos */}
      <Box mt={3}>
        <Checkbox checked={checked} onChange={handleCheck} />
        <Typography component="span">Declaro que li e aceito os termos.</Typography>
      </Box>
            {/* <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
              <Typography variant="body1">
                Ao fazer o upload concordo com as regras de imagem de perfil
              </Typography>
            </Box> */}
            <Box mt={3} display={"flex"} flexDirection={"column"} gap={1}>
              <input
                accept="image/*"
                style={{ display: "none" }}
                id="image-upload"
                type="file"
                onChange={handleProfileImage}
                disabled={!checked}
              />
              <label htmlFor="image-upload">
                <ActionButton
                  title="Escolha uma foto"
                  icon={<PictureInPicture />}
                  disabled={!checked}
                />
              </label>
            </Box>

          </BaseModal>
        </Box>
        <Divider />
        <Typography variant="h2" fontSize={"24px"}>
          Dados pessoais
        </Typography>
        <TextField
          value={motivations}
          onChange={(e) => setMotivations(removeEmojis(e.target.value))}
          label="Quais suas motivações?"
        />
        <TextField
          value={targets}
          onChange={(e) => setTargets(removeEmojis(e.target.value))}
          label="Objetivos?"
        />
        <Autocomplete
          multiple
          id="tags-outlined"
          options={hobbiesShow}
          getOptionLabel={(option) => option.HOBBY}
          defaultValue={selectedHobbies}
          renderInput={(params) => <TextField {...params} label="Hobbies" />}
          onChange={(event, newValue) => {
            setSelectedHobbies(newValue);
          }}
        />
      </Stack>
      <Stack direction={"row"} justifyContent={"flex-end"} mt={"30px"}>
        <Button variant="contained" onClick={handleSave}>
          Salvar alterações
        </Button>
      </Stack>
    </Stack>
  );
}
