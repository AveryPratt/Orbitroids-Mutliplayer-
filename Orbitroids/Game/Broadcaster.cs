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
            this.Commands = new Queue<Command>();

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

        public DateTime LastUpdate { get; set; }
        public Queue<Command> Commands { get; set; }

        public void RenderClientFrame(object state)
        {
            update(model);
            hubContext.Clients.All.receiveUpdate(model, broadcastInterval.TotalMilliseconds);
        }

        private void update(Engine model)
        {
            while (Commands.Count > 0)
            {
                Command command = Commands.Dequeue();
                model.ExecuteCommand(command);
            }
        }

        public void Enter(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("enter", time, caller));
        }

        public void Pause(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("pause", time, caller));
        }

        public void Shoot(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("shoot", time, caller));
        }

        public void Burn(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("burn", time, caller));
        }

        public void ReleaseBurn(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("releaseBurn", time, caller));
        }

        public void SlowBurn(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("slowBurn", time, caller));
        }

        public void ReleaseSlowBurn(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("releaseSlowBurn", time, caller));
        }

        public void Rotate(string direction, dynamic caller, DateTime time)
        {
            if (direction == "left")
            {
                this.Commands.Enqueue(new Command("rotateLeft", time, caller));
            }
            else if (direction == "right")
            {
                this.Commands.Enqueue(new Command("rotateRight", time, caller));
            }
        }

        public void ReleaseRotate(string direction, dynamic caller, DateTime time)
        {
            if (direction == "left")
            {
                this.Commands.Enqueue(new Command("releaseRotateLeft", time, caller));
            }
            else if (direction == "right")
            {
                this.Commands.Enqueue(new Command("releaseRotateRight", time, caller));
            }
        }

        public void DampenControls(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("dampenRot", time, caller));
        }

        public void ReleaseDampenControls(dynamic caller, DateTime time)
        {
            this.Commands.Enqueue(new Command("releaseDampenRot", time, caller));
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