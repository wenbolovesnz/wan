$(function() {
    setTimeout(function () {
        $.connection.joinmeHub.client.newGroupMememberArrived = function (data) {

        };

        $.connection.joinmeHub.client.newGroupMessage = function (data) {

        };

        $.connection.joinmeHub.client.newPersonalMessage = function (data) {

        };

        $.connection.hub.logging = true;
        $.connection.hub.start();
    }, 1000);
});

