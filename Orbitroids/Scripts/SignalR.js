'use strict';

$(document).ready(function () {
    var game = $.connection.gameHub;
    var canvas = document.getElementById('gamescreen');
    var ctx = canvas.getContext('2d');

    game.client.log = function (message) {
        console.log(message);
    };

    game.client.renderFrame = function (model) {
        var ship = model.Ships[0];
        var center = {
            x: ship.Vel.Origin.X + 50,
            y: ship.Vel.Origin.Y + 50
        };
        var nose = {
            x: ship.Arms[0].Head.X + 50,
            y: ship.Arms[0].Head.Y + 50
        };
        var right = {
            x: ship.Arms[1].Head.X + 50,
            y: ship.Arms[1].Head.Y + 50
        };
        var left = {
            x: ship.Arms[3].Head.X + 50,
            y: ship.Arms[3].Head.Y + 50
        };
        //ctx.strokeStyle = 'rgba(' + ship.Color + ')';
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(nose.x, nose.y);
        ctx.lineTo(right.x, right.y);
        ctx.lineTo(center.x, center.y);
        ctx.lineTo(left.x, left.y);
        ctx.closePath();
        ctx.stroke();
        console.log("Center: " +
            ship.Vel.Origin.X + ", " + ship.Vel.Origin.Y + " | Nose: " +
            ship.Arms[0].Head.X + ", " + ship.Arms[0].Head.Y + " | Right: " +
            ship.Arms[1].Head.X + ", " + ship.Arms[1].Head.Y + " | Left: " +
            ship.Arms[3].Head.X + ", " + ship.Arms[3].Head.Y);
    }

    $(document).keydown(function (event) {
        switch (event.keyCode) {
            case 13: // enter
                game.server.enter();
                break;
            case 80: // p
                game.server.pause();
                break;
            case 32: // space
                game.server.shoot();
                break;
            case 38: // up
            case 87: // w
                game.server.burn();
                break;
            case 40: // down
            case 83: // s
                game.server.slowBurn();
                break;
            case 37: // left
            case 65: // a
                game.server.rotate("left");
                break;
            case 39: // right
            case 68: // d
                game.server.rotate("right");
                break;
            case 16: // shift
                game.server.dampenControls();
                break;
            default:
                break;
        }
    });

    $(document).keyup(function (event) {
        switch (event.keyCode) {
            case 38: // up
            case 87: // w
                game.server.releaseBurn();
                break;
            case 40: // down
            case 83: // s
                game.server.releaseSlowBurn();
                break;
            case 37: // left
            case 65: // a
                game.server.releaseRotate("left");
                break;
            case 39: // right
            case 68: // d
                game.server.releaseRotate("right");
                break;
            case 16: // shift
                game.server.releaseDampenControls();
                break;
            default:
                break;
        }
    });

    $.connection.hub.start();
});