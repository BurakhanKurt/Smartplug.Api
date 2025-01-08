using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Smartplug.Persistence;


namespace Smartplug.Application.Scoket
{
    public class PlugHub(SmartplugDbContext dbContext)
        : Hub
    {
        public static ConcurrentDictionary<Guid, string> ConnectedClients { get; private set; } = new();

        public async Task AddList(string deviceId)
        {
            ConnectedClients.TryRemove(Guid.Parse(deviceId), out _);
            ConnectedClients.TryAdd(Guid.Parse(deviceId), Context.ConnectionId);
            await dbContext.Devices.Where(x => x.Id == Guid.Parse(deviceId))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.IsOnline, true));
        }

        public async Task ChangeStatus(bool status)
        {
            var key = ConnectedClients.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            await dbContext.Devices.Where(x => x.Id == key)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.IsWorking, status));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var key = ConnectedClients.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            ConnectedClients.TryRemove(key, out _);

            await dbContext.Devices.Where(x => x.Id == key)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.IsOnline, false));
        }
    }
}