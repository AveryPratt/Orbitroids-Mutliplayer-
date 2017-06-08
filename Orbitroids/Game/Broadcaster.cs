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
        private readonly TimeSpan BroadcastInterval = TimeSpan.FromMilliseconds(40);
        private readonly IHubContext hubContext;
        private Level level;
        private Timer broadcastLoop;
        private GameInstance model;
        private bool modelUpdated;
        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            model = new GameInstance(level);
            modelUpdated = false;
            // Start the broadcast loop
            broadcastLoop = new Timer(
                BroadcastShape,
                null,
                BroadcastInterval,
                BroadcastInterval);
        }
        public void BroadcastShape(object state)
        {
            // No need to send anything if our model hasn't changed
            if (modelUpdated)
            {
                // This is how we can access the Clients property 
                // in a static hub method or outside of the hub entirely
                hubContext.Clients.All.updateShape(model);
                modelUpdated = false;
            }
        }

        public void Enter(dynamic caller)
        {
        }

        public void Pause(dynamic caller)
        {
        }

        public void Shoot(dynamic caller)
        {
            model.Ships[0].Shoot();
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
                model.Ships[0].AccelRot = model.Ships[0].RotPower;
            else if (direction == "right")
                model.Ships[0].AccelRot = -model.Ships[0].RotPower;
        }

        public void ReleaseRotate(string direction, dynamic caller)
        {
            if ((direction == "left" && model.Ships[0].AccelRot > 0) ||
                (direction == "right" && model.Ships[0].AccelRot < 0))
                model.Ships[0].AccelRot = 0;
        }

        public void DampenControls(dynamic caller)
        {
            model.Ships[0].RotPower = .03;
        }

        public void ReleaseDampenControls(dynamic caller)
        {
            model.Ships[0].RotPower = .1;
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