﻿'use strict';

var orbs = {};
orbs.gameParams = [];

//orbs.selectGameType = function (number) {
//    orbs.gameParams['type'] = number;

//    var buttons = $('.game-type');
//    buttons.css('background-color', 'rgb(64, 192, 192)');
//    buttons.css('border-color', 'rgb(64, 128, 128)');

//    if (number === 1) {
//        var toHighlight = $('.game-type[value="Collaborative"]');
//        $('.players[value="1"]').show();
//    } else if (number === 2) {
//        toHighlight = $('.game-type[value="Competitive"]');
//        if (orbs.gameParams['players'] === 1) {
//            orbs.gameParams['players'] = 2;
//            orbs.selectPlayerNumber(orbs.gameParams['players']);
//        }
//        $('.players[value="1"]').hide();
//    }
//    toHighlight.css('background-color', 'rgb(192, 64, 192)');
//    toHighlight.css('border-color', 'rgb(128, 64, 128)');
//};

//orbs.selectPlayerNumber = function (number) {
//    orbs.gameParams['players'] = number;

//    var buttons = $('.players');
//    buttons.css('background-color', 'rgb(64, 192, 192)');
//    buttons.css('border-color', 'rgb(64, 128, 128)');

//    var toHighlight = $('.players[value="' + number + '"]');
//    toHighlight.css('background-color', 'rgb(192, 64, 192)');
//    toHighlight.css('border-color', 'rgb(128, 64, 128)');
//};

orbs.selectLevel = function (number) {
    orbs.gameParams['level'] = number;

    var buttons = $('.levels');
    buttons.css('background-color', 'rgb(64, 192, 192)');
    buttons.css('border-color', 'rgb(64, 128, 128)');

    var toHighlight = $('.levels[value="' + number + '"]');
    toHighlight.css('background-color', 'rgb(192, 64, 192)');
    toHighlight.css('border-color', 'rgb(128, 64, 128)');
};

orbs.gameParams['type'] = 1;
orbs.gameParams['players'] = 1;
orbs.gameParams['level'] = 0;

orbs.selectLevel(orbs.gameParams['level']);
//orbs.selectGameType(orbs.gameParams['type']);
//orbs.selectPlayerNumber(orbs.gameParams['players']);

