'use strict';

$(document).ready(function () {
    orbs.animate = {
        updateOrbital: function (orbital, planets, dt) {
            for (var idx in planets) {
                // add gravity vector to accel
                // add accel to vel
            }
            orbital.Vel.Origin.X += dt * orbital.Vel.Delta.X;
            orbital.Vel.Origin.Y += dt * orbital.Vel.Delta.Y;
        },
        updateRotational: function (rotational, dt) {
            rotational.ForwardAngle += rotational.DeltaRot;
        }
    };
});