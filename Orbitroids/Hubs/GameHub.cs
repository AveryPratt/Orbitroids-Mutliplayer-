using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orbitroids.Hubs
{
    public class GameHub : Hub
    {
        public void SubmitInput()
        {
            Clients.All.update();
        }
    }
}