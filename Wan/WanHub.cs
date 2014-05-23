using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Wan.Controllers.ApiControllers;

namespace Wan
{
    [HubName("joinmeHub")]
    public class WanHub : Hub
    {
        public void NewGroup(GroupViewModel groupViewModel)
        {
            Clients.All.showNewGroup(groupViewModel);
        }
    }
}