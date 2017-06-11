﻿'use strict';

$(document).ready(function () {
    var game = $.connection.gameHub;

    game.client.log = function (message) {
        console.log(message);
    };

    game.client.renderFrame = function (model) {
        console.log(model);
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