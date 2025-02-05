using ApiC.Class.DowloadFile;
using ApiRepositorio.Class;

using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;

using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.WebSockets;
using ApiC.Class;
using System.Data.SqlClient;
using System.Web;
using System.Threading;
using Fleck;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.Application;
using System.Security.Cryptography.X509Certificates;
using static TokenService;

[assembly: OwinStartup(typeof(ApiRepositorio.Startup))]

namespace ApiRepositorio
{

    public class messageReturned
    {
        public string type { get; set; }
        public dataMessage data { get; set; }
    }

    public class dataMessage
    {
        public int idUserReceive { get; set; }
        public int idUserSend { get; set; }
        public int idNotification { get; set; }
        public int idNotificationUser { get; set; }
        public string urlUserSend { get; set; }
        public string nameUserSend { get; set; }
        public string message { get; set; }
        public List<urlFiles> urlFilesMessage { get; set; }
    }

    public class urlFiles
    {
        public string url { get; set; }
    }

    public class WebSocketUser
    {
        public DateTime LoginTime { get; set; }
        public IWebSocketConnection Socket { get; set; }
    }
    //public class UserMessage
    //{
    //    public int userId { get; set; }
    //    public string message { get; set; }
    //    public MessageType type { get; set; }
    //}

    public enum MessageType
    {
        Notification,
        Mesage
    }

    public class RemoveXPoweredByMiddleware : OwinMiddleware
    {
        public RemoveXPoweredByMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            context.Response.OnSendingHeaders(state =>
            {
                var owinContext = (IOwinContext)state;
                owinContext.Response.Headers.Remove("X-Powered-By");

                System.Diagnostics.Debug.WriteLine("X-Powered-By header removed.");
            }, context);

            await Next.Invoke(context);
        }
    }

    public class Startup
    {
        private static ConcurrentDictionary<int, (int, WebSocketUser)> connectedUsers = new ConcurrentDictionary<int, (int, WebSocketUser)>();

        private DateTime loginTime;
        public static ConcurrentQueue<messageReturned> messageQueue = new ConcurrentQueue<messageReturned>();


        public void Configuration(IAppBuilder app)
        {
            app.Use<RemoveXPoweredByMiddleware>();
            // configuracao WebApi
            var config = new HttpConfiguration();
            // configurando rotas
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                  routeTemplate: "api/{controller}/{id}",
                  defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ExceptionHandlingAttribute());

            // ativando cors
            app.UseCors(CorsOptions.AllowAll);

            // Configuração do Swagger
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "API Documentation");

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // Usa o nome completo do tipo para evitar conflitos de schemaId
                c.UseFullTypeNameInSchemaIds();
            })
            .EnableSwaggerUi(); // Ativa a interface do Swagger UI


            // ativando a geração do token
            // AtivarGeracaoTokenAcesso(app);

            // ativando configuração WebApi
            app.UseWebApi(config);

            // Alimentando variavel de conexão
            Database.Conn = Database.retornaConn();

            //Processo para espelhar algumas tabelas para homologação as 2 da manhã
            if (Database.retornaConn().ToString().Contains("10.115.193.41;"))
            {
                ScheduledTask.Start();
            }
            if (Database.retornaConn().ToString().Contains("10.115.65.41;"))
            {
                ScheduledTask.StartReports();

            }

            //Agendamento Notificação e Quiz
            ScheduledNotification.Scheduled();

            //WebSocket


            //app.MapWebSocketRoute<MyWebSocket>("/websocket");
            // Configurando servidor WebSocket Fleck

            // string ip = Database.ObterEnderecoIpDaMaquina();

            var server = new Fleck.WebSocketServer($"ws://0.0.0.0:3004/websocket");
            //string enderecoIp = Database.ObterEnderecoIpDaMaquina();

            //var server = new Fleck.WebSocketServer("wss://0.0.0.0:8443/websocket");

            //try
            //{
            //    server.Certificate = new X509Certificate2("C:/nginx-1.24.0/ssl-certs/tahto.pfx", "");
            //}
            //catch (Exception)
            //{

            //}


            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    loginTime = DateTime.Now;
                    var queryString = socket.ConnectionInfo.Path;
                    var queryParams = HttpUtility.ParseQueryString(queryString);
                    string token = queryParams["/websocket?token"];

                    InfsUser inf = TokenService.TryDecodeToken(token);

                    if (inf != null)
                    {
                        int userId = inf.personauserId;
                        int codHash = inf.hash;

                        var userData = new WebSocketUser
                        {
                            LoginTime = loginTime,
                            Socket = socket
                        };
                        insertLogWebSocket(userId, 1, 0, 0);

                        connectedUsers.TryAdd(codHash, (userId, userData));
                    }

                };
                socket.OnClose = () =>
                {

                    var queryString = socket.ConnectionInfo.Path;
                    var queryParams = HttpUtility.ParseQueryString(queryString);
                    string token = queryParams["/websocket?token"];

                    InfsUser inf = TokenService.TryDecodeToken(token);

                    if (inf != null)
                    {
                        int userId = inf.personauserId;
                        int codLog = inf.codLog;
                        int hash = inf.hash;

                        WebSocketUser userE;
                        //connectedUsers.TryGetValue(userId, out userE);

                        if (connectedUsers.TryGetValue(hash, out var userTuple))
                        {
                            // Desempacote o tuplo para obter os valores individuais
                            var (intValue, webSocketUser) = userTuple;

                            userE = userTuple.Item2;
                            // Agora você pode usar 'intValue' e 'webSocketUser'

                            DateTime lgInit = userE.LoginTime;

                            DateTime logoutTime = DateTime.Now;
                            TimeSpan timeLoggedIn = logoutTime - lgInit;

                            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                            {

                                StringBuilder stb = new StringBuilder();
                                stb.AppendFormat("UPDATE GDA_LOGIN_ACCESS SET TEMPOLOGIN = {0}+ISNULL(TEMPOLOGIN, 0) ", Convert.ToInt32(timeLoggedIn.TotalSeconds));
                                stb.AppendFormat("where IDGDA_LOGIN_ACCESS = '{0}' ", codLog);

                                try
                                {
                                    connection.Open();
                                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception)
                                {
                                }

                                connection.Close();
                            }
                            insertLogWebSocket(userId, 0, 1, 0);
                            connectedUsers.TryRemove(hash, out _);
                        }


                    }

                };
                socket.OnMessage = message =>
                {
                    // Adicione a mensagem à fila para processamento posterior
                    var queryString = socket.ConnectionInfo.Path;
                    var queryParams = HttpUtility.ParseQueryString(queryString);

                    messageReturned msgInput = JsonConvert.DeserializeObject<messageReturned>(message);

                    messageQueue.Enqueue(msgInput);
                };
            });


            Task.Run(() => ProcessMessageQueueAsync());
        }

        private MessageType tipoMessage(string mensagemTipo)
        {
            MessageType type;
            if (mensagemTipo == "notificação")
            {
                type = MessageType.Notification;
            }
            else
            {
                type = MessageType.Mesage;
            }
            return type;
        }

        private async Task ProcessMessageQueueAsync()
        {
            while (true)
            {
                try
                {
                    if (messageQueue.TryDequeue(out messageReturned msgInput))
                    {
                        // Processar a mensagem aqui
                        await SendMessageToUserAsync(msgInput); // Exemplo de chamada para enviar mensagem ao usuário
                    }
                    await Task.Delay(100); // Aguarde antes de verificar novamente a fila
                }
                catch (Exception)
                {
                    await Task.Delay(5000);
                }
            }
        }

        public static void insertLogWebSocket(int idPersona, int Open, int Close, int Send)
        {
            StringBuilder stbNot = new StringBuilder();
            stbNot.Append($"INSERT INTO GDA_LOG_WEBSOCKET (IDPERSONA, [OPEN], [CLOSE], [SEND], CREATED_AT) ");
            stbNot.Append($"VALUES ({idPersona}, {Open}, {Close}, {Send}, GETDATE()) ");

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(stbNot.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }
        }


        public async Task SendMessageToUserAsync(messageReturned msgInput)
        {

            var usersToSend = connectedUsers
                .Where(kv => kv.Value.Item1 == msgInput.data.idUserReceive)
                .Select(kv => kv.Value.Item2);


            string jsonMessage = JsonConvert.SerializeObject(msgInput);
            foreach (var userE in usersToSend)
            {
                insertLogWebSocket(msgInput.data.idUserReceive, 0, 0, 1); 
                IWebSocketConnection userSocket = userE.Socket;
                await userSocket.Send(jsonMessage);
            }






            //if (connectedUsers.TryGetValue(msgInput.data.idUserReceive, out var userTuple))
            //{
            //    //Select Na notificação
            //    WebSocketUser userE;
            //    userE = userTuple.Item2;


            //    string jsonMessage = JsonConvert.SerializeObject(msgInput);
            //    IWebSocketConnection userSocket = userE.Socket;
            //    await userSocket.Send(jsonMessage);

            //}
            //else
            //{


            //    // Lidar com o caso em que o usuário não está conectado
            //}
        }

        //public class MyWebSocket : WebSocketConnection
        //{
        //    private int userId;
        //    private DateTime loginTime;
        //    private static ConcurrentDictionary<int, WebSocket> connectedUsers = new ConcurrentDictionary<int, WebSocket>();

        //    private string GetQueryParamValue(string queryString, string paramName)
        //    {
        //        var queryParams = HttpUtility.ParseQueryString(queryString);
        //        return queryParams[paramName];
        //    }

        //    public override void OnOpen()
        //    {
        //        //userId = Guid.NewGuid();
        //        loginTime = DateTime.Now;

        //        //var headers = Context.Request.Headers;
        //        //string token = headers["token"].ToString();
        //        var queryParams = Context.Request.Uri.Query;
        //        string token = GetQueryParamValue(queryParams, "token");

        //        bool tokn = TokenService.TryDecodeToken(token);
        //        if (tokn == true)
        //        {
        //            userId = TokenService.InfsUsers.collaboratorId;
        //            Startup.userLogins.TryAdd(userId, loginTime);

        //            var webSocketContext = Context.Request.Environment["websocket.AcceptWebSocketContext"] as System.Net.WebSockets.WebSocketContext;
        //            var webSocket = webSocketContext.WebSocket;

        //            connectedUsers.TryAdd(userId, webSocket);
        //        }


        //    }

        //    public override async Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType messageType)
        //    {
        //        var receivedMessage = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
        //        //Console.WriteLine($"Mensagem recebida: {receivedMessage}");
        //        // Analisar a mensagem recebida para determinar o destinatário e conteúdo
        //        var messageParts = receivedMessage.Split(':');
        //        if (messageParts.Length == 2 && int.TryParse(messageParts[0], out int recipientId))
        //        {
        //            var content = messageParts[1];
        //            // Enviar a mensagem para o destinatário, se estiver conectado
        //            if (connectedUsers.TryGetValue(recipientId, out WebSocket recipientSocket))
        //            {

        //                await recipientSocket.SendAsync(
        //                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(content)),
        //                    WebSocketMessageType.Text,
        //                    true,
        //                    CancellationToken.None);
        //            }
        //            else
        //            {
        //                // Lidar com o destinatário não estar conectado
        //            }
        //        }
        //        else
        //        {
        //            // Lidar com a mensagem inválida
        //        }

        //    }

        //    public override void OnClose(WebSocketCloseStatus? closeStatus, string closeDescription)
        //    {
        //        int codLog = 0;
        //        //var headers = Context.Request.Headers;
        //        //string token = headers["token"].ToString();
        //        var queryParams = Context.Request.Uri.Query;
        //        string token = GetQueryParamValue(queryParams, "token");
        //        bool tokn = TokenService.TryDecodeToken(token);
        //        if (tokn == true)
        //        {
        //            userId = TokenService.InfsUsers.collaboratorId;
        //            codLog = TokenService.InfsUsers.codLog;
        //        }

        //        // Remover usuário da lista de logins
        //        DateTime lgInit;
        //        Startup.userLogins.TryGetValue(userId, out lgInit);

        //        DateTime logoutTime = DateTime.Now;
        //        TimeSpan timeLoggedIn = logoutTime - lgInit;

        //        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //        {

        //            StringBuilder stb = new StringBuilder();
        //            stb.AppendFormat("UPDATE GDA_LOGIN_ACCESS SET TEMPOLOGIN = {0}+ISNULL(TEMPOLOGIN, 0) ", Convert.ToInt32(timeLoggedIn.TotalSeconds));
        //            stb.AppendFormat("where IDGDA_LOGIN_ACCESS = '{0}' ", codLog);

        //            try
        //            {
        //                connection.Open();
        //                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //            catch (Exception)
        //            {
        //            }

        //            //StringBuilder stb = new StringBuilder();
        //            //stb.AppendFormat("SELECT TOP 1 IDGDA_LOGIN_ACCESS AS COD FROM GDA_LOGIN_ACCESS WHERE IDGDA_COLLABORATOR = {0} ", userId);
        //            //stb.Append("AND CONVERT(DATE, DATE_ACCESS) >= CONVERT(DATE, DATEADD(DAY, -0, GETDATE())) ");
        //            //stb.Append("ORDER BY DATE_ACCESS DESC ");

        //            //try
        //            //{
        //            //    connection.Open();
        //            //    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            //    {
        //            //        using (SqlDataReader reader = command.ExecuteReader())
        //            //        {
        //            //            if (reader.Read())
        //            //            {
        //            //                codLog = reader["COD"].ToString();
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //catch (Exception ex)
        //            //{
        //            //}

        //            //if (codLog != "")
        //            //{
        //            //    stb.Clear();
        //            //    stb.AppendFormat("UPDATE GDA_LOGIN_ACCESS SET TEMPOLOGIN = {0}+ISNULL(TEMPOLOGIN, 0) ", Convert.ToInt32(timeLoggedIn.TotalSeconds));
        //            //    stb.AppendFormat("where IDGDA_LOGIN_ACCESS = '{0}' ", codLog);

        //            //    try
        //            //    {
        //            //        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            //        {
        //            //            command.ExecuteNonQuery();
        //            //        }
        //            //    }
        //            //    catch (Exception)
        //            //    {
        //            //    }

        //            //}
        //            connection.Close();
        //        }

        //        Startup.userLogins.TryRemove(userId, out _);
        //    }
        //}
    }
}
