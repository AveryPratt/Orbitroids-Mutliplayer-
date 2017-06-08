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

        public class Planet : Orbital, ICircular, IMassive
        {
            public Planet(int mass, int radius, Vector vel = null, Color color = new Color(), bool atmosphere = false, double forwardAngle = 0, double deltaRot = 0)
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
            public int Radius { get; set; }
            public Color Color { get; set; }
            public bool IsAtmosphere { get; set; }
        }

        public class Ship : Orbital, IPolygon
        {
            public Ship(Vector vel, Color color = new Color(), double forwardAngle = 0, double deltaRot = 0)
            {
                this.Vel = vel;
                this.Color = color;

                this.Burning = false;
                this.RotPower = .1;
                this.DampenBurn = false;
                this.Loaded = false;
                this.Exploded = false;
                this.Destroyed = false;
                this.TrueAnomaly = new Vector();
                this.Accel = VecCirc();

                this.alignPoints();
            }

            public double RotPower { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }
            public bool Burning { get; set; }
            public bool DampenRot { get; set; }
            public bool DampenBurn { get; set; }
            public bool Loaded { get; set; }
            public bool Exploded { get; set; }
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
                Vector[] arms = this.Arms.ToArray();
                arms[0] = VecCirc(this.ForwardAngle, this.MaxRadius, this.Vel.Origin); // nose
                arms[1] = VecCirc(this.ForwardAngle + 5 * Math.PI / 6, this.MaxRadius, this.Vel.Origin); // left
                arms[2] = VecCirc(this.ForwardAngle - Math.PI, this.MaxRadius, this.Vel.Origin); // rear
                arms[3] = VecCirc(this.ForwardAngle - 5 * Math.PI / 6, this.MaxRadius, this.Vel.Origin); // right
            }
            new public void Rotate()
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
                this.Accel = AddVectors(this.Accel, VecCirc(this.ForwardAngle - Math.PI, .5 * unit));
                var projection = VecCirc(this.ForwardAngle, 2.5 * unit, this.Arms.ToArray()[0].Head);
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

        public class Asteroid : Orbital, IPolygon
        {
            public Asteroid(Vector vel, int radius, double roughness, double deltaRot = 0, double forwardAngle = 0)
            {
                this.Vel = vel;
                this.Radius = radius;
                this.Roughness = roughness;
                this.DeltaRot = deltaRot;
                this.ForwardAngle = forwardAngle;

                var rand = new Random();
                var arms = new List<Vector>();
                for (int i = 0; i < 1 + Math.Sqrt(radius / unit); i++)
                {
                    arms.Add(new Vector()
                    {
                        Length = radius - rand.Next() * radius * roughness
                    });
                }
                this.Arms = arms;
                this.AlignPoints();
            }

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
                    var angle = this.ForwardAngle + i * 2 * Math.PI / arms[i].Length;
                    arms[i] = VecCirc(angle, arms[i].Length, this.Vel.Origin, this.DeltaRot);
                }
            }
        }
    }
}