using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using static Orbitroids.Game.Objects;

namespace Orbitroids.Game
{
    public class Geometry
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
            double Radius { get; set; }
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
            
            protected double forwardAngle;
            public double ForwardAngle {
                get
                {
                    return this.forwardAngle;
                }
                set
                {
                    while (value >= Math.PI * 2)
                        value -= Math.PI * 2;
                    while (value < 0)
                        value += Math.PI * 2;
                    this.forwardAngle = value;
                }
            }
            public double AccelRot { get; set; }
            public double DeltaRot { get; set; }
            public string IsRotating { get; set; }
            public double RotPower { get; set; }

            protected void Rotate(double timespan)
            {
                if (this.IsRotating == "right")
                    this.AccelRot -= this.RotPower;
                else if (this.IsRotating == "left")
                    this.AccelRot += this.RotPower;

                this.ForwardAngle += DeltaRot * timespan / 1000;
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

            public static Vector Extend(Vector vec, double mult)
            {
                return VecDelta(
                    new Coordinate(vec.Delta.X * mult, vec.Delta.Y * mult),
                    new Coordinate(vec.Origin.X, vec.Origin.Y),
                    vec.DeltaRot
                    );
            }
        }

        public static Vector VecCart(Coordinate head, Coordinate origin = null, double deltaRot = 0)
        {
            Vector vector = new Vector();
            vector.Head = head;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Delta = new Coordinate(head.X - vector.Origin.X, head.Y - vector.Origin.Y);
            vector.Length = Math.Sqrt(Math.Pow(vector.Delta.X, 2) + Math.Pow(vector.Delta.Y, 2));
            Coordinate unitDelta = new Coordinate(
                vector.Length == 0 ? 0 : vector.Delta.X / vector.Length,
                vector.Length == 0 ? 0 : vector.Delta.Y / vector.Length
                );
            vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);

            return vector;
        }
        public static Vector VecDelta(Coordinate delta, Coordinate origin = null, double deltaRot = 0)
        {
            Vector vector = new Vector();
            vector.Delta = delta;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Head = new Coordinate(origin.X + delta.X, origin.Y + delta.Y);
            vector.Length = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));
            Coordinate unitDelta = new Coordinate(
                vector.Length == 0 ? 0 : delta.X / vector.Length,
                vector.Length == 0 ? 0 : delta.Y / vector.Length
                );
            vector.ForwardAngle = unitDelta.Y < 0 ? Math.PI - Math.Asin(unitDelta.X) : Math.Asin(unitDelta.X);

            return vector;
        }
        public static Vector VecCirc(double forwardAngle = 0, double length = 0, Coordinate origin = null, double deltaRot = 0)
        {
            Vector vector = new Vector();
            vector.ForwardAngle = forwardAngle;
            vector.Length = length;
            vector.Origin = origin ?? new Coordinate(0, 0);
            vector.DeltaRot = deltaRot;

            vector.Delta = new Coordinate(length * Math.Sin(forwardAngle), length * Math.Cos(forwardAngle));
            vector.Head = new Coordinate(vector.Origin.X + vector.Delta.X, vector.Origin.Y + vector.Delta.Y);

            return vector;
        }
        public static Vector AddVectors(Vector originalVec, Vector deltaVec)
        {
            Coordinate delta = new Coordinate(originalVec.Delta.X + deltaVec.Delta.X, originalVec.Delta.Y + deltaVec.Delta.Y);
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
                double distanceSquared = Math.Pow(distVec.Length, 2);
                double force = distanceSquared == 0 ? 0 : massive.Mass / distanceSquared;
                Vector forceVec = VecCirc(distVec.ForwardAngle, force, this.Vel.Origin);
                this.Accel = AddVectors(this.Accel, forceVec);
            }
            public void ApplyAccel(Vector accel)
            {
                this.Accel = AddVectors(this.Accel, accel);
            }
            public void ApplyMotion(double timespan)
            {
                this.Rotate(timespan);
                this.Vel = AddVectors(this.Vel, Vector.Extend(this.Accel, timespan));
                Vector ext = Vector.Extend(this.Vel, timespan / 1000);
                this.Vel = VecDelta(this.Vel.Delta, ext.Head, this.Vel.DeltaRot);
                this.Accel = new Vector();
                this.AccelRot = 0;
            }
        }
    }
}