
wan.controller('NotificationMessagesCtrl',
    ['$scope', 'datacontext','$location','userService', 'hub',
    function ($scope, datacontext, $location, userService, hub) {


        $scope.processing = true;
        $scope.messages = [];
        $scope.personalMessages = [];

        datacontext.joinGroupRequest().query(function (result) {
            if (result.length > 0) {
                userService.messages = result;
                
            } else {
                userService.messages = [];
            }

            $scope.messages = userService.messages;

            datacontext.personalMessage().query(function (pmrs) {
                if (pmrs.length > 0) {
                    userService.personalMessages = pmrs;
                    $scope.personalMessages = userService.personalMessages;
                }
                
                $scope.processing = false;
            });

        });


        $scope.read = function (pm) {
            $scope.processing = true;
            pm.isRead = true;

            pm.$update(function () {
                $scope.processing = false;
                removeItemFromArray(userService.personalMessages, pm);

                var pmToRemove = _.find(userService.messages, function (m) {
                    return m.content == pm.content;
                });
                if (pmToRemove) {
                    removeItemFromArray(userService.messages, pmToRemove);
                }
                
            });
        };

        $scope.accept = function(message) {
            $scope.processing = true;
            message.isProcessed = true;
            message.isApproved = true;
            
            message.$update(function() {
                $scope.processing = false;
                hub.server.sendPersonalMessage(message);
                removeItemFromArray(userService.messages, message);
            });

        };
        
        $scope.decline = function (message) {

            bootbox.prompt("Please state the reason for this?", function (result) {
                if (result === null) {

                } else {
                    $scope.processing = true;
                    message.isProcessed = true;
                    message.isApproved = false;
                    message.declineReason = result;

                    message.$update(function () {
                        $scope.processing = false;
                        hub.server.sendPersonalMessage(message);
                        removeItemFromArray(userService.messages, message);
                    });
                }
            });


        };
        
        function removeItemFromArray(array, item) {
            var index = _.indexOf(array, item);
            if (index != -1) {
                array.splice(index, 1);
            }
        };
    }]);