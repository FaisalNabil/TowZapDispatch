using Microsoft.AspNetCore.SignalR;

namespace Dispatch.API.Hubs
{
    public class JobUpdateHub : Hub
    {
        public async Task JoinJobRoom(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
        }

        public async Task LeaveJobRoom(string jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
        }
    }
}
