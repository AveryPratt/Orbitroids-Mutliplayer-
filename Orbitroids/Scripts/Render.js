'use strict';

$(document).ready(function () {
    orbs.drawings = {
        renderPlanets: function (dt) {
            for (var idx in orbs.model.Planets) {
                var otherplanets = [];
                for (var i = 0; i < orbs.model.Planets.length; i++) {
                    if (i != idx) {
                        otherplanets.push(orbs.model.Planets[i]);
                    }
                }
                orbs.model.Planets[idx] = orbs.animate.updateOrbital(orbs.model.Planets[idx], otherplanets, dt);

                var center = orbs.convertPoint(orbs.model.Planets[idx].Vel.Origin);

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, planet.Radius, center);
                //var grd = ctx.createLinearGradient(sunVec.head.x, sunVec.head.y, planet.Radius, center.x, center.y, planet.Radius * 2);
                //grd.addColorStop(0, planet.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                orbs.ctx.fillStyle = orbs.model.Planets[idx].Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, orbs.model.Planets[idx].Radius * orbs.unit, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderAsteroids: function (dt) {
            for (var idx in orbs.model.Asteroids) {
                orbs.model.Asteroids[idx] = orbs.animate.updatePolygon(orbs.model.Asteroids[idx], orbs.model.Planets, dt);

                var points = [];
                for (var a in orbs.model.Asteroids[idx].Arms) {
                    points.push(orbs.convertPoint(orbs.model.Asteroids[idx].Arms[a].Head));
                }

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, asteroid.Radius, center);
                //var grd = ctx.createRadialGradiant(sunVec.head.x, sunVec.head.y, asteroid.Radius, center.x, center.y, asteroid.Radius * 2);
                //grd.addColorStop(0, asteroid.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                orbs.ctx.fillStyle = orbs.model.Asteroids[idx].Color;

                orbs.ctx.beginPath();
                orbs.ctx.moveTo(points[points.length - 1].x, points[points.length - 1].y);
                for (var p in points) {
                    orbs.ctx.lineTo(points[p].x, points[p].y);
                }
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShots: function (dt) {
            for (var idx in orbs.model.Shots) {
                orbs.model.Shots[idx] = orbs.animate.updateOrbital(orbs.model.Shots[idx], orbs.model.Planets, dt);

                var center = orbs.convertPoint(orbs.model.Shots[idx].Vel.Origin);

                orbs.ctx.fillStyle = orbs.model.Shots[idx].Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, 1.5 * orbs.unit, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShips: function (dt) {
            for (var idx in orbs.model.Ships) {
                orbs.model.Ships[idx] = orbs.animate.updatePolygon(orbs.model.Ships[idx], orbs.model.Planets, dt);

                var points = [];
                for (var a in orbs.model.Ships[idx].Arms) {
                    points.push(orbs.convertPoint(orbs.model.Ships[idx].Arms[a].Head));
                }

                orbs.ctx.strokeStyle = orbs.model.Ships[idx].Color;
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
    };
});