using Microsoft.AspNet.SignalR;
using Orbitroids.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Orbitroids.Hubs
{
    public class GameHub : Hub
    {
        private Broadcaster broadcaster;
        public GameHub() : this(Broadcaster.Instance)
        {

        }
        public GameHub(Broadcaster caster)
        {
            broadcaster = caster;
        }

        public void Enter()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.Enter(Clients.Caller, time);
            Clients.All.log("enter");
        }

        public void Pause()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.Pause(Clients.Caller, time);
            Clients.All.log("pause");
        }

        public void Shoot()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.Shoot(Clients.Caller, time);
            Clients.All.log("shoot");
        }

        public void Burn()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.Burn(Clients.Caller, time);
            Clients.All.log("burn");
        }

        public void ReleaseBurn()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.ReleaseBurn(Clients.Caller, time);
            Clients.All.log("release burn");
        }

        public void SlowBurn()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.SlowBurn(Clients.Caller, time);
            Clients.All.log("slow burn");
        }

        public void ReleaseSlowBurn()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.ReleaseSlowBurn(Clients.Caller, time);
            Clients.All.log("release slow burn");
        }

        public void Rotate(string direction)
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.Rotate(direction, Clients.Caller, time);
            Clients.All.log("rotate " + direction);
        }

        public void ReleaseRotate(string direction)
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.ReleaseRotate(direction, Clients.Caller, time);
            Clients.All.log("release rotate " + direction);
        }

        public void DampenControls()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.DampenControls(Clients.Caller, time);
            Clients.All.log("dampen conrols");
        }

        public void ReleaseDampenControls()
        {
            DateTime time = DateTime.UtcNow;
            broadcaster.ReleaseDampenControls(Clients.Caller, time);
            Clients.All.log("release dampen controls");
        }
    }
}