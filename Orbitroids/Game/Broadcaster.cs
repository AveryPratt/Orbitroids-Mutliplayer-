﻿using Microsoft.AspNet.SignalR;
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
        private readonly TimeSpan broadcastInterval = TimeSpan.FromMilliseconds(16);
        private readonly IHubContext hubContext;
        private Timer broadcastLoop;
        private Engine model;
        public int Level { get; set; }
        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            model = new Engine(this.Level);
            // Start the broadcast loop
            broadcastLoop = new Timer(
                RenderFrame,
                null,
                TimeSpan.FromMilliseconds(0),
                broadcastInterval);
        }

        public void SetLevel(int level)
        {
            this.Level = level;
            this.model = new Engine(level);
        }

        public void RenderFrame(object state)
        {
            model.Update();
            hubContext.Clients.All.renderFrame(model);
            hubContext.Clients.All.log(model.Timespan);
        }

        public void Enter(dynamic caller)
        {
            model.Ships[0].Destroyed = false;
            model.Ships[0].Loaded = false;
            model.SetShip();
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
                model.Ships[0].IsRotating = "left";
            else if (direction == "right")
                model.Ships[0].IsRotating = "right";
        }

        public void ReleaseRotate(string direction, dynamic caller)
        {
            model.Ships[0].IsRotating = null;
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