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
    public class Engine
    {
        public Engine(Level level, double framerate)
        {
            this.Framerate = framerate;
            this.Planets = new List<Planet>();
            this.Asteroids = new List<Asteroid>();
            this.Shots = new List<Shot>();
            this.Ships = new List<Ship>();
            this.MaxShots = 30;
            this.MaxAsteroids = 30;

            Planet planet = new Planet(Math.Pow(this.Framerate, 2), 50, color: "#0080ff");
            Coordinate asteroidStartCoord = new Coordinate(0, 100);
            Coordinate shipStartCoord = new Coordinate(0, -100);

            this.Planets.Add(planet);
            this.Asteroids.Add(new Asteroid(VecCirc(3 * Math.PI / 2, Physics.GetOrbitalVelocity(asteroidStartCoord, planet), asteroidStartCoord)));
            this.Ships.Add(new Ship(VecCirc(Math.PI / 2, Physics.GetOrbitalVelocity(shipStartCoord, planet), shipStartCoord), this.Framerate));

            double barycenterMass = 0;
            foreach (Planet p in this.Planets)
            {
                barycenterMass += p.Mass;
            }
            this.Barycenter = new Planet(barycenterMass, 0);
        }
        public List<Planet> Planets { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Ship> Ships { get; set; }
        public IMassive Barycenter { get; set; }
        public int MaxShots { get; set; }
        public int MaxAsteroids { get; set; }
        public double Framerate { get; set; }

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
                //int splits = Convert.ToInt32(Math.Floor(Math.Sqrt(asteroid.Radius / 2))); // 8-2 18-3 32-4 50-5
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
                for (int i = 0; i < splits; i++)
                {
                    Random rand = new Random();
                    double area = rand.Next(1, 4) * .5 * remainingArea / (splits - i);
                    double angle = 2 * Math.PI / (splits - i);
                    double speed = power / area;
                    newAsteroids[i] = new Asteroid(AddVectors(asteroid.Vel, VecCirc(angle, speed)), Math.Sqrt(area), asteroid.Roughness, asteroid.Color, asteroid.DeltaRot);
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

        private void destroyExtraShots()
        {
            // removes oldest shots
            if (this.Shots.Count() > this.MaxShots)
            {
                List<Shot> remainingShots = this.Shots.ToList();
                remainingShots.RemoveRange(0, this.Shots.Count() - this.MaxShots);
                this.Shots = remainingShots;
            }
        }

        private void destroyExtraAsteroids()
        {
            // removes most recent asteroids
            if (this.Asteroids.Count() > this.MaxAsteroids)
            {
                List<Asteroid> remainingAsteroids = this.Asteroids.ToList();
                remainingAsteroids.RemoveRange(this.MaxAsteroids, this.Asteroids.Count() - this.MaxAsteroids);
                this.Asteroids = remainingAsteroids;
            }
        }

        public void Update()
        {
            DateTime time = DateTime.UtcNow;
            applyMotion();

            Collisions.HandleCollisions(
                this.Shots,
                this.Asteroids,
                this.Planets,
                this.Ships,
                this.Barycenter,
                this.destroyShots,
                this.destroyShips,
                this.destroyAsteroids);
            destroyExtraShots();
            destroyExtraAsteroids();
        }
    }
}