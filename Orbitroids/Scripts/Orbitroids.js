'use strict';

$(document).ready(function () {
    orbs.drawings = {
        renderPlanets: function (planets, ctx) {
            for (var idx in planets) {
                var planet = planets[idx];
                var center = orbs.convertPoint(planet.Vel.Origin);
                ctx.arc(center.x, center.y, planet.Radius, 0, 2 * Math.PI, false);

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, planet.Radius, center);
                //var grd = ctx.createLinearGradient(sunVec.head.x, sunVec.head.y, planet.Radius, center.x, center.y, planet.Radius * 2);
                //grd.addColorStop(0, planet.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                ctx.fillStyle = planet.Color;

                ctx.fill();
            }
        },
        renderAsteroids: function (asteroids, ctx) {
            for (var idx in asteroids) {
                var asteroid = asteroids[idx];
                var center = orbs.convertPoint(asteroid.Vel.Origin);
                var points = [];
                for (var arm in asteroid.Arms) {
                    points.push(orbs.convertPoint(arm.Head));
                }
                orbs.ctx.moveTo(points[points.length - 1].x, points[points.length - 1].y);
                for (var point in points) {
                    ctx.lineTo(point.x, point.y);
                }
                ctx.closePath();

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, asteroid.Radius, center);
                //var grd = ctx.createRadialGradiant(sunVec.head.x, sunVec.head.y, asteroid.Radius, center.x, center.y, asteroid.Radius * 2);
                //grd.addColorStop(0, asteroid.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                ctx.fillStyle = asteroid.Color;

                ctx.fill();
            }
        },
        renderShots: function (shots, ctx) {
            for (var idx in shots) {
                var shot = shots[idx];
                var center = orbs.convertPoint(shot.Vel.Origin);
                ctx.beginPath();
                ctx.arc(center.x, center.y, 1.5 * orbs.unit, 0, 2 * Math.PI, false);
                ctx.fillStyle = shot.Color;
                ctx.fill();
            }
        },
        renderShips: function (ships, ctx) {
            for (var idx in ships) {
                var ship = ships[idx];
                var center = orbs.convertPoint(ship.Vel.Origin);
                var nose = orbs.convertPoint(ship.Arms[0].Head);
                var right = orbs.convertPoint(ship.Arms[1].Head);
                var left = orbs.convertPoint(ship.Arms[3].Head);
                ctx.strokeStyle = ship.Color;
                ctx.lineWidth = 1;
                ctx.beginPath();
                ctx.moveTo(nose.x, nose.y);
                ctx.lineTo(right.x, right.y);
                ctx.lineTo(center.x, center.y);
                ctx.lineTo(left.x, left.y);
                ctx.closePath();
                ctx.stroke();
            }
        }
    };
});