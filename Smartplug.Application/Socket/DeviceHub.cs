using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Smartplug.Application.Scoket;

public class DeviceHub : Hub
{
    public static ConcurrentDictionary<Guid, string> ConnectedClients { get; private set; } = new();
    
    public async Task AddList(string userId)
    {
        ConnectedClients.TryRemove(Guid.Parse(userId), out _);
        ConnectedClients.TryAdd(Guid.Parse(userId), Context.ConnectionId);
        await Clients.Caller.SendAsync("StatusChangeOnDevice");
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("StatusChangeOnDevice");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedClients.TryRemove(Guid.Parse(Context.ConnectionId), out _);
    }
}