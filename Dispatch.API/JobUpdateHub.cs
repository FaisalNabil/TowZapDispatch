using Microsoft.AspNetCore.SignalR;

namespace Dispatch.API.Hubs
{
    public class JobUpdateHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
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
        public async Task SendLocationUpdate(string jobId, double lat, double lng)
        {
            await Clients.Group(jobId).SendAsync("ReceiveLocationUpdate", lat, lng);
        }

    }
}
