'use strict';

$(document).ready(function () {
    var game = $.connection.gameHub;
    game.update = function () {
        $('canvas').css('background-color', 'red');
    };
    $(document).keydown(function () {
        chat.submitInput();
    });
    $.connection.start();
});