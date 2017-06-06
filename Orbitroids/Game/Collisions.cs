using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Orbitroids.Game.Engine;
using static Orbitroids.Game.Objects;

namespace Orbitroids.Game
{
    public static class Collisions
    {
        private static bool checkPolygonPlanetCollision(IEnumerable<Vector> polygon, Planet planet)
        {

        }
        private static bool checkMultiplePolygonCollision(IEnumerable<Vector> polygon1, IEnumerable<Vector> polygon2)
        {

        }
        private static bool checkCoordinatePolygonCollision(Coordinate coord, IEnumerable<Vector> polygon)
        {
            List<double> angles = new List<double>();
            double temp;
            double total = 0;
            for (int i = 0; i < polygon.Count(); i++)
            {
                angles.Add(VecCart(coord, ((Vector[])polygon)[i].Head).ForwardAngle);
            }
            temp = angles[angles.Count() - 1];
            for (int i = 0; i < angles.Count(); i++)
            {
                var diff = angles[i] - temp;
                if(Math.Abs(diff) > Math.PI)
                {
                    if (diff < 0)
                    {
                        diff -= 2 * Math.PI;
                    }
                    else
                    {
                        diff += 2 * Math.PI;
                    }
                }
                total += diff;
                temp = angles[i];
            }
            if (Math.Round(total) == 0)
            {
                return false;
            }
            return true;
        }
    }
}