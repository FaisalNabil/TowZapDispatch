using Microsoft.AspNetCore.SignalR;

namespace Dispatch.API.Hubs
{
    public class JobUpdateHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            // Still allow jobId or companyId group joining if needed
            var httpContext = Context.GetHttpContext();
            var jobId = httpContext?.Request.Query["jobId"].ToString();
            if (!string.IsNullOrEmpty(jobId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var jobId = httpContext?.Request.Query["jobId"].ToString();

            if (!string.IsNullOrEmpty(jobId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task JoinJobRoom(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
        }

        public async Task LeaveJobRoom(string jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
        }
        public async Task SendLocationUpdate(string jobId, double lat, double lng)
        {
            await Clients.Group(jobId).SendAsync("ReceiveLocationUpdate", lat, lng);
        }
        public async Task JobDeclined(Guid jobId)
        {
            // Optional: get dispatcher group or company info from context or database
            // For now, broadcast to the job group
            await Clients.Group(jobId.ToString()).SendAsync("ReceiveJobDeclined", jobId);

            // OR: if you have a dispatcher group or companyId
            // await Clients.Group("dispatchers").SendAsync("ReceiveJobDeclined", jobId);
        }


    }
}
