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
        public Engine(Level level)
        {
            this.Planets = new List<Planet>();
            this.Asteroids = new List<Asteroid>();
            this.Shots = new List<Shot>();
            this.Ships = new List<Ship>();
            this.MaxShots = 30;
            this.MaxAsteroids = 30;

            this.Planets.Add(new Planet(1000, 50, color: "#0080ff"));
            this.Asteroids.Add(new Asteroid(VecCirc(), 50));
            this.Ships.Add(new Ship(VecCirc()));

            int barycenterMass = 0;
            foreach (Planet p in this.Planets)
            {
                barycenterMass += p.Mass;
            }
            this.Barycenter = new Planet(barycenterMass, 0);

            this.SetShip(this.Ships[0]);
            this.SetAsteroid(this.Asteroids[0]);
        }

        public List<Planet> Planets { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Ship> Ships { get; set; }
        public IMassive Barycenter { get; private set; }
        public int MaxShots { get; set; }
        public int MaxAsteroids { get; set; }

        public void SetShip(Ship ship)
        {
            Coordinate shipStartCoord = new Coordinate(0, -150);
            ship.Vel = VecCirc(Math.PI / 2, Physics.GetOrbitalVelocity(shipStartCoord, this.Barycenter), shipStartCoord);
            ship.DeltaRot = 0;
        }

        public void SetAsteroid(Asteroid asteroid)
        {
            Coordinate asteroidStartCoord = new Coordinate(0, 150);
            asteroid.Vel = VecCirc(3 * Math.PI / 2, Physics.GetOrbitalVelocity(asteroidStartCoord, this.Barycenter), asteroidStartCoord);
            Random rand = new Random();
            asteroid.DeltaRot = (rand.NextDouble() - .5) / 10;
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
                for (int i = 0; i < splits; i++)
                {
                    Random rand = new Random();
                    double area = rand.Next(1, 4) * .5 * remainingArea / (splits - i);
                    double angle = 2 * Math.PI / (splits - i);
                    double speed = power / area;
                    newAsteroids[i] = new Asteroid(AddVectors(asteroid.Vel, VecCirc(angle, speed)), Math.Sqrt(area), asteroid.Roughness, asteroid.Color, (rand.NextDouble() - .5) / 10);
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
                Asteroid newAst = new Asteroid(VecCirc(), 50);
                this.Asteroids.Add(newAst);
                SetAsteroid(newAst);
            }
        }
    }
}