using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orbitroids.Hubs
{
    public class GameHub : Hub
    {
        public void FlashColor()
        {
            Clients.All.flashColor();
        }

        public void Enter()
        {
            Clients.All.log("enter");
        }

        public void Pause()
        {
            Clients.All.log("pause");
        }

        public void Shoot()
        {
            Clients.All.log("shoot");
        }

        public void Burn()
        {
            Clients.All.log("burn");
        }

        public void ReleaseBurn()
        {
            Clients.All.log("release burn");
        }

        public void SlowBurn()
        {
            Clients.All.log("slow burn");
        }

        public void ReleaseSlowBurn()
        {
            Clients.All.log("release slow burn");
        }

        public void Rotate(string direction)
        {
            Clients.All.log("rotate " + direction);
        }

        public void ReleaseRotate(string direction)
        {
            Clients.All.log("release rotate " + direction);
        }

        public void DampenControls()
        {
            Clients.All.log("dampen conrols");
        }

        public void ReleaseDampenControls()
        {
            Clients.All.log("release dampen controls");
        }
    }
}