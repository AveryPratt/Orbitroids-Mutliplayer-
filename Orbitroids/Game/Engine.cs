using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using static Orbitroids.Game.Objects;
using static Orbitroids.Game.Geometry;

namespace Orbitroids.Game
{
    [DataContract]
    public class Engine
    {
        public Engine(Level level)
        {
            this.Ships = new List<Ship>();
            this.Shots = new List<Shot>();
            this.Asteroids = new List<Asteroid>();
            this.Planets = new List<Planet>();

            this.Ships.Add(new Ship(VecCirc()));
        }
        [DataMember]
        public List<Ship> Ships { get; set; }
        [DataMember]
        public List<Shot> Shots { get; set; }
        [DataMember]
        public List<Asteroid> Asteroids { get; set; }
        [DataMember]
        public List<Planet> Planets { get; set; }
        
        public void Update()
        {

        }
    }
}