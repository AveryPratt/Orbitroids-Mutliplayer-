﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using static Orbitroids.Game.Geometry;

namespace Orbitroids.Game
{
    public class Objects
    {
        interface IColorable
        {
            string Color { get; set; }
        }
        
        public class Planet : Orbital, ICircular, IMassive, IColorable
        {
            public Planet(int mass, int radius, Vector vel = null, string color = "#ffffff", bool atmosphere = false, double forwardAngle = 0, double deltaRot = 0)
            {
                this.Mass = mass;
                this.Radius = radius;
                this.Vel = vel ?? new Vector();
                this.Color = color;
                this.IsAtmosphere = atmosphere;
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }
            
            public int Mass { get; set; }
            public double Radius { get; set; }
            private Color color;
            public string Color
            {
                get { return String.Format("#{0}{1}{2}", this.color.R.ToString("X2"), this.color.G.ToString("X2"), this.color.B.ToString("X2")); }
                set { this.color = ColorTranslator.FromHtml(value); }
            }
            public bool IsAtmosphere { get; set; }
        }
        
        public class Ship : Orbital, IPolygon, IColorable
        {
            public Ship(Vector vel, string color = "#ffffff", double forwardAngle = 0, double deltaRot = 0)
            {
                this.Vel = vel;
                this.Color = color;
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;

                this.Burning = false;
                this.ShotPower = .15;
                this.RotPower = .0005;
                this.BurnPower = .01;
                this.DampenBurnPower = .00333;
                this.DampenBurn = false;
                this.Loaded = false;
                this.Destroyed = false;
                this.TrueAnomaly = new Vector();
                this.Accel = VecCirc();
                this.Radius = 10;

                this.alignPoints();
            }

            public double ShotPower { get; set; }
            public double BurnPower { get; set; }
            public double DampenBurnPower { get; set; }
            public double Radius { get; set; }
            private Color color;
            public string Color
            {
                get { return String.Format("#{0}{1}{2}", this.color.R.ToString("X2"), this.color.G.ToString("X2"), this.color.B.ToString("X2")); }
                set { this.color = ColorTranslator.FromHtml(value); }
            }
            public bool Burning { get; set; }
            public bool DampenRot { get; set; }
            public bool DampenBurn { get; set; }
            public bool Loaded { get; set; }
            public bool Destroyed { get; set; }
            public IEnumerable<Vector> Arms { get; set; }
            public Vector TrueAnomaly { get; set; }

            public IEnumerable<Vector> ConstructSides()
            {
                Vector[] arms = this.Arms.ToArray();
                List<Vector> sides = new List<Vector>();
                sides.Add(VecCart(arms[arms.Count() - 1].Head, arms[0].Head));
                for (int i = 0; i < arms.Count() - 1; i++)
                {
                    sides.Add(VecCart(arms[i].Head, arms[i + 1].Head));
                }
                return sides;
            }
            private void alignPoints()
            {
                this.Arms = new Vector[4]
                {
                    VecCirc(this.ForwardAngle, this.Radius, this.Vel.Origin),
                    VecCirc(this.ForwardAngle + 5 * Math.PI / 6, this.Radius, this.Vel.Origin),
                    VecCirc(this.ForwardAngle - Math.PI, 0, this.Vel.Origin),
                    VecCirc(this.ForwardAngle - 5 * Math.PI / 6, this.Radius, this.Vel.Origin)
                };
            }
            new protected void Rotate(double timespan)
            {
                double maxRotPower = this.RotPower;

                if (this.DampenRot)
                {
                    // slows rotation to 0
                    if (this.DeltaRot < 0)
                    {
                        this.AccelRot = this.RotPower / 2;
                        this.DeltaRot += this.AccelRot;
                        if (this.DeltaRot > 0)
                        {
                            this.DeltaRot = 0;
                            this.AccelRot = 0;
                        }
                    }
                    else if (this.DeltaRot > 0)
                    {
                        this.AccelRot = -this.RotPower / 2;
                        this.DeltaRot += this.AccelRot;
                        if (this.DeltaRot < 0)
                        {
                            this.DeltaRot = 0;
                            this.AccelRot = 0;
                        }
                    }
                }
                else
                    maxRotPower = .01;

                if (this.IsRotating == "right")
                    this.AccelRot = -this.RotPower;
                else if (this.IsRotating == "left")
                    this.AccelRot = this.RotPower;
                if ((this.AccelRot < 0 && this.DeltaRot > -maxRotPower) || 
                    (this.AccelRot > 0 && this.DeltaRot < maxRotPower))
                {
                    this.DeltaRot += this.AccelRot;
                }

                this.ForwardAngle += this.DeltaRot * timespan;
            }
            new public void ApplyMotion(double timespan)
            {
                this.Rotate(timespan);
                this.Vel = AddVectors(this.Vel, this.Accel);
                this.Vel.Extend(timespan);
                this.Vel = VecDelta(this.Vel.Delta, this.Vel.Head, this.Vel.DeltaRot);
                this.Vel.Extend(1 / timespan);
                this.Accel = new Vector();
                this.AccelRot = 0;
                this.TrueAnomaly = VecCart(this.Vel.Origin, new Coordinate());
                this.alignPoints();
            }
            public void Burn(double force)
            {
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle, force));
            }
            public Shot Shoot(double timespan)
            {
                this.Loaded = false;
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle - Math.PI, this.ShotPower / 4));
                Vector projection = VecCirc(this.ForwardAngle, this.ShotPower, this.Arms.ToArray()[0].Head);
                projection = AddVectors(projection, this.Vel);
                return new Shot(projection);
            }
        }
        
        public class Shot : Orbital, IColorable
        {
            public Shot(Vector vel, string color = "#ffffff")
            {
                this.Vel = vel;
                this.Color = color;
            }

            private Color color;
            public string Color
            {
                get { return String.Format("#{0}{1}{2}", this.color.R.ToString("X2"), this.color.G.ToString("X2"), this.color.B.ToString("X2")); }
                set { this.color = ColorTranslator.FromHtml(value); }
            }
        }
        
        public class Asteroid : Orbital, IPolygon, IColorable
        {
            public Asteroid(Random rand, Vector vel, double radius = 50, double roughness = .5, string color = "#808080", double deltaRot = 0, double forwardAngle = 0)
            {
                this.Vel = vel;
                this.Radius = radius;
                this.Roughness = roughness;
                this.DeltaRot = deltaRot;
                this.ForwardAngle = forwardAngle;
                this.Color = color;
                
                List<Vector> arms = new List<Vector>();
                double armNum = 1 + Math.Sqrt(radius);
                for (int i = 0; i < armNum; i++)
                {
                    double angle = forwardAngle + i * 2 * Math.PI / (armNum + 1);
                    double length = radius - rand.NextDouble() * radius * roughness;
                    arms.Add(VecCirc(angle, length, vel.Origin, deltaRot));
                }
                this.Arms = arms;
                this.AlignPoints();
            }

            private Color color;
            public string Color
            {
                get { return String.Format("#{0}{1}{2}", this.color.R.ToString("X2"), this.color.G.ToString("X2"), this.color.B.ToString("X2")); }
                set { this.color = ColorTranslator.FromHtml(value); }
            }
            public double Radius { get; set; }
            public double Roughness { get; set; }
            public IEnumerable<Vector> Arms { get; set; }
            
            new public void ApplyMotion(double timespan)
            {
                base.ApplyMotion(timespan);
                this.AlignPoints();
            }
            public IEnumerable<Vector> ConstructSides()
            {
                Vector[] arms = this.Arms.ToArray();
                List<Vector> sides = new List<Vector>();
                sides.Add(VecCart(arms[arms.Count() - 1].Head, arms[0].Head));
                for (int i = 0; i < arms.Count() - 1; i++)
                {
                    sides.Add(VecCart(arms[i].Head, arms[i + 1].Head));
                }
                return sides;
            }
            public void AlignPoints()
            {
                Vector[] arms = this.Arms.ToArray();
                for (int i = 0; i < this.Arms.Count(); i++)
                {
                    int armNum = arms.Count();
                    double angle = this.ForwardAngle + i * 2 * (armNum == 0 ? 0 : Math.PI / armNum);
                    arms[i] = VecCirc(angle, arms[i].Length, this.Vel.Origin, this.DeltaRot);
                }
                this.Arms = arms;
            }
        }
    }
}