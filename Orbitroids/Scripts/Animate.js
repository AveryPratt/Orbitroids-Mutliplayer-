﻿'use strict';

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
                polygon.Arms[i] = orbs.animate.translateVector(orbs.animate.rotateVector(polygon.Arms[i], polygon.DeltaRot * dt / orbs.updateRate), dx, dy);
            }
            return polygon;
        },
        updateOrbital: function (orbital, planets, dt) {
            for (var idx in planets) {
                // add gravity vector to accel
                // add accel to vel
            }
            var dx = dt * orbital.Vel.Delta.X / orbs.updateRate;
            var dy = dt * orbital.Vel.Delta.Y / orbs.updateRate;
            orbital.Vel = orbs.animate.translateVector(orbital.Vel, dx, dy);
            return orbital;
        },
        updateRotational: function (rotational, dt) {
            rotational.ForwardAngle += rotational.DeltaRot * dt / orbs.updateRate;
            return rotational;
        },
        rotateVector: function (vector, angle) {
            // TODO: add accelRot to deltaRot
            if (angle != 0) {
                console.log("Hey, it moves!");
            }
            angle = -angle;
            vector.ForwardAngle += angle;

            var newX = vector.Length * ((vector.Delta.X / vector.Length) * Math.cos(angle) - (vector.Delta.Y / vector.Length) * Math.sin(angle));
            var newY = vector.Length * ((vector.Delta.X / vector.Length) * Math.sin(angle) + (vector.Delta.Y / vector.Length) * Math.cos(angle));
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