'use strict';

$(document).ready(function () {
    orbs.animate = {
        updatePolygon: function (polygon, planets, dt) {
            var x = polygon.Vel.Origin.X;
            var y = polygon.Vel.Origin.Y;
            // updates origin
            polygon = orbs.animate.updateOrbital(polygon, planets, dt);

            var dx = polygon.Vel.Origin.X - x;
            var dy = polygon.Vel.Origin.Y - y;

            // updates arms
            for (var i = 0; i < polygon.Arms.length; i++) {
                polygon.Arms[i] = orbs.animate.translateVector(orbs.animate.rotateVector(polygon.Arms[i], polygon.DeltaRot), dx, dy);
            }
            return polygon;
        },
        updateOrbital: function (orbital, planets, dt) {
            for (var idx in planets) {
                // add gravity vector to accel
                // add accel to vel
            }
            var dx = dt * orbital.Vel.Delta.X / orbs.ups;
            var dy = dt * orbital.Vel.Delta.Y / orbs.ups;
            orbital.Vel = orbs.animate.translateVector(orbital.Vel, dx, dy);
            return orbital;
        },
        updateRotational: function (rotational, dt) {
            rotational.ForwardAngle += dt * rotational.DeltaRot / orbs.ups;
            return rotational;
        },
        rotateVector: function (vector, angle) {
            // add accelRot to 
            vector.ForwardAngle += angle;

            var newX = vector.Length * ((vector.Delta.X / vector.Length) * Math.cos(vector.ForwardAngle) - (vector.Delta.Y / vector.Length) * Math.sin(vector.ForwardAngle));
            var newY = vector.Length * ((vector.Delta.X / vector.Length) * Math.sin(vector.ForwardAngle) + (vector.Delta.Y / vector.Length) * Math.cos(vector.ForwardAngle));
            vector.Delta.X = newX;
            vector.Delta.Y = newY;
            vector.Head.X = vector.Origin.X + vector.Delta.X;
            vector.Head.Y = vector.Origin.Y + vector.Delta.Y;
            return vector;
        },
        translateVector: function (vector, dx, dy) {
            vector.Origin.X += dx;
            vector.Origin.Y += dy;
            vector.Head.X += dx;
            vector.Head.Y += dy;
            return vector;
        }
    };
    orbs.renderFrame();
});