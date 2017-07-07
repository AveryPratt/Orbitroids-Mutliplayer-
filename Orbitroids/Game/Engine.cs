using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using static Orbitroids.Game.Objects;
using static Orbitroids.Game.Geometry;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Orbitroids.Game
{
    public class Engine
    {
        public Engine(int levelNumber)
        {
            this.Planets = new List<Planet>();
            this.Asteroids = new List<Asteroid>();
            this.Shots = new List<Shot>();
            this.Ships = new List<Ship>();
            this.MaxShots = 30;
            this.MaxAsteroids = 30;

            XmlDocument levelConfig = new XmlDocument();
            levelConfig.Load(HttpContext.Current.Server.MapPath("/Levels/Levels.xml"));
            this.Level = levelConfig.DocumentElement.ChildNodes[levelNumber];

            this.SunAngle = Math.PI;
            this.SunRot = Convert.ToDouble((from XmlNode node in this.Level.ChildNodes
                                            where node.Name == "SunRot"
                                            select node).First().InnerText);

            SetPlanets();

            int barycenterMass = 0;
            foreach (Planet p in this.Planets)
            {
                barycenterMass += p.Mass;
            }
            this.Barycenter = new Planet(barycenterMass, 0);

            this.SetShip();
            this.SetNewWave();
        }

        public double SunRot { get; set; }
        public double SunAngle { get; set; }
        public XmlNode Level { get; private set; }
        public List<Planet> Planets { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Ship> Ships { get; set; }
        public IMassive Barycenter { get; private set; }
        public int MaxShots { get; set; }
        public int MaxAsteroids { get; set; }

        private void SetPlanets()
        {
            XmlNode planetConfig = (from XmlNode node in this.Level.ChildNodes
                                    where node.Name == "Planets"
                                    select node).First();

            int  spread = 0, totalMass = 0, count = planetConfig.ChildNodes.Count;
            if (count > 1)
                Int32.TryParse(planetConfig.Attributes["spread"].Value, out spread);

            foreach (XmlNode planet in planetConfig.ChildNodes)
            {
                int mass;
                Int32.TryParse(planet.Attributes["mass"].Value, out mass);
                totalMass += mass;
            }

            this.Barycenter = new Planet(totalMass, 0);

            for (int i = 0; i < count; i++)
            {
                double vel = 0;
                double forwardAngle = 0;
                Coordinate coord = new Coordinate(0, 0); 
                int mass, radius;                   
                bool atmosphere;
                Int32.TryParse(planetConfig.ChildNodes[i].Attributes["mass"].Value, out mass);
                Int32.TryParse(planetConfig.ChildNodes[i].Attributes["radius"].Value, out radius);
                bool.TryParse(planetConfig.ChildNodes[i].Attributes["atmosphere"].Value, out atmosphere);
                string color = planetConfig.ChildNodes[i].Attributes["color"].Value;
                
                if (count == 2)
                {
                    forwardAngle = i == 0 ? Math.PI : 0;
                    double dist = spread - spread * mass / totalMass;
                    coord.X = i == 0 ? dist : -dist;

                    vel = Physics.GetOrbitalVelocity(new Coordinate(0, spread), this.Barycenter);
                    vel = vel * dist / spread;
                }

                this.Planets.Add(new Planet(mass, radius, VecCirc(forwardAngle, vel, coord), color, atmosphere));
            }
        }

        public void SetShip()
        {
            XmlNode shipConfig = (from XmlNode node in this.Level.ChildNodes
                                  where node.Name == "Ship"
                                  select node).First();

            if (this.Ships.Count == 0)
                this.Ships.Add(new Ship(VecCirc()));

            int altitude = 0, parent = 0;
            Int32.TryParse(shipConfig.Attributes["altitude"].Value, out altitude);
            Int32.TryParse(shipConfig.Attributes["parent"].Value, out parent);
            
            IMassive parentBody = parent == 0 ? this.Barycenter : this.Planets[parent - 1];
            Vector distVec = VecCirc(Math.PI, altitude, parentBody.Vel.Origin);
            this.Ships[0].Vel = VecCirc(Math.PI / 2, Physics.GetOrbitalVelocity(distVec.Head, parentBody), distVec.Head);
            this.Ships[0].DeltaRot = 0;
            this.Ships[0].ForwardAngle = 0;
        }

        public void SetNewWave()
        {
            XmlNode asteroidConfig = (from XmlNode node in this.Level.ChildNodes
                                      where node.Name == "Asteroid"
                                      select node).First();

            int number = 0, altitude = 0, parent = 0;
            Int32.TryParse(asteroidConfig.Attributes["number"].Value, out number);
            Int32.TryParse(asteroidConfig.Attributes["altitude"].Value, out altitude);
            Int32.TryParse(asteroidConfig.Attributes["parent"].Value, out parent);
            Random rand = new Random();

            for (int i = 0; i < number; i++)
            {
                this.Asteroids.Add(new Asteroid(rand, VecCirc(), 50));
                
                double forwardAngle = i * 2 * Math.PI / number - Math.PI / 2;
                IMassive parentBody = parent == 0 ? this.Barycenter : this.Planets[parent - 1];
                Vector distVec = VecCirc(i * 2 * Math.PI / number, altitude, parentBody.Vel.Origin);
                this.Asteroids.Last().Vel = VecCirc(forwardAngle, Physics.GetOrbitalVelocity(distVec.Head, parentBody), distVec.Head);
                this.Asteroids.Last().DeltaRot = (rand.NextDouble() - .5) / 10;
            }
        }

        private void applyMotion()
        {
            applyShipMotion();
            applyPlanetMotion();
            applyShotMotion();
            applyAsteroidMotion();
        }

        private void applyAsteroidMotion()
        {
            foreach (Asteroid asteroid in this.Asteroids)
            {
                foreach (Planet planet in this.Planets)
                {
                    asteroid.ApplyGravity(planet);
                }
                asteroid.ApplyMotion();
            }
        }

        private void applyShotMotion()
        {
            foreach (Shot shot in this.Shots)
            {
                foreach (Planet planet in this.Planets)
                {
                    shot.ApplyGravity(planet);
                }
                shot.ApplyMotion();
            }
        }

        private void applyPlanetMotion()
        {
            foreach (Planet planet in this.Planets)
            {
                foreach (Planet otherPlanet in this.Planets)
                {
                    if (!ReferenceEquals(planet, otherPlanet))
                        planet.ApplyGravity(otherPlanet);
                }
                planet.ApplyMotion();
            }
        }

        private void applyShipMotion()
        {
            foreach (Ship ship in this.Ships)
            {
                if (ship.Destroyed)
                {
                    ship.Vel = null;
                    continue;
                }

                foreach (Planet planet in this.Planets)
                {
                    ship.ApplyGravity(planet);
                }
                if (ship.Burning)
                {
                    if (ship.DampenBurn)
                        ship.Burn(ship.DampenBurnPower);
                    else
                        ship.Burn(ship.BurnPower);
                }
                if (ship.Loaded)
                    this.Shots.Add(ship.Shoot());

                ship.ApplyMotion();
            }
        }

        private void destroyShots(IEnumerable<Shot> shots)
        {
            foreach (Shot shot in shots)
            {
                this.Shots.Remove(shot);
            }
        }

        private void destroyAsteroids(IEnumerable<Asteroid> asteroids, bool split)
        {
            foreach (Asteroid asteroid in asteroids)
            {
                this.Asteroids.Remove(asteroid);
                if (split)
                {
                    Asteroid[] newAsteroids = splitAsteroid(asteroid, 3, 100);
                    if (newAsteroids != null)
                        this.Asteroids.AddRange(newAsteroids);
                }
            }
        }

        private Asteroid[] splitAsteroid(Asteroid asteroid, int splits, double power)
        {
            if(asteroid.Radius > 15)
            {
                double asteroidArea = Math.Pow(asteroid.Radius, 2);
                double remainingArea = asteroidArea;
                Asteroid[] newAsteroids = new Asteroid[splits];
                Random rand = new Random();
                double randAngle = rand.NextDouble() * 2 * Math.PI;
                for (int i = 0; i < splits; i++)
                {
                    double area = rand.Next(1, 4) * .5 * remainingArea / (splits - i);
                    double angle = 2 * Math.PI / (splits - i);
                    double speed = power / area;
                    newAsteroids[i] = new Asteroid(rand, AddVectors(asteroid.Vel, VecCirc(randAngle + angle, speed)), Math.Sqrt(area), asteroid.Roughness, asteroid.Color, (rand.NextDouble() - .5) / 10);
                    remainingArea -= area;
                }
                return newAsteroids;
            }
            return null;
        }

        private void destroyShips(IEnumerable<Ship> ships)
        {
            foreach (Ship ship in ships)
            {
                ship.Destroyed = true;
            }
        }

        public void Update()
        {
            this.SunAngle += this.SunRot;
            applyMotion();
            checkWave();
            Collisions.HandleCollisions(
                this.Shots,
                this.Asteroids,
                this.Planets,
                this.Ships,
                this.Barycenter,
                this.destroyShots,
                this.destroyShips,
                this.destroyAsteroids,
                this.MaxShots,
                this.MaxAsteroids);
        }

        private void checkWave()
        {
            bool toLaunch = false;
            if (this.Asteroids.Count == 0)
                toLaunch = true;
            else
            {
                toLaunch = true;
                foreach (Asteroid asteroid in this.Asteroids)
                {
                    if (Math.Abs(asteroid.Vel.Origin.X) <= 500 && Math.Abs(asteroid.Vel.Origin.Y) <= 500)
                        toLaunch = false;
                }
            }
            if (toLaunch)
                SetNewWave();
        }
    }
}