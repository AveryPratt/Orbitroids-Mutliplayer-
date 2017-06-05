using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orbitroids.Game
{
    public class Engine
    {
        public class Point
        {
            public Point(double x = 0, double y = 0)
            {
                this.X = x;
                this.Y = y;
            }

            public double X { get; set; }
            public double Y { get; set; }
        }

        public class Rotational
        {
            public Rotational(double forwardAngle = 0, double deltaRot = 0)
            {
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }

            public double ForwardAngle { get; set; }
            public double DeltaRot { get; set; }

            public void Rotate(double accelRot = 0)
            {
                this.DeltaRot += accelRot;
                this.ForwardAngle += DeltaRot;
                this.RefineForwardAngle();
            }
            protected void RefineForwardAngle()
            {
                while (this.ForwardAngle >= Math.PI * 2)
                {
                    this.ForwardAngle -= Math.PI * 2;
                }
                while (this.ForwardAngle < 0)
                {
                    this.ForwardAngle += Math.PI * 2;
                }
            }
        }

        public class Vector : Rotational
        {
            public Vector()
            {
                this.Delta = new Point(0, 0);
                this.Head = new Point(0, 0);
                this.Origin = new Point(0, 0);
                this.ForwardAngle = 0;
                this.DeltaRot = 0;
                this.Length = 0;
            }

            public Point Origin { get; set; }
            public Point Head { get; set; }
            public Point Delta { get; set; }
            public double Length { get; set; }
            
            public static Vector VecCart(Point head, Point origin = null, double deltaRot = 0)
            {
                var vector = new Vector();
                vector.Head = head;
                vector.Origin = origin ?? new Point(0, 0);
                vector.DeltaRot = deltaRot;

                vector.Delta = new Point(head.X - origin.X, head.Y - origin.Y);
                vector.Length = Math.Sqrt(Math.Pow(vector.Delta.X, 2) + Math.Pow(vector.Delta.Y, 2));
                var unitDelta = new Point(vector.Delta.X / vector.Length, vector.Delta.Y / vector.Length);
                vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);
                vector.RefineForwardAngle();

                return vector;
            }
            public static Vector VecDelta(Point delta, Point origin = null, double deltaRot = 0)
            {
                var vector = new Vector();
                vector.Delta = delta;
                vector.Origin = origin ?? new Point(0, 0);
                vector.DeltaRot = deltaRot;

                vector.Head = new Point(origin.X + delta.X, origin.Y + delta.Y);
                vector.Length = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));
                var unitDelta = new Point(delta.X / vector.Length, delta.Y / vector.Length);
                vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);
                vector.RefineForwardAngle();

                return vector;
            }
            public static Vector VecCirc(double forwardAngle, double length, Point origin = null, double deltaRot = 0)
            {
                var vector = new Vector();
                vector.ForwardAngle = forwardAngle;
                vector.Length = length;
                vector.Origin = origin ?? new Point(0, 0);
                vector.DeltaRot = deltaRot;

                vector.Delta = new Point(length * Math.Sin(forwardAngle), length * Math.Cos(forwardAngle));
                vector.Head = new Point(origin.X + vector.Delta.X, origin.Y + vector.Delta.Y);

                return vector;
            }
            public static Vector AddVectors(Vector originalVec, Vector deltaVec)
            {
                var delta = new Point(originalVec.Delta.X + deltaVec.Delta.X, originalVec.Delta.Y + deltaVec.Delta.Y);
                return VecDelta(delta, originalVec.Origin);
            }
        }
        
        public class Orbital : Rotational
        {
            public Orbital(Vector vel = null, Vector accel = null, double forwardAngle = 0, double deltaRot = 0)
            {
                this.Vel = vel ?? new Vector();
                this.Accel = accel ?? new Vector();
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }

            public Vector Vel { get; set; }
            public Vector Accel { get; set; }

            //public void ApplyGravity(Planet planet)
            //{

            //}
            public void applyAccel(Vector accel)
            {
                this.Accel = Vector.AddVectors(this.Accel, accel);
            }
            public void resetAccel()
            {
                this.Accel = new Vector();
            }
            public void applyMotion()
            {
                // converts acceleration to velocity
                this.Vel = Vector.AddVectors(this.Vel, this.Accel);
                // converts velocity to distance
                this.Vel = Vector.VecDelta(this.Vel.Delta, this.Vel.Head, this.Vel.DeltaRot);
            }
        }
    }
}