'use strict';

$(document).ready(function () {
    var game = $.connection.gameHub;
    game.client.update = function () {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        $('canvas').css('background-color', color);
    };
    $(document).keydown(function () {
        game.server.submitInput();
    });
    $.connection.hub.start();
});