'use strict';

$(document).ready(function () {
    orbs.game = $.connection.gameHub,
    orbs.canvas = document.getElementById('gamescreen'),
    orbs.ctx = document.getElementById('gamescreen').getContext('2d'),
    orbs.unit = 1,
    orbs.updateRate = 1000 / 100;
    orbs.lastTime;
    orbs.fps;
    orbs.convertPoint = function (point) {
        return {
            x: point.X * orbs.unit + orbs.canvas.width / 2,
            y: orbs.canvas.height / 2 - orbs.unit * point.Y
        };
    }

    orbs.game.client.log = function (message) {
        console.log(message);
    };

    orbs.game.client.receiveUpdate = function (model, time) {
        orbs.model = model;
        orbs.lastTime = time;
        orbs.renderFrame();
    }

    orbs.renderFrame = function () {
        var time = Date.now();

        // fps counter
        var delta = (time - orbs.lastTime) / 1000;
        orbs.fps = 1 / delta;
        console.log(orbs.fps);

        if (window.innerWidth >= window.innerHeight) {
            orbs.canvas.width = window.innerHeight;
            orbs.canvas.height = window.innerHeight;
            orbs.unit = window.innerHeight / 1000;
        }
        else {
            orbs.canvas.width = window.innerWidth;
            orbs.canvas.height = window.innerWidth;
            orbs.unit = window.innerWidth / 1000;
        }

        requestAnimationFrame(orbs.renderFrame);

        var dt = (time - orbs.lastTime) / orbs.updateRate;
        orbs.lastTime = time;

        orbs.ctx.fillStyle = "#000000";
        orbs.ctx.fillRect(0, 0, orbs.canvas.width, orbs.canvas.height);

        orbs.drawings.renderPlanets(orbs.model.Planets, orbs.ctx, dt);
        orbs.drawings.renderAsteroids(orbs.model.Asteroids, orbs.model.Planets, orbs.ctx, dt);
        orbs.drawings.renderShots(orbs.model.Shots, orbs.model.Planets, orbs.ctx, dt);
        orbs.drawings.renderShips(orbs.model.Ships, orbs.model.Planets, orbs.ctx, dt);
        console.log(orbs.model);
    }

    $(document).keydown(function (event) {
        switch (event.keyCode) {
            case 13: // enter
                event.preventDefault();
                orbs.game.server.enter();
                break;
            case 80: // p
                event.preventDefault();
                orbs.game.server.pause();
                break;
            case 32: // space
                event.preventDefault();
                orbs.game.server.shoot();
                break;
            case 38: // up
            case 87: // w
                event.preventDefault();
                orbs.game.server.burn();
                break;
            case 40: // down
            case 83: // s
                event.preventDefault();
                orbs.game.server.slowBurn();
                break;
            case 37: // left
            case 65: // a
                event.preventDefault();
                orbs.game.server.rotate("right");
                break;
            case 39: // right
            case 68: // d
                event.preventDefault();
                orbs.game.server.rotate("left");
                break;
            case 16: // shift
                event.preventDefault();
                orbs.game.server.dampenControls();
                break;
            default:
                break;
        }
    });

    $(document).keyup(function (event) {
        switch (event.keyCode) {
            case 38: // up
            case 87: // w
                event.preventDefault();
                orbs.game.server.releaseBurn();
                break;
            case 40: // down
            case 83: // s
                event.preventDefault();
                orbs.game.server.releaseSlowBurn();
                break;
            case 37: // left
            case 65: // a
                event.preventDefault();
                orbs.game.server.releaseRotate("right");
                break;
            case 39: // right
            case 68: // d
                event.preventDefault();
                orbs.game.server.releaseRotate("left");
                break;
            case 16: // shift
                event.preventDefault();
                orbs.game.server.releaseDampenControls();
                break;
            default:
                break;
        }
    });
    $.connection.hub.start();
});