import { useContext } from "react";
import AccountBalance from "@mui/icons-material/AccountBalance";
import Add from "@mui/icons-material/Add";
import Category from "@mui/icons-material/Category";
import ConfirmationNumber from "@mui/icons-material/ConfirmationNumber";
import FormatListBulleted from "@mui/icons-material/FormatListBulleted";
import ImportExport from "@mui/icons-material/ImportExport";
import Inventory from "@mui/icons-material/Inventory";
import ManageAccounts from "@mui/icons-material/ManageAccounts";
import Sell from "@mui/icons-material/Sell";
import Storefront from "@mui/icons-material/Storefront";
import SquareFoot from "@mui/icons-material/SquareFoot";
import FileOpen from "@mui/icons-material/FileOpen";
import Savings from "@mui/icons-material/Savings";
import PeopleAlt from "@mui/icons-material/PeopleAlt";
import Settings from "@mui/icons-material/Settings";
import BarChart from "@mui/icons-material/BarChart";
import AdminPanelSettings from "@mui/icons-material/AdminPanelSettings";
import Collections from "@mui/icons-material/Collections";
import abilityFor from "src/utils/ability-for";
import AccountCircleOutlinedIcon from '@mui/icons-material/AccountCircleOutlined';
import { ExpandIcon } from "src/components/icons/expand.icon";
import { SideBarItem } from "../sideBarItem/sideBarItem";
import { HomeIcon } from "src/components/icons/home.icon";
import { CartIcon } from "src/components/icons/cart.icon";
import GroupOutlined from "@mui/icons-material/GroupOutlined";
import ApartmentOutlined from "@mui/icons-material/ApartmentOutlined";
import { UserPersonaContext } from "../../../../contexts/user-persona/user-persona.context";
import InsightsOutlined from "@mui/icons-material/InsightsOutlined";
import DashboardOutlined from "@mui/icons-material/DashboardOutlined";
import MonetizationOnOutlined from "@mui/icons-material/MonetizationOnOutlined";
import PersonSearchOutlined from "@mui/icons-material/PersonSearchOutlined";
import SquareFootOutlined from "@mui/icons-material/SquareFootOutlined";
import SummarizeOutlined from "@mui/icons-material/SummarizeOutlined";
import DataUsageOutlined from "@mui/icons-material/DataUsageOutlined";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import SupervisedUserCircleOutlined from "@mui/icons-material/SupervisedUserCircleOutlined";
import TheatersOutlined from "@mui/icons-material/TheatersOutlined";
import GppBadOutlined from "@mui/icons-material/GppBadOutlined";
import NotificationsOutlined from "@mui/icons-material/NotificationsOutlined";
import QuizIcon from "@mui/icons-material/Quiz";
import ListIcon from "@mui/icons-material/List";
import {
  Feedback,
  FeedbackOutlined,
  ListAltOutlined,
  Stars,
  ThermostatOutlined,
} from "@mui/icons-material";

interface MainNavigationProps {
  isExpanded?: boolean;
}

export function MainNavigation(props: MainNavigationProps) {
  const { myPermissions } = useContext(PermissionsContext);
  const { myUser } = useContext(UserInfoContext);
  const { isExpanded } = props;
  const { personaShowUser } = useContext(UserPersonaContext);

  const NavbarItems = [
    {
      title: "Início",
      href: "/",
      icon: <HomeIcon width={20} height={17} />,
    },
    {
      title: "Resultados",
      href: "/results",
      icon: <DataUsageOutlined />,
    },
    {
      title: "Meu perfil",
      href: `/profile/edit-profile/${personaShowUser?.ID_PERSON_ACCOUNT}`,

      icon: <AccountCircleOutlinedIcon />,
    },
    {
      title: "Grupos",
      href: "/groups",
      icon: <GroupOutlined />,
      visible: abilityFor(myPermissions).can("Ver Grupos", "Grupos"),
      subMenuItems: [
        {
          title: "Ver grupos",
          href: "/groups",
          icon: <FormatListBulleted fontSize={"small"} />,
          visible: abilityFor(myPermissions).can("Ver Grupos", "Grupos"),
        },
      ],
    },
    {
      title: "Setores",
      href: "/sectors",
      icon: <ApartmentOutlined />,
      visible: abilityFor(myPermissions).can("Ver Setores", "Setores"),
      subMenuItems: [
        {
          title: "Ver setores",
          href: "/sectors",
          icon: <FormatListBulleted fontSize={"small"} />,
          visible: abilityFor(myPermissions).can("Ver Setores", "Setores"),
        },
        {
          title: "Criar setor",
          href: "/sectors/create",
          icon: <Add fontSize={"small"} />,
          visible: abilityFor(myPermissions).can("Editar Setores", "Setores"),
        },
        {
          title: "Importação massiva de setores",
          href: "/sectors/massive-import",
          icon: <FileOpen fontSize={"small"} />,
          visible: abilityFor(myPermissions).can("Editar Setores", "Setores"),
        },
      ],
    },
    {
      title: "Indicadores",
      href: "/indicators",
      icon: <InsightsOutlined />,
      visible: abilityFor(myPermissions).can("Ver Indicadores", "Indicadores"),
      subMenuItems: [
        {
          title: "Ver indicadores",
          href: "/indicators",
          icon: <FormatListBulleted fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Ver Indicadores",
            "Indicadores"
          ),
        },
        {
          title: "Criar indicador",
          href: "/indicators/create",
          icon: <Add fontSize={"small"} />,
          visible: abilityFor(myPermissions).can("CREATE", "Indicadores"),
        },
        {
          title: "Associar setores a um indicador",
          href: "/indicators/connect",
          icon: <ImportExport fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Editar Indicadores",
            "Indicadores"
          ),
        },
        {
          title: "Associar indicadores a um setor",
          href: "/indicators/connect-indicators",
          icon: <ImportExport fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Editar Indicadores",
            "Indicadores"
          ),
        },
        {
          title: "Importação massiva de indicadores",
          href: "/indicators/import-indicators",
          icon: <FileOpen fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Editar Indicadores",
            "Indicadores"
          ),
        },
        {
          title: "Cesta de indicadores",
          href: "/indicators/indicators-basket",
          icon: <Settings fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Ver Cesta de Indicadores",
            "Indicadores"
          ),
        },
        {
          title: "Score dos indicadores",
          href: "/indicators/indicator-score",
          icon: <FileOpen fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Editar Indicadores",
            "Indicadores"
          ),
        },
      ],
    },
    {
      title: "Métricas",
      href: "/metrics-settings",
      icon: <SquareFootOutlined />,
      visible: abilityFor(myPermissions).can("Ver Metricas", "Metricas"),
      subMenuItems: [
        {
          title: "Criar configurações de métricas",
          href: "/metric-settings/create",
          icon: <SquareFoot />,
          visible: abilityFor(myPermissions).can("Editar Metricas", "Metricas"),
        },
        {
          title: "Associação massiva de indicador por setor",
          href: "/metric-settings/massive-associate",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can("Editar Metricas", "Metricas"),
        },
      ],
    },
    {
      title: "Perfis",
      href: "/profile",
      icon: <PersonSearchOutlined />,
      visible:
        abilityFor(myPermissions).can("Ver Perfis", "Perfis") ||
        abilityFor(myPermissions).can("Ver Colaboradores", "Perfis") ||
        abilityFor(myPermissions).can("Ver Permissões", "Perfis"),
      subMenuItems: [
        {
          title: "Perfis administrativos",
          href: "/profile/profile-list",
          icon: <ManageAccounts />,
          visible: abilityFor(myPermissions).can("Ver Perfis", "Perfis"),
        },
        // {
        //     title: "Configurações de perfis",
        //     href: "/profile/profiles-settings",
        //     icon: <ManageAccounts />,
        //     visible: abilityFor(myPermissions).can(
        //         "UPDATE_PERMISSION",
        //         "profiles"
        //     ),
        // },
        {
          title: "Ver colaboradores",
          href: "/profile/list-collaborators",
          icon: <PeopleAlt />,
          visible: abilityFor(myPermissions).can("Ver Colaboradores", "Perfis"),
        },
        {
          title: "Importação massiva de colaboradores",
          href: "/profile/import-collaborators",
          icon: <PeopleAlt />,
          visible: abilityFor(myPermissions).can(
            "Editar Colaboradores",
            "Perfis"
          ),
        },
        {
          title: "Associação massiva de perfis",
          href: "/profile/massive-associate",
          icon: <PeopleAlt />,
          visible: abilityFor(myPermissions).can(
            "Editar Colaboradores",
            "Perfis"
          ),
        },
        {
          title: "Visão Administrador",
          href: "/profile/adm-view",
          icon: <AdminPanelSettings />,
          visible: abilityFor(myPermissions).can(
            "Editar Colaboradores",
            "Perfis"
          ),
        },
      ],
    },
    {
      title: "Monetização",
      href: "/monetization",
      icon: <MonetizationOnOutlined />,
      visible:
        abilityFor(myPermissions).can("Extrato da conta", "Monetização") ||
        abilityFor(myPermissions).can(
          "Adicionar Creditos a um colaborador",
          "Monetização"
        ),
      subMenuItems: [
        {
          title: "Extrato da Conta",
          href: "/monetization/balance",
          icon: <AccountBalance />,
          visible: abilityFor(myPermissions).can(
            "Extrato da conta",
            "Monetização"
          ),
        },
        {
          title: "Adicionar créditos a um colaborador",
          href: "/monetization/add-credit",
          icon: <Savings />,
          visible: abilityFor(myPermissions).can(
            "Adicionar Creditos a um colaborador",
            "Monetização"
          ),
        },
        {
          title: "Extrato a Expirar",
          href: "/monetization/list-expiration",
          icon: <AccountBalance />,
          //visible: abilityFor(myPermissions).can(
          //    "Adicionar Creditos a um colaborador",
          //    "Monetização"
          //),
        },
        {
          title: "Configurar datas Expiração",
          href: "/monetization/config-monetization-expired",
          icon: <Settings fontSize={"small"} />,
          visible: abilityFor(myPermissions).can(
            "Config Expiração Moedas",
            "Expiração Moedas"
          ),
        },
      ],
    },
    {
      title: "MarketPlace",
      href: "/marketing-place",
      icon: <CartIcon width={20} height={20} />,
      visible: abilityFor(myPermissions).can("Ver Loja", "Marketing Place"),
      subMenuItems: [
        {
          title: "Loja",
          href: "/marketing-place",
          icon: <Storefront />,
          visible: abilityFor(myPermissions).can("Ver Loja", "Marketing Place"),
        },
        {
          title: "Produtos Cadastrados",
          href: "/marketing-place/registered-products",
          icon: <Category />,
          visible: abilityFor(myPermissions).can(
            "Ver Produto",
            "Marketing Place"
          ),
        },
        {
          title: "Catalogo",
          href: "/marketing-place/catalog",
          icon: <Category />,
          visible: abilityFor(myPermissions).can(
            "Ver Produto",
            "Marketing Place"
          ),
        },
        {
          title: "Estoques",
          href: "/marketing-place/registered-stocks",
          icon: <Inventory />,
          visible: abilityFor(myPermissions).can(
            "Ver Estoque",
            "Marketing Place"
          ),
        },
        {
          title: "Categorias",
          href: "/marketing-place/category",
          icon: <Inventory />,
          visible: abilityFor(myPermissions).can(
            "Ver Estoque",
            "Marketing Place"
          ),
        },
        // {
        //     title: "Fornecedores",
        //     href: "/marketing-place/supplier",
        //     icon: <Inventory />,
        //     visible: abilityFor(myPermissions).can(
        //         "Ver Estoque",
        //         "Marketing Place"
        //     ),
        // },
        {
          title: "Feriados",
          href: "/marketing-place/holidays",
          icon: <Inventory />,
          visible: abilityFor(myPermissions).can(
            "Ver Estoque",
            "Marketing Place"
          ),
        },
        {
          title: "Entrada de produtos",
          href: "/marketing-place/product-quantity",
          icon: <Inventory />,
          visible: abilityFor(myPermissions).can(
            "Ver Estoque",
            "Marketing Place"
          ),
        },
        {
          title: "Criar Estoque",
          href: "/marketing-place/create-stock",
          icon: <Add />,
          visible: abilityFor(myPermissions).can(
            "Editar Estoque",
            "Marketing Place"
          ),
        },
        {
          title: "Administrar Pedidos",
          href: "/marketing-place/manage-orders",
          icon: <Sell />,
          visible: abilityFor(myPermissions).can(
            "Ver administrar Pedidos",
            "Marketing Place"
          ),
        },
        {
          title: "Meus Pedidos",
          href: "/marketing-place/my-orders",
          icon: <Sell />,
          visible: abilityFor(myPermissions).can(
            "Ver Meus Pedidos",
            "Marketing Place"
          ),
        },
        {
          title: "Meus vouchers",
          href: "/marketing-place/my-vouchers",
          icon: <ConfirmationNumber />,
          visible: abilityFor(myPermissions).can(
            "Ver Meus Vouchers",
            "Marketing Place"
          ),
        },
        {
          title: "Importação massiva de produtos",
          href: "/marketing-place/massive-create-product",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Cadastrar Produto",
            "Marketing Place"
          ),
        },
        {
          title: "Importação massiva de quantidade de produtos",
          href: "/marketing-place/massive-stock-manager",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Cadastrar Produto",
            "Marketing Place"
          ),
        },
        {
          title: "Configurar vitrines",
          href: "/marketing-place/display-config",
          icon: <Settings />,
          visible: abilityFor(myPermissions).can(
            "Editar Configurar vitrine",
            "Marketing Place"
          ),
        },
        {
          title: "Prazos de Expiração",
          href: "/marketing-place/expiration-config",
          icon: <Settings />,
          visible: abilityFor(myPermissions).can(
            "Editar Configurar vitrine",
            "Marketing Place"
          ),
        },
        {
          title: "Galeria de imagens",
          href: "/marketing-place/gallery",
          icon: <Collections />,
          visible: abilityFor(myPermissions).can(
            "Cadastrar Produto",
            "Marketing Place"
          ),
        },
      ],
    },
    {
      title: "Relatórios",
      href: "/reports",
      icon: <SummarizeOutlined />,
      visible: abilityFor(myPermissions).can("Relatorio de resultados", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de setores", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio Feedback", "Feedback") || 
      abilityFor(myPermissions).can("Relatorio de monetização", "Relatorios") ||

      abilityFor(myPermissions).can("Relatorio de estoque", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorios de estoque - padrão", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorios de estoque - gerenciar estoque", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorios de estoque - entrada voucher", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorios de estoque - saida voucher", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorios de estoque - movimentação", "Relatorios") ||

      abilityFor(myPermissions).can("Relatorio geral de estoque", "Marketing Place") ||
      abilityFor(myPermissions).can("Relatorio de pedidos", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de fechamento de caixa", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de tempo logado", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de quiz", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de notificação", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de plano de ação", "Relatorios") ||
      abilityFor(myPermissions).can("Relatorio de campanha", "Relatorios") 
      ,
      subMenuItems: [
        {
          title: "Relatório de feedback",
          href: "/reports/feedback-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio Feedback",
            "Feedback"
          ),
        },
        {
          title: "Relatório de resultados",
          href: "/reports/results-report",
          icon: <FileOpen />,
          visible:
            abilityFor(myPermissions).can("Relatorio de resultados", "Relatorios"),
        },
        {
          title: "Relatório de setores",
          href: "/reports/sectors-consolidated-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can("Relatorio de setores", "Relatorios"),
        },
        {
          title: "Relatórios de monetização",
          href: "/reports/balance-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de monetização",
            "Relatorios"
          ),
        },
        {
          title: "Relatórios de estoque",
          href: "/reports/stock-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can("Relatorio de estoque", "Relatorios") ||
          abilityFor(myPermissions).can("Relatorios de estoque - padrão", "Relatorios") ||
          abilityFor(myPermissions).can("Relatorios de estoque - gerenciar estoque", "Relatorios") ||
          abilityFor(myPermissions).can("Relatorios de estoque - entrada voucher", "Relatorios") ||
          abilityFor(myPermissions).can("Relatorios de estoque - saida voucher", "Relatorios") ||
          abilityFor(myPermissions).can("Relatorios de estoque - movimentação", "Relatorios") 
          ,
        },
        {
          title: "Relatório geral de estoque",
          href: "/reports/general-stock-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio geral de estoque",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de pedidos",
          href: "/reports/orders-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de pedidos",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de fechamento de caixa",
          href: "/reports/financial-summary-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de fechamento de caixa",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de tempo logado",
          href: "/reports/day-loggeds-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de tempo logado",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de notificações",
          href: "/reports/notifications-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de notificação",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de quiz",
          href: "/reports/quiz-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de quiz",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de plano de ação",
          href: "/reports/action-escalation-report",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de plano de ação",
            "Relatorios"
          ),
        },
        {
          title: "Relatório de campanha",
          href: "/reports/campaing",
          icon: <FileOpen />,
          visible: abilityFor(myPermissions).can(
            "Relatorio de campanha",
            "Relatorios"
          ),
        },
      ],
    },
    {
      title: "Paineis",
      href: "/panel",
      icon: <DashboardOutlined />,
      visible:
        abilityFor(myPermissions).can("Painel de integração", "Painel") ||
        abilityFor(myPermissions).can("Painel de verificação", "Painel"),
      subMenuItems: [
        {
          title: "Painel de Integração",
          href: "/panel/integrations-panel",
          icon: <BarChart />,
          visible: abilityFor(myPermissions).can("Painel de integração", "Painel"),
        },
        {
          title: "Painel de Verificação",
          href: "/panel/verification-panel",
          icon: <BarChart />,
          visible: abilityFor(myPermissions).can("Painel de verificação", "Painel"),
        },
      ], 
    },
    {
      title: "Persona",
      href: "/personas",
      icon: <SupervisedUserCircleOutlined />,
      visible:
        abilityFor(myPermissions).can("Editar Persona ADM", "Persona") ||
        abilityFor(myPermissions).can("Editar Hobby", "Persona") ||
        abilityFor(myPermissions).can("Editar Blacklist", "Persona"),
      subMenuItems: [
        {
          title: "Ver personas",
          href: "/personas",
          icon: <SupervisedUserCircleOutlined />,
          visible: abilityFor(myPermissions).can(
            "Editar Persona ADM",
            "Persona"
          ),
        },
        {
          title: "Hobbies",
          href: "/personas/hobbies",
          icon: <TheatersOutlined />,
          visible: abilityFor(myPermissions).can("Editar Hobby", "Persona"),
        },
        {
          title: "Blacklist",
          href: "/personas/blacklist",
          icon: <GppBadOutlined />,
          visible: abilityFor(myPermissions).can("Editar Blacklist", "Persona"),
        },
        {
          title: "Público e Privado",
          href: "/personas/config-privacy",
          icon: <GppBadOutlined />,
          visible: abilityFor(myPermissions).can(
            "Configuração Publico e Privado",
            "Persona"
          ),
        },
      ],
    },
    {
      title: "Notificações",
      href: "/notifications",
      icon: <NotificationsOutlined />,
      visible: abilityFor(myPermissions).can(
        "Gerenciar Notificacoes",
        "Notificacao"
      ),
    },
    {
      title: "Quiz",
      href: "/quiz",
      icon: <QuizIcon />,
      //visible: abilityFor(myPermissions).can("Gerenciar Quiz", "Quiz"),

      visible: abilityFor(myPermissions).can("Gerenciar Quiz", "Quiz") ||
               abilityFor(myPermissions).can("Meus Quizzes", "Quiz"),
      subMenuItems: [
        {
          title: "Ver quizzes",
          href: "/quiz/list-quiz",
          icon: <ListIcon />,
          visible: abilityFor(myPermissions).can("Gerenciar Quiz", "Quiz"),
        },
        {
          title: "Meus quizzes",
          href: "/quiz/my-quiz",
          icon: <ListIcon />,
          visible: abilityFor(myPermissions).can("Meus Quizzes", "Quiz"),
        },
      ],
    },
    {
      title: "Clima",
      href: "/climate",
      icon: <ThermostatOutlined />,
      visible: abilityFor(myPermissions).can("Ver Clima", "Clima"),
      subMenuItems: [
        {
          title: "Clima",
          href: "/climate",
          icon: <ListIcon />,
        },
        {
          title: "Listar Climas",
          href: "/climate/list-climate",
          icon: <ListIcon />,
        },
      ],
    },
    {
      title: "Feedback",
      href: "/feedback",
      icon: <FeedbackOutlined />,
      visible: abilityFor(myPermissions).can("Meus Feedbacks","Feedback") ||
      abilityFor(myPermissions).can("Gerenciar Feedback","Feedback"),
      subMenuItems: [
        {
          title: "Meus Feedbacks",
          href: "/feedback/my-feedbacks",
          icon: <FeedbackOutlined />,
          visible: abilityFor(myPermissions).can(
            "Meus Feedbacks",
            "Feedback"
          ),
        },
        {
          title: "Administrar Feedbacks",
          href: "/feedback",
          icon: <FeedbackOutlined />,
          visible: abilityFor(myPermissions).can(
            "Gerenciar Feedback",
            "Feedback"
          ),
        },
      ],
    },
    {
      title: "Escalation",
      href: "/escalation",
      icon: <ListAltOutlined />,
      visible: abilityFor(myPermissions).can(
        "Ver Escalation",
        "Escalation"
      ),  
    },
    {
      title: "Campanhas",
      href: "/campaigns",
      icon: <Stars />,
      visible: abilityFor(myPermissions).can(
        "Gerenciar Campanha",
        "Campanha Operacional"
      ),
    },
  ];

  return (
    <>
      {NavbarItems.map((item, index) => {
        if (item.visible == false) {
          return null;
        }
        return (
          <SideBarItem
            key={index}
            icon={item.icon}
            title={item.title}
            onClick={() => {}}
            isExpanded={isExpanded}
            href={item.href}
            subMenuItems={item.subMenuItems}
          />
        );
      })}
    </>
  );
}
