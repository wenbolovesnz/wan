﻿setTimeout(function () {
    $.connection.joinmeHub.client.newGroupMememberArrived = function(data) {

    };
    
    $.connection.joinmeHub.client.newGroupMessage = function (data) {

    };
    
    $.connection.hub.logging = true;
    $.connection.hub.start();
}, 2000);