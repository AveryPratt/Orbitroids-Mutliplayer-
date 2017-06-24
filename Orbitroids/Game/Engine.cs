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
            XmlNode level = levelConfig.DocumentElement.ChildNodes[levelNumber];

            XmlNodeList planetConfig = (from XmlNode node in level.ChildNodes
                                        where node.Name == "Planets"
                                        select node.ChildNodes).First();

            XmlNode shipConfig = (from XmlNode node in level.ChildNodes
                                  where node.Name == "Ships"
                                  select node).First();

            XmlNode asteroidConfig = (from XmlNode node in level.ChildNodes
                                      where node.Name == "Asteroid"
                                      select node).First();

            foreach (XmlNode planet in planetConfig)
            {
                Vector vel = VecCirc();
                int mass, radius;
                bool atmosphere;
                Int32.TryParse(planet.Attributes["mass"].Value, out mass);
                Int32.TryParse(planet.Attributes["radius"].Value, out radius);
                string color = planet.Attributes["color"].Value;
                bool.TryParse(planet.Attributes["atmosphere"].Value, out atmosphere);

                this.Planets.Add(new Planet(mass, radius, vel, color, atmosphere));
            }

            int barycenterMass = 0;
            foreach (Planet p in this.Planets)
            {
                barycenterMass += p.Mass;
            }
            this.Barycenter = new Planet(barycenterMass, 0);

            this.SetShip(shipConfig);
            this.SetAsteroid(asteroidConfig);
        }

        public List<Planet> Planets { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Ship> Ships { get; set; }
        public IMassive Barycenter { get; private set; }
        public int MaxShots { get; set; }
        public int MaxAsteroids { get; set; }

        public void SetPlanets(XmlNodeList config)
        {

        }

        public void SetShip(XmlNode config)
        {
            this.Ships.Add(new Ship(VecCirc()));

            int altitude;
            Int32.TryParse(config.Attributes["altitude"].Value, out altitude);

            Coordinate shipStartCoord = new Coordinate(0, -altitude);
            this.Ships[0].Vel = VecCirc(Math.PI / 2, Physics.GetOrbitalVelocity(shipStartCoord, this.Barycenter), shipStartCoord);
            this.Ships[0].DeltaRot = 0;
            this.Ships[0].ForwardAngle = 0;
        }

        public void SetAsteroid(XmlNode config)
        {
            this.Asteroids.Add(new Asteroid(new Random(), VecCirc(), 50));

            int altitude;
            Int32.TryParse(config.Attributes["altitude"].Value, out altitude);

            Coordinate asteroidStartCoord = new Coordinate(0, altitude);
            this.Asteroids[0].Vel = VecCirc(3 * Math.PI / 2, Physics.GetOrbitalVelocity(asteroidStartCoord, this.Barycenter), asteroidStartCoord);
            Random rand = new Random();
            this.Asteroids[0].DeltaRot = (rand.NextDouble() - .5) / 10;
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

        private void destroyAsteroids(IEnumerable<Asteroid> asteroids)
        {
            foreach (Asteroid asteroid in asteroids)
            {
                this.Asteroids.Remove(asteroid);
                Asteroid[] newAsteroids = splitAsteroid(asteroid, 3, 50);
                if(newAsteroids != null)
                    this.Asteroids.AddRange(newAsteroids);
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
                for (int i = 0; i < splits; i++)
                {
                    double area = rand.Next(1, 4) * .5 * remainingArea / (splits - i);
                    double angle = 2 * Math.PI / (splits - i);
                    double speed = power / area;
                    newAsteroids[i] = new Asteroid(rand, AddVectors(asteroid.Vel, VecCirc(angle, speed)), Math.Sqrt(area), asteroid.Roughness, asteroid.Color, (rand.NextDouble() - .5) / 10);
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
            applyMotion();
            setNewWave();
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

        private void setNewWave()
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
            {
                Asteroid newAst = new Asteroid(new Random(), VecCirc(), 50);
                this.Asteroids.Add(newAst);
                //SetAsteroid(newAst);
            }
        }
    }
}