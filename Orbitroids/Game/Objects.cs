using System;
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
            Color Color { get; set; }
        }
        
        public class Planet : Orbital, ICircular, IMassive, IColorable
        {
            public Planet(int mass, int radius, Vector vel = null, Color? color = null, bool atmosphere = false, double forwardAngle = 0, double deltaRot = 0)
            {
                this.Mass = mass;
                this.Radius = radius;
                this.Vel = vel ?? new Vector();
                this.Color = color ?? Color.FromArgb(1, 0, 0, 0);
                this.IsAtmosphere = atmosphere;
                this.ForwardAngle = forwardAngle;
                this.DeltaRot = deltaRot;
            }
            
            public int Mass { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }
            public bool IsAtmosphere { get; set; }
        }
        
        public class Ship : Orbital, IPolygon, IColorable
        {
            public Ship(Vector vel, Color? color = null, double forwardAngle = 0, double deltaRot = 0)
            {
                this.Vel = vel;
                this.Color = color ?? Color.FromArgb(1, 0, 0, 0);

                this.Burning = false;
                this.RotPower = .1;
                this.DampenBurn = false;
                this.Loaded = false;
                this.Destroyed = false;
                this.TrueAnomaly = new Vector();
                this.Accel = VecCirc();
                this.MaxRadius = 10;

                this.alignPoints();
            }

            new public double ForwardAngle
            {
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
            
            new public Vector Vel { get; set; }
            public double RotPower { get; set; }
            public double BurnPower { get; set; }
            public double DampenBurnPower { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }
            public bool Burning { get; set; }
            public bool DampenRot { get; set; }
            public bool DampenBurn { get; set; }
            public bool Loaded { get; set; }
            public bool Destroyed { get; set; }
            public int MaxRadius { get; set; }
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
                    VecCirc(this.ForwardAngle, this.MaxRadius, this.Vel.Origin),
                    VecCirc(this.ForwardAngle + 5 * Math.PI / 6, this.MaxRadius, this.Vel.Origin),
                    VecCirc(this.ForwardAngle - Math.PI, 0, this.Vel.Origin),
                    VecCirc(this.ForwardAngle - 5 * Math.PI / 6, this.MaxRadius, this.Vel.Origin)
                };
            }
            public void Rotate()
            {
                if ((this.AccelRot < 0 && this.DeltaRot > -.2) || 
                    (this.AccelRot > 0 && this.DeltaRot < .2))
                {
                    this.DeltaRot += this.AccelRot;
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
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle - Math.PI, .5));
                var projection = VecCirc(this.ForwardAngle, 2.5, this.Arms.ToArray()[0].Head);
                projection = AddVectors(projection, this.Vel);
                new Shot(projection);
            }
        }
        
        public class Shot : Orbital, IColorable
        {
            public Shot(Vector vel, Color? color = null)
            {
                this.Vel = vel;
                this.Color = color ?? Color.FromArgb(1, 0, 0, 0);
            }
            
            public Color Color { get; set; }
        }
        
        public class Asteroid : Orbital, IPolygon, IColorable
        {
            public Asteroid(Vector vel, int radius, double roughness, Color? color = null, double deltaRot = 0, double forwardAngle = 0)
            {
                this.Vel = vel;
                this.Radius = radius;
                this.Roughness = roughness;
                this.DeltaRot = deltaRot;
                this.ForwardAngle = forwardAngle;
                this.Color = color ?? Color.FromArgb(1, 0, 0, 0);

                var rand = new Random();
                var arms = new List<Vector>();
                for (int i = 0; i < 1 + Math.Sqrt(radius); i++)
                {
                    arms.Add(new Vector()
                    {
                        Length = radius - rand.Next() * radius * roughness
                    });
                }
                this.Arms = arms;
                this.AlignPoints();
            }

            public Color Color { get; set; }
            public int Radius { get; set; }
            public double Roughness { get; set; }
            public IEnumerable<Vector> Arms { get; set; }
            
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
                var arms = this.Arms.ToArray();
                for (int i = 0; i < this.Arms.Count(); i++)
                {
                    var angle = this.ForwardAngle + i * 2 * arms[i].Length == 0 ? 0 : Math.PI / arms[i].Length;
                    arms[i] = VecCirc(angle, arms[i].Length, this.Vel.Origin, this.DeltaRot);
                }
            }
        }
    }
}