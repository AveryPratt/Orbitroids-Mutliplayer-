'use strict';

$(document).ready(function () {
    var game = $.connection.gameHub;
    game.client.update = function () {
        $('canvas').css('background-color', 'red');
    };
    $(document).keydown(function () {
        game.server.submitInput();
    });
    $.connection.hub.start();
});