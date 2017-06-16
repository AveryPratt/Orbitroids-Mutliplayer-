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
        private readonly TimeSpan broadcastInterval = TimeSpan.FromMilliseconds(1000 / 10);
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

        public void Enter(dynamic caller, double time)
        {
        }

        public void Pause(dynamic caller, double time)
        {
        }

        public void Shoot(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "shoot");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void Burn(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "burn");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void ReleaseBurn(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "releaseBurn");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void SlowBurn(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "slow");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void ReleaseSlowBurn(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "releaseSlow");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void Rotate(dynamic caller, double time, string direction)
        {
            if (direction == "left")
            {
                KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "rotateRight");
                model.Ships[0].Commands.Enqueue(command);
            }
            else if (direction == "right")
            {
                KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "rotateLeft");
                model.Ships[0].Commands.Enqueue(command);
            }
        }

        public void ReleaseRotate(dynamic caller, double time, string direction)
        {
            if (direction == "left")
            {
                KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "releaseRotateRight");
                model.Ships[0].Commands.Enqueue(command);
            }
            else if (direction == "right")
            {
                KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "releaseRotateLeft");
                model.Ships[0].Commands.Enqueue(command);
            }
        }

        public void DampenControls(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "dampenRot");
            model.Ships[0].Commands.Enqueue(command);
        }

        public void ReleaseDampenControls(dynamic caller, double time)
        {
            KeyValuePair<double, string> command = new KeyValuePair<double, string>(time, "releaseDampenRot");
            model.Ships[0].Commands.Enqueue(command);
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