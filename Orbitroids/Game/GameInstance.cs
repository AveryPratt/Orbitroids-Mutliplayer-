using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Orbitroids.Game.Objects;
using static Orbitroids.Game.Engine;

namespace Orbitroids.Game
{
    public class GameInstance
    {
        public GameInstance(Level level)
        {
            this.Ships.Add(new Ship(VecCirc()));
        }
        public List<Ship> Ships { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Planet> Planets { get; set; }
    }
}