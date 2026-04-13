using Microsoft.AspNetCore.SignalR;
using Tenfluxa.Application.Interfaces;
using Tenfluxa.Api.Hubs;

namespace Tenfluxa.Api.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<JobHub> _hubContext;

    public SignalRNotificationService(IHubContext<JobHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyJobAssignedAsync(Guid jobId, Guid workerId)
    {
        await _hubContext.Clients.All.SendAsync(
            "JobAssigned",
            new
            {
                jobId,
                workerId
            });
    }
}