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
            // modified from solution: https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision

            bool collided = false;
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
                        collided = true;
                }
            }
            return collided;
        }

        private static bool checkCoordinatePolygonCollision(Coordinate coord, IPolygon polygon)
        {
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