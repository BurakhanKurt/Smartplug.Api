using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Smartplug.Application.Scoket;

public class DeviceHub : Hub
{
    public static ConcurrentDictionary<Guid, List<string>> ConnectedClients { get; private set; } = new();
    
    public async Task AddList(string userId)
    {
        if (ConnectedClients.TryGetValue(Guid.Parse(userId), out var list))
        {
            list.Add(Context.ConnectionId);
        }
        else
        {
            ConnectedClients.TryAdd(Guid.Parse(userId), new List<string> { Context.ConnectionId });
        }
        await Clients.Caller.SendAsync("StatusChangeOnDevice");
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("StatusChangeOnDevice");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var item in ConnectedClients)
        {
            if (item.Value.Contains(Context.ConnectionId))
            {
                item.Value.Remove(Context.ConnectionId);
            }
        }
        await Clients.Caller.SendAsync("StatusChangeOnDevice");
    }
}