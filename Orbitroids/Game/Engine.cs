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

            Planet planet = new Planet(100, 50);
            Coordinate asteroidStartCoord = new Coordinate(0, 100);
            Coordinate shipStartCoord = new Coordinate(0, -100);

            this.Planets.Add(planet);
            this.Asteroids.Add(new Asteroid(VecCirc(Math.PI / 2, Physics.GetOrbitalVelocity(asteroidStartCoord, planet))));
            this.Shots.Add(new Shot(VecCirc()));
            this.Ships.Add(new Ship(VecCirc( 3 * Math.PI / 2, Physics.GetOrbitalVelocity(shipStartCoord, planet), shipStartCoord)));
        }
        public List<Planet> Planets { get; set; }
        public List<Asteroid> Asteroids { get; set; }
        public List<Shot> Shots { get; set; }
        public List<Ship> Ships { get; set; }

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
                    {
                        planet.ApplyGravity(otherPlanet);
                    }
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
                    {
                        ship.Burn(ship.DampenBurnPower);
                    }
                    else
                    {
                        ship.Burn(ship.BurnPower);
                    }
                }
                if (ship.Loaded)
                {
                    ship.Shoot();
                }
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
            }
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
            Collisions.HandleCollisions(
                this.Shots, 
                this.Asteroids, 
                this.Planets, 
                this.Ships, 
                this.destroyShots, 
                this.destroyShips, 
                this.destroyAsteroids);
        }
    }
}