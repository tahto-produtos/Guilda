import Inventory from "@mui/icons-material/Inventory";
import {
  Box,
  Button,
  CircularProgress,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { isAxiosError } from "axios";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import PaginationComponent from "src/components/navigation/pagination/pagination";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import { StockDetailsByTypeUseCase } from "src/modules/marketing-place/use-cases/stock-details-by-type.use-case";
import { TotalDetailsStockUseCase } from "src/modules/marketing-place/use-cases/total-details-stock.use-case";
import { EXCEPTION_CODES } from "src/typings";
import { Product } from "src/typings/models/product.model";
import { StockProduct } from "src/typings/models/stock-product.model";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, getLayout } from "src/utils";
import { formatCurrency } from "src/utils/format-currency";

export default function StockTotal() {
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const router = useRouter();
  const queryType = router.query.type || null;
  const [products, setProducts] = useState<Product[]>([]);
  const [totalItems, setTotalItems] = useState<number>(0);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [isAuthVerified, setIsAuthVerified] = useState<boolean>(false);
  const [authVerifyUser, setAuthVerifyUser] = useState<string>("");
  const [authVerifyPassword, setAuthVerifyPassword] = useState<string>("");
  const [authVerifyIsLoading, setAuthVerifyIsLoading] =
    useState<boolean>(false);
  const { myUser } = useContext(UserInfoContext);

  function handleAuthVerify() {
    if (!myUser) return;

    setAuthVerifyIsLoading(true);

    const payload = {
      username: authVerifyUser,
      password: authVerifyPassword,
      currentUserId: myUser.id,
    };

    new AuthVerify()
      .handle(payload)
      .then((data) => {
        if (data == true) {
          setIsAuthVerified(true);
        } else {
          toast.warning("Usuário não autenticado");
        }
      })
      .catch((e) => {
        if (isAxiosError(e)) {
          const errorCode = e?.response?.data?.code;
          let message = INTERNAL_SERVER_ERROR_MESSAGE;

          if (
            errorCode === EXCEPTION_CODES.PASSWORD_NOT_MATCH ||
            errorCode === EXCEPTION_CODES.NOT_FOUND
          ) {
            message = "Usuário e senha não combinam";
          }

          toast.error(message);
        }
      })
      .finally(() => {
        finishLoading();
        setAuthVerifyIsLoading(false);
      });
  }

  interface ProductItemProps {
    code: string;
    id: number;
    collaboratorId: number;
    createdAt: string | Date | null;
    description: string;
    price: string | number | undefined;
    image?: string;
    amount: number;
  }

  const ProductItem = (props: ProductItemProps) => {
    const {
      code,
      collaboratorId,
      createdAt,
      id,
      description,
      price,
      image,
      amount,
    } = props;

    return (
      <Box
        border={`solid 1px ${grey[200]}`}
        display={"flex"}
        width={"100%"}
        borderRadius={2}
        p={2}
        alignItems={"center"}
        justifyContent={"space-between"}
      >
        <Box display={"flex"} alignItems={"center"} gap={2}>
          {image && (
            <Box
              sx={{
                width: "70px",
                height: "70px",
                backgroundColor: grey[100],
                borderRadius: 2,
              }}
            >
              {image && image !== undefined && (
                <img
                  alt="imagem"
                  src={image}
                  style={{ width: "100%", height: "100%" }}
                />
              )}
            </Box>
          )}
          <Box>
            <Typography variant="body2" fontSize={"16px"}>
              {description} ({amount} itens)
              <Typography
                variant="body2"
                component={"span"}
                fontSize={"12px"}
                color={"primary"}
                ml={1}
                fontWeight={"500"}
              >
                #{code}
              </Typography>
            </Typography>
            <Typography variant="body2" fontSize={"12px"} color={grey[600]}>
              {/* criado por: {collaboratorName}{" "} */}
              {createdAt &&
                `criado em: ${DateUtils.formatDatePtBRWithTime(createdAt)}`}
              {}
            </Typography>
            <Typography
              variant="body2"
              fontSize={"13px"}
              mt={1}
              fontWeight={"500"}
            >
              {formatCurrency(parseFloat(price?.toString() || "0"))} moedas
            </Typography>
          </Box>
        </Box>
        <Box display={"flex"} gap={1}>
          <Button
            color="primary"
            variant="outlined"
            onClick={() =>
              router.push(`/marketing-place/registered-products/${id}`)
            }
          >
            Editar produto
          </Button>
        </Box>
      </Box>
    );
  };

  async function getTotalStockDetails() {
    startLoading();

    await new StockDetailsByTypeUseCase()
      .handle({
        limit: 30,
        offset: (pageNumber - 1) * 30,
        type: queryType as string,
      })
      .then((data) => {
        setTotalItems(data.totalItems);
        setProducts(data.items as Product[]);
      })
      .catch(() => {
        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
      })
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    if (queryType == "PHYSICAL") {
      getTotalStockDetails();
    }
    isAuthVerified && getTotalStockDetails();
  }, [isAuthVerified, pageNumber]);

  return (
    <Card width={"100%"} display={"flex"} flexDirection={"column"}>
      <PageHeader title={`Estoque Total`} headerIcon={<Inventory />} />
      <Stack px={2} py={4} width={"100%"} gap={2}>
        {!isAuthVerified && queryType !== "PHYSICAL" && (
          <Stack
            direction={"column"}
            gap={2}
            justifyContent={"center"}
            alignItems={"center"}
          >
            <Typography variant="body2">
              É necessário confirmar seu login para acessar este conteúdo
            </Typography>
            <Box
              width={"100%"}
              display={"flex"}
              flexDirection={"column"}
              gap={1}
              maxWidth={"400px"}
            >
              <TextField
                label="Nome de Usuário"
                fullWidth
                value={authVerifyUser}
                onChange={(e) => setAuthVerifyUser(e.target.value)}
              />
              <TextField
                label="Senha"
                fullWidth
                value={authVerifyPassword}
                onChange={(e) => setAuthVerifyPassword(e.target.value)}
                type="password"
              />
              <Button
                variant="contained"
                onClick={handleAuthVerify}
                disabled={authVerifyIsLoading}
              >
                Autenticar
              </Button>
            </Box>
          </Stack>
        )}
        {isLoading && <CircularProgress />}
        {products.map((product, index) => (
          <ProductItem
            key={index}
            description={
              product.comercialName
                ? product.comercialName
                : product.description
            }
            code={product.code}
            id={product.id}
            collaboratorId={product.collaboratorId}
            createdAt={product.createdAt}
            price={product.price}
            amount={product.totalAmount || 0}
            // image={product.productImages[0]?.upload.url}
          />
        ))}
        <PaginationComponent 
          totalItems={totalItems}
          limit={30}
          page={pageNumber}
          getPage={(page: number) => setPageNumber(page)}
        />
      </Stack>
    </Card>
  );
}

StockTotal.getLayout = getLayout("private");
