using System;
using System.Collections.Generic;
using System.Linq;
using static Orbitroids.Game.Engine;
using static Orbitroids.Game.Objects;
using static Orbitroids.Game.Physics;

namespace Orbitroids.Game
{
    public static class Collisions
    {
        public delegate void DestroyShots(IEnumerable<Shot> shots);
        public delegate void DestroyAsteroids(IEnumerable<Asteroid> asteroids);
        public delegate void DestroyShips(IEnumerable<Ship> ships);

        public static void HandleCollisions(IEnumerable<Shot> shots, IEnumerable<Asteroid> asteroids, IEnumerable<Planet> planets, IEnumerable<Ship> ships, DestroyShots destroyShots, DestroyShips destroyShips, DestroyAsteroids destroyAsteroids)
        {
            CheckShotAsteroidCollision(shots, asteroids, destroyShots, destroyAsteroids);
            CheckShotShipCollision(shots, ships, destroyShots, destroyShips);
            CheckShotPlanetCollision(shots, planets, destroyShots);
            CheckAsteroidShipCollision(asteroids, ships, destroyAsteroids);
            CheckAsteroidPlanetCollision(asteroids, planets, destroyAsteroids);
            CheckShipPlanetCollision(ships, planets, destroyShips);
        }
        public static void CheckShotAsteroidCollision(IEnumerable<Shot> shots, IEnumerable<Asteroid> asteroids, DestroyShots destroyShots, DestroyAsteroids destroyAsteroids) // 50 * 30 = 1500 / 2082
        {
            List<Shot> doomedShots = new List<Shot>();
            List<Asteroid> doomedAsteroids = new List<Asteroid>();
            foreach (Shot shot in shots)
            {
                foreach (Asteroid asteroid in asteroids)
                {
                    if (checkCoordinatePolygonCollision(shot.Vel.Origin, asteroid))
                    {
                        doomedShots.Add(shot);
                        doomedAsteroids.Add(asteroid);
                    }
                }
            }
            destroyShots(doomedShots);
            destroyAsteroids(doomedAsteroids);
        }

        public static void CheckShotShipCollision(IEnumerable<Shot> shots, IEnumerable<Ship> ships, DestroyShots destroyShots, DestroyShips destroyShips) // 50 * 4 = 200 / 2082
        {
            List<Shot> doomedShots = new List<Shot>();
            List<Ship> doomedShips = new List<Ship>();
            foreach (Shot shot in shots)
            {
                foreach (Ship ship in ships)
                {
                    if (checkCoordinatePolygonCollision(shot.Vel.Origin, ship))
                    {
                        doomedShots.Add(shot);
                        doomedShips.Add(ship);
                    }
                }
            }
            destroyShots(doomedShots);
            destroyShips(doomedShips);
        }

        public static void CheckShotPlanetCollision(IEnumerable<Shot> shots, IEnumerable<Planet> planets, DestroyShots destroyShots) // 50 * 3 = 150 / 2082
        {
            List<Shot> doomedShots = new List<Shot>();
            foreach (Shot shot in shots)
            {
                foreach (Planet planet in planets)
                {
                    if (checkCoordinateCircleCollision(shot.Vel.Origin, planet))
                    {
                        doomedShots.Add(shot);
                    }
                }
            }
            destroyShots(doomedShots);
        }

        public static void CheckAsteroidShipCollision(IEnumerable<Asteroid> asteroids, IEnumerable<Ship> ships, DestroyAsteroids destroyAsteroids) // 30 * 4 = 120 / 2082
        {
            List<Asteroid> doomedAsteroids = new List<Asteroid>();
            List<Ship> doomedShips = new List<Ship>();
            foreach (Asteroid asteroid in asteroids)
            {
                foreach (Ship ship in ships)
                {
                    if (checkMultiplePolygonCollision(asteroid, ship))
                    {
                        doomedAsteroids.Add(asteroid);
                        doomedShips.Add(ship);
                    }
                }
            }
            destroyAsteroids(doomedAsteroids);
        }

        public static void CheckAsteroidPlanetCollision(IEnumerable<Asteroid> asteroids, IEnumerable<Planet> planets, DestroyAsteroids destroyAsteroids) // 30 * 3 = 90 / 2082
        {
            List<Asteroid> doomedAsteroids = new List<Asteroid>();
            foreach (Asteroid asteroid in asteroids)
            {
                foreach (Planet planet in planets)
                {
                    if (checkPolygonCircleCollision(asteroid, planet))
                    {
                        doomedAsteroids.Add(asteroid);
                    }
                }
            }
            destroyAsteroids(doomedAsteroids);
        }

        public static void CheckShipPlanetCollision(IEnumerable<Ship> ships, IEnumerable<Planet> planets, DestroyShips destroyShips) // 4 * 3 = 12 / 2082
        {
            List<Ship> doomedShips = new List<Ship>();
            foreach (Ship ship in ships)
            {
                foreach (Planet planet in planets)
                {
                    if (checkPolygonCircleCollision(ship, planet))
                    {
                        doomedShips.Add(ship);
                    }
                }
            }
            destroyShips(doomedShips);
        }

        [Obsolete("Method will always return false. This requires a system barycenter and a function to calculate escape velocity to be implemented.")]
        private static bool checkObjectEscaped(IMovable obj, int systemMass)
        {
            if (obj.Vel.Length > GetEscapeVelocity(obj, systemMass))
                return true;
            return false;
        }

        private static bool checkCircleCircleCollision(ICircular circRed, ICircular circBlue)
        {
            Vector distVec = VecCart(circRed.Vel.Origin, circBlue.Vel.Origin);
            return distVec.Length < circRed.Radius + circBlue.Radius;
        }

        private static bool checkCoordinateCircleCollision(Coordinate coord, ICircular circle)
        {
            Vector distVec = VecCart(coord, circle.Vel.Origin);
            return distVec.Length < circle.Radius;
        }

        [Obsolete("This method only checks the vertices of the polygon, not the edges.")]
        private static bool checkPolygonCircleCollision(IPolygon polygon, ICircular circle)
        {
            Vector[] arms = polygon.Arms.ToArray();
            for (int i = 0; i < arms.Count(); i++)
            {
                if (checkCoordinateCircleCollision(arms[i].Head, circle))
                    return true;
            }
            return false;
        }

        private static bool checkMultiplePolygonCollision(IPolygon polyRed, IPolygon polyBlue)
        {
            if (VecCart(polyRed.Vel.Origin, polyBlue.Vel.Origin).Length > polyRed.Radius + polyBlue.Radius)
                return false;

            // modified from solution: https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision
            
            IEnumerable<Vector> red = polyRed.ConstructSides();
            IEnumerable<Vector> blue = polyBlue.ConstructSides();

            foreach (var r in red)
            {
                foreach (var b in blue)
                {
                    double denominator = ((r.Origin.X - r.Head.X) * (b.Origin.Y - b.Head.Y)) - ((r.Origin.Y - r.Head.Y) * (b.Origin.X - b.Head.X));
                    double numerator1 = ((r.Head.Y - b.Head.Y) * (b.Origin.X - b.Head.X)) - ((r.Head.X - b.Head.X) * (b.Origin.Y - b.Head.Y));
                    double numerator2 = ((r.Head.Y - b.Head.Y) * (r.Origin.X - r.Head.X)) - ((r.Head.X - b.Head.X) * (r.Origin.Y - r.Head.Y));
                    
                    if (denominator == 0)
                        return numerator1 == 0 && numerator2 == 0;

                    double x = numerator1 / denominator;
                    double y = numerator2 / denominator;

                    if ((x >= 0 && x <= 1) && (y >= 0 && y <= 1))
                        return true;
                }
            }
            return true;
        }

        private static bool checkCoordinatePolygonCollision(Coordinate coord, IPolygon polygon)
        {
            if(VecCart(coord, polygon.Vel.Origin).Length > polygon.Radius)
                return false;

            Vector[] arms = polygon.Arms.ToArray();
            List<double> angles = new List<double>();
            double temp;
            double total = 0;
            for (int i = 0; i < arms.Count(); i++)
            {
                angles.Add(VecCart(coord, arms[i].Head).ForwardAngle);
            }
            temp = angles[angles.Count() - 1];
            for (int i = 0; i < angles.Count(); i++)
            {
                var diff = angles[i] - temp;
                if(Math.Abs(diff) > Math.PI)
                {
                    if (diff < 0)
                        diff -= 2 * Math.PI;
                    else
                        diff += 2 * Math.PI;
                }
                total += diff;
                temp = angles[i];
            }

            if (Math.Round(total) == 0)
                return false;
            return true;
        }
    }
}