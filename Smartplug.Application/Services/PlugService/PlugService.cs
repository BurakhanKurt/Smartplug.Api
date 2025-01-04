using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Smartplug.Application.Services.PlugService
{
    public class PlugService : IPlugService
    {
        private readonly RequestDelegate _next;
        private static WebSocket _webSocket;

        public PlugService(RequestDelegate next)
        {
            _next = next;
        }

        // WebSocket dinleme işlemi
        public async Task HandleAsync(HttpContext context)
        {
            if (context.Request.Headers["Upgrade"] == "websocket")
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                _webSocket = socket;

                await ListenForMessagesAsync();
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        // WebSocket üzerinden gelen mesajları dinle
        private async Task ListenForMessagesAsync()
        {
            var buffer = new byte[1024];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {message}");

                    if (message == "set_relay_on")
                    {
                        // Röleyi aç
                        Console.WriteLine("Turning relay ON");
                        await SendMessageAsync("Relay is now ON");
                    }
                    else if (message == "check_relay_status")
                    {
                        // Röle durumu
                        string relayStatus = "ON"; // Örneğin, röleyi açtığınızda
                        await SendMessageAsync($"Relay status: {relayStatus}");
                    }
                }
            }
        }

        // WebSocket'e mesaj gönder
        public async Task SendMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }


        // WebSocket bağlantısını kapat
        public async Task CloseConnectionAsync()
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
            }
        }
    }
}
