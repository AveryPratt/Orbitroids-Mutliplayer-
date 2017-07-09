'use strict';

$(document).ready(function () {
    orbs.drawings = {
        convertPoint: function (point) {
            return {
                x: point.X + orbs.canvas.width / 2,
                y: orbs.canvas.height / 2 - point.Y
            };
        },
        amplifyColor: function(color, mult){
            if (color[0] == '#') {
                var r = Math.round(parseInt(color.slice(1, 3), 16) * mult);
                var g = Math.round(parseInt(color.slice(3, 5), 16) * mult);
                var b = Math.round(parseInt(color.slice(5, 7), 16) * mult);
                var rHex = r.toString(16);
                rHex = rHex.length === 1 ? '0' + rHex : rHex;
                var gHex = g.toString(16);
                gHex = gHex.length === 1 ? '0' + gHex : gHex;
                var bHex = b.toString(16);
                bHex = bHex.length === 1 ? '0' + bHex : bHex;
                return '#' + rHex + gHex + bHex;
            }
            return color;
        },
        renderPlanets: function (sunAngle, planets) {
            for (var idx in planets) {
                var planet = planets[idx];
                var center = orbs.drawings.convertPoint(planet.Vel.Origin);

                var sunVec = orbs.geometry.vecCirc(sunAngle + Math.PI, planet.Radius, planet.Vel.Origin);
                var sunVecHead = orbs.drawings.convertPoint(sunVec.Head);
                var grd = orbs.ctx.createLinearGradient(sunVecHead.x, sunVecHead.y, center.x, center.y);
                grd.addColorStop(0, planet.Color);
                grd.addColorStop(1, orbs.drawings.amplifyColor(planet.Color, .25));
                orbs.ctx.fillStyle = grd;
                //orbs.ctx.fillStyle = planet.Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, planet.Radius, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();
                orbs.ctx.fill();
            }
        },
        renderAsteroids: function (sunAngle, asteroids) {
            for (var idx in asteroids) {
                var asteroid = asteroids[idx];

                var points = [];
                for (var a in asteroid.Arms) {
                    points.push(orbs.drawings.convertPoint(asteroid.Arms[a].Head));
                }

                var center = orbs.drawings.convertPoint(asteroid.Vel.Origin);
                var sunVec = orbs.geometry.vecCirc(sunAngle, asteroid.Radius, asteroid.Vel.Origin);
                var sunVecHead = orbs.drawings.convertPoint(sunVec.Head);
                var grd = orbs.ctx.createRadialGradient(sunVecHead.x, sunVecHead.y, asteroid.Radius, center.x, center.y, asteroid.Radius * 2);
                grd.addColorStop(0, asteroid.Color);
                grd.addColorStop(1, '#FFFFFF');
                orbs.ctx.fillStyle = grd;
                //orbs.ctx.fillStyle = asteroid.Color;

                orbs.ctx.beginPath();
                orbs.ctx.moveTo(points[points.length - 1].x, points[points.length - 1].y);
                for (var p in points) {
                    orbs.ctx.lineTo(points[p].x, points[p].y);
                }
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShots: function (sunAngle, shots) {
            for (var idx in shots) {
                var shot = shots[idx];

                var center = orbs.drawings.convertPoint(shot.Vel.Origin);

                orbs.ctx.fillStyle = shot.Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, 1.5, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShips: function (sunAngle, ships) {
            for (var idx in ships) {
                var ship = ships[idx];

                if (!ship.Destroyed) {
                    var points = [];
                    for (var a in ship.Arms) {
                        points.push(orbs.drawings.convertPoint(ship.Arms[a].Head));
                    }

                    orbs.ctx.strokeStyle = ship.Color;
                    orbs.ctx.lineWidth = 1;

                    orbs.ctx.beginPath();
                    orbs.ctx.moveTo(points[points.length - 1].x, points[points.length - 1].y);
                    for (var p in points) {
                        orbs.ctx.lineTo(points[p].x, points[p].y);
                    }
                    orbs.ctx.closePath();
                    orbs.ctx.stroke();
                }
            }
        }
    };

    orbs.geometry = {
        Point: function (x, y) {
            this.X = x;
            this.Y = y;
        },
        Rotational: function (forwardAngle, deltaRot) {
            this.forwardAngle = forwardAngle;
            this.deltaRot = deltaRot;

            this.rotate = function (accelRot) {
                if (accelRot) { this.deltaRot += accelRot; }
                this.forwardAngle += this.deltaRot;
                this.refineForwardAngle();
            };
            this.refineForwardAngle = function () {
                while (this.forwardAngle >= Math.PI * 2) {
                    this.forwardAngle -= Math.PI * 2;
                }
                while (this.forwardAngle < 0) {
                    this.forwardAngle += Math.PI * 2;
                }
            };
        },
        Vector: function () {
            this.Origin;
            this.Head;
            this.Delta;
            this.len;

            this.extend = function (add, mult) {
                this.len += add;
                if (typeof mult === 'number') {
                    this.len *= mult;
                }
                this.Delta.X *= mult;
                this.Delta.Y *= mult;
                this.Head.X = this.Origin.X + this.Delta.X;
                this.Head.Y = this.Origin.Y + this.Delta.Y;
            };
            this.rotate = function (accelRot) {
                if (accelRot) { this.deltaRot += accelRot; }
                this.forwardAngle += this.deltaRot;
                this.refineForwardAngle();

                var newX = this.len * ((this.Delta.X / this.len) * Math.cos(this.forwardAngle) - (this.Delta.Y / this.len) * Math.sin(this.forwardAngle));
                var newY = this.len * ((this.Delta.X / this.len) * Math.sin(this.forwardAngle) + (this.Delta.Y / this.len) * Math.cos(this.forwardAngle));
                this.Delta.X = newX;
                this.Delta.Y = newY;
                this.Head.X = this.Origin.X + this.Delta.X;
                this.Head.Y = this.Origin.Y + this.Delta.Y;
            };
            this.addVector = function (vec) {
                this.Delta.X += vec.Delta.X;
                this.Delta.Y += vec.Delta.Y;
                this.Head = new orbs.geometry.Point(this.Origin.X + this.Delta.X, this.Origin.Y + this.Delta.Y);
            };
        },

        vecCart: function (head, origin, deltaRot) {
            var vec = new orbs.geometry.Vector();

            if (origin) {
                vec.Origin = new orbs.geometry.Point(origin.X, origin.Y);
            } else { vec.Origin = new orbs.geometry.Point(0, 0); }

            if (head) { vec.Head = head; }
            else { vec.Head = new orbs.geometry.Point(vec.Origin.X, vec.Origin.Y); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.Delta = new orbs.geometry.Point(vec.Head.X - vec.Origin.X, vec.Head.Y - vec.Origin.Y);
            vec.len = Math.sqrt(Math.pow(vec.Delta.X, 2) + Math.pow(vec.Delta.Y, 2));
            var unitDelta = new orbs.geometry.Point(vec.Delta.X / vec.len, vec.Delta.Y / vec.len);
            vec.forwardAngle = Math.asin(unitDelta.X);
            if (unitDelta.Y < 0) {
                vec.forwardAngle = Math.PI - vec.forwardAngle;
                vec.refineForwardAngle();
            }
            return vec;
        },
        vecDelta: function (delta, origin, deltaRot) {
            var vec = new orbs.geometry.Vector();

            if (delta) { vec.Delta = delta; }
            else { vec.Delta = new orbs.geometry.Point(0, 0); }

            if (origin) {
                vec.Origin = new orbs.geometry.Point(origin.X, origin.Y);
            } else { vec.Origin = new orbs.geometry.Point(0, 0); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.Head = new orbs.geometry.Point(vec.Origin.X + vec.Delta.X, vec.Origin.Y + vec.deltaDeltaY);
            vec.len = Math.sqrt(Math.pow(vec.Delta.X, 2) + Math.pow(vec.Delta.Y, 2));
            var unitDelta = new orbs.geometry.Point(vec.Delta.X / vec.len, vec.Delta.Y / vec.len);
            vec.forwardAngle = Math.asin(unitDelta.X);
            if (unitDelta.Y < 0) {
                vec.forwardAngle = Math.PI - vec.forwardAngle;
                vec.refineForwardAngle();
            }
            return vec;
        },
        vecCirc: function (forwardAngle, len, origin, deltaRot) {
            var vec = new orbs.geometry.Vector();

            if (forwardAngle) { vec.forwardAngle = forwardAngle; }
            else { vec.forwardAngle = 0; }

            if (len) { vec.len = len; }
            else { vec.len = 0; }

            if (origin) { vec.Origin = new orbs.geometry.Point(origin.X, origin.Y); }
            else { vec.Origin = new orbs.geometry.Point(0, 0); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.Delta = new orbs.geometry.Point(vec.len * Math.sin(vec.forwardAngle), vec.len * Math.cos(vec.forwardAngle));
            vec.Head = new orbs.geometry.Point(vec.Origin.X + vec.Delta.X, vec.Origin.Y + vec.Delta.Y);

            return vec;
        },
        addVectors: function (vec1, vec2) {
            var delta = {
                X: vec1.Delta.X + vec2.Delta.X,
                Y: vec1.Delta.Y + vec2.Delta.Y
            };
            return orbs.geometry.vecDelta(delta, vec1.Origin);
        },

        Orbital: function (vel, accel, forwardAngle, deltaRot) {
            if (forwardAngle) { this.forwardAngle = forwardAngle; }
            else { this.forwardAngle = 0; }

            if (deltaRot) { this.deltaRot = deltaRot; }
            else { this.deltaRot = 0; }

            if (vel) { this.vel = vel; }
            else { this.vel = orbs.geometry.vecCirc(); }

            if (accel) { this.accel = accel; }
            else { this.accel = orbs.geometry.vecCirc(); }

            this.applyGravity = function (planet) {
                var distVec = orbs.geometry.vecCart(planet.vel.Origin, this.vel.Origin);
                var force = planet.mass / (Math.pow(distVec.len, 2));
                var forceVec = orbs.geometry.vecCirc(distVec.forwardAngle, force, this.vel.Origin);
                this.accel.addVector(forceVec);
            };
            this.applyAccel = function (accel) {
                this.accel.addVector(accel);
            };
            this.resetAccel = function () {
                this.accel = orbs.geometry.vecCirc();
            };
            this.applyMotion = function () {
                this.vel.addVector(this.accel);
                this.vel = orbs.geometry.vecDelta(this.vel.delta, this.vel.Head, this.vel.deltaRot);
            };
        }
    };
    orbs.geometry.Vector.prototype = new orbs.geometry.Rotational(0, 0);
    orbs.geometry.Orbital.prototype = new orbs.geometry.Rotational(0, 0);

});