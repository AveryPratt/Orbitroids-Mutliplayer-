using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Orbitroids.Game.Geometry;
using static Orbitroids.Game.Objects;

namespace Orbitroids.Game
{
    public static class Physics
    {
        public static double GetEscapeVelocity(Coordinate coord, IMassive body)
        {
            // V(esc) = 2 * G * M / r
            double dist = VecCart(coord).Length;
            return dist == 0 ? 0 : body.Mass / dist;
        }

        public static double GetOrbitalVelocity(Coordinate coord, IMassive body)
        {
            double dist = VecCart(coord, body.Vel.Origin).Length;
            return dist == 0 ? 0 : Math.Sqrt(body.Mass / dist);
        }
    }
}