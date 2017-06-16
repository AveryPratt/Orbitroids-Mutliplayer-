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

        public void Enter(double time)
        {
            broadcaster.Enter(Clients.Caller, time);
            Clients.All.log("enter");
        }

        public void Pause(double time)
        {
            broadcaster.Pause(Clients.Caller, time);
            Clients.All.log("pause");
        }

        public void Shoot(double time)
        {
            broadcaster.Shoot(Clients.Caller, time);
            Clients.All.log("shoot");
        }

        public void Burn(double time)
        {
            broadcaster.Burn(Clients.Caller, time);
            Clients.All.log("burn");
        }

        public void ReleaseBurn(double time)
        {
            broadcaster.ReleaseBurn(Clients.Caller, time);
            Clients.All.log("release burn");
        }

        public void SlowBurn(double time)
        {
            broadcaster.SlowBurn(Clients.Caller, time);
            Clients.All.log("slow burn");
        }

        public void ReleaseSlowBurn(double time)
        {
            broadcaster.ReleaseSlowBurn(Clients.Caller, time);
            Clients.All.log("release slow burn");
        }

        public void Rotate(double time, string direction)
        {
            broadcaster.Rotate(Clients.Caller, time, direction);
            Clients.All.log("rotate " + direction);
        }

        public void ReleaseRotate(double time, string direction)
        {
            broadcaster.ReleaseRotate(Clients.Caller, time, direction);
            Clients.All.log("release rotate " + direction);
        }

        public void DampenControls(double time)
        {
            broadcaster.DampenControls(Clients.Caller, time);
            Clients.All.log("dampen conrols");
        }

        public void ReleaseDampenControls(double time)
        {
            broadcaster.ReleaseDampenControls(Clients.Caller, time);
            Clients.All.log("release dampen controls");
        }
    }
}