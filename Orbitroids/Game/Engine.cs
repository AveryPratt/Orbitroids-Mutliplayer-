using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Orbitroids.Game
{
    public class Engine
    {
        public interface IMovable
        {
            Vector Vel { get; set; }
        }
        public interface IMassive : IMovable
        {
            int Mass { get; set; }
        }

        public interface ICircular : IMovable
        {
            int Radius { get; set; }
        }

        public interface IPolygon : ICircular
        {
            IEnumerable<Vector> Arms { get; set; }

            IEnumerable<Vector> ConstructSides();
        }

        public class Coordinate
        {
            public Coordinate(double x = 0, double y = 0)
            {
                this.X = x;
                this.Y = y;
            }

            public double X { get; set; }
            public double Y { get; set; }
        }

        public abstract class Rotational
        {
            public Rotational(double forwardAngle = 0, double deltaRot = 0)
            {
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }

            public double ForwardAngle {
                get
                {
                    return this.ForwardAngle;
                }
                set
                {
                    this.ForwardAngle = value;
                    this.RefineForwardAngle();
                }
            }
            public double AccelRot { get; set; }
            public double DeltaRot { get; set; }

            public void Rotate(double accelRot = 0)
            {
                this.DeltaRot += accelRot;
                this.ForwardAngle += DeltaRot;
            }
            protected internal void RefineForwardAngle()
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
                this.Delta = new Coordinate(0, 0);
                this.Head = new Coordinate(0, 0);
                this.Origin = new Coordinate(0, 0);
                this.ForwardAngle = 0;
                this.DeltaRot = 0;
                this.Length = 0;
            }

            public Coordinate Origin { get; set; }
            public Coordinate Head { get; set; }
            public Coordinate Delta { get; set; }
            public double Length { get; set; }
        }

        public static Vector VecCart(Coordinate head, Coordinate origin = null, double deltaRot = 0)
        {
            var vector = new Vector();
            vector.Head = head;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Delta = new Coordinate(head.X - origin.X, head.Y - origin.Y);
            vector.Length = Math.Sqrt(Math.Pow(vector.Delta.X, 2) + Math.Pow(vector.Delta.Y, 2));
            var unitDelta = new Coordinate(vector.Delta.X / vector.Length, vector.Delta.Y / vector.Length);
            vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);

            return vector;
        }
        public static Vector VecDelta(Coordinate delta, Coordinate origin = null, double deltaRot = 0)
        {
            var vector = new Vector();
            vector.Delta = delta;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Head = new Coordinate(origin.X + delta.X, origin.Y + delta.Y);
            vector.Length = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));
            var unitDelta = new Coordinate(delta.X / vector.Length, delta.Y / vector.Length);
            vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);

            return vector;
        }
        public static Vector VecCirc(double forwardAngle = 0, double length = 0, Coordinate origin = null, double deltaRot = 0)
        {
            var vector = new Vector();
            vector.ForwardAngle = forwardAngle;
            vector.Length = length;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Delta = new Coordinate(length * Math.Sin(forwardAngle), length * Math.Cos(forwardAngle));
            vector.Head = new Coordinate(origin.X + vector.Delta.X, origin.Y + vector.Delta.Y);

            return vector;
        }
        public static Vector AddVectors(Vector originalVec, Vector deltaVec)
        {
            var delta = new Coordinate(originalVec.Delta.X + deltaVec.Delta.X, originalVec.Delta.Y + deltaVec.Delta.Y);
            return VecDelta(delta, originalVec.Origin);
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

            public void ApplyGravity(IMassive massive)
            {
                Vector distVec = VecCart(massive.Vel.Origin, this.Vel.Origin);
                var force = massive.Mass / Math.Pow(distVec.Length, 2);
                var forceVec = VecCirc(distVec.ForwardAngle, force, this.Vel.Origin);
                this.Accel = AddVectors(this.Accel, forceVec);
            }
            public void ApplyAccel(Vector accel)
            {
                this.Accel = AddVectors(this.Accel, accel);
            }
            public void ResetAccel()
            {
                this.Accel = new Vector();
            }
            public void ApplyMotion()
            {
                // converts acceleration to velocity
                this.Vel = AddVectors(this.Vel, this.Accel);
                // converts velocity to distance
                this.Vel = VecDelta(this.Vel.Delta, this.Vel.Head, this.Vel.DeltaRot);
            }
        }
    }
}