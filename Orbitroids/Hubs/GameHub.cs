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
        public GameHub() : this(Broadcaster.Instance) { }
        public GameHub(Broadcaster caster)
        {
            broadcaster = caster;
        }

        public void Enter()
        {
            broadcaster.Enter(Clients.Caller);
            Clients.All.log("enter");
        }

        public void Pause()
        {
            broadcaster.Pause(Clients.Caller);
            Clients.All.log("pause");
        }

        public void Shoot()
        {
            broadcaster.Shoot(Clients.Caller);
            Clients.All.log("shoot");
        }

        public void Burn()
        {
            broadcaster.Burn(Clients.Caller);
            Clients.All.log("burn");
        }

        public void ReleaseBurn()
        {
            broadcaster.ReleaseBurn(Clients.Caller);
            Clients.All.log("release burn");
        }

        public void SlowBurn()
        {
            broadcaster.SlowBurn(Clients.Caller);
            Clients.All.log("slow burn");
        }

        public void ReleaseSlowBurn()
        {
            broadcaster.ReleaseSlowBurn(Clients.Caller);
            Clients.All.log("release slow burn");
        }

        public void Rotate(string direction)
        {
            broadcaster.Rotate(direction, Clients.Caller);
            Clients.All.log("rotate " + direction);
        }

        public void ReleaseRotate(string direction)
        {
            broadcaster.ReleaseRotate(direction, Clients.Caller);
            Clients.All.log("release rotate " + direction);
        }

        public void DampenControls()
        {
            broadcaster.DampenControls(Clients.Caller);
            Clients.All.log("dampen conrols");
        }

        public void ReleaseDampenControls()
        {
            broadcaster.ReleaseDampenControls(Clients.Caller);
            Clients.All.log("release dampen controls");
        }
    }
}