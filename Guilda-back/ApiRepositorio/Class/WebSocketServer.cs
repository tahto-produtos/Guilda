using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ApiC.Class
{
    public class WebSocketServer
    {
        private static Dictionary<Guid, DateTime> userLogins = new Dictionary<Guid, DateTime>();

        static async Task Main(string[] args)
        {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();

            Console.WriteLine("Servidor WebSocket iniciado...");

            while (true)
            {
                var context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private static async void ProcessRequest(HttpListenerContext context)
        {
            var webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;

            Guid userId = Guid.NewGuid();
            userLogins.Add(userId, DateTime.Now);

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    // Envie o tempo de login atual para o cliente a cada segundo
                    var loginTime = DateTime.Now - userLogins[userId];
                    var buffer = System.Text.Encoding.UTF8.GetBytes($"Tempo logado: {loginTime}");
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    await Task.Delay(1000); // aguarde 1 segundo
                }
            }
            catch (WebSocketException) { }

            // Quando a conexão é fechada, remova o usuário da lista de logins
            userLogins.Remove(userId);
        }
    }
}