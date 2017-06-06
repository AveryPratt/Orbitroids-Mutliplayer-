using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using static Orbitroids.Game.Engine;

namespace Orbitroids.Game
{
    public class Objects
    {
        public const double unit = 1;

        public class Planet : Orbital
        {
            public Planet(int mass, int radius, Vector vel = null, Color color = new Color(), bool atmosphere = false, double forwardAngle = 0, double deltaRot = 0)
            {
                this.Mass = mass;
                this.Radius = radius;
                this.Vel = vel ?? new Vector();
                this.Color = color;
                this.Atmosphere = atmosphere;
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }

            public int Mass { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }
            public bool Atmosphere { get; set; }
        }

        public class Ship : Orbital
        {
            public Ship(Vector vel, Color color = new Color(), double forwardAngle = 0, double deltaRot = 0)
            {
                this.Vel = vel;
                this.Color = color;

                this.Burning = false;
                this.DampenRot = false;
                this.DampenBurn = false;
                this.Loaded = false;
                this.Exploded = false;
                this.Destroyed = false;
                this.TrueAnomaly = new Vector();
                this.Accel = VecCirc();

                this.alignPoints();
            }

            public Color Color { get; set; }
            public bool Burning { get; set; }
            public bool DampenRot { get; set; }
            public bool DampenBurn { get; set; }
            public bool Loaded { get; set; }
            public bool Exploded { get; set; }
            public bool Destroyed { get; set; }
            public Coordinate Nose { get; set; }
            public Coordinate LeftSide { get; set; }
            public Coordinate RightSide { get; set; }
            public Coordinate Rear { get; set; }
            public Vector TrueAnomaly { get; set; }

            private void alignPoints()
            {
                this.Nose = VecCirc(this.ForwardAngle, 10 * unit, this.Vel.Origin).Head;
                this.LeftSide = VecCirc(this.ForwardAngle + 5 * Math.PI / 6, 10 * unit, this.Vel.Origin).Head;
                this.RightSide = VecCirc(this.ForwardAngle - 5 * Math.PI / 6, 10 * unit, this.Vel.Origin).Head;
                this.Rear = VecCirc(this.ForwardAngle - Math.PI, 10 * unit, this.Vel.Origin).Head;
            }
            new public void Rotate(double accelRot = 0)
            {
                if ((accelRot < 0 && this.DeltaRot > -.2) || 
                    (accelRot > 0 && this.DeltaRot < .2))
                {
                    this.DeltaRot += accelRot;
                }
                this.ForwardAngle += this.DeltaRot;
            }
            new public void ApplyMotion()
            {
                base.ApplyMotion();
                this.TrueAnomaly = VecCart(this.Vel.Origin, new Coordinate());
                this.alignPoints();
            }
            public void Burn(double force)
            {
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle, force));
            }
            public void Shoot()
            {
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle - Math.PI, .5 * unit));
                var projection = VecCirc(this.ForwardAngle, 2.5 * unit, this.Nose);
                projection = AddVectors(projection, this.Vel);
                new Shot(projection);
            }
        }

        public class Shot : Orbital
        {
            public Shot(Vector vel, Color color = new Color())
            {
                this.Vel = vel;
                this.Color = color;
            }

            public Color Color { get; set; }
        }

        public class Asteroid : Orbital
        {
            public Asteroid(Vector vel, int maxRadius, double roughness, double deltaRot = 0, double forwardAngle = 0)
            {
                this.Vel = vel;
                this.MaxRadius = maxRadius;
                this.Roughness = roughness;
                this.DeltaRot = deltaRot;
                this.ForwardAngle = forwardAngle;

                var rand = new Random();

                for (int i = 0; i < 1 + Math.Sqrt(maxRadius / unit); i++)
                {
                    this.Arms.Add(new Vector()
                    {
                        Length = maxRadius - rand.Next() * maxRadius * roughness
                    });
                }
                this.AlignPoints();
            }

            public int MaxRadius { get; set; }
            public double Roughness { get; set; }
            public List<Vector> Arms { get; set; }

            public void AlignPoints()
            {
                for (int i = 0; i < this.Arms.Count(); i++)
                {
                    var angle = this.ForwardAngle + i * 2 * Math.PI / this.Arms[i].Length;
                    this.Arms[i] = VecCirc(angle, this.Arms[i].Length, this.Vel.Origin, this.DeltaRot);
                }
            }
        }
    }
}