using Microsoft.AspNet.SignalR;
using Orbitroids.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Orbitroids.Game
{
    // Broadcaster class is modified from https://docs.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-high-frequency-realtime-with-signalr
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> instance = new Lazy<Broadcaster>(() => new Broadcaster());
        private readonly TimeSpan broadcastInterval = TimeSpan.FromMilliseconds(1000 / 60);
        private readonly IHubContext hubContext;
        private Level level;
        private Timer broadcastLoop;
        private Engine model;
        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            model = new Engine(level, broadcastInterval.TotalMilliseconds);
            // Start the broadcast loop
            broadcastLoop = new Timer(
                RenderClientFrame,
                null,
                broadcastInterval,
                broadcastInterval);
        }
        
        public void RenderClientFrame(object state)
        {
            model.Update();
            hubContext.Clients.All.receiveUpdate(model, broadcastInterval.TotalMilliseconds);
        }

        public void Enter(dynamic caller)
        {
        }

        public void Pause(dynamic caller)
        {
        }

        public void Shoot(dynamic caller)
        {
            model.Ships[0].Loaded = true;
        }

        public void Burn(dynamic caller)
        {
            model.Ships[0].Burning = true;
        }

        public void ReleaseBurn(dynamic caller)
        {
            model.Ships[0].Burning = false;
        }

        public void SlowBurn(dynamic caller)
        {
            model.Ships[0].DampenBurn = true;
        }

        public void ReleaseSlowBurn(dynamic caller)
        {
            model.Ships[0].DampenBurn = false;
        }

        public void Rotate(string direction, dynamic caller)
        {
            if (direction == "left")
                model.Ships[0].RotDirection = "left";
            else if (direction == "right")
                model.Ships[0].RotDirection = "right";
        }

        public void ReleaseRotate(string direction, dynamic caller)
        {
            model.Ships[0].RotDirection = null;
        }

        public void DampenControls(dynamic caller)
        {
            model.Ships[0].DampenRot = true;
        }

        public void ReleaseDampenControls(dynamic caller)
        {
            model.Ships[0].DampenRot = false;
        }
        public static Broadcaster Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}