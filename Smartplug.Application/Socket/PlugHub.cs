﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Smartplug.Persistence;


namespace Smartplug.Application.Scoket
{
    public class PlugHub(SmartplugDbContext dbContext, ILogger<PlugHub> logger,IHubContext<DeviceHub> hubContext)
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
            Console.WriteLine(status);
            var key = ConnectedClients.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            await dbContext.Devices.Where(x => x.Id == key)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.IsWorking, status));
            
            var userId = await dbContext.Devices
                .Where(x => x.Id == key)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();
            
            var connectionIds = DeviceHub.ConnectedClients.Where(x => x.Key == userId).SelectMany(x => x.Value).ToList();
            
            if (connectionIds.Any())
            {
                foreach (var connectionId in connectionIds)
                {
                    await hubContext.Clients.Client(connectionId).SendAsync("StatusChangeOnDevice");
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("DeviceStatus", "Cihaz baglandi su id ile:", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var key = ConnectedClients.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            ConnectedClients.TryRemove(key, out _);

            await dbContext.Devices.Where(x => x.Id == key)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.IsOnline, false)
                        .SetProperty(p => p.IsWorking, false));

            await Clients.All.SendAsync("DeviceStatus", "Cihaz baglantisi kesildi su id ile:", Context.ConnectionId);
        }
    }
}