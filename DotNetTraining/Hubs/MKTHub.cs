using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BPMaster.Hubs
{
    public class MKTHub : Hub
    {
        public async Task RequestTagValues()
        {
            // This method can be called from the client to request tag values
            await Clients.Caller.SendAsync("RequestTagValues");
        }
    }
}
