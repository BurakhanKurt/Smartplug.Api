using Microsoft.AspNetCore.SignalR;


namespace Smartplug.Application.Scoket
{
    public class PlugHub : Hub
    {
        private static readonly HashSet<string> ConnectedClients = new HashSet<string>();
        public override async Task OnConnectedAsync()
        {
            ConnectedClients.Add(Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedClients.Remove(Context.ConnectionId);
        }
    }
}
