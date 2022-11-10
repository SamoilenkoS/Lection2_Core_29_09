using Lection2_Core.Core;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Jobs
{
    public class SendRegularMessageToClientsJob : IJob
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public SendRegularMessageToClientsJob(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _hubContext.Clients.All.SendAsync("GetMessage", new MessageSnapshot
            {
                IsPersonal = false,
                Message = "Hello from server",
                SenderUserInfo = new PublicUserInfo
                {
                    Nickname = "Server"
                }
            });
        }
    }
}
