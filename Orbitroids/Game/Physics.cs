using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Orbitroids.Game.Geometry;

namespace Orbitroids.Game
{
    public static class Physics
    {
        public static double GetEscapeVelocity(IMovable obj, int systemMass)
        {
            // V(esc) = 2 * G * M / r
            double length = VecCart(obj.Vel.Origin).Length;
            return length == 0 ? Double.PositiveInfinity : systemMass / length;
        }
    }
}