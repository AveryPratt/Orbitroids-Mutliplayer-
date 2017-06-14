'use strict';

$(document).ready(function () {
    orbs.game = $.connection.gameHub,
    orbs.canvas = document.getElementById('gamescreen'),
    orbs.ctx = document.getElementById('gamescreen').getContext('2d'),
    orbs.unit = 1,
    orbs.ups;
    orbs.lastModelUpdate;
    orbs.lastClientUpdate;
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

    orbs.game.client.receiveUpdate = function (model, ups) {
        orbs.model = model;
        orbs.ups = ups;
        orbs.lastModelUpdate = Date.now();
        console.log(orbs.model);
    }
    
    orbs.renderFrame = function () {
        requestAnimationFrame(orbs.renderFrame);

        var time = Date.now();

        // fps counter
        var delta = (time - orbs.lastClientUpdate) / 1000;
        orbs.fps = 1 / delta;
        orbs.lastClientUpdate = time;
        if (orbs.model) {
            console.log(orbs.model.Ships[0].Arms[0].Head.X + ", " + orbs.model.Ships[0].Arms[0].Head.Y);
        }

        var dt = time - orbs.lastModelUpdate;

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

        if (orbs.model) {
            orbs.ctx.fillStyle = "#000000";
            orbs.ctx.fillRect(0, 0, orbs.canvas.width, orbs.canvas.height);

            orbs.drawings.renderPlanets(dt);
            orbs.drawings.renderAsteroids(dt);
            orbs.drawings.renderShots(dt);
            orbs.drawings.renderShips(dt);
        }
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