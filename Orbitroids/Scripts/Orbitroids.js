'use strict';

$(document).ready(function () {
    orbs.drawings = {
        renderPlanets: function (sunAngle, planets) {
            for (var idx in planets) {
                var planet = planets[idx];

                var sunVec = orbs.geometry.vecCirc(sunAngle + Math.PI, planet.Radius, planet.Vel.Origin);
                var grd = orbs.ctx.createLinearGradient(sunVec.head.x, sunVec.head.y, planet.Radius, planet.Vel.Origin.X, planet.Vel.Origin.Y, planet.Radius * 2);
                grd.addColorStop(0, planet.Color);
                grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                orbs.ctx.fillStyle = grd;
                //orbs.ctx.fillStyle = planet.Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(planet.Vel.Origin.x, planet.Vel.Origin.y, planet.Radius, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderAsteroids: function (sunAngle, asteroids) {
            for (var idx in asteroids) {
                var asteroid = asteroids[idx];

                var points = [];
                for (var a in asteroid.Arms) {
                    points.push(orbs.convertPoint(asteroid.Arms[a].Head));
                }

                var sunVec = orbs.geometry.vecCirc(sunAngle + Math.PI, asteroid.Radius, asteroid.Vel.Origin);
                var grd = orbs.ctx.createRadialGradient(sunVec.head.x, sunVec.head.y, asteroid.Radius, asteroid.Vel.Origin.X, asteroid.Vel.Origin.Y, asteroid.Radius * 2);
                grd.addColorStop(0, asteroid.Color);
                grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
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

                var center = orbs.convertPoint(shot.Vel.Origin);

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
                        points.push(orbs.convertPoint(ship.Arms[a].Head));
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
            this.x = x;
            this.y = y;
            this.convert = function () {
                return { x: orbs.view.center.x + (this.x / orbs.unit), y: orbs.view.center.y - (this.y / orbs.unit) };
            };
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
            this.origin;
            this.head;
            this.delta;
            this.len;

            this.extend = function (add, mult) {
                this.len += add;
                if (typeof mult === 'number') {
                    this.len *= mult;
                }
                this.delta.x *= mult;
                this.delta.y *= mult;
                this.head.x = this.origin.x + this.delta.x;
                this.head.y = this.origin.y + this.delta.y;
            };
            this.rotate = function (accelRot) {
                if (accelRot) { this.deltaRot += accelRot; }
                this.forwardAngle += this.deltaRot;
                this.refineForwardAngle();

                var newX = this.len * ((this.delta.x / this.len) * Math.cos(this.forwardAngle) - (this.delta.y / this.len) * Math.sin(this.forwardAngle));
                var newY = this.len * ((this.delta.x / this.len) * Math.sin(this.forwardAngle) + (this.delta.y / this.len) * Math.cos(this.forwardAngle));
                this.delta.x = newX;
                this.delta.y = newY;
                this.head.x = this.origin.x + this.delta.x;
                this.head.y = this.origin.y + this.delta.y;
            };
            this.addVector = function (vec) {
                this.delta.x += vec.delta.x;
                this.delta.y += vec.delta.y;
                this.head = new orbs.geometry.Point(this.origin.x + this.delta.x, this.origin.y + this.delta.y);
            };
        },

        vecCart: function (head, origin, deltaRot) {
            var vec = new orbs.geometry.Vector();

            if (origin) {
                vec.origin = new orbs.geometry.Point(origin.X, origin.Y);
            } else { vec.origin = new orbs.geometry.Point(0, 0); }

            if (head) { vec.head = head; }
            else { vec.head = new orbs.geometry.Point(vec.origin.x, vec.origin.y); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.delta = new orbs.geometry.Point(vec.head.x - vec.origin.x, vec.head.y - vec.origin.y);
            vec.len = Math.sqrt(Math.pow(vec.delta.x, 2) + Math.pow(vec.delta.y, 2));
            var unitDelta = new orbs.geometry.Point(vec.delta.x / vec.len, vec.delta.y / vec.len);
            vec.forwardAngle = Math.asin(unitDelta.x);
            if (unitDelta.y < 0) {
                vec.forwardAngle = Math.PI - vec.forwardAngle;
                vec.refineForwardAngle();
            }
            return vec;
        },
        vecDelta: function (delta, origin, deltaRot) {
            var vec = new orbs.geometry.Vector();

            if (delta) { vec.delta = delta; }
            else { vec.delta = new orbs.geometry.Point(0, 0); }

            if (origin) {
                vec.origin = new orbs.geometry.Point(origin.X, origin.Y);
            } else { vec.origin = new orbs.geometry.Point(0, 0); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.head = new orbs.geometry.Point(vec.origin.x + vec.delta.x, vec.origin.y + vec.delta.y);
            vec.len = Math.sqrt(Math.pow(vec.delta.x, 2) + Math.pow(vec.delta.y, 2));
            var unitDelta = new orbs.geometry.Point(vec.delta.x / vec.len, vec.delta.y / vec.len);
            vec.forwardAngle = Math.asin(unitDelta.x);
            if (unitDelta.y < 0) {
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

            if (origin) { vec.origin = new orbs.geometry.Point(origin.X, origin.Y); }
            else { vec.origin = new orbs.geometry.Point(0, 0); }

            if (deltaRot) { vec.deltaRot = deltaRot; }
            else { vec.deltaRot = 0; }

            vec.delta = new orbs.geometry.Point(vec.len * Math.sin(vec.forwardAngle), vec.len * Math.cos(vec.forwardAngle));
            vec.head = new orbs.geometry.Point(vec.origin.x + vec.delta.x, vec.origin.y + vec.delta.y);

            return vec;
        },
        addVectors: function (vec1, vec2) {
            var delta = {
                x: vec1.delta.x + vec2.delta.x,
                y: vec1.delta.y + vec2.delta.y
            };
            return orbs.geometry.vecDelta(delta, vec1.origin);
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
                var distVec = orbs.geometry.vecCart(planet.vel.origin, this.vel.origin);
                var force = planet.mass / (Math.pow(distVec.len, 2));
                var forceVec = orbs.geometry.vecCirc(distVec.forwardAngle, force, this.vel.origin);
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
                this.vel = orbs.geometry.vecDelta(this.vel.delta, this.vel.head, this.vel.deltaRot);
            };
        }
    };
    orbs.geometry.Vector.prototype = new orbs.geometry.Rotational(0, 0);
    orbs.geometry.Orbital.prototype = new orbs.geometry.Rotational(0, 0);

});