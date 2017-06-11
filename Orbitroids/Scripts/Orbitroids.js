'use strict';

$(document).ready(function () {
    orbs.drawings = {
        renderPlanets: function (planets) {
            for (var idx in planets) {
                var planet = planets[idx];

                var center = orbs.convertPoint(planet.Vel.Origin);

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, planet.Radius, center);
                //var grd = ctx.createLinearGradient(sunVec.head.x, sunVec.head.y, planet.Radius, center.x, center.y, planet.Radius * 2);
                //grd.addColorStop(0, planet.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                orbs.ctx.fillStyle = planet.Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, planet.Radius, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderAsteroids: function (asteroids) {
            for (var idx in asteroids) {
                var asteroid = asteroids[idx];

                var points = [];
                for (var a in asteroid.Arms) {
                    points.push(orbs.convertPoint(asteroid.Arms[a].Head));
                }

                //var sunVec = orbs.vecCirc(orbs.sunAngle + Math.PI, asteroid.Radius, center);
                //var grd = ctx.createRadialGradiant(sunVec.head.x, sunVec.head.y, asteroid.Radius, center.x, center.y, asteroid.Radius * 2);
                //grd.addColorStop(0, asteroid.Color);
                //grd.addColorStop(1, 'rgba(255, 255, 255, 1)');
                //ctx.fillStyle = grd;
                orbs.ctx.fillStyle = asteroid.Color;

                orbs.ctx.beginPath();
                orbs.ctx.moveTo(points[points.length - 1].x, points[points.length - 1].y);
                for (var p in points) {
                    orbs.ctx.lineTo(points[p].x, points[p].y);
                }
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShots: function (shots) {
            for (var idx in shots) {
                var shot = shots[idx];

                var center = orbs.convertPoint(shot.Vel.Origin);

                orbs.ctx.fillStyle = shot.Color;

                orbs.ctx.beginPath();
                orbs.ctx.arc(center.x, center.y, 1.5 * orbs.unit, 0, 2 * Math.PI, false);
                orbs.ctx.closePath();

                orbs.ctx.fill();
            }
        },
        renderShips: function (ships) {
            for (var idx in ships) {
                var ship = ships[idx];

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
    };
});